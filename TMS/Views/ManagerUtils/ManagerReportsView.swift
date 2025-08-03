//
//  ManagerReportsView.swift
//  TMS
//
//  Created by Evan Heidenreich on 7/31/25.
//
import SwiftUI
import Foundation
import Supabase

struct ManagerReportsView: View {
    @Binding var selectedPage: String
    @State private var selectedDate = Date()
    @State private var shifts: [ShiftReport] = []
    @State private var isLoading = false
    @State private var errorMessage: String?
    private let supabase = SupabaseManager.shared.supabase
    
    var body: some View {
        VStack(alignment: .leading, spacing: 0) {
            HStack {
                Spacer()
                Text("Reports")
                    .font(.headline)
                    .padding()
                Spacer()
            }
            Button("<- Back"){
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
                ProgressView("Loading Shifts...")
                    .padding()
            } else if let errorMessage = errorMessage {
                Text("Error: \(errorMessage)")
                    .foregroundStyle(.red)
                    .padding()
            } else if shifts.isEmpty {
                Text("No employees scheduled for this date.")
                    .foregroundStyle(.gray)
                    .padding()
            } else {
                List(shifts) { shift in
                    VStack(alignment: .leading) {
                        Text("\(shift.employeeName)")
                            .font(.headline)
                        Text("\(shift.shiftType)")
                            .foregroundColor(.purple)
                        
                        // Show times and hours
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
                // Step 1: Fetch time_log with clock-in/out/hours
                let shiftRows: [TimeLogRow] = try await supabase
                    .from("time_log")
                    .select("id, shift_date, shift_type, clock_in, clock_out, hours")
                    .eq("shift_date", value: dateString)
                    .order("shift_type", ascending: true)
                    .execute()
                    .value
                
                let userIds = shiftRows.map { $0.userId }
                
                // Step 2: Fetch names from user_data
                let userRows: [UserData] = try await supabase
                    .from("user_data")
                    .select("id, First, Last")
                    .in("id", values: userIds)
                    .execute()
                    .value
                
                // Step 3: Merge results
                let mergedShifts: [ShiftReport] = shiftRows.compactMap { shift in
                    if let user = userRows.first(where: { $0.id == shift.userId }) {
                        return ShiftReport(
                            userId: shift.userId,
                            shiftDate: shift.shiftDate,
                            shiftType: shift.shiftType,
                            employeeName: "\(user.First) \(user.Last)",
                            clockIn: shift.clockIn,
                            clockOut: shift.clockOut,
                            hours: shift.hours
                        )
                    }
                    return nil
                }
                
                DispatchQueue.main.async {
                    self.shifts = mergedShifts
                    self.isLoading = false
                }
            } catch {
                DispatchQueue.main.async {
                    self.errorMessage = "Failed to load shifts: \(error.localizedDescription)"
                    self.isLoading = false
                }
            }
        }
    }
}

