using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DeliverySystem.Variables.BaseObject;

namespace DeliverySystem.Variables.Example
{
    /// <summary>
    /// Create Task Response
    /// </summary>
    public class CreateTaskResponseEntitycs
    {
        /// <summary>
        /// Task Id
        /// </summary>
        [JsonPropertyName("taskId")]
        public long TaskId { set; get; }

        /// <summary>
        /// 狀態
        /// </summary>
        [JsonPropertyName("status")]
        public int Status { set; get; }
    }
}
