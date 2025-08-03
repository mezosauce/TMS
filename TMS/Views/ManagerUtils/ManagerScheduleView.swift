//
//  ManagerScheduleView.swift
//  TMS
//
//  Created by Evan Heidenreich on 7/12/25.
//
import SwiftUI
import Foundation
import Supabase

struct ShiftAssignment: Identifiable {
    let id = UUID()
    let user: scheduleUser
    let color: Color
}

public struct ManagerScheduleView: View {
    private let supabase = SupabaseManager.shared.supabase
    @Binding var selectedPage: String
    @State private var selectedDate =  Date()
    @State private var users: [scheduleUser] = []
    @State private var selectedUser: scheduleUser? = nil
    @State private var selectedShift = "First Shift"
    @State private var assignedUser: [ShiftAssignment] = []
    private  let shiftOptions: [String] = ["First Shift", "Second Shift", "Third Shift"]
    @State private var shiftSchedule: [String: [ShiftRecord]] = [:]
    
    public var body: some View {
        NavigationView {
            VStack(alignment: .leading, spacing: 10) {
//                DatePicker("Select Date", selection: $selectedDate, displayedComponents: .date)
//                Picker("Select Shift", selection: $selectedShift) {
//                    ForEach(shiftOptions, id: \.self) { shift in
//                        Text(shift)
//                    }
//                }
//                .pickerStyle(SegmentedPickerStyle())
                
                Text("Select Shift to Assign")
                    .font(.headline)
                
                Picker("Shift", selection: $selectedShift) {
                    ForEach(shiftOptions, id: \.self) { shift in
                        Text(shift)
                    }
                }
                .pickerStyle(MenuPickerStyle())
                
                Picker("Assign User", selection: $selectedUser) {
                    Text("Select a User").tag(Optional<scheduleUser>.none)
                    ForEach(users, id: \.self) { user in
                        Text(user.fullName).tag(Optional(user))
                    }
                }
                .pickerStyle(MenuPickerStyle())
                
                Button("Add User to shift") {
                    if let user = selectedUser,
                       !assignedUser.contains(where: { $0.user.id == user.id }) {
                        assignedUser.append(ShiftAssignment(user: user, color: randomColor()))
                        selectedUser = nil
                    }
                }
                .disabled(selectedUser == nil)
                
                if !assignedUser.isEmpty {
                    Text("Assigned Users (Mon-Fri):")
                        .font(.headline)
                    
                    ForEach(assignedUser) { assignment in
                        //                        HStack {
                        //                            Circle()
                        //                                .fill(assignment.color)
                        //                                .frame(width: 20, height: 20)
                        //
                        //                            Text(assignment.user.fullName)
                        //                        }
                        //                    }
                        VStack(alignment: .leading) {
                            HStack {
                                Circle()
                                    .fill(assignment.color)
                                    .frame(width: 20, height: 20)
                                Text(assignment.user.fullName)
                            }
                            
                            if let shifts = shiftSchedule[assignment.user.id] {
                                ForEach(shifts) { shift in
                                    Text("📅 \(formattedDate(shift.shiftDate)) – \(shift.shiftType)")
                                        .font(.caption)
                                        .foregroundColor(.gray)
                                }
                            } else {
                                Text("No shifts yet.")
                                    .font(.caption)
                                    .foregroundColor(.secondary)
                            }
                        }
                    }
                }
                Spacer()
                
                Button(action: saveShiftsForWeek) {
                    Text("Save Shift")
                        .padding()
                        .frame(width: 200, height: 50)
                        .background(Color.blue)
                        .foregroundColor(.white)
                        .cornerRadius(10)
                }
                .padding(.top)
            }
            .padding()
            .navigationTitle("Schedule Shift")
            .onAppear{
                Task {
                    await fetchEmployees()
                    await fetchShiftAssignments()
                }
            }
        }
    }
    
    private func randomColor() -> Color {
        let colors: [Color] = [.red, .green, .blue, .yellow, .orange, .purple, .pink, .cyan, .indigo, .teal]
        return colors.randomElement() ?? .gray
    }
    
    private func fetchEmployees() async {
        do {
            let employees: [scheduleUser] = try await supabase
                .from("user_data")
                .select("*")
                .eq("Position", value: "Employee")
                .execute()
                .value
            
            users = employees
        } catch {
            print("Fetch employees failed: \(error)")
        }
    }
    
    private func saveShiftsForWeek() {
        let calendar = Calendar.current
        let today = Date()
        //let hours = 8

        guard let nextMonday = calendar.nextDate(after: today, matching: DateComponents(weekday: 2), matchingPolicy: .nextTime) else {
            print("Could not find next Monday")
            return
        }

        Task {
            for i in 0..<5 {
                let date = calendar.date(byAdding: .day, value: i, to: nextMonday)!
                let isoDate = ISO8601DateFormatter().string(from: date)

                for assignment in assignedUser {
                    do {
                        try await supabase
                            .from("time_log")
                            .insert([
                                "id": assignment.user.id,
                                "shift_date": isoDate,
                                "shift_type": selectedShift,
                                "hours":  String(format: "%.1f", 8.0)
                                //"status": false
                            ])
                            .execute()
                    } catch {
                        print("Failed to save shift for \(assignment.user.fullName) on \(isoDate): \(error)")
                    }
                }
            }

            // Reset
            assignedUser.removeAll()
            selectedUser = nil
            await fetchShiftAssignments()
        }
    }
    
    private func fetchShiftAssignments() async {
        do {
            let records: [ShiftRecord] = try await supabase
                .from("time_log")
                .select("*")
                .execute()
                .value
            
            // Group by userID
            shiftSchedule = Dictionary(grouping: records, by: { $0.userID.uuidString })
            print("Grouped Shift Schedule:", shiftSchedule)
            print("Scheduled Shift: \(shiftSchedule)")
            
        } catch {
            print("Failed to fetch shift assignments: \(error)")
        }
    }
    
    private func formattedDate(_ date: Date) -> String {
        let formatter = DateFormatter()
        formatter.dateStyle = .medium
        return formatter.string(from: date)
    }
    
//    private func saveShift() {
//        print("Saving shift for \(selectedDate) - \(selectedShift)")
//        for assignment in assignedUser {
//            print("User: \(assignment.user.fullName), Color: \(assignment.color)")
//            
//        }
//
//        // Reset form
//        selectedUser = nil
//        assignedUser.removeAll()
//    }
}


