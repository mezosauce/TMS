using Time_Managmeent_System.Services;
using Time_Managmeent_System.Models;
using Time_Managmeent_System.ViewModels;
using Supabase;

namespace Time_Managmeent_System.Pages;


public partial class LoginPage : ContentPage
{

    private readonly DataService _dataservice;
    public LoginPage(DataService dataservice)
    {
        InitializeComponent();
        _dataservice = dataservice;
    }


    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginType.SignIn(_dataservice));
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
      
            await Navigation.PushAsync(new LoginType.Guest(_dataservice));
        
        
    }

    private void OnToggleDarkModeClicked(object sender, EventArgs e)
    {
        if (App.Current.UserAppTheme == AppTheme.Dark)
        {
            App.Current.UserAppTheme = AppTheme.Light;
            if (sender is Button btn)
            {
                btn.Text = "\uf185";
                btn.TextColor = Colors.Black;
            }
        }
        else
        {
            App.Current.UserAppTheme = AppTheme.Dark;
            if (sender is Button btn)
            {
                btn.Text = "\uf186";
                btn.TextColor = Colors.White;
            }
        }
    }
}