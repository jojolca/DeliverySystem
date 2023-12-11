using System.Text.Json.Serialization;
using System;
using DeliverySystem.Variables.BaseObject;

namespace DeliverySystem.Variables.Example
{
    public class GetShippingLabelResponseEntity 
    {
        [JsonPropertyName("id")]
        public long ShippingLabel_Id { get; set; }

        [JsonPropertyName("barCode")]
        public string ShippingLabel_BarCode { get; set; }

        [JsonPropertyName("salesOffice")]
        public string ShippingLabel_SalesOffice { get; set; }

        [JsonPropertyName("zipCode")]
        public string ShippingLabel_ZipCode { get; set; }

        [JsonPropertyName("zipCodeVersion")]
        public string ShippingLabel_ZipCodeVersion { get; set; }

        [JsonIgnore]
        public float ShippingLabel_PakageSize { get; set; }

        [JsonPropertyName("pakageSize")]
        public string PakageSize { get { return $"{ShippingLabel_PakageSize}cm"; } }

        [JsonPropertyName("createdDateTime")]
        public DateTime ShippingLabel_CreatedDateTime { get; set; }

        [JsonPropertyName("estimatedDeliveryDateTime")]
        public DateTime ShippingLabel_EstimatedDeliveryDateTime { get; set; }

        [JsonPropertyName("customID")]
        public string ShippingLabel_CustomID { get; set; }

        [JsonPropertyName("originalTrackingNumber")]
        public string ShippingLabel_ShippingOriginalTrackingNumber { get; set; }

        [JsonPropertyName("collectedMoney")]
        public decimal ShippingLabel_ShippingCollectedMoney { get; set; }

        [JsonPropertyName("recipientName")]
        public string ShippingLabel_ShippingRecipientName { get; set; }

        [JsonPropertyName("recipientAddress")]
        public string ShippingLabel_ShippingRecipientAddress { get; set; }

        [JsonPropertyName("recipientPhoneNumber")]
        public string ShippingLabel_ShippingRecipientPhoneNumber { get; set; }

        [JsonPropertyName("senderName")]
        public string ShippingLabel_ShippingSenderName { get; set; }

        [JsonPropertyName("senderAddress")]
        public string ShippingLabel_ShippingSenderAddress { get; set; }

        [JsonPropertyName("senderPhoneNumber")]
        public string ShippingLabel_ShippingSenderPhoneNumber { get; set; }

        [JsonPropertyName("senderCompany")]
        public string ShippingLabel_ShippingSenderCompany { get; set; }
    }
}
