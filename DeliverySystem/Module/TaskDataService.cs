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
    public class TaskDataService : ITaskDataService
    {
        private readonly IRepositoryOperater _repository;

        private ConcurrentDictionary<long,int> _processingTasks = new ConcurrentDictionary<long,int>();

        private  ConcurrentQueue<TaskSlave> _waitingToDoTaskSlaves = new ConcurrentQueue<TaskSlave>();

        private ConcurrentDictionary<long, ConcurrentQueue<TaskSlave>> _successTaskSlaves = new ConcurrentDictionary<long, ConcurrentQueue<TaskSlave>>();

        private ConcurrentDictionary<long, ConcurrentQueue<TaskSlave>> _failTaskSlaves = new ConcurrentDictionary<long, ConcurrentQueue<TaskSlave>>();

        private readonly ILog _logger;

        public TaskDataService(IRepositoryOperater repositoryOperater, ILog log)
        {
            _repository = repositoryOperater;
            _logger = log;
        }

        public async Task<long> CreateTasks(CreateTaskRequestEntity data)
        {
            long taskId = 0;

            try 
            {
                ConcurrentBag<TaskSlave> failedTaskSlave = new ConcurrentBag<TaskSlave>();
                ConcurrentBag<TaskSlave> waitingToDoTaskSlave = new ConcurrentBag<TaskSlave>();
                var task = new TaskObject()
                {
                    Task_CreatedDateTime = DateTime.UtcNow.GetTWTime(),
                    Task_CreatedUser = data.User,
                    Task_IsDeleted = false,
                    Task_Status = "Ready",
                    Task_StatusUpdatedDateTime = DateTime.UtcNow.GetTWTime(),
                    Task_UpdatedUser = data.User,
                    Task_UpdatedDateTime = DateTime.UtcNow.GetTWTime(),
                };

                taskId = await _repository.InsertTask(task);

                if (taskId > 0)
                {
                    task.Task_Id = taskId;
                    if (_processingTasks.TryAdd(taskId, data.RowData.Count) == false)
                    {
                        //delete database task
                        return 0;
                    }

                    var rowDataTasks = data.RowData.Select(row => Task.Run(async () =>
                    {
                        var taskSlaveData = new TaskSlaveData()
                        {
                            Column = data.Column,
                            RowData = row,
                        };

                        var taskSlave = new TaskSlave()
                        {
                            TaskSlave_TaskId = taskId,
                            TaskSlave_CreatedDateTime = DateTime.UtcNow.GetTWTime(),
                            TaskSlave_CreatedUser = data.User,
                            TaskSlave_Data = JsonConvert.SerializeObject(taskSlaveData),
                            TaskSlave_IsDeleted = false,
                            TaskSlave_Status = "Ready",
                            TaskSlave_StatusUpdatedDateTime = DateTime.UtcNow.GetTWTime(),
                            TaskSlave_UpdatedUser = data.User,
                            TaskSlave_UpdatedDateTime = DateTime.UtcNow.GetTWTime()
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

                if (failedTaskSlave.Count > 0)
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
            }
            catch (Exception ex) 
            {
                var log = new LogInformation()
                {
                    ObjectType = "TaskDataService_CreateTasks",
                    LogType = "Error",
                    Message = ex.ToString(),
                    IsDeleted = false,
                    CreatedDateTime = DateTime.UtcNow.GetTWTime()
                };
                _logger.AddLog(log);
            }
            
            
            return taskId;
        }

        private void Initial()
        {

        }

        public string GetTaskStatus(long taskId)
        {
            var successCount = _successTaskSlaves.TryGetValue(taskId, out var successTask) ? successTask.Count : 0;
            var failCount = _failTaskSlaves.TryGetValue(taskId, out var failTask) ? failTask.Count : 0;
            var totalTaskCount = _processingTasks.TryGetValue(taskId, out var count) ? count : 0;
            
            string status = "Processing";   
            var isFinish = ((successCount + failCount) == totalTaskCount) && totalTaskCount > 0;
            if(isFinish && failCount > 0) //完成但有錯誤
            {
                status = totalTaskCount == failCount ? "Fail" : "PartialFail";
            }
            else if(isFinish && failCount == 0) //j完成沒錯誤
            {
                status = "Finish";
            }

            return status;
        }

        public RawShippingInformation GetRawShippingInformation(string taskSlaveDataStr)
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

        public ShippingInformation GetShippingInformation(RawShippingInformation rawData, string createdUser) 
        {
            return new ShippingInformation()
            {
                ShippingInformation_TrackingNumber  = rawData.ShippingInformation_TrackingNumber,
                ShippingInformation_CollectedFee = rawData.ShippingInformation_CollectedFee??0,
                ShippingInformation_CollectedMoney = rawData.ShippingInformation_CollectedMoney??0,
                ShippingInformation_CreatedDateTime = DateTime.UtcNow.GetTWTime(),
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
                ShippingInformation_StatusUpdatedDateTime = DateTime.UtcNow.GetTWTime(),
                ShippingInformation_Tax = rawData.ShippingInformation_Tax??0,
                ShippingInformation_TotalCount = rawData.ShippingInformation_TotalCount??0,
                ShippingInformation_UnionContractId = rawData.ShippingInformation_UnionContractId,
                ShippingInformation_UnionTrackingNumber= rawData.ShippingInformation_UnionTrackingNumber,
                ShippingInformation_Weight = rawData.ShippingInformation_Weight??0,
                ShippingInformation_CreatedUser = createdUser
            };
        }

        public double GetSuccessTaskCount(long taskId)
        {
            return _successTaskSlaves.TryGetValue(taskId, out var successTask) ? (double)successTask.Count : 0;
        }

        public double GetFailTaskCount(long taskId)
        {            
            return _failTaskSlaves.TryGetValue(taskId, out var failTask) ? (double)failTask.Count : 0;
        }

        public double GetTotalTaskCount(long taskId)
        {
            return _processingTasks.TryGetValue(taskId, out var count) ? (double)count : 0;
        }

        public List<long> GetProcessingTaskKeys()
        {
            return _processingTasks.Keys.ToList();
        }

        public bool TryDeququeWaitingToDoTaskSlaves(out TaskSlave taskSlave)
        {
            return _waitingToDoTaskSlaves.TryDequeue(out taskSlave);
        }

        public async Task<long> InsertShippingInformation(ShippingInformation dataBaseShippingInformation)
        {
           return await _repository.InsertShippingInformation(dataBaseShippingInformation);
        }

        public async Task<long> InsertShippingLabel(ShippingLabel label)
        {
            return await _repository.InsertShippingLabel(label);
        }

        public async Task AddOrUpdateSuccessTask( TaskSlave taskSlave)
        {
            _successTaskSlaves.AddOrUpdate(taskSlave.TaskSlave_TaskId,
                    key => new ConcurrentQueue<TaskSlave>(new[] { taskSlave }),
                    (key, existingQueue) =>
                    {
                        existingQueue.Enqueue(taskSlave);
                        return existingQueue;
                    });

            await _repository.UpdateTaskSlave("Finish", taskSlave.TaskSlave_Id, string.Empty);
        }

        public async Task AddOrUpdateFailTask(TaskSlave taskSlave)
        {
            _failTaskSlaves.AddOrUpdate(taskSlave.TaskSlave_TaskId,
                    key => new ConcurrentQueue<TaskSlave>(new[] { taskSlave }),
                    (key, existingQueue) =>
                    {
                        existingQueue.Enqueue(taskSlave);
                        return existingQueue;
                    });

            await _repository.UpdateTaskSlave("Fail", taskSlave.TaskSlave_Id, taskSlave.TaskSlave_ErrorMsg);
        }

        public async Task RemoveFinishTask(long taskId, string status)
        {
            await _repository.UpdateTaskStatus(status, taskId);
            _processingTasks.TryRemove(taskId, out _);
            _failTaskSlaves.TryRemove(taskId, out _);
            _successTaskSlaves.TryRemove(taskId, out _);
        }
    }
}
