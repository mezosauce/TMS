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
    
    public var body: some View {
        NavigationView {
            VStack(alignment: .leading, spacing: 10) {
                DatePicker("Select Date", selection: $selectedDate, displayedComponents: .date)
                Picker("Select Shift", selection: $selectedShift) {
                    ForEach(shiftOptions, id: \.self) { shift in
                        Text(shift)
                    }
                }
                .pickerStyle(SegmentedPickerStyle())
                
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
                    Text("Assigned Users:")
                        .font(.headline)
                    
                    ForEach(assignedUser) { assignment in
                        HStack {
                            Circle()
                                .fill(assignment.color)
                                .frame(width: 20, height: 20)
                            
                            Text(assignment.user.fullName)
                        }
                    }
                }
                Spacer()
                
                Button(action: saveShift) {
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
            print("❌ Fetch employees failed: \(error)")
        }
    }
    
    private func saveShift() {
        print("Saving shift for \(selectedDate) - \(selectedShift)")
        for assignment in assignedUser {
            print("User: \(assignment.user.fullName), Color: \(assignment.color)")
            
        }

        // Reset form
        selectedUser = nil
        assignedUser.removeAll()
    }
}

