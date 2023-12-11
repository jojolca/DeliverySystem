using DeliverySystem.Variables.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliverySystem.Interface
{
    public interface IRepositoryOperater
    {
        Task<IEnumerable<ShippingInformation>> GetShippingInformation();

        Task<ShippingLabel> GetShippingLabel(string originalTrackingNumber);

        Task<IEnumerable<TaskSlave>> GetTaskSlave(long taskId);

        Task<long> InsertTask(TaskObject task);

        Task<long> InsertTaskSlave(TaskSlave taskSlave);

        Task<long> InsertShippingLabel(ShippingLabel shippingLabel);

        Task<long> InsertShippingInformation(ShippingInformation shippingInformation);

        Task<long> InsertLog(LogInformation log);

        Task<bool> UpdateTaskStatus(string status, long id);

        Task<bool> UpdateTaskSlave(string status, long id, string message);        
    }
}
