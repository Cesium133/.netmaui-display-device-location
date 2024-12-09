using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Devices.Sensors;
using Resource = Microsoft.Maui.Resource;

[Service(ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeLocation)]
public class LocationService : Service
{
    private const string Tag = "LocationService";

    public override IBinder OnBind(Intent intent) => null;
    private int totalCount = 0;

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        StartForeground(1, CreateNotification());
        StartLocationUpdates();
        return StartCommandResult.Sticky;
    }

    private Notification CreateNotification()
    {
        var channelId = "location_notification";
        var channelName = "Location Tracking";
        var notificationManager = (NotificationManager)GetSystemService(NotificationService);
        var channel = new NotificationChannel(channelId, channelName, NotificationImportance.Default);
        notificationManager.CreateNotificationChannel(channel);

        var notification = new NotificationCompat.Builder(this, channelId)
            .SetContentTitle("Location Tracking")
            .SetContentText("Tracking location in the background")
            //.SetSmallIcon(Resource.Drawable.icon)
            .Build();

        return notification;
    }

    private void StartLocationUpdates()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                 var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.High,
                    RequestFullAccuracy = true,
                    Timeout = TimeSpan.FromSeconds(10)
                });

                if (location != null)
                {
                    Console.WriteLine($"{location.Latitude},{location.Longitude}");
                }

                totalCount += 1;
                if (totalCount == 10)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Shell.Current.DisplayAlert("Alert", "Location was captured 10 times", "OK");
                    });
                }
                await Task.Delay(3000); // Delay between location updates

                

            }
        });
    }


}
