//
//  ShiftRecord.swift
//  TMS
//
//  Created by Evan Heidenreich on 7/17/25.
//
import Supabase
import Foundation

struct ShiftRecord: Identifiable, Decodable {
    let id = UUID()
    let userID: UUID
    let shiftDate: Date
    let shiftType: String
    
    enum CodingKeys: String, CodingKey {
        case userID = "id"
        case shiftDate = "shift_date"
        case shiftType = "shift_type"
    }
}
