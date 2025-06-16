//
//  AppUser.swift
//  TMS
//
//  Created by Evan Heidenreich on 6/5/25.
//

import Supabase
import Foundation

struct AppUser: Codable {
    var id: String
    var email: String
    var Password: String
    var Position: String
    var First: String
    var Last: String
    
}

