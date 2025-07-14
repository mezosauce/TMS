//
//  EditAccountsView.swift
//  TMS
//
//  Created by Evan Heidenreich on 7/9/25.
//
import SwiftUI
import Foundation

struct EditAccountsView: View {
    @Binding var selectedPage: String
    @State private var position: String = ""
    @State private var positions = ["Employee", "Manager", "Admin"]
    @State private var selectedPosition: String = ""
    @State private var selectedUserId: String = ""
    @State private var users: [BasicUser] = []
    @State private var firstName: String = ""
    @State private var lastName: String = ""
    
    private let supabase = SupabaseManager.shared.supabase

    var body: some View {
        VStack(alignment: .leading, spacing: 20) {
            Button("<- Back") {
                selectedPage = "home"
            }
            .buttonStyle(.bordered)
            .tint(.purple)
            .padding()
            
            Text("Edit Accounts Page")
                .font(.headline)
                .padding()

            // Position Picker Dropdown
            Menu {
                ForEach(positions, id: \.self) { option in
                    Button(action: {
                        position = option
                        selectedPosition = option
                        Task {
                            await fetchUsersByPosition()
                        }
                    }) {
                        Text(option)
                    }
                }
            } label: {
                HStack {
                    Text(position.isEmpty ? "Select Position" : "Position: \(position)")
                        .foregroundColor(position.isEmpty ? .gray : .primary)
                    Spacer()
                    Image(systemName: "chevron.down")
                }
                .padding()
                .frame(maxWidth: .infinity)
                .background(Color.gray.opacity(0.1))
                .cornerRadius(8)
                .padding(.horizontal)
            }
            
            if !users.isEmpty {
                Text("Select User:")
                Menu {
                    ForEach(users, id: \.id) { user in
                        Button(action: {
                            selectedUserId = user.id
                            populateUserDetails()
                        }) {
                            Text("\(user.First) \(user.Last)")
                        }
                        
                    }
                }
                label: {
                    HStack {
                        Text(selectedUserId.isEmpty
                            ? "Select User"
                            : (users.first(where: { $0.id == selectedUserId })?.First ?? "") + " " + (users.first(where: { $0.id == selectedUserId })?.Last ?? ""))
                            .foregroundColor(selectedUserId.isEmpty ? .gray : .primary)
                        Spacer()
                        Image(systemName: "chevron.down")
                    }
                    .padding()
                    .frame(maxWidth: .infinity)
                    .background(Color.gray.opacity(0.1))
                    .cornerRadius(8)
                    .padding(.horizontal)
                }
            }
            
            if !selectedUserId.isEmpty {
                Text("First Name: ")
                TextField("First Name: ", text: $firstName)
                    .textFieldStyle(RoundedBorderTextFieldStyle())
                
                Text("Last Name: ")
                TextField("Last Name: ", text: $lastName)
                    .textFieldStyle(RoundedBorderTextFieldStyle())
                
                Button("Update Profile") {
                    Task { await updateUser() }
                }
                .buttonStyle(.borderedProminent)
                .padding(.top)
                
                Button("Delete User") {
                    Task {
                        await deleteUser()
                    }
                }
                .buttonStyle(.borderedProminent)
                .foregroundStyle(.red)
                .foregroundColor(.red)
                .padding(.top)
            }
        }
        .padding()
    }
    
    func fetchUsersByPosition() async {
        do {
            print("Fetching Position: \(selectedPosition)")
            let result: [BasicUser] = try await supabase
                .from("user_data")
                .select("id, First, Last, Position")
                .eq("Position", value:selectedPosition)
                .execute()
                .value
            print("First and Last Names: \(result.map { "\($0.First) \($0.Last)" })")
            self.users = result
            self.selectedUserId = ""
            self.firstName = ""
            self.lastName = ""
            
        } catch {
            print("Failed to fetch users: \(error)")
        }
    }
    
    func populateUserDetails() {
        if let selectedUser = users.first(where: { $0.id == selectedUserId }) {
            firstName = selectedUser.First
            lastName = selectedUser.Last
        }
    }
    
    func updateUser() async {
        do {
            try await supabase
                .from("user_data")
                .update(["First": firstName, "Last": lastName])
                .eq("id", value: selectedUserId)
                .execute()
            print("User updated successfully!")
            await fetchUsersByPosition()
        } catch {
            print("Failed to udapte user: \(error)")
        }
    }
    
    func deleteUser() async {
        guard let url = URL(string: "https://shdzcbiecsofkxhvscdn.functions.supabase.co/delete_user") else { return }
        guard let uuid = users.first(where: { $0.id == selectedUserId })?.id else { return }
        
        print("Selected User Id: \(selectedUserId)")
        print("🆔 Deleting user with UUID: \(uuid)")
        
        var request = URLRequest(url: url)
        request.httpMethod = "POST"
        let serviceToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InNoZHpjYmllY3NvZmt4aHZzY2RuIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc0ODk4MzgzOSwiZXhwIjoyMDY0NTU5ODM5fQ.nxEWL3YGS41_dPejNErdhj_vc_dtP-G1nKDMwHov9kw"
        request.addValue("Bearer \(serviceToken)", forHTTPHeaderField: "Authorization")
        request.addValue("application/json", forHTTPHeaderField: "Content-Type")
        
        let body = ["user_id": uuid]
        request.httpBody = try? JSONEncoder().encode(body)
        print("Body Request: \(body)")
        do {
            let (_, response) = try await URLSession.shared.data(for: request)
            if let httpResp = response as? HTTPURLResponse, httpResp.statusCode == 200 {
                print("✅ User deleted successfully.")
                // Refresh user list
                await fetchUsersByPosition()
            } else {
                print("❌ Failed to delete user. Response: \(response)")
            }
        } catch {
            print("❌ Error deleting user: \(error)")
        }
    }
}

struct BasicUser: Identifiable, Codable {
    var id: String
    var First: String
    var Last: String
    var Position: String
}
