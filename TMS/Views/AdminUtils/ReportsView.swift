//
//  ReportsView.swift
//  TMS
//
//  Created by Evan Heidenreich on 7/9/25.
//
import SwiftUI
import Foundation

struct ReportsView: View {
    @Binding var selectedPage: String
    
    var body: some View {
        VStack(alignment: .leading, spacing: 0) {
            HStack {
                Spacer()
                Text("Reports")
                    .font(.headline)
                    .padding()
                Spacer()
            }
            Button("<- Back"){
                selectedPage = "home"
            }
            .buttonStyle(.bordered)
            .tint(.purple)
            .padding()
        }
    }
}
