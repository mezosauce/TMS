//
//  GeoLocationView.swift
//  TMS
//
//  Created by Evan Heidenreich on 7/9/25.
//
import SwiftUI
import Foundation
import Supabase

struct GeoLocationView: View {
    private let supabase = SupabaseManager.shared.supabase
    @Binding var selectedPage: String
    
    @State private var name = ""
    @State private var latitude = ""
    @State private var longitude = ""
    @State private var locations: [GeoLocation] = []
    @State private var isAdmin = false
    
    var body: some View {
        VStack {
            Button("<- Back"){
                selectedPage = "home"
            }
            .buttonStyle(.bordered)
            .tint(.purple)
            .padding()
            Text("Geo Location Editor")
                .font(.headline)
                .padding()
            
            List {
                ForEach(locations, id: \.id) { location in
                    HStack {
                        VStack(alignment: .leading) {
                            Text(location.name).bold()
                            Text("Latitude: \(location.latitude), Longitude: \(location.longitude)")
                                .font(.caption)
                        }
                        Spacer()
                        if isAdmin {
                            Button("Delete") {
                                deleteLocation(id: location.id)
                            }
                            .foregroundColor(.red)
                        }
                    }
                }
            }
            
            if isAdmin {
                Divider()
                
                Text("Add New Location").font(.headline)
                TextField("Location Name", text: $name)
                    .textFieldStyle(RoundedBorderTextFieldStyle())
                TextField("Latitude", text: $latitude)
                    .textFieldStyle(RoundedBorderTextFieldStyle())
                TextField("Longitude", text: $longitude)
                    .textFieldStyle(RoundedBorderTextFieldStyle())
                
                Button("Save Location") {
                    addLocation()
                }
                .buttonStyle(.borderedProminent)
                .padding(.top)
            }
            Spacer()
        }
        .padding()
        .onAppear {
            Task{
                await checkAdminStatus()
                await fetchLocations()
            }
        }
    }
    
    func fetchLocations() async {
        do {
            let results: [GeoLocation] = try await supabase
                .from("geolocating")
                .select()
                .execute()
                .value
            print("Fetched locations: \(results)")
            self.locations = results
        } catch {
            print("Error fetching locations: \(error)")
        }
    }
    
    func addLocation() {
        Task {
            do {
                let newLocation = [
                    "longitude": longitude,
                    "latitude": latitude,
                    "name": name
                ]
                
                try await supabase
                    .from("geolocating")
                    .insert([newLocation])
                    .execute()
                
                print("Successfully inserted location")
                
                // Clear form and refresh
                await fetchLocations()
                name = ""
                latitude = ""
                longitude = ""
                
            } catch {
                print("Error adding location: \(error.localizedDescription)")
            }
        }
    }
    
    func deleteLocation(id: Int64) {
        Task {
            do {
                let response = try await supabase
                    .from("geolocating")
                    .delete()
                    .eq("id", value: Int(id))
                    .execute()
                
                
                print("✅ Supabase delete response: \(response)")
                await fetchLocations()
            } catch {
                print("❌ Error deleting location: \(error.localizedDescription)")
            }
        }
    }
    
    func checkAdminStatus() async {
        do {
            let session = try await supabase.auth.session
            let userID = session.user.id.uuidString
            
            struct UserRole: Decodable {
                let Position: String
            }
            
            let roles: [UserRole] = try await supabase
                .from("user_data")
                .select("Position")
                .eq("id", value: session.user.id.uuidString)
                .execute()
                .value
            
            print("User Meta Data: \(session.user.userMetadata)")
            if let role = roles.first?.Position {
                isAdmin = (role.lowercased() == "admin")
                print("Role from user_data: \(role) → isAdmin: \(isAdmin)")
            } else {
                print("No matching user_data entry found for user ID.")
            }
        } catch {
            print("Error checking admin status: \(error.localizedDescription)")
        }
    }
}


    
