//
//  AsyncLocationManager.swift
//  TMS
//
//  Created by Evan Heidenreich on 7/26/25.
//

import CoreLocation
import Foundation

class AsyncLocationManager: NSObject, CLLocationManagerDelegate {
    private var locationManager: CLLocationManager
    private var locationContinuation: CheckedContinuation<CLLocation, Error>?

    override init() {
        locationManager = CLLocationManager()
        super.init()
        locationManager.delegate = self
    }

    func requestLocation() async throws -> CLLocation {
        locationManager.requestWhenInUseAuthorization()

        guard CLLocationManager.locationServicesEnabled() else {
            throw NSError(domain: "LocationError", code: 1, userInfo: [NSLocalizedDescriptionKey: "Location services disabled"])
        }

        return try await withCheckedThrowingContinuation { continuation in
            self.locationContinuation = continuation
            locationManager.requestLocation()
        }
    }

    func locationManager(_ manager: CLLocationManager, didUpdateLocations locations: [CLLocation]) {
        if let location = locations.first {
            locationContinuation?.resume(returning: location)
            locationContinuation = nil
        }
    }

    func locationManager(_ manager: CLLocationManager, didFailWithError error: Error) {
        locationContinuation?.resume(throwing: error)
        locationContinuation = nil
    }
}
