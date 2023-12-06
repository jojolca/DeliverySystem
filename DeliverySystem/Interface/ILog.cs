using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliverySystem.Variables.Repository;

namespace DeliverySystem.Interface
{
    public interface ILog
    {
        /// <summary>
        /// 加入log
        /// </summary>
        /// <param name="log"></param>
        void AddLog(LogInformation log);
    }
}
