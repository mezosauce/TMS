//
//  WelcomeView.swift
//  TMS
//
//  Created by Evan Heidenreich on 6/5/25.
//

import SwiftUI

struct WelcomeView: View {
    var body: some View {
        NavigationStack {
            VStack(spacing: 20) {
                Text("Welcome to Hour Base").font(.largeTitle)
                NavigationLink("Login", destination: LoginView())
                NavigationLink("Sign Up", destination: SignupView())
            }
            .padding()
        }
    }
}
