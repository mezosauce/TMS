using Time_Managmeent_System.Services;

namespace Time_Managmeent_System.Pages;

public partial class EditAccount : ContentPage
{
	private readonly DataService _dataService;
	public EditAccount(DataService dataService)

    {
		InitializeComponent();
		_dataService = dataService;
	}
}