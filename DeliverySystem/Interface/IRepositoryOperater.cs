using DeliverySystem.Variables.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliverySystem.Interface
{
    public interface IRepositoryOperater
    {
        Task<IEnumerable<ShippingInformation>> GetShippingInformation();
    }
}
