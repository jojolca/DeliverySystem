using DeliverySystem.Variables.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliverySystem.Interface
{
    public interface IRepositoryOperater
    {
        Task<IEnumerable<ShippingInformation>> GetShippingInformation();

        Task<long> InsertTask(TaskObject task);

        Task<long> InsertTaskSlave(TaskSlave taskSlave);

        Task<long> InsertShippingLabel(ShippingLabel shippingLabel);

        Task<long> InsertShippingInformation(ShippingInformation shippingInformation);

        Task<long> InsertLog(LogInformation log);

        Task<ShippingLabel> GetShippingLabel(string originalTrackingNumber);

        Task<bool> UpdateTaskStatus(string status, long id);

        Task<bool> UpdateTaskSlaveStatus(string status, long id, string message);        
    }
}
