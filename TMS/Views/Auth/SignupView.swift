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
    @State private var positions = ["Employee", "Manager", "Admin"]
    @State private var email = ""
    @State private var password = ""
    @State private var position = ""
    @State private var first = ""
    @State private var last = ""
    
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
            
            Menu {
                ForEach(positions, id: \.self) { option in
                    Button(action: {
                        position = option
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
                .background(Color.gray.opacity(0.1))
                .cornerRadius(8)
            }
            //TextField("Position", text: $position)
              //  .textFieldStyle(RoundedBorderTextFieldStyle())
                //.autocapitalization(.none)
            
            TextField("First Name", text: $first)
                .textFieldStyle(RoundedBorderTextFieldStyle())
                .autocapitalization(.none)
            
            TextField("Last Name", text: $last)
                .textFieldStyle(RoundedBorderTextFieldStyle())
                .autocapitalization(.none)
            
            Button("Create Account") {
                Task { await authViewModel.signUp(Email: email, Password: password, Position: position, First: first, Last: last)}
            }
            .buttonStyle(.borderedProminent)
            
            NavigationLink("Already have an account? Login", destination: LoginView())
            
            if let error = authViewModel.errorMessage {
                Text(error).foregroundColor(.red)
            }
        }
    }
}

