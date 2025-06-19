using Time_Managmeent_System.ViewModels;
namespace Time_Managmeent_System.Pages;

public partial class AddEmployeePage : ContentPage
{
	public AddEmployeePage(AddEmployeeViewModel addEmployeeViewModel)
	{
		InitializeComponent();
		BindingContext = addEmployeeViewModel;
	}
}