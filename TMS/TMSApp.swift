//
//  TMSApp.swift
//  TMS
//
//  Created by Evan Heidenreich on 6/1/25.
//
import Supabase
import SwiftUI
import SwiftData

@main
struct TMSApp: App {
    var sharedModelContainer: ModelContainer = {
        let schema = Schema([
            Item.self,
        ])
        let modelConfiguration = ModelConfiguration(schema: schema, isStoredInMemoryOnly: false)

        do {
            return try ModelContainer(for: schema, configurations: [modelConfiguration])
        } catch {
            fatalError("Could not create ModelContainer: \(error)")
        }
    }()
    
    @StateObject var auth = AuthViewModels()
    
    var body: some Scene {
        WindowGroup {
            WelcomeView()
                .environmentObject(auth)
        }
    }
/*
    @State private var isLoggedIn = false
    var body: some Scene {
        WindowGroup {
            if isLoggedIn {
                ContentView()
            }else{
                LoginView(isLoggedIn: $isLoggedIn)
            }
        }
        .modelContainer(sharedModelContainer)
    }*/
}

