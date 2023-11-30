using System;

namespace DeliverySystem.Variables.Repository
{
    public class TaskSlave
    {
        public long TaskSlave_Id { get; set; }
        public long TaskSlave_TaskId { get; set; }
        public string TaskSlave_Status { get; set; }
        public DateTime TaskSlave_StatusUpdatedDateTime { get; set; }
        public string TaskSlave_CreatedUser { get; set; }
        public DateTime TaskSlave_CreatedDateTime { get; set; }
        public string TaskSlave_UpdatedUser { get; set; }
        public DateTime TaskSlave_UpdatedDateTime { get; set; }
        public string TaskSlave_Data { get; set; }
        public string TaskSlave_ErrorMsg { get; set; }
        public bool TaskSlave_IsDeleted { get; set; }
    }

}
