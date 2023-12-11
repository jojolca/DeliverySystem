using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DeliverySystem.Variables.BaseObject;

namespace DeliverySystem.Variables.Example
{
    public class GetTaskSlaveListResponseEntity 
    {
        [JsonPropertyName("OriginalTrackingNumber")]
        public string OriginalTrackingNumber { get; set; }

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { set; get; }

        [JsonPropertyName("status")]
        public string Status { set; get; }

        [JsonPropertyName("createdDateTime")]
        public DateTime CreatedDateTime { set; get; }
    }
}
