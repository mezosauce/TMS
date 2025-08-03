//
//  TimeLog.swift
//  TMS
//
//  Created by Evan Heidenreich on 7/23/25.
//
import Foundation
import Supabase

struct TimeLog: Decodable, Identifiable {
    var id: Int64 { tid }  // Conforming to Identifiable
    let tid: Int64
    let userId: UUID
    let shiftDate: String  // Note: Supabase returns plain "yyyy-MM-dd"
    let shiftType: String
    let shift: Date?
    var status: Bool
    var clock_in: Date?
    var clock_out: Date?
    let hoursWorked: Double?

    enum CodingKeys: String, CodingKey {
        case tid
        case userId = "id"
        case shiftDate = "shift_date"
        case shiftType = "shift_type"
        case shift
        case status
        case clock_in
        case clock_out
        case hoursWorked = "hours_worked"
        
    }
    
    var shiftDateParsed: Date? {
        let formatter = DateFormatter()
        formatter.dateFormat = "yyyy-MM-dd"
        formatter.timeZone = TimeZone.current
        return formatter.date(from: shiftDate)
    }

    var formattedWeekday: String {
        guard let date = shiftDateParsed else { return "Invalid date" }
        return Calendar.current.weekdaySymbols[Calendar.current.component(.weekday, from: date) - 1]
    }

    var formattedDateDisplay: String {
        guard let date = shiftDateParsed else { return shiftDate }
        return date.formatted(date: .abbreviated, time: .omitted)
    }
}
