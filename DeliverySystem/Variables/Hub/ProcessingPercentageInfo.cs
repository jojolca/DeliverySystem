using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverySystem.Variables.Hub
{
    public class ProcessingPercentageInfo
    {
        public long TaskId { set; get; }

        public double ProcessingPercentage { set; get; }

        public string Status { set; get; }

        public string Message { set; get; }
    }
}
