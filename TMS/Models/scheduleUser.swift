//
//  scheduleUser.swift
//  TMS
//
//  Created by Evan Heidenreich on 7/12/25.
//
import Supabase
import Foundation

struct scheduleUser: Identifiable, Codable, Hashable {
    var id: String
    var First: String
    var Last: String
    var Position: String
    
    var fullName: String {
        "\(First) \(Last)"
    }
}
