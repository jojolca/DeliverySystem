using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeliverySystem.Variables.Example
{
    /// <summary>
    /// Create Task Request
    /// </summary>
    public class CreateTaskRequestEntity
    {
        /// <summary>
        /// User
        /// </summary>
        [JsonPropertyName("user")]
        public string User { set; get; }

        /// <summary>
        /// 欄位名稱內容
        /// </summary>
        [JsonPropertyName("column")]
        public string[] Column { set; get; }

        /// <summary>
        /// 每一列的內容(使用逗號分隔)
        /// </summary>
        [JsonPropertyName("rowData")]
        public List<string[]> RowData { set; get; }
    }
}
