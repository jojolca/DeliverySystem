using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DeliverySystem.Module
{
    public static class Extension
    {
        public static DateTime GetTWTime(this DateTime utcTime)
        {
            string id = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Taipei Standard Time" : "Asia/Taipei";
            TimeZoneInfo twtzinfo = TimeZoneInfo.FindSystemTimeZoneById(id);

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, twtzinfo);
        }

    }
}
