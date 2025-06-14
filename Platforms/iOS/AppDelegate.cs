using Foundation;
using Time_Management_System;
namespace Time_Managmeent_System
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp()
        {
            // Await the Task and return the result
            return MauiProgram.CreateMauiApp().GetAwaiter().GetResult();
        }
    }
}
