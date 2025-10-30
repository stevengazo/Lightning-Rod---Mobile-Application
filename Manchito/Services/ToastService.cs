using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manchito.Services
{
    public class ToastService
    {
        public static async Task ShowAsync(string message, bool isLong = false)
        {
            var duration = isLong ? ToastDuration.Long : ToastDuration.Short;
            var toast = Toast.Make(message, duration, 14);
            await toast.Show(CancellationToken.None);
        }
    }
}
