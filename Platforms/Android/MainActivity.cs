using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace DisplayDeviceLocation
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Existing code...

            var intent = new Intent(this, typeof(LocationService));
            StartForegroundService(intent);
        }

    }
}
