//
//  AppUser.swift
//  TMS
//
//  Created by Evan Heidenreich on 6/5/25.
//

import Supabase
import Foundation

struct AppUser: Codable {
    var id: UUID
    var email: String
    var first_name: String
    var last_name: String
    var role: String
    
}

