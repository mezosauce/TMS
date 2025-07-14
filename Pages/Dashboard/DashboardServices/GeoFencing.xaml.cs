using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using Supabase;
using Supabase.Gotrue;
using Supabase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Time_Management_System.Control;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Services;

namespace Time_Management_System.Pages;

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
        _locations = await GetAllGeolocationsAsync();
        LocationList.ItemsSource = _locations;
    }


    private async Task<List<Geolocating>> GetAllGeolocationsAsync()
    {
        var response = await _dataService.SupabaseClient.From<Geolocating>().Get();
        return response.Models;
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

                if (activateResponse == null)
                {
                    Console.WriteLine("activateResponse is null");
                }
                else
                {
                    var json = JsonConvert.SerializeObject(activateResponse, Formatting.Indented);
                    Console.WriteLine($"activateResponse content:\n{json}");
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
    private async void OnDeleteLocation(object sender, EventArgs e)
{
    if (sender is Button button && button.BindingContext is Geolocating geo)
    {
        var confirm = await DisplayAlert("Delete", $"Are you sure you want to delete '{geo.Name}'?", "Yes", "No");
        if (!confirm) return;

        try
        {
                await _dataService.SupabaseClient
                .From<Geolocating>()
                .Where(x => x.Id == geo.Id)
                .Delete();

            

            await LoadGeolocationsAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to delete: {ex.Message}", "OK");
            Console.WriteLine($"Delete error: {ex}");
        }
    }
}




}
