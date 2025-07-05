
using Time_Managmeent_System.Services;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Pages;

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
