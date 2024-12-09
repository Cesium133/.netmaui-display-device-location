//   Copyright 2023 Esri
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//   https://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
namespace DisplayDeviceLocation;

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Location;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System.ComponentModel;
using Geolocation = Microsoft.Maui.Devices.Sensors.Geolocation;


public partial class MainPage : ContentPage
{

    // Location data source provides location data updates.
    private LocationDataSource _locationDataSource;

    // Graphics overlay to display the location history (points).
    private GraphicsOverlay _locationHistoryOverlay;

    // Graphics overlay to display the line created by the location points.
    private GraphicsOverlay _locationHistoryLineOverlay;

    // Polyline builder to more efficiently manage large location history graphic.
    private PolylineBuilder _polylineBuilder;

    // Track previous location to ensure the route line appears behind the animating location symbol.
    private MapPoint _lastPosition;

    private bool trackingOn = false;

    private SystemLocationDataSource locationDataSource = new();

    public MainPage()
    {
        InitializeComponent();

        MainMapView.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
        {
            // The map view's location display is initially null, so check for a location display property change before enabling it.
            if (e.PropertyName == nameof(LocationDisplay))
            {
                _ = DisplayDeviceLocationAsync();
            }
        };


        // Create and add graphics overlay for displaying the trail.
        _locationHistoryLineOverlay = new GraphicsOverlay();
        SimpleLineSymbol locationLineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Lime, 2);
        _locationHistoryLineOverlay.Renderer = new SimpleRenderer(locationLineSymbol);
        MainMapView.GraphicsOverlays.Add(_locationHistoryLineOverlay);

        // Create and add graphics overlay for showing points.
        _locationHistoryOverlay = new GraphicsOverlay();
        SimpleMarkerSymbol locationPointSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Red, 3);
        _locationHistoryOverlay.Renderer = new SimpleRenderer(locationPointSymbol);
        MainMapView.GraphicsOverlays.Add(_locationHistoryOverlay);

        // Create the polyline builder.
        _polylineBuilder = new PolylineBuilder(SpatialReferences.WebMercator);


    }



    private async Task DisplayDeviceLocationAsync()
    {

        PermissionStatus status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (status == PermissionStatus.Denied || status == PermissionStatus.Unknown)
        {
            await DisplayAlert("Access Requested", "Please allow precise location all the time to track while phone is locked or viewing other applications.", "OK");
            status = await Permissions.RequestAsync<Permissions.LocationAlways>();
        }
        if (status != PermissionStatus.Granted)
        {
            return;
        }

        //Microsoft.Maui.Devices.Sensors.Location location = await Geolocation.GetLocationAsync(new GeolocationRequest
        //{
        //    DesiredAccuracy = GeolocationAccuracy.High,
        //    RequestFullAccuracy = true,
        //    Timeout = TimeSpan.FromSeconds(2)
        //});

        //Geolocation.LocationChanged += async (sender,e) => await GeolocationChanged(sender, e);




        await locationDataSource.StartAsync();

        if (locationDataSource.Status == LocationDataSourceStatus.Started)
        {
            MainMapView.LocationDisplay.DataSource = locationDataSource;
            MainMapView.LocationDisplay.IsEnabled = status == PermissionStatus.Granted || status == PermissionStatus.Restricted;
            MainMapView.LocationDisplay.InitialZoomScale = 1000;
            MainMapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Recenter;
        }

        locationDataSource.LocationChanged += async (sender, e) => await LocationChanged(sender, e);


    }


    private async Task LocationChanged(object sender, Esri.ArcGISRuntime.Location.Location e)
    {
        await Task.Run(() => { 
            _locationHistoryLineOverlay.Graphics.Clear();
            MapPoint mp = (MapPoint)GeometryEngine.Project(e.Position, SpatialReferences.WebMercator);
            if (mp != null)
            {
                MainThread.BeginInvokeOnMainThread(() => { 
                    _polylineBuilder.AddPoint(mp);
                    _locationHistoryOverlay.Graphics.Add(new Graphic(mp));
                    _locationHistoryLineOverlay.Graphics.Add(new Graphic(_polylineBuilder.ToGeometry()));
                });
            }
        });

    }

}
