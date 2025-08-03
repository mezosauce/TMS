//
//  ManagerEditAccountsView.swift
//  TMS
//
//  Created by Evan Heidenreich on 7/30/25.
//
import SwiftUI
import Foundation
import Supabase

struct ManagerEditAccountsView: View {
    @Binding var selectedPage: String
    @State private var position: String = ""
    @State private var positions = ["Employee", "Manager", "Admin"]
    @State private var selectedPosition: String = ""
    @State private var selectedUserId: String = ""
    @State private var users: [editBasicUser] = []
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
            let result: [editBasicUser] = try await supabase
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
        guard let uuid = users.first(where: { $0.id == selectedUserId })?.id else { return }
        guard let url = URL(string: "https://shdzcbiecsofkxhvscdn.functions.supabase.co/swift-user-delete") else { return }
        
        var request = URLRequest(url: url)
        request.httpMethod = "POST"
        request.addValue("application/json", forHTTPHeaderField: "Content-Type")
        
        let serviceRoleKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InNoZHpjYmllY3NvZmt4aHZzY2RuIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc0ODk4MzgzOSwiZXhwIjoyMDY0NTU5ODM5fQ.nxEWL3YGS41_dPejNErdhj_vc_dtP-G1nKDMwHov9kw"
        request.addValue("Bearer \(serviceRoleKey)", forHTTPHeaderField: "Authorization")
        
        let body = ["user_id": uuid]
        request.httpBody = try? JSONEncoder().encode(body)
        
        do {
            let (data, response) = try await URLSession.shared.data(for: request)
            
            if let httpResp = response as? HTTPURLResponse {
                print("HTTP Status Code: \(httpResp.statusCode)")
                
                if httpResp.statusCode != 200 {
                    // Print raw error message
                    if let errorMessage = String(data: data, encoding: .utf8) {
                        print("Server error response: \(errorMessage)")
                    }
                    return // Don’t try to decode if not JSON
                }
            }
            
            // Only decode if the status was 200
            let decodedResponse = try JSONDecoder().decode(DeleteResponse.self, from: data)
            print("Server response success: \(decodedResponse.success)")
            await fetchUsersByPosition()
            
        } catch {
            print("Error during request or decoding: \(error)")
        }
    }
}

struct DeleteResponses: Decodable {
    let success: Bool
    //let response: String
}

struct editBasicUser: Identifiable, Codable {
    var id: String
    var First: String
    var Last: String
    var Position: String
}
