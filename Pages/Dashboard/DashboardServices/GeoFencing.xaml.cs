using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Time_Management_System.Control;
using Time_Managmeent_System.Services;
using Time_Managmeent_System.Models;

namespace Time_Management_System.Pages
{
    public partial class GeoFencingPage : ContentPage
    {
        private readonly DataService _dataService;
        private List<Time_Managmeent_System.Models.Geolocation> _locations = new();

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
            if (sender is Button button && button.BindingContext is Time_Managmeent_System.Models.Geolocation geo)
            {
                await _dataService.SetGeolocationActiveAsync(geo.Id);

                // Reload list to reflect new active state
                await LoadGeolocationsAsync();
            }
        }
    }
}
