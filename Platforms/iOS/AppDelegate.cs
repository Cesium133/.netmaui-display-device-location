using CoreLocation;
using Foundation;
using UIKit;

namespace DisplayDeviceLocation
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
        public override void DidEnterBackground(UIApplication uiApplication)
        {
            CLLocationManager locationManager = new CLLocationManager();
            locationManager.AllowsBackgroundLocationUpdates = true;
            locationManager.StartUpdatingLocation();
        }
    }
}

