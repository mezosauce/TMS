//
//  AuthViewModels.swift
//  TMS
//
//  Created by Evan Heidenreich on 6/5/25.
//
import Supabase
import Foundation

@MainActor
class AuthViewModels: ObservableObject {
    @Published var user: User?
    @Published var isAuthenticated: Bool = false
    @Published var userRole: String?
    @Published var errorMessage: String?
    
    private let supabase = SupabaseManager.shared.supabase
    
    init(){
        Task {
            await checkSession()
        }
    }
    
    struct UserRole: Decodable {
        let Position: String
    }
    
    // Login Function
    func login(email: String, password: String) async {
        do {
            print("Start Login")
            let session = try await supabase.auth.signIn(email: email, password: password)
            print("Login Successful \(session.user)")
            
            user = session.user
            isAuthenticated = true
            
            let roles: [UserRole] = try await supabase
                .from("user_data")
                .select("Position")
                .eq("id", value: session.user.id.uuidString)
                .execute()
                .value
            
            if let role = roles.first?.Position {
                userRole = role
                print("Fetched role: \(role)")
            } else {
                print("No role found for user.")
            }
            
        } catch {
            errorMessage = "Login failed: \(error.localizedDescription)"
            isAuthenticated = false
        }
    }
    // signUp Function
    func signUp(Email: String, Password: String, Position: String, First: String, Last: String) async {
        do {
            print("Start Signup")
            let session = try await supabase.auth.signUp(email: Email, password: Password)
            let user = session.user
            
            self.user = user
            isAuthenticated = true
            print("Signup Successful \(user)")
            
            let newUser = AppUser(id: user.id.uuidString, email: Email, Password: Password, Position: Position, First: First, Last: Last)
            print("user_data \(newUser)")
            try await supabase
                .from("user_data")
                .insert([
                    "id": newUser.id,
                    "First": newUser.First,
                    "Last": newUser.Last,
                    "Position": newUser.Position,
                ])  
                .execute()
            print("Success Inserting Table")
            
        } catch {
            errorMessage = "Signup failed: \(error.localizedDescription)"
        }
    }
    // signOut Function
    func signOut() {
        Task {
            do {
                try await supabase.auth.signOut()
                user = nil
                isAuthenticated = false
            } catch {
                errorMessage = "Logout failed: \(error.localizedDescription)"
            }
        }
    }
    // checkSession function checks if the user is authenticated within the active session.
    func checkSession() async {
        if supabase.auth.currentSession != nil {
            do {
                try await supabase.auth.refreshSession()
                user = supabase.auth.currentSession?.user
                isAuthenticated = true
            } catch {
                errorMessage = "Session exists, but refresh failed: \(error.localizedDescription)"
                isAuthenticated = false
            }
        } else {
            user = nil
            isAuthenticated = false
            errorMessage = nil
        }
    }
}
