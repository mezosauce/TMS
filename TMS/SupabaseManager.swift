//
//  SupabaseManager.swift
//  TMS
//
//  Created by Evan Heidenreich on 6/11/25.
//

import Foundation
import Supabase

class SupabaseManager {
    static let shared = SupabaseManager()
    
    private init() {}
    
    let supabase = SupabaseClient(supabaseURL: URL(string: "https://shdzcbiecsofkxhvscdn.supabase.co")!, supabaseKey: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InNoZHpjYmllY3NvZmt4aHZzY2RuIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDg5ODM4MzksImV4cCI6MjA2NDU1OTgzOX0.LD52Hgg-T76kOnsdCg5sFzR9Z_Lf42v3Ty1h0fSAX8A"
    )
    
}
