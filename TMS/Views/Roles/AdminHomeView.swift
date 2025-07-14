//
//  AdminHomeView.swift
//  TMS
//
//  Created by Evan Heidenreich on 6/5/25.
//

import Foundation
import SwiftUI

struct AdminHomeView: View {
    @EnvironmentObject var authViewModel: AuthViewModels
    @State private var showMenu: Bool = false
    @State private var selectedPage: String = "home"
    
    var body: some View {
        ZStack(alignment: .leading) {
            NavigationView {
                VStack {
                    if selectedPage == "home" {
                        Text("Admin Dashboard")
                            .font(.title)
                    } else if selectedPage == "schedule" {
                        ScheduleView(selectedPage: $selectedPage)
                    } else if selectedPage == "reports" {
                        ReportsView(selectedPage: $selectedPage)
                    } else if selectedPage == "edits" {
                        EditAccountsView(selectedPage: $selectedPage)
                    } else if selectedPage == "geo" {
                        GeoLocationView(selectedPage: $selectedPage)
                    }
                    Spacer()
                }
                .navigationBarItems(leading: Button(action: {
                    withAnimation {
                        showMenu.toggle()
                    }
                }) {
                    Image(systemName: "line.3.horizontal")
                        .imageScale(.large)
                })
                .navigationBarTitle("Home", displayMode: .inline)
            }
            if showMenu {
                Color.black.opacity(0.3)
                    .ignoresSafeArea()
                    .onTapGesture {
                        withAnimation {
                            showMenu = false
                        }
                    }
                    .zIndex(1)  // Behind the SideMenuView but in front of the main content
            }
            
            if showMenu {
                AdminSideMenuView(selectedPage: $selectedPage, showMenu: $showMenu)
                    .frame(width: 200)
                    .transition(.move(edge: .leading))
                    .zIndex(2)
            }
        }
        
        .background(
            Color.black.opacity(showMenu ? 0.5 : 0)
                .edgesIgnoringSafeArea(.all)
                .onTapGesture {
                    withAnimation {
                        showMenu.toggle()
                    }
                }
        )
    }
}

struct AdminSideMenuView: View {
    @EnvironmentObject var authViewModel: AuthViewModels
    @Binding var selectedPage: String
    @Binding var showMenu: Bool
    
    var body: some View {
        
        VStack(alignment: .leading, spacing: 20) {
            Button("Home") {
                selectedPage = "home"
                showMenu = false
            }
            .buttonStyle(.bordered)
            .tint(.purple)
            Button("Schedule") {
                selectedPage = "schedule"
                showMenu = false
            }
            .buttonStyle(.bordered)
            .tint(.purple)
            Button("Reports") {
                selectedPage = "reports"
                showMenu = false
            }
            .buttonStyle(.bordered)
            .tint(.purple)
            Button("Edit Accounts") {
                selectedPage = "edits"
                showMenu = false
            }
            .buttonStyle(.bordered)
            .tint(.purple)
            Button("Geo-Locations"){
                selectedPage = "geo"
                showMenu = false
            }
                .buttonStyle(.bordered)
                .tint(.purple)
            Spacer()
            
            Button("Logout") {
                withAnimation {
                    showMenu = false
                }
                authViewModel.signOut()
            }
            .buttonStyle(.borderedProminent)
            .tint(.red)
            
        }
        .padding(.top, 100)
        .padding(.horizontal, 20)
        .frame(maxWidth: .infinity, alignment: .leading)
        .background(Color(.systemGray6))
        .edgesIgnoringSafeArea(.all)
    }
}
 
