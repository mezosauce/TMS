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
    @Published var errorMessage: String?
    
    private let supabase = SupabaseManager.shared.supabase
    
    init(){
        Task {
            await checkSession()
        }
    }
    
    // Login Function
    func login(email: String, password: String) async {
        do {
            let session = try await supabase.auth.signIn(email: email, password: password)
            user = session.user
            isAuthenticated = true
        } catch {
            errorMessage = "Login failed: \(error.localizedDescription)"
        }
    }
    // signUp Function
    func signUp(email: String, password: String) async {
        do {
            let session = try await supabase.auth.signUp(email: email, password: password)
            user = session.user
            isAuthenticated = true
            
            let newUser = AppUser(id: session.user.id, email: email, first_name: "Evan", last_name: "Heidenreich", role: "user")
            try await supabase
                .from("users")
                .insert(newUser)
                .execute()
            
        } catch {
            errorMessage = "Signup failed: \(error.localizedDescription)"
        }
    }
    // signOut Function
    func signOut() async {
        do {
            try await supabase.auth.signOut()
            user = nil
            isAuthenticated = false
        } catch {
            errorMessage = "Logout failed: \(error.localizedDescription)"
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
