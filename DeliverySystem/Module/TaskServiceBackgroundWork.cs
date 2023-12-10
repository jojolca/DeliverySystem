using DeliverySystem.Interface;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using DeliverySystem.Variables.Hub;
using Newtonsoft.Json;
using System.Collections.Generic;
using DeliverySystem.Variables.Repository;

namespace DeliverySystem.Module
{
    public class TaskServiceBackgroundWork : BackgroundService
    {
        private readonly ITaskDataService _taskDataService;

        private readonly IHubContext<SignalRHub> _hubContext;

        private readonly IThirdPartyAPIOperater _thirdPartyAPIOperater;

        private readonly ILog _logger;

        private readonly string _dirFileName = "TaskServiceBackgroundWork";

        public TaskServiceBackgroundWork(ITaskDataService taskService, IHubContext<SignalRHub> hubContext, IThirdPartyAPIOperater thirdPartyAPIOperater, ILog log)
        {
            _taskDataService = taskService;
            _hubContext = hubContext;
            _thirdPartyAPIOperater = thirdPartyAPIOperater;
            _logger = log;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var longInterval = TimeSpan.FromMilliseconds(100); // 較長的時間間隔
            var shortInterval = TimeSpan.FromMilliseconds(20); // 較短的時間間隔
            var isLongIntervalTask = true;

            while (!stoppingToken.IsCancellationRequested)
            {
                try 
                {

                    if (isLongIntervalTask)
                    {
                        //_logger.LogInformation("長時間間隔背景任務...");

                        // 傳送通知
                        await NotifyTaskProcessingPercentage();

                        await Task.Delay(longInterval, stoppingToken);
                    }
                    else
                    {
                        //_logger.LogInformation("短時間間隔背景任務...");

                        // 詢問label
                        await GetLabel();

                        await Task.Delay(shortInterval, stoppingToken);
                    }

                    isLongIntervalTask = !isLongIntervalTask;
                }
                catch(Exception ex)
                {
                    var log = new LogInformation()
                    {
                        ObjectType = "TaskServiceBackgroundWork_ExecuteAsync",
                        LogType = "Error",
                        Message = ex.ToString(),
                        IsDeleted = false,
                        CreatedDateTime = DateTime.UtcNow.GetTWTime()
                    };
                    _logger.AddLog(log);
                }
            }
        }

        public async Task NotifyTaskProcessingPercentage()
        {
            try 
            {
                var keys = _taskDataService.GetProcessingTaskKeys();
                List<ProcessingPercentageInfo> finisheTask = new List<ProcessingPercentageInfo>();
                foreach (var taskId in keys)
                {
                    var successCount = _taskDataService.GetSuccessTaskCount(taskId);
                    var totalCount = _taskDataService.GetTotalTaskCount(taskId);
                    var status = _taskDataService.GetTaskStatus(taskId);
                    if (status == "Processing")
                    {
                        double pct = successCount / totalCount;
                        NotifyProcessingPercentage(taskId, pct, status);
                    }
                    else
                    {
                        await _taskDataService.RemoveFinishTask(taskId, status);
                        finisheTask.Add(new ProcessingPercentageInfo()
                        {
                            TaskId = taskId,
                            ProcessingPercentage = 1,
                            Status = status,
                            Message = string.Empty
                        });
                    }
                }

                if (finisheTask.Count > 0)
                {
                    NotifyMutipleProcessingPercentage(finisheTask);
                }
            }
            catch(Exception ex)
            {
                var log = new LogInformation()
                {
                    ObjectType = "TaskServiceBackgroundWork_NotifyTaskProcessingPercentage",
                    LogType = "Error",
                    Message = ex.ToString(),
                    IsDeleted = false,
                    CreatedDateTime = DateTime.UtcNow.GetTWTime()
                };
                _logger.AddLog(log);
            }            
        }

        public async Task GetLabel()
        {
            if (_taskDataService.TryDeququeWaitingToDoTaskSlaves(out var taskSlave))
            {
                try 
                {
                    var rawShippingInformation = _taskDataService.GetRawShippingInformation(taskSlave.TaskSlave_Data);
                    var dataBaseShippingInformation = _taskDataService.GetShippingInformation(rawShippingInformation, taskSlave.TaskSlave_CreatedUser);
                    await _taskDataService.InsertShippingInformation(dataBaseShippingInformation);

                    var label = _thirdPartyAPIOperater.GetLabel(rawShippingInformation);
                    var labelId = await _taskDataService.InsertShippingLabel(label);
                    if (labelId > 0)
                    {
                        await _taskDataService.AddOrUpdateSuccessTask(taskSlave);
                    }
                    else
                    {
                        await _taskDataService.AddOrUpdateFailTask(taskSlave);
                    }

                    string status = _taskDataService.GetTaskStatus(taskSlave.TaskSlave_Id);
                    if (status == "Finish" || status == "Fail" || status == "PartialFail")
                    {
                        await _taskDataService.RemoveFinishTask(taskSlave.TaskSlave_Id, status);
                        NotifyProcessingPercentage(taskSlave.TaskSlave_Id, 1, status);
                    }
                }
                catch(Exception ex)
                {
                    var log = new LogInformation()
                    {
                        ObjectType = "TaskServiceBackgroundWork_GetLabel",
                        LogType = "Error",
                        Message = ex.ToString(),
                        IsDeleted = false,
                        CreatedDateTime = DateTime.UtcNow.GetTWTime()
                    };
                    _logger.AddLog(log);

                    //// 資料執行失敗
                    await _taskDataService.AddOrUpdateFailTask(taskSlave);
                }                
            }
        }

        private async void NotifyProcessingPercentage(long taskId, double percentage, string status, string message = "")
        {
            var notification = new List<ProcessingPercentageInfo>(){new ProcessingPercentageInfo()
                            {
                                TaskId = taskId,
                                ProcessingPercentage = percentage,
                                Status = status,
                                Message = message
                            }};
            string notifyMessage = JsonConvert.SerializeObject(notification);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Server", notifyMessage);
        }

        private async void NotifyMutipleProcessingPercentage(List<ProcessingPercentageInfo> data)
        {
            string notifyMessage = JsonConvert.SerializeObject(data);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Server", notifyMessage);
        }
    }
}
