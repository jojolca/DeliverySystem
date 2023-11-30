using System.Text.Json.Serialization;

namespace DeliverySystem.Variables
{
    public class RawShippingInformation
    {
        [JsonPropertyName("運單號")]
        public string ShippingInformation_TrackingNumber { get; set; }

        [JsonPropertyName("子單號")]
        public string ShippingInformation_SlaveTrackingNumber { get; set; }

        [JsonPropertyName("原單號")]
        public string ShippingInformation_OriginalTrackingNumber { get; set; }

        [JsonPropertyName("原單號2")]
        public string ShippingInformation_OriginalTrackingNumber2 { get; set; }

        [JsonPropertyName("原單號3")]
        public string ShippingInformation_OriginalTrackingNumber3 { get; set; }

        [JsonPropertyName("聯運單號")]
        public string ShippingInformation_UnionTrackingNumber { get; set; }

        [JsonPropertyName("聯運合約號")]
        public string ShippingInformation_UnionContractId { get; set; }

        [JsonPropertyName("總件數")]
        public int ShippingInformation_TotalCount { get; set; }

        [JsonPropertyName("運單重量")]
        public double ShippingInformation_Weight { get; set; }

        [JsonPropertyName("品名")]
        public string ShippingInformation_ProductName { get; set; }

        [JsonPropertyName("代收貨款")]
        public decimal ShippingInformation_CollectedMoney { get; set; }

        [JsonPropertyName("稅金")]
        public decimal ShippingInformation_Tax { get; set; }

        [JsonPropertyName("代收手續費")]
        public decimal ShippingInformation_CollectedFee { get; set; }

        [JsonPropertyName("站點代碼")]
        public string ShippingInformation_SiteId { get; set; }

        [JsonPropertyName("站點名稱")]
        public string ShippingInformation_SiteName { get; set; }

        [JsonPropertyName("收件人公司")]
        public string ShippingInformation_RecipientCompany { get; set; }

        [JsonPropertyName("收件人姓名")]
        public string ShippingInformation_RecipientName { get; set; }

        [JsonPropertyName("收件人地址")]
        public string ShippingInformation_RecipientAddress { get; set; }

        [JsonPropertyName("收件人電話")]
        public string ShippingInformation_RecipientPhoneNumber { get; set; }

        [JsonPropertyName("寄件人電話")]
        public string ShippingInformation_SenderPhoneNumber { get; set; }

        [JsonPropertyName("寄件人公司")]
        public string ShippingInformation_SenderCompany { get; set; }

        [JsonPropertyName("寄件人姓名")]
        public string ShippingInformation_SenderName { get; set; }

        [JsonPropertyName("寄件人地址")]
        public string ShippingInformation_SenderAddress { get; set; }

        [JsonPropertyName("備註")]
        public string ShippingInformation_Memo { get; set; }
    }
}
