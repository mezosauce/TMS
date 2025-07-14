using Microsoft.EntityFrameworkCore.Metadata;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.Services;

namespace Time_Managmeent_System.Pages.Dashboard.DashboardServices;

public partial class AssignShiftPage : ContentPage
{
	private readonly DataService _dataService;
    public AssignShiftPage(DataService dataservice)
	
	{
		_dataService = dataservice;
        InitializeComponent();
	}
}