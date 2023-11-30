using DeliverySystem.Variables;
using DeliverySystem.Variables.Repository;

namespace DeliverySystem.Interface
{
    public interface IThirdPartyAPIOperater
    {
        ShippingLabel GetLabel(RawShippingInformation information);
    }
}
