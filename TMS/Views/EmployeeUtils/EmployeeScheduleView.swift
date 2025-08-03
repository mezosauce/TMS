import SwiftUI
import Supabase
import Foundation
import CoreLocation

struct GeoLocations: Decodable {
    let id: UUID
    let latitiude: Double
    let longitude: Double
    let radius_meters: Double
}

struct EmployeeShift: Identifiable, Decodable {
    var id: Int64 { tid }
    let tid: Int64
    let userId: UUID
    let shiftDate: String  
    let shiftType: String
    let shift: Date?
    let status: Bool

    enum CodingKeys: String, CodingKey {
        case tid
        case userId = "id"
        case shiftDate = "shift_date"
        case shiftType = "shift_type"
        case shift
        case status
    }

    var shiftDateParsed: Date? {
        let formatter = DateFormatter()
        formatter.dateFormat = "yyyy-MM-dd"
        formatter.timeZone = TimeZone.current
        return formatter.date(from: shiftDate)
    }

    var formattedWeekday: String {
        guard let date = shiftDateParsed else { return "Invalid date" }
        return Calendar.current.weekdaySymbols[Calendar.current.component(.weekday, from: date) - 1]
    }

    var formattedDateDisplay: String {
        guard let date = shiftDateParsed else { return shiftDate }
        return date.formatted(date: .abbreviated, time: .omitted)
    }
}

struct EmployeeScheduleView: View {
    private let supabase = SupabaseManager.shared.supabase
    @State private var shifts: [EmployeeShift] = []
    @State private var currentTime = Date()
    @State private var clockedIn = false
    @State private var locationStatus = ""

    let locationManager = CLLocationManager()

    var body: some View {
        VStack(spacing: 20) {
            Text("Employee Schedule View")
                .font(.title)
                .fontWeight(.bold)

            Text("Current Time: \(currentTime.formatted(date: .omitted, time: .standard))")
                .font(.headline)

            if shifts.isEmpty {
                Text("No shifts this week.")
                    .foregroundColor(.gray)
            } else {
                VStack(alignment: .leading, spacing: 10) {
                    Text("Your Shifts this week:")
                        .font(.headline)

                    ForEach(shifts, id: \.id) { shift in
                        HStack {
                            Text("\(shift.formattedWeekday), \(shift.formattedDateDisplay)")
                            Spacer()
                            Text(shift.shiftType)
                                .foregroundColor(.purple)
                        }
                    }
                }
            }
        }
        .padding()
        .onAppear {
            Task {
                await fetchShifts()
            }
            Timer.scheduledTimer(withTimeInterval: 1.0, repeats: true) { _ in
                currentTime = Date()
            }
        }
    }

    private func fetchShifts() async {
        guard let userId = try? supabase.auth.session.user.id else {
            print("No logged-in user")
            return
        }

        let calendar = Calendar.current
        guard let startOfWeek = calendar.date(from: calendar.dateComponents([.yearForWeekOfYear, .weekOfYear], from: Date())),
              let endOfWeek = calendar.date(byAdding: .day, value: 6, to: startOfWeek) else {
            return
        }

        let dateFormatter = DateFormatter()
        dateFormatter.dateFormat = "yyyy-MM-dd"
        dateFormatter.timeZone = TimeZone.current

        let isoStart = dateFormatter.string(from: startOfWeek)
        let isoEnd = dateFormatter.string(from: endOfWeek)

        do {
            let results: [EmployeeShift] = try await supabase
                .from("time_log")
                .select("tid, id, shift_date, shift_type, shift, status")
                .eq("id", value: userId.uuidString)
                .gte("shift_date", value: isoStart)
                .lte("shift_date", value: isoEnd)
                .order("shift_date", ascending: true)
                .execute()
                .value

            self.shifts = results
            print("Fetched shifts: \(results)")
        } catch {
            print("Error fetching shifts: \(error)")
        }
    }
    
}
