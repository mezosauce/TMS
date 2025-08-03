//
//  EmployeeHomeView.swift
//  TMS
//
//  Created by Evan Heidenreich on 6/5/25.
//

import Foundation
import SwiftUI
import Supabase
import CoreLocation

struct TimeLogUpdate: Encodable {
    var clock_in: Date?
    var clock_out: Date?
    var status: Bool
}

struct EmployeeHomeView: View {
    @EnvironmentObject var authViewModel: AuthViewModels
    private let supabase = SupabaseManager.shared.supabase
    @State private var showMenu: Bool = false
    @State private var selectedPage: String = "home"
    @State private var isClockedIn = false
    @State private var currentShift: TimeLog? = nil
    @State private var currentTime = Date()
    @State private var isLoadingShift = true
    
    var body: some View {
        ZStack(alignment: .leading) {
            NavigationView {
                VStack {
                    if selectedPage == "home" {
                        VStack(spacing: 20) {
                            Text("Employee Dashboard")
                                .font(.title)

                            Text("Current Time: \(currentTime, formatter: timeFormatter)")
                                .font(.headline)

                            if isLoadingShift {
                                ProgressView("Loading your shift...")
                            } else if let shift = currentShift {
                                Text("Your Shift: \(shift.shiftType) on \(shift.shiftDateParsed?.formatted(date: .abbreviated, time: .shortened) ?? "Invalid Date")")

                                
                                Text(isClockedIn ? "Status: Clocked In" : "Status: Clocked Out")
                                    .foregroundColor(isClockedIn ? .green : .red)

                                Button(action: {
                                    Task {
                                        await handleClockInOut()
                                    }
                                }) {
                                    Text(isClockedIn ? "Clock Out" : "Clock In")
                                        .padding()
                                        .frame(maxWidth: .infinity)
                                        .background(isClockedIn ? Color.red : Color.green)
                                        .foregroundColor(.white)
                                        .cornerRadius(10)
                                }
                            } else {
                                Text("No upcoming shifts.")
                            }
                        }
                        .padding()
                    } else if selectedPage == "schedule" {
                        EmployeeScheduleView()
                    } else if selectedPage == "reports" {
                        EmployeeReportsView(selectedPage: $selectedPage)
                    } else if selectedPage == "timeoff" {
                        
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
                
                .onAppear {
                    LoadTodayOrUpcomingShift()
                    Timer.scheduledTimer(withTimeInterval: 1.0, repeats: true) { _ in
                        currentTime = Date()
                    }
                }
            }
            if showMenu {
                Color.black.opacity(0.3)
                    .ignoresSafeArea()
                    .onTapGesture {
                        withAnimation {
                            showMenu = false
                        }
                    }
                    .zIndex(1)
            }
            
            if showMenu {
                SideMenuView(selectedPage: $selectedPage, showMenu: $showMenu)
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
    
    func LoadTodayOrUpcomingShift() {
        guard let userId = authViewModel.user?.id else { return }
        
        let today = ISO8601DateFormatter().string(from: Date())
        
        Task{
            do {
                let result: [TimeLog] = try await supabase
                    .from("time_log")
                    .select("*")
                    .eq("id", value: userId.uuidString)
                    .gte("shift_date", value: today)
                    .order("shift_date", ascending: true)
                    .limit(1)
                    .execute()
                    .value
                print(result)
                
                if let shift = result.first {
                    currentShift = shift
                    isClockedIn = shift.status ?? false
                }
                isLoadingShift = false
            } catch {
                print("Error loading shift: \(error)")
            }
        }
    }
    
    func handleClockInOut() async {
        guard let userId = authViewModel.user?.id else { return }
        guard var shift = currentShift else { return }

        do {
            let userLocation = try await getCurrentLocation()
            print("User Location: \(userLocation.coordinate.latitude), \(userLocation.coordinate.longitude)")
            let allowedLocations = try await fetchGeoLocations()

            let isAllowed = isWithinRange(
                userLat: userLocation.coordinate.latitude,
                userLon: userLocation.coordinate.longitude,
                allowedLocations: allowedLocations
            )

            guard isAllowed else {
                print("User is not within allowed geofence range.")
                return
            }

            let now = Date()
            let updatePayload = isClockedIn
                ? TimeLogUpdate(clock_in: nil, clock_out: now, status: false)
                : TimeLogUpdate(clock_in: now, clock_out: nil, status: true)

            try await supabase
                .from("time_log")
                .update(updatePayload)
                .eq("id", value: userId.uuidString)
                .eq("tid", value: String(shift.tid))
                .execute()

            isClockedIn.toggle()
            if isClockedIn {
                shift.clock_in = now
            } else {
                shift.clock_out = now
            }
            shift.status = isClockedIn
            currentShift = shift

        } catch {
            print("Clock-in/Clock-out Failed: \(error.localizedDescription)")
        }
    }
    
    private let timeFormatter: DateFormatter = {
        let formatter = DateFormatter()
        formatter.timeStyle = .medium
        formatter.dateStyle = .none
        return formatter
    }()
    
    func getCurrentLocation() async throws -> CLLocation {
        let locationManager = AsyncLocationManager()
        return try await locationManager.requestLocation()
    }
    
    func fetchGeoLocations() async throws -> [GeoLocation] {
        return try await supabase.from("geolocating").select("*").execute().value
    }
    
    func haversineDistance(lat1: Double, lon1: Double, lat2: Double, lon2: Double) -> Double {
        let earthRadius = 6371000.0 // meters
    
        let dLat = degreesToRadians(lat2 - lat1)
        let dLon = degreesToRadians(lon2 - lon1)

        let radLat1 = degreesToRadians(lat1)
        let radLat2 = degreesToRadians(lat2)

        let a = sin(dLat / 2) * sin(dLat / 2) +
                cos(radLat1) * cos(radLat2) *
                sin(dLon / 2) * sin(dLon / 2)

        let c = 2 * atan2(sqrt(a), sqrt(1 - a))

        return earthRadius * c
        
    }
    
    func degreesToRadians(_ degrees: Double) -> Double {
        return degrees * .pi / 180
    }
    
    // macDistanceMeters = 100 meters can change appropriately 5 miles = 8046.72
    func isWithinRange(userLat: Double, userLon: Double, allowedLocations: [GeoLocation], maxDistanceMeters: Double = 250) -> Bool {
        for location in allowedLocations {
            guard
                let locationLat = Double(location.latitude),
                let locationLon = Double(location.longitude)
            else {
                print("Invalid location data")
                continue
            }
            
            let distance = haversineDistance(lat1: userLat, lon1: userLon, lat2: locationLat, lon2: locationLon)
            
            print("Comparing user location: (\(userLat), \(userLon))")
            print("With allowed location: (\(locationLat), \(locationLon))")
            print("Calculated distance: \(distance) meters (allowed: \(maxDistanceMeters))")
            
            if distance <= maxDistanceMeters {
                return true
            }
        }
        return false
    }
}

struct SideMenuView: View {
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
            Button("View Schedule") {
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
            Button("Request Time Off") {
                selectedPage = "timeoff"
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

extension Date {
    var iso8601String: String {
        ISO8601DateFormatter().string(from: self)
    }
}	
