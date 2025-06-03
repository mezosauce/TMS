namespace Time_Managmeent_System
{   
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Set the root page of the application in the CreateWindow method
            var window = new Window(new NavigationPage(new Pages.LoginPage()));
            return window;
        }

        public void ToggleTheme()
        {
            // Toggle between Light and Dark mode
            if (App.Current.UserAppTheme == AppTheme.Dark)
            {
                App.Current.UserAppTheme = AppTheme.Light;
            }
            else
            {
                App.Current.UserAppTheme = AppTheme.Dark;
            }
        }

      
    }
}