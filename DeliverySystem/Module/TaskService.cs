using DeliverySystem.Interface;
using DeliverySystem.Variables;
using DeliverySystem.Variables.Example;
using DeliverySystem.Variables.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverySystem.Module
{
    public class TaskService : ITaskService
    {
        private readonly IHubContext<SignalRHub> _hubContext;

        private readonly IRepositoryOperater _repository;

        private readonly IThirdPartyAPIOperater _thirdPartyAPIOperater;

        private ConcurrentDictionary<long,TaskObject> _processingTasks = new ConcurrentDictionary<long, TaskObject>();

        private  ConcurrentQueue<TaskSlave> _waitingToDoTaskSlaves = new ConcurrentQueue<TaskSlave>();

        private ConcurrentDictionary<long, ConcurrentQueue<TaskSlave>> _successTaskSlaves = new ConcurrentDictionary<long, ConcurrentQueue<TaskSlave>>();

        private ConcurrentDictionary<long, ConcurrentQueue<TaskSlave>> _failTaskSlaves = new ConcurrentDictionary<long, ConcurrentQueue<TaskSlave>>();

        public TaskService(IHubContext<SignalRHub> hubContext, IRepositoryOperater repositoryOperater, IThirdPartyAPIOperater thirdPartyAPIOperater)
        {
            _hubContext = hubContext;
            _repository = repositoryOperater;
            _thirdPartyAPIOperater = thirdPartyAPIOperater;
        }

        public async Task SendMsg(string msg, string user)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, $"[{DateTime.Now}]From {user}:{msg}"); 
        }

        public async Task<bool> CreateTasks(CreateTaskRequestEntity data)
        {
            List<TaskSlave> failedTaskSlave = new List<TaskSlave>();
            List<TaskSlave> waitingToDoTaskSlave = new List<TaskSlave>();
            var task = new TaskObject()
            {
                Task_CreatedDateTime = DateTime.Now,
                Task_CreatedUser = data.User,
                Task_IsDeleted = false,
                Task_Status = "Ready",
                Task_StatusUpdatedDateTime = DateTime.Now,
                Task_UpdatedUser = data.User,
                Task_UpdatedDateTime = DateTime.Now,
            };

            var taskId = await _repository.InsertTask(task);

            if(taskId > 0)
            {
                task.Task_Id = taskId;
                _processingTasks.TryAdd(taskId, task);

                foreach (var row in data.RowData)
                {
                    var taskSlaveData = new TaskSlaveData()
                    {
                        Column = data.Column,
                        RowData = row.Split(",").ToArray(),
                    };

                    var taskSlave = new TaskSlave()
                    {
                        TaskSlave_TaskId = taskId,
                        TaskSlave_CreatedDateTime = DateTime.Now,
                        TaskSlave_CreatedUser = data.User,
                        TaskSlave_Data = JsonConvert.SerializeObject(taskSlaveData),
                        TaskSlave_IsDeleted = false,
                        TaskSlave_Status = "Ready",
                        TaskSlave_StatusUpdatedDateTime = DateTime.Now
                    };

                    var taskSlaveId = await _repository.InsertTaskSlave(taskSlave);
                    if (taskSlaveId > 0)
                    {
                        taskSlave.TaskSlave_Id = taskSlaveId;
                        waitingToDoTaskSlave.Add(taskSlave);

                    }
                    else
                    {
                        failedTaskSlave.Add(taskSlave);
                    }

                }
            }
            else 
            {
                // log 建立task失敗
                return false;
            }

            if(failedTaskSlave.Count > 0)
            {
                _processingTasks.TryRemove(taskId, out _);
                //update task DB data、taskSlave DB data
                return false;
            }

            return true;
        }

        private void Initial()
        {

        }

        private void GetLael()
        {
            Task.Run(async () => 
            {
                while(true)
                {
                    if(_waitingToDoTaskSlaves.TryDequeue(out var taskSlave))
                    {
                        var rawShippingInformation = GetRawShippingInformation(taskSlave.TaskSlave_Data);
                        var dataBaseShippingInformation = GetShippingInformation(rawShippingInformation);
                        var shippingInformationId = await _repository.InsertShippingInformation(dataBaseShippingInformation);

                        var label = _thirdPartyAPIOperater.GetLabel(rawShippingInformation);
                        var labelId = await _repository.InsertShippingLabel(label);
                        if(labelId > 0) 
                        {
                            if(_successTaskSlaves.TryGetValue(taskSlave.TaskSlave_TaskId, out var queue))
                            {
                                queue.Enqueue(taskSlave);
                            }
                            else
                            {
                                ConcurrentQueue<TaskSlave> newQueue = new ConcurrentQueue<TaskSlave>();
                                newQueue.Enqueue(taskSlave);
                                _successTaskSlaves.TryAdd(taskSlave.TaskSlave_TaskId, newQueue);
                            }
                        }
                        else 
                        {
                            if (_failTaskSlaves.TryGetValue(taskSlave.TaskSlave_TaskId, out var queue))
                            {
                                queue.Enqueue(taskSlave);
                            }
                            else
                            {
                                ConcurrentQueue<TaskSlave> newQueue = new ConcurrentQueue<TaskSlave>();
                                newQueue.Enqueue(taskSlave);
                                _failTaskSlaves.TryAdd(taskSlave.TaskSlave_TaskId, newQueue);
                            }
                        }
                    }
                }
            });
        }

        private RawShippingInformation GetRawShippingInformation(string taskSlaveDataStr)
        {
            var rawShippingInfo = new Dictionary<string, string>();
            var taskSlaveData = JsonConvert.DeserializeObject<TaskSlaveData>(taskSlaveDataStr);
            for (int i = 0; i < taskSlaveData.RowData.Length; i++)
            {
                rawShippingInfo.Add(taskSlaveData.Column[i], taskSlaveData.RowData[i]);
            }

            var serializeObject = JsonConvert.SerializeObject(rawShippingInfo);
            return JsonConvert.DeserializeObject<RawShippingInformation>(serializeObject);
        }
        
        private ShippingInformation GetShippingInformation(RawShippingInformation rawData) 
        {
            return new ShippingInformation()
            {
                ShippingInformation_TrackingNumber  = rawData.ShippingInformation_TrackingNumber,
                ShippingInformation_CollectedFee = rawData.ShippingInformation_CollectedFee,
                ShippingInformation_CollectedMoney = rawData.ShippingInformation_CollectedMoney,
                ShippingInformation_CreatedDateTime = DateTime.Now,
                ShippingInformation_IsDeleted = false,
                ShippingInformation_Memo = rawData.ShippingInformation_Memo,
                ShippingInformation_OriginalTrackingNumber = rawData.ShippingInformation_OriginalTrackingNumber,
                ShippingInformation_OriginalTrackingNumber2 = rawData.ShippingInformation_OriginalTrackingNumber2,
                ShippingInformation_OriginalTrackingNumber3 = rawData.ShippingInformation_OriginalTrackingNumber3,
                ShippingInformation_ProductName = rawData.ShippingInformation_ProductName,
                ShippingInformation_RecipientAddress = rawData.ShippingInformation_RecipientAddress,
                ShippingInformation_RecipientCompany =  rawData.ShippingInformation_RecipientCompany,
                ShippingInformation_RecipientName = rawData.ShippingInformation_RecipientName,
                ShippingInformation_RecipientPhoneNumber = rawData.ShippingInformation_RecipientPhoneNumber,
                ShippingInformation_SenderAddress = rawData.ShippingInformation_SenderAddress,
                ShippingInformation_SenderCompany = rawData.ShippingInformation_SenderCompany,
                ShippingInformation_SenderName = rawData.ShippingInformation_SenderName,
                ShippingInformation_SenderPhoneNumber = rawData.ShippingInformation_SenderPhoneNumber,
                ShippingInformation_SiteId = rawData.ShippingInformation_SiteId,
                ShippingInformation_SiteName = rawData.ShippingInformation_SiteName,
                ShippingInformation_SlaveTrackingNumber =  rawData.ShippingInformation_SlaveTrackingNumber,
                ShippingInformation_Status = "Processing",
                ShippingInformation_StatusUpdatedDateTime = DateTime.Now,
                ShippingInformation_Tax = rawData.ShippingInformation_Tax,
                ShippingInformation_TotalCount = rawData.ShippingInformation_TotalCount,
                ShippingInformation_UnionContractId = rawData.ShippingInformation_UnionContractId,
                ShippingInformation_UnionTrackingNumber= rawData.ShippingInformation_UnionTrackingNumber,
                ShippingInformation_Weight = rawData.ShippingInformation_Weight
            };
        }
    }
}
