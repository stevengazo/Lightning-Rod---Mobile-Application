using Manchito.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manchito.Services
{
    public class LocationService
    {
        private CancellationTokenSource _cts;

        public async Task<GPSLocation> GetCurrentLocationAsync()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                _cts = new CancellationTokenSource();

                var location = await Geolocation.Default.GetLocationAsync(request, _cts.Token);

                if (location != null)
                {
                    return new GPSLocation
                    {
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                        Altitude = location.Altitude ?? 0
                    };
                }
            }
            catch
            {
                // Silencioso o con log si usas algún logger
            }
            return null;
        }

        public void Cancel()
        {
            _cts?.Cancel();
        }
    }
}
