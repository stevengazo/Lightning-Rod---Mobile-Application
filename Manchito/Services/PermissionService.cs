using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manchito.Services
{
    public class PermissionService
    {
        public async Task<bool> CheckAndRequestAsync<T>() where T : Permissions.BasePermission, new()
        {
            var status = await Permissions.CheckStatusAsync<T>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<T>();
            }
            return status == PermissionStatus.Granted;
        }

    }
}
