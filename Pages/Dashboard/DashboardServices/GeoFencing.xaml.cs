using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Time_Management_System.Control;
using Microsoft.Maui.Controls.Compatibility;
using Time_Managmeent_System.Services;
namespace Time_Management_System.Pages
{
    public partial class GeoFencingPage : ContentPage
    {
        private readonly DataService _dataService;
        public GeoFencingPage(DataService dataService)
        {
            InitializeComponent();
            _dataService = dataService;
        }
    }
}
