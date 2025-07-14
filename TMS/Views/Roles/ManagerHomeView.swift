//
//  ManagerHomeView.swift
//  TMS
//
//  Created by Evan Heidenreich on 6/5/25.
//

import Foundation
import SwiftUI

struct ManagerHomeView: View {
    
    @EnvironmentObject var authViewModel: AuthViewModels
    @State private var showMenu: Bool = false
    @State private var selectedPage: String = "home"
    
    var body: some View {
        ZStack(alignment: .leading) {
            NavigationView {
                VStack {
                    if selectedPage == "home" {
                        Text("Manager Dashboard")
                            .font(.title)
                    } else if selectedPage == "schedules" {
                        ManagerScheduleView(selectedPage: $selectedPage)
                        
                    } else if selectedPage == "reports" {
                        
                    } else if selectedPage == "editEmployees" {
                        
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
                ManagerSideMenuView(selectedPage: $selectedPage, showMenu: $showMenu)
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

struct ManagerSideMenuView: View {
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
            Button("Schedules") {
                selectedPage = "schedules"
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
            Button("Edit Employees") {
                selectedPage = "editEmployees"
                showMenu = false
            }
            .buttonStyle(.bordered)
            .tint(.purple)
            Spacer()
            
        }
        .padding(.top, 100)
        .padding(.horizontal, 20)
        .frame(maxWidth: .infinity, alignment: .leading)
        .background(Color(.systemGray6))
        .edgesIgnoringSafeArea(.all)
    }
}
