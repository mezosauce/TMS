//
//  ContentView.swift
//  TMS
//
//  Created by Evan Heidenreich on 6/1/25.
//

import SwiftUI
import SwiftData
import Supabase

struct ContentView: View {
    @Environment(\.modelContext) private var modelContext
    @Query private var items: [Item]
    
    @StateObject private var authViewModel = AuthViewModels()
    
    var body: some View {
        NavigationView {
            if authViewModel.isAuthenticated {
                EmployeeHomeView()
                    .environmentObject(authViewModel)
            } else {
                LoginView()
                    .environmentObject(authViewModel)
            }
        }
        .id(authViewModel.isAuthenticated)
        .onAppear {
            Task {
                await authViewModel.checkSession()
            }
        }
    }
}
