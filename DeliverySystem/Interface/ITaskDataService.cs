using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliverySystem.Variables;
using DeliverySystem.Variables.Example;
using DeliverySystem.Variables.Repository;

namespace DeliverySystem.Interface
{
    public interface ITaskDataService
    {
        Task<long> CreateTasks(CreateTaskRequestEntity data);

        RawShippingInformation GetRawShippingInformation(string taskSlaveDataStr);

        ShippingInformation GetShippingInformation(RawShippingInformation rawData, string createdUser);

        string GetTaskStatus(long taskId);

        double GetSuccessTaskCount(long taskId);

        double GetFailTaskCount(long taskId);

        double GetTotalTaskCount(long taskId);

        List<long> GetProcessingTaskKeys();

        bool TryDeququeWaitingToDoTaskSlaves(out TaskSlave taskSlave);

        Task<long> InsertShippingInformation(ShippingInformation dataBaseShippingInformation);

        Task<long> InsertShippingLabel(ShippingLabel label);

        Task AddOrUpdateSuccessTask(TaskSlave taskSlave);

        Task AddOrUpdateFailTask(TaskSlave taskSlave);

        Task RemoveFinishTask(long taskId, string status);
    }
}
