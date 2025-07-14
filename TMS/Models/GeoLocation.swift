//
//  GeoLocation.swift
//  TMS
//
//  Created by Evan Heidenreich on 7/10/25.
//
import Supabase
import Foundation

struct GeoLocation: Codable {
    let id: Int64
    let name: String
    let latitude: String
    let longitude: String
}
