//
//  AdminScheduleView.swift
//  TMS
//
//  Created by Evan Heidenreich on 7/31/25.
//

import SwiftUI
import Foundation
import Supabase

struct AdminShiftAssignment: Identifiable {
    let id = UUID()
    let user: scheduleUser
    let color: Color
}

public struct ScheduleView: View {
    private let supabase = SupabaseManager.shared.supabase
    
    @Binding var selectedPage: String
    
    @State private var selectedDate = Date()
    @State private var users: [scheduleUser] = []
    @State private var selectedUser: scheduleUser? = nil
    @State private var selectedRole = "Employee"
    @State private var selectedShift = "First Shift"
    
    @State private var assignedUsers: [AdminShiftAssignment] = []
    @State private var shiftSchedule: [String: [ShiftRecord]] = [:]
    
    private let shiftOptions: [String] = ["First Shift", "Second Shift", "Third Shift"]
    private let roleOptions: [String] = ["Employee", "Manager"]
    
    public var body: some View {
        NavigationView {
            ScrollView {
                VStack(spacing: 20) {
                    
                    // Back button
                    HStack {
                        Button(action: { selectedPage = "home" }) {
                            Label("Back", systemImage: "chevron.left")
                        }
                        .buttonStyle(.bordered)
                        .tint(.purple)
                        Spacer()
                    }
                    
                    Text("Admin Shift Scheduler")
                        .font(.largeTitle)
                        .fontWeight(.bold)
                        .padding(.bottom, 10)
                    
                    // Role Selection Card
                    cardView(title: "Select Role") {
                        Picker("", selection: $selectedRole) {
                            ForEach(roleOptions, id: \.self) { role in
                                Text(role)
                            }
                        }
                        .pickerStyle(SegmentedPickerStyle())
                        .onChange(of: selectedRole) { _ in
                            Task { await fetchUsers() }
                        }
                    }
                    
                    // Shift Selection Card
                    cardView(title: "Select Shift") {
                        Picker("", selection: $selectedShift) {
                            ForEach(shiftOptions, id: \.self) { shift in
                                Text(shift)
                            }
                        }
                        .pickerStyle(MenuPickerStyle())
                        .frame(maxWidth: .infinity)
                    }
                    
                    // User Selection Card
                    cardView(title: "Assign User") {
                        Picker("Choose \(selectedRole)", selection: $selectedUser) {
                            Text("Select a \(selectedRole)").tag(Optional<scheduleUser>.none)
                            ForEach(users, id: \.self) { user in
                                Text(user.fullName).tag(Optional(user))
                            }
                        }
                        .pickerStyle(MenuPickerStyle())
                        
                        Button(action: {
                            if let user = selectedUser,
                               !assignedUsers.contains(where: { $0.user.id == user.id }) {
                                assignedUsers.append(AdminShiftAssignment(user: user, color: randomColor()))
                                selectedUser = nil
                            }
                        }) {
                            Label("Add to Shift", systemImage: "plus.circle.fill")
                                .frame(maxWidth: .infinity)
                        }
                        .buttonStyle(.borderedProminent)
                        .tint(.blue)
                        .disabled(selectedUser == nil)
                    }
                    
                    // Assigned Users Card
                    if !assignedUsers.isEmpty {
                        cardView(title: "Assigned \(selectedRole)s (Mon–Fri)") {
                            VStack(alignment: .leading, spacing: 8) {
                                ForEach(assignedUsers) { assignment in
                                    VStack(alignment: .leading) {
                                        HStack {
                                            Circle()
                                                .fill(assignment.color)
                                                .frame(width: 12, height: 12)
                                            
                                            Text(assignment.user.fullName)
                                                .fontWeight(.medium)
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
                                    Divider()
                                }
                            }
                        }
                    }
                    
                    // Save Button
                    Button(action: saveShiftsForWeek) {
                        Label("Save Shifts", systemImage: "tray.and.arrow.down.fill")
                            .frame(maxWidth: .infinity)
                            .padding()
                    }
                    .buttonStyle(.borderedProminent)
                    .tint(.green)
                    .cornerRadius(12)
                }
                .padding()
            }
            .navigationBarHidden(true)
            .onAppear {
                Task {
                    await fetchUsers()
                    await fetchShiftAssignments()
                }
            }
        }
    }
    
    // MARK: - Reusable Card View
    private func cardView<Content: View>(title: String, @ViewBuilder content: () -> Content) -> some View {
        VStack(alignment: .leading, spacing: 10) {
            Text(title)
                .font(.headline)
            content()
        }
        .padding()
        .frame(maxWidth: .infinity)
        .background(Color(.systemGray6))
        .cornerRadius(12)
        .shadow(radius: 2)
    }
    
    // MARK: - Logic
    private func randomColor() -> Color {
        let colors: [Color] = [.red, .green, .blue, .yellow, .orange, .purple, .pink, .cyan, .indigo, .teal]
        return colors.randomElement() ?? .gray
    }
    
    private func fetchUsers() async {
        do {
            let fetchedUsers: [scheduleUser] = try await supabase
                .from("user_data")
                .select("*")
                .eq("Position", value: selectedRole)
                .execute()
                .value
            
            users = fetchedUsers
        } catch {
            print("Fetch \(selectedRole.lowercased())s failed: \(error)")
        }
    }
    
    private func saveShiftsForWeek() {
        let calendar = Calendar.current
        let today = Date()
        
        guard let nextMonday = calendar.nextDate(after: today, matching: DateComponents(weekday: 2), matchingPolicy: .nextTime) else {
            print("Could not find next Monday")
            return
        }
        
        Task {
            for i in 0..<5 {
                let date = calendar.date(byAdding: .day, value: i, to: nextMonday)!
                let isoDate = ISO8601DateFormatter().string(from: date)
                
                for assignment in assignedUsers {
                    do {
                        try await supabase
                            .from("time_log")
                            .insert([
                                "id": assignment.user.id,
                                "shift_date": isoDate,
                                "shift_type": selectedShift,
                                "hours": String(format: "%.1f", 8.0)
                            ])
                            .execute()
                    } catch {
                        print("Failed to save shift for \(assignment.user.fullName) on \(isoDate): \(error)")
                    }
                }
            }
            
            assignedUsers.removeAll()
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
            
            shiftSchedule = Dictionary(grouping: records, by: { $0.userID.uuidString })
        } catch {
            print("Failed to fetch shift assignments: \(error)")
        }
    }
    
    private func formattedDate(_ date: Date) -> String {
        let formatter = DateFormatter()
        formatter.dateStyle = .medium
        return formatter.string(from: date)
    }
}
