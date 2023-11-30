using DeliverySystem.Interface;
using DeliverySystem.Variables;
using DeliverySystem.Variables.Example;
using DeliverySystem.Variables.Hub;
using DeliverySystem.Variables.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeliverySystem.Module
{
    public class TaskService : ITaskService
    {
        private readonly IHubContext<SignalRHub> _hubContext;

        private readonly IRepositoryOperater _repository;

        private readonly IThirdPartyAPIOperater _thirdPartyAPIOperater;

        private ConcurrentDictionary<long,int> _processingTasks = new ConcurrentDictionary<long,int>();

        private  ConcurrentQueue<TaskSlave> _waitingToDoTaskSlaves = new ConcurrentQueue<TaskSlave>();

        private ConcurrentDictionary<long, ConcurrentQueue<TaskSlave>> _successTaskSlaves = new ConcurrentDictionary<long, ConcurrentQueue<TaskSlave>>();

        private ConcurrentDictionary<long, ConcurrentQueue<TaskSlave>> _failTaskSlaves = new ConcurrentDictionary<long, ConcurrentQueue<TaskSlave>>();

        public TaskService(IHubContext<SignalRHub> hubContext, IRepositoryOperater repositoryOperater, IThirdPartyAPIOperater thirdPartyAPIOperater)
        {
            _hubContext = hubContext;
            _repository = repositoryOperater;
            _thirdPartyAPIOperater = thirdPartyAPIOperater;
            GetLabel();
        }

        public async Task SendMsg(string msg, string user)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, $"[{DateTime.Now}]From {user}:{msg}"); 
        }

        public async Task<long> CreateTasks(CreateTaskRequestEntity data)
        {
            ConcurrentBag<TaskSlave> failedTaskSlave = new ConcurrentBag<TaskSlave>();
            ConcurrentBag<TaskSlave> waitingToDoTaskSlave = new ConcurrentBag<TaskSlave>();
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
                if(_processingTasks.TryAdd(taskId, data.RowData.Count) == false)
                {
                    //delete database task
                    return 0;
                }

                var rowDataTasks = data.RowData.Select( row => Task.Run(async()=>
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
                                   
                                   })).ToList();

                await Task.WhenAll(rowDataTasks);
            }
            else 
            {
                // log 建立task失敗
                return 0;
            }

            if(failedTaskSlave.Count > 0)
            {
                _processingTasks.TryRemove(taskId, out _);
                //Todo: delete task DB data、taskSlave DB data
                return 0;
            }

            Parallel.ForEach(waitingToDoTaskSlave, slave =>
            {
                //task建立正常，全部加入待處理中
                _waitingToDoTaskSlaves.Enqueue(slave);
            });
            
            return taskId;
        }

        private void Initial()
        {

        }

        private void GetLabel()
        {
            var task = Task.Run(async () => 
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
                            _successTaskSlaves.AddOrUpdate(taskSlave.TaskSlave_TaskId,
                            key => new ConcurrentQueue<TaskSlave>(new[] { taskSlave }),
                            (key, existingQueue) =>
                            {
                                existingQueue.Enqueue(taskSlave);
                                return existingQueue;
                            });

                            await _repository.UpdateTaskSlaveStatus("Finish", taskSlave.TaskSlave_Id);
                        }
                        else 
                        {
                            _failTaskSlaves.AddOrUpdate(taskSlave.TaskSlave_TaskId,
                            key => new ConcurrentQueue<TaskSlave>(new[] { taskSlave }),
                            (key, existingQueue) =>
                            {
                                existingQueue.Enqueue(taskSlave);
                                return existingQueue;
                            });

                            await _repository.UpdateTaskSlaveStatus("Fail", taskSlave.TaskSlave_Id);
                        }

                        string status = GetTaskStatus(taskSlave.TaskSlave_Id);
                        if (status == "Finish" || status == "Fail" || status == "PartialFail")
                        {
                            await _repository.UpdateTaskSlaveStatus(status, taskSlave.TaskSlave_TaskId);
                            _processingTasks.TryRemove(taskSlave.TaskSlave_Id, out _);
                            _failTaskSlaves.TryRemove(taskSlave.TaskSlave_Id, out _);
                            _successTaskSlaves.TryRemove(taskSlave.TaskSlave_Id, out _);

                            NotifyProcessingPercentage(taskSlave.TaskSlave_Id, 1, status);
                        }
                    }

                    Thread.Sleep(10);
                }
            });

            task.Wait();
        }

        private void NotifyTaskProcessingPercentage()
        {
            var task = Task.Run(() => {
                foreach (var taskId in _processingTasks.Keys)
                {
                    _successTaskSlaves.TryGetValue(taskId, out var successTask);
                    _processingTasks.TryGetValue(taskId, out var totalTaskCount);
                    var status = GetTaskStatus(taskId);
                    if (status == "Processing")
                    {
                        double pct = (double)successTask.Count / (double)totalTaskCount;
                        NotifyProcessingPercentage(taskId, pct, status);
                    }
                    else
                    {
                        NotifyProcessingPercentage(taskId, 1, status);
                    }
                }
            });

            Task.WaitAll(task);
        }

        private string GetTaskStatus(long taskId)
        {
            _successTaskSlaves.TryGetValue(taskId, out var successTask);
            _failTaskSlaves.TryGetValue(taskId, out var failTask);
            _processingTasks.TryGetValue(taskId, out var totalTaskCount);
            
            string status = "Processing";   
            var isFinish = (successTask.Count + failTask.Count) == totalTaskCount;
            if(isFinish && failTask.Count > 0) //完成但有錯誤
            {
                status = totalTaskCount == failTask.Count ? "Fail" : "PartialFail";
            }
            else if(isFinish && failTask.Count == 0) //j完成沒錯誤
            {
                status = "Finish";
            }

            return status;
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

        private async void NotifyProcessingPercentage(long taskId, double percentage, string status)
        {
            var notification = new List<ProcessingPercentageInfo>(){new ProcessingPercentageInfo()
                            {
                                TaskId = taskId,
                                ProcessingPercentage = percentage,
                                Status = status
                            }};
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Server", JsonConvert.SerializeObject(notification));
        }
    }
}
