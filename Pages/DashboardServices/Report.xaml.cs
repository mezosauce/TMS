using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Time_Management_System.Control;
using Microsoft.Maui.Controls.Compatibility;
using Time_Management_System.Services;
using Time_Management_System.Models;
using Time_Management_System.Pages;

namespace Time_Management_System.Pages
{
    public partial class ReportPage : ContentPage
    {
        private readonly DataService _dataService;
        public ReportPage(DataService dataService)
        {
            InitializeComponent();
            _dataService = dataService;
        }
    }
}
