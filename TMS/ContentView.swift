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
        if authViewModel.isAuthenticated {
            switch authViewModel.userRole {
            case "Employee":
                EmployeeHomeView()
                    .environmentObject(authViewModel)
            case "Manager":
                ManagerHomeView()
                    .environmentObject(authViewModel)
            case "Admin":
                AdminHomeView()
                    .environmentObject(authViewModel)
            default:
                Text("Unknown Role")
            }
        } else {
            NavigationView {
                LoginView()
                    .environmentObject(authViewModel)
            }
        }
    }
}
