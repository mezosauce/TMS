//
//  ScheduleView.swift
//  TMS
//
//  Created by Evan Heidenreich on 7/9/25.
//
import SwiftUI

struct ScheduleView: View {
    @Binding var selectedPage: String
    @State private var selectedDate: Date = Date()
    var body: some View {
        VStack(alignment: .leading, spacing: 0) {
            HStack {
                Spacer()
                Text("Schedules")
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
