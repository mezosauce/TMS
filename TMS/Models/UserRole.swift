//
//  UserRole.swift
//  TMS
//
//  Created by Evan Heidenreich on 6/5/25.
//
import Supabase
import Foundation

enum UserRole: String, CaseIterable, Identifiable, Codable {
    case employee = "Employee"
    case manager = "Manager"
    case admin = "Admin"
    
    var id: String {self.rawValue}
}
