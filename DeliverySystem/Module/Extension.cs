using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverySystem.Module
{
    public static class Extension
    {
        public static DateTime GetTWTime(this DateTime utcTime)
        {
            var twtzinfo = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");

           return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, twtzinfo);
        }

    }
}
