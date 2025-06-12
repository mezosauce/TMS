//
//  LoginView.swift
//  TMS
//
//  Created by Evan Heidenreich on 6/5/25.
//
import SwiftUI
import Foundation
import Supabase


struct LoginView: View {
    @StateObject private var authViewModel = AuthViewModels()
    @State private var email = ""
    @State private var password = ""
    
    var body: some View {
        VStack {
            Text("Login").font(.largeTitle)
            
            TextField("Email", text: $email)
                .textFieldStyle(RoundedBorderTextFieldStyle())
                .autocapitalization(.none)
            
            SecureField("Password", text: $password)
                .textFieldStyle(RoundedBorderTextFieldStyle())
                .autocapitalization(.none)
            
            Button("Login") {
                Task { await authViewModel.login(email: email, password: password)}
            }
            .buttonStyle(.borderedProminent)
            
            NavigationLink("Dont have an account? Sign Up", destination: SignupView())
            
            if let error = authViewModel.errorMessage {
                Text(error).foregroundColor(.red)
            }
        }
    }
}

