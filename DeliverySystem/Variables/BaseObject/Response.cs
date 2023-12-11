using System.Text.Json.Serialization;

namespace DeliverySystem.Variables.BaseObject
{
    public class Response<T>
    {
        /// <summary>
        /// Data
        /// </summary>
        [JsonPropertyName("data")]
        public T Data { set; get; }

        /// <summary>
        /// Message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { set; get; }
    }
}
