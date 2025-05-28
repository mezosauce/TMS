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
    }
}