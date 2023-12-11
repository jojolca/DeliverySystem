using DeliverySystem.Variables;
using DeliverySystem.Variables.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverySystem.Validators
{
    public class RawShippingInformationValidator 
    {
        private RawShippingInformation data;

        public RawShippingInformationValidator(RawShippingInformation inputData)
        {
            data = inputData;
        }

        public string Verify()
        {
            string message = string.Empty;
            
            if (string.IsNullOrEmpty(data.ShippingInformation_TrackingNumber))
            {
                return "運單號資料不得為空";
            }
            else if (data.ShippingInformation_TrackingNumber.Length > 20)
            {
                return "運單號資料長度超過20字";
            }
            else if (string.IsNullOrEmpty(data.ShippingInformation_OriginalTrackingNumber))
            {
                return "原單號資料不得為空";
            }
            else if(data.ShippingInformation_OriginalTrackingNumber.Length > 20)
            {
                return "原單號資料長度超過20字";
            }
            else if (data.ShippingInformation_OriginalTrackingNumber2.Length > 20)
            {
                return "原單號2資料長度超過20字";
            }
            else if (data.ShippingInformation_OriginalTrackingNumber3.Length > 20)
            {
                return "原單號3資料長度超過20字";
            }
            else if (data.ShippingInformation_UnionTrackingNumber.Length > 20)
            {
                return "聯運單號資料長度超過20字";
            }
            else if (data.ShippingInformation_UnionContractId.Length > 20)
            {
                return "聯運合約號資料長度超過20字";
            }
            else if (data.ShippingInformation_TotalCount == null)
            {
                return "總件數資料不得為空";
            }
            else if (data.ShippingInformation_Weight == null)
            {
                return "運單重量資料不得為空";
            }
            else if (data.ShippingInformation_CollectedMoney == null)
            {
                return "代收貨款資料不得為空";
            }
            else if (data.ShippingInformation_Tax == null)
            {
                return "稅金資料不得為空";
            }
            else if (data.ShippingInformation_CollectedFee == null)
            {
                return "代收手續費資料不得為空";
            }
            else if (data.ShippingInformation_SiteId.Length > 20)
            {
                return "站點代碼資料長度超過20字";
            }
            else if (data.ShippingInformation_SiteName.Length > 20)
            {
                return "站點名稱資料長度超過20字";
            }
            else if (data.ShippingInformation_RecipientCompany.Length > 20)
            {
                return "收件人公司資料長度超過20字";
            }
            else if (string.IsNullOrEmpty(data.ShippingInformation_RecipientName))
            {
                return "收件人姓名資料不得為空";
            }
            else if (data.ShippingInformation_RecipientName.Length > 20)
            {
                return "收件人姓名資料長度超過20字";
            }
            else if (string.IsNullOrEmpty(data.ShippingInformation_RecipientAddress))
            {
                return "收件人地址資料不得為空";
            }
            else if (data.ShippingInformation_RecipientAddress.Length > 100)
            {
                return "收件人地址資料長度超過100字";
            }
            else if (string.IsNullOrEmpty(data.ShippingInformation_RecipientPhoneNumber))
            {
                return "收件人電話資料不得為空";
            }
            else if (data.ShippingInformation_RecipientPhoneNumber.Length > 20)
            {
                return "收件人電話資料長度超過20字";
            }
            else if (string.IsNullOrEmpty(data.ShippingInformation_SenderAddress))
            {
                return "寄件人地址資料不得為空";
            }
            else if (data.ShippingInformation_SenderAddress.Length > 100)
            {
                return "寄件人地址資料長度超過100字";
            }
            else if (string.IsNullOrEmpty(data.ShippingInformation_SenderPhoneNumber))
            {
                return "寄件人電話資料不得為空";
            }
            else if (data.ShippingInformation_SenderPhoneNumber.Length > 20)
            {
                return "寄件人電話資料長度超過20字";
            }
            else if (string.IsNullOrEmpty(data.ShippingInformation_SenderName))
            {
                return "寄件人姓名資料不得為空";
            }
            else if (data.ShippingInformation_SenderName.Length > 20)
            {
                return "寄件人姓名資料長度超過20字";
            }


            return message;
        }
    }
}
