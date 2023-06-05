using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraficaCurone.Utils
{
    public class PermissionUtils
    {
        public static async Task<bool> CheckForStoragePermission()
        {
            bool permitted = false;


            var statusWrite = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            var statusRead = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (statusWrite == PermissionStatus.Granted && statusRead == PermissionStatus.Granted)
                permitted = true;
            else
            {
                statusWrite = await Permissions.RequestAsync<Permissions.StorageWrite>();
                statusRead = await Permissions.RequestAsync<Permissions.StorageRead>();
            }
            return permitted;
        }
    }
}
