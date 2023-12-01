using Newtonsoft.Json;

namespace DeliverySystem.Variables
{
    public class RawShippingInformation
    {
        [JsonProperty(PropertyName ="運單號")]
        public string ShippingInformation_TrackingNumber { get; set; }

        [JsonProperty(PropertyName ="子單號")]
        public string ShippingInformation_SlaveTrackingNumber { get; set; }

        [JsonProperty(PropertyName ="原單號")]
        public string ShippingInformation_OriginalTrackingNumber { get; set; }

        [JsonProperty(PropertyName ="原單號2")]
        public string ShippingInformation_OriginalTrackingNumber2 { get; set; }

        [JsonProperty(PropertyName ="原單號3")]
        public string ShippingInformation_OriginalTrackingNumber3 { get; set; }

        [JsonProperty(PropertyName ="聯運單號")]
        public string ShippingInformation_UnionTrackingNumber { get; set; }

        [JsonProperty(PropertyName ="聯運合約號")]
        public string ShippingInformation_UnionContractId { get; set; }

        [JsonProperty(PropertyName ="總件數")]
        public int ShippingInformation_TotalCount { get; set; }

        [JsonProperty(PropertyName ="運單重量")]
        public double ShippingInformation_Weight { get; set; }

        [JsonProperty(PropertyName ="品名")]
        public string ShippingInformation_ProductName { get; set; }

        [JsonProperty(PropertyName ="代收貨款")]
        public decimal ShippingInformation_CollectedMoney { get; set; }

        [JsonProperty(PropertyName ="稅金")]
        public decimal ShippingInformation_Tax { get; set; }

        [JsonProperty(PropertyName ="代收手續費")]
        public decimal ShippingInformation_CollectedFee { get; set; }

        [JsonProperty(PropertyName ="站點代碼")]
        public string ShippingInformation_SiteId { get; set; }

        [JsonProperty(PropertyName ="站點名稱")]
        public string ShippingInformation_SiteName { get; set; }

        [JsonProperty(PropertyName ="收件人公司")]
        public string ShippingInformation_RecipientCompany { get; set; }

        [JsonProperty(PropertyName ="收件人姓名")]
        public string ShippingInformation_RecipientName { get; set; }

        [JsonProperty(PropertyName ="收件人地址")]
        public string ShippingInformation_RecipientAddress { get; set; }

        [JsonProperty(PropertyName ="收件人電話")]
        public string ShippingInformation_RecipientPhoneNumber { get; set; }

        [JsonProperty(PropertyName ="寄件人電話")]
        public string ShippingInformation_SenderPhoneNumber { get; set; }

        [JsonProperty(PropertyName ="寄件人公司")]
        public string ShippingInformation_SenderCompany { get; set; }

        [JsonProperty(PropertyName ="寄件人姓名")]
        public string ShippingInformation_SenderName { get; set; }

        [JsonProperty(PropertyName ="寄件人地址")]
        public string ShippingInformation_SenderAddress { get; set; }

        [JsonProperty(PropertyName ="備註")]
        public string ShippingInformation_Memo { get; set; }
    }
}
