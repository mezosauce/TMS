using Time_Managmeent_System.ViewModels;
namespace Time_Managmeent_System.Pages;


public partial class UpdateEmployeePage : ContentPage
{
	public UpdateEmployeePage(UpdateEmployeeViewModel updateEmployeeViewModel)
	{
		InitializeComponent();
		BindingContext = updateEmployeeViewModel;

    }
}