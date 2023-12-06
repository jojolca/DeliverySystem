using System;
using Microsoft.VisualBasic;

namespace DeliverySystem.Variables.Repository
{
    public class LogInformation
    {
        public long Id { get; set; }

        public string ObjectType { set; get; }

        /// <summary>
        /// Log Type : Info,Warring,Error
        /// </summary>
        public string LogType { set; get; }

        public string Message { set; get; }

        public DateTime CreatedDateTime { set; get; }

        public bool IsDeleted { set;get;}
    }
}
