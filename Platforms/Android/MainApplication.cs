using Android.App;
using Android.Runtime;
using Time_Management_System;

namespace Time_Managmeent_System
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp()
        {
            // Await the Task and return the result
            return MauiProgram.CreateMauiApp().GetAwaiter().GetResult();
        }
    }
}
