//
//  EmployeeReportsView.swift
//  TMS
//
//  Created by Evan Heidenreich on 8/2/25.
//

import SwiftUI
import Foundation
import Supabase

struct EmployeeReportsView: View {
    @Binding var selectedPage: String
    @State private var selectedDate = Date()
    @State private var shifts: [EmployeeShiftReport] = []
    @State private var isLoading = false
    @State private var errorMessage: String?
    private let supabase = SupabaseManager.shared.supabase

    var body: some View {
        VStack(alignment: .leading, spacing: 0) {
            HStack {
                Spacer()
                Text("My Shifts")
                    .font(.headline)
                    .padding()
                Spacer()
            }
            Button("<- Back") {
                selectedPage = "home"
            }
            .buttonStyle(.bordered)
            .tint(.purple)
            .padding()

            DatePicker("Select Date", selection: $selectedDate, displayedComponents: .date)
                .datePickerStyle(.graphical)
                .padding()
                .onChange(of: selectedDate) { _ in
                    fetchShiftsForDate()
                }

            Divider()

            if isLoading {
                ProgressView("Loading Your Shift...")
                    .padding()
            } else if let errorMessage = errorMessage {
                Text("Error: \(errorMessage)")
                    .foregroundColor(.red)
                    .padding()
            } else if shifts.isEmpty {
                Text("No shift scheduled for this date.")
                    .foregroundColor(.gray)
                    .padding()
            } else {
                List(shifts) { shift in
                    VStack(alignment: .leading) {
                        Text(shift.shiftType)
                            .foregroundColor(.purple)
                            .font(.headline)

                        if let clockIn = shift.clockInFormatted,
                           let clockOut = shift.clockOutFormatted {
                            Text("Clock In: \(clockIn)")
                            Text("Clock Out: \(clockOut)")
                        }
                        Text("Total Hours: \(String(format: "%.2f", shift.hours)) hrs")
                            .foregroundColor(.secondary)
                    }
                    .padding(.vertical, 4)
                }
            }
        }
        .padding()
        .onAppear {
            fetchShiftsForDate()
        }
    }

    private func fetchShiftsForDate() {
        isLoading = true
        errorMessage = nil

        let dateFormatter = DateFormatter()
        dateFormatter.dateFormat = "yyyy-MM-dd"
        let dateString = dateFormatter.string(from: selectedDate)

        Task {
            do {
                // Get the current user ID from session
                guard let userId = try? supabase.auth.session.user.id else {
                    throw NSError(domain: "", code: 401, userInfo: [NSLocalizedDescriptionKey: "User not logged in"])
                }

                // Fetch the employee's own shifts for the selected date
                let shiftRows: [EmployeeTimeLogRow] = try await supabase
                    .from("time_log")
                    .select("id, shift_date, shift_type, clock_in, clock_out, hours")
                    .eq("shift_date", value: dateString)
                    .eq("id", value: userId.uuidString)
                    .order("shift_type", ascending: true)
                    .execute()
                    .value

                let mergedShifts: [EmployeeShiftReport] = shiftRows.map { shift in
                    EmployeeShiftReport(
                        userId: shift.userId,
                        shiftDate: shift.shiftDate,
                        shiftType: shift.shiftType,
                        employeeName: "", 
                        clockIn: shift.clockIn,
                        clockOut: shift.clockOut,
                        hours: shift.hours
                    )
                }

                DispatchQueue.main.async {
                    self.shifts = mergedShifts
                    self.isLoading = false
                }
            } catch {
                DispatchQueue.main.async {
                    self.errorMessage = "Failed to load your shifts: \(error.localizedDescription)"
                    self.isLoading = false
                }
            }
        }
    }
}

struct EmployeeTimeLogRow: Decodable {
    let userId: UUID
    let shiftDate: String
    let shiftType: String
    let clockIn: Date?
    let clockOut: Date?
    let hours: Double

    enum CodingKeys: String, CodingKey {
        case userId = "id"
        case shiftDate = "shift_date"
        case shiftType = "shift_type"
        case clockIn = "clock_in"
        case clockOut = "clock_out"
        case hours
    }
}

struct EmployeeShiftReport: Identifiable {
    var id: String { "\(userId)-\(shiftDate)-\(shiftType)" }

    let userId: UUID
    let shiftDate: String
    let shiftType: String
    let employeeName: String
    let clockIn: Date?
    let clockOut: Date?
    let hours: Double

    var clockInFormatted: String? {
        guard let clockIn = clockIn else { return nil }
        let formatter = DateFormatter()
        formatter.timeStyle = .short
        return formatter.string(from: clockIn)
    }

    var clockOutFormatted: String? {
        guard let clockOut = clockOut else { return nil }
        let formatter = DateFormatter()
        formatter.timeStyle = .short
        return formatter.string(from: clockOut)
    }
}
