using System;
using System.Text.Json.Serialization;

namespace DeliverySystem.Variables.Repository
{
    public class ShippingInformation
    {
        [JsonPropertyName("Id")]
        public long ShippingInformation_Id { get; set; }

        [JsonPropertyName("TrackingNumber")]
        public string ShippingInformation_TrackingNumber { get; set; }

        [JsonPropertyName("SlaveTrackingNumber")]
        public string ShippingInformation_SlaveTrackingNumber { get; set; }

        [JsonPropertyName("OriginalTrackingNumber")]
        public string ShippingInformation_OriginalTrackingNumber { get; set; }

        [JsonPropertyName("OriginalTrackingNumber2")]
        public string ShippingInformation_OriginalTrackingNumber2 { get; set; }

        [JsonPropertyName("OriginalTrackingNumber3")]
        public string ShippingInformation_OriginalTrackingNumber3 { get; set; }

        [JsonPropertyName("UnionTrackingNumber")]
        public string ShippingInformation_UnionTrackingNumber { get; set; }

        [JsonPropertyName("UnionContractId")]
        public string ShippingInformation_UnionContractId { get; set; }

        [JsonPropertyName("TotalCount")]
        public int ShippingInformation_TotalCount { get; set; }

        [JsonPropertyName("Weight")]
        public double ShippingInformation_Weight { get; set; }

        [JsonPropertyName("ProductName")]
        public string ShippingInformation_ProductName { get; set; }

        [JsonPropertyName("CollectedMoney")]
        public decimal ShippingInformation_CollectedMoney { get; set; }

        [JsonPropertyName("Tax")]
        public decimal ShippingInformation_Tax { get; set; }

        [JsonPropertyName("CollectedFee")]
        public decimal ShippingInformation_CollectedFee { get; set; }

        [JsonPropertyName("SiteId")]
        public string ShippingInformation_SiteId { get; set; }

        [JsonPropertyName("SiteName")]
        public string ShippingInformation_SiteName { get; set; }

        [JsonPropertyName("RecipientCompany")]
        public string ShippingInformation_RecipientCompany { get; set; }

        [JsonPropertyName("RecipientName")]
        public string ShippingInformation_RecipientName { get; set; }

        [JsonPropertyName("RecipientAddress")]
        public string ShippingInformation_RecipientAddress { get; set; }

        [JsonPropertyName("RecipientPhoneNumber")]
        public string ShippingInformation_RecipientPhoneNumber { get; set; }

        [JsonPropertyName("SenderPhoneNumber")]
        public string ShippingInformation_SenderPhoneNumber { get; set; }

        [JsonPropertyName("SenderCompany")]
        public string ShippingInformation_SenderCompany { get; set; }

        [JsonPropertyName("SenderName")]
        public string ShippingInformation_SenderName { get; set; }

        [JsonPropertyName("SenderAddress")]
        public string ShippingInformation_SenderAddress { get; set; }

        [JsonPropertyName("Memo")]
        public string ShippingInformation_Memo { get; set; }

        [JsonPropertyName("Status")]
        public string ShippingInformation_Status { get; set; }

        [JsonPropertyName("CreatedDateTime")]
        public DateTime ShippingInformation_CreatedDateTime { get; set; }

        [JsonPropertyName("StatusUpdatedDateTime")]
        public DateTime ShippingInformation_StatusUpdatedDateTime { get; set; }

        [JsonPropertyName("CreatedUser")]
        public string ShippingInformation_CreatedUser { get; set; }

        [JsonPropertyName("UpdatedUser")]
        public string ShippingInformation_UpdatedUser { get; set; }

        [JsonPropertyName("IsDeleted")]
        public bool ShippingInformation_IsDeleted { get; set; }
    }
}
