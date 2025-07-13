using Microsoft.Maui.Controls;
using Supabase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Time_Management_System.Control;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Services;

namespace Time_Management_System.Pages
{
    public partial class GeoFencingPage : ContentPage
    {
        private readonly DataService _dataService;
        private List<Geolocating> _locations = new();

        public GeoFencingPage(DataService dataService)
        {
            InitializeComponent();
            _dataService = dataService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadGeolocationsAsync();
        }

        private async Task LoadGeolocationsAsync()
        {
            _locations = await _dataService.GetAllGeolocationsAsync();
            LocationList.ItemsSource = _locations;
        }

        private async void OnSelectLocation(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Geolocating geo)
            {
                try
                {
                    var allEntries = await _dataService.SupabaseClient
                        .From<Geolocating>()
                        .Get();

                    // Set all to inactive
                    foreach (var entry in allEntries.Models)
                    {
                        if (entry.Active)
                        {
                            var response = await _dataService.SupabaseClient
                                .From<Geolocating>()
                                .Where(x => x.Id == entry.Id)
                                .Set(x => x.Active, false)
                                .Update();

                            if (response == null || response.Models.Count == 0)
                            {
                                Console.WriteLine($"Failed to deactivate entry with Id: {entry.Id}");
                            }
                        }
                    }

                    // Activate selected
                    var activateResponse = await _dataService.SupabaseClient
                        .From<Geolocating>()
                        .Where(x => x.Id == geo.Id)
                        .Set(x => x.Active, true)
                        .Update();

                    if (activateResponse == null || activateResponse.Models.Count == 0)
                    {
                        await DisplayAlert("Failed", "to activate selected geolocation. Check Id or RLS policies.", "OK");
                    }

                    // Force refresh
                    await LoadGeolocationsAsync();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred while selecting the location: {ex.Message}", "OK");
                    Console.WriteLine($"Error selecting location: {ex}");
                }
            }
            

        }



    }
}
