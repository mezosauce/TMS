namespace Time_Managmeent_System
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Set the main page to the login page
            MainPage = new NavigationPage(new Pages.LoginPage());
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}