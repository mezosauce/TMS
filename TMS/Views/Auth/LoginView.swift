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
    @EnvironmentObject var authViewModel: AuthViewModels
    @State private var email = ""
    @State private var password = ""
    
    var body: some View {
        ZStack {
            LinearGradient(
                gradient: Gradient(colors: [Color.black, Color.purple]),
                startPoint: .bottom,
                endPoint: .top
            )
            .ignoresSafeArea()
            
            VStack {
                Text("Login")
                    .font(.largeTitle)
                    .bold()
                    .foregroundColor(.white)  // Make text visible on dark bg
                
                TextField("Email", text: $email)
                    .textFieldStyle(RoundedBorderTextFieldStyle())
                    .autocapitalization(.none)
                    .padding(.vertical, 8)
                
                SecureField("Password", text: $password)
                    .textFieldStyle(RoundedBorderTextFieldStyle())
                    .autocapitalization(.none)
                    .padding(.vertical, 8)
                
                Button("Login") {
                    Task {
                        await authViewModel.login(email: email, password: password)
                    }
                }
                .buttonStyle(.borderedProminent)
                .padding(.vertical)
                
                NavigationLink("Don't have an account? Sign Up", destination: SignupView())
                    .foregroundColor(.white) // visible link
                
                if let error = authViewModel.errorMessage {
                    Text(error)
                        .foregroundColor(.red)
                        .padding(.top)
                }
            }
            .padding(.horizontal, 40)
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
    }
}

