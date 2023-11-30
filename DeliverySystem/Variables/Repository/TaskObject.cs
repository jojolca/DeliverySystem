using System;

namespace DeliverySystem.Variables.Repository
{
    public class TaskObject
    {
        public long Task_Id { get; set; }
        public string Task_Status { get; set; }
        public DateTime Task_StatusUpdatedDateTime { get; set; }
        public string Task_CreatedUser { get; set; }
        public DateTime Task_CreatedDateTime { get; set; }
        public string Task_UpdatedUser { get; set; }
        public DateTime Task_UpdatedDateTime { get; set; }
        public bool Task_IsDeleted { get; set; }
    }

}
