//
//  SignupView.swift
//  TMS
//
//  Created by Evan Heidenreich on 6/5/25.
//
import SwiftUI
import Foundation
import Supabase

struct SignupView: View {
    @StateObject private var authViewModel = AuthViewModels()
    @State private var email = ""
    @State private var password = ""
    
    var body: some View {
        VStack {
            Text("Sign Up")
                .font(.largeTitle)
                .bold()
            
            TextField("Email", text: $email)
                .textFieldStyle(RoundedBorderTextFieldStyle())
                .autocapitalization(.none)
            
            SecureField("Password", text: $password)
                .textFieldStyle(RoundedBorderTextFieldStyle())
                .autocapitalization(.none)
            
            Button("Create Account") {
                Task { await authViewModel.signUp(email: email, password: password)}
            }
            .buttonStyle(.borderedProminent)
            
            NavigationLink("Already have an account? Login", destination: LoginView())
            
            if let error = authViewModel.errorMessage {
                Text(error).foregroundColor(.red)
            }
        }
    }
}

