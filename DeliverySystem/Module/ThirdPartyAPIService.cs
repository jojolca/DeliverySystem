using System;
using System.Threading.Tasks;
using DeliverySystem.Interface;
using DeliverySystem.Variables;
using DeliverySystem.Variables.Repository;

namespace DeliverySystem.Module
{
    public class ThirdPartyAPIService : IThirdPartyAPIOperater
    {
        public ShippingLabel GetLabel(RawShippingInformation information)
        {
            Random random = new Random();
            return  new ShippingLabel()
            {
                ShippingLabel_BarCode = DateTime.Now.ToString("yyyyMMddHHmmss"),
                ShippingLabel_CreatedDateTime = DateTime.Now,
                ShippingLabel_CreatedUser = "user1",
                ShippingLabel_CustomID = "Customer",
                ShippingLabel_EstimatedDeliveryDateTime = DateTime.Now.AddDays(1),
                ShippingLabel_PakageSize = random.Next(1, 100),
                ShippingLabel_SalesOffoice = "營業所",
                ShippingLabel_ShippingCollectedMoney = information.ShippingInformation_CollectedMoney,
                ShippingLabel_ShippingOriginalTrackingNumber = information.ShippingInformation_OriginalTrackingNumber,
                ShippingLabel_ShippingRecipientAddress = information.ShippingInformation_RecipientAddress,
                ShippingLabel_ShippingRecipientName = information.ShippingInformation_RecipientName,
                ShippingLabel_ShippingRecipientPhoneNumber = information.ShippingInformation_RecipientPhoneNumber,
                ShippingLabel_ShippingSenderAddress = information.ShippingInformation_SenderAddress,
                ShippingLabel_ShippingSenderCompany = information.ShippingInformation_SenderCompany,
                ShippingLabel_ShippingSenderName = information.ShippingInformation_SenderName,
                ShippingLabel_ShippingSenderPhoneNumber = information.ShippingInformation_SenderPhoneNumber,
                ShippingLabel_ZipCode = "23-11-30",
                ShippingLabel_ZipCodeVersion = DateTime.Now.ToString("yyyyMMdd")

            };
        }
    }
}
