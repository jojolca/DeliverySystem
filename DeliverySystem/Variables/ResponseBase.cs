using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeliverySystem.Variables
{
    /// <summary>
    /// Response Base
    /// </summary>
    public class ResponseBase
    {
        /// <summary>
        /// Message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { set; get; }
    }
}
