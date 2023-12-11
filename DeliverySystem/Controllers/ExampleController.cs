using DeliverySystem.Interface;
using DeliverySystem.Module;
using DeliverySystem.Variables;
using DeliverySystem.Variables.BaseObject;
using DeliverySystem.Variables.Example;
using DeliverySystem.Variables.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DeliverySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExampleController : ControllerBase
    {
        private ITaskDataService _taskService;

        private IRepositoryOperater _repository;

        public ExampleController(ITaskDataService taskService, IRepositoryOperater repository)
        {
            _taskService = taskService;
            _repository = repository;
        }


        /// <summary>
        /// Get All ShippingInformation
        /// </summary>
        /// <returns></returns>
        [HttpGet("/shippinginformation")]
        public async Task<ActionResult<Response<IEnumerable<ShippingInformation>>>> Get()
        {
            var data = await _repository.GetShippingInformation();
            return Ok(new Response<IEnumerable<ShippingInformation>>()
            {
                Data = data
            });
        }

        /// <summary>
        /// Get ShippingLabel
        /// </summary>
        /// <returns></returns>
        [HttpGet("/shippinglabel/{originalTrackingNumber}")]
        public async Task<ActionResult<Response<GetShippingLabelResponseEntity>>> Get(string originalTrackingNumber)
        {
            var rawLabels = await _repository.GetShippingLabel(originalTrackingNumber);

            if(rawLabels != null)
            {
                return Ok(new Response<GetShippingLabelResponseEntity>()
                {
                    Data = new GetShippingLabelResponseEntity()
                    {
                        ShippingLabel_Id = rawLabels.ShippingLabel_Id,
                        ShippingLabel_BarCode = rawLabels.ShippingLabel_BarCode,
                        ShippingLabel_SalesOffice = rawLabels.ShippingLabel_SalesOffice,
                        ShippingLabel_ZipCode = rawLabels.ShippingLabel_ZipCode,
                        ShippingLabel_ZipCodeVersion = rawLabels.ShippingLabel_ZipCodeVersion,
                        ShippingLabel_PakageSize = rawLabels.ShippingLabel_PakageSize,
                        ShippingLabel_CreatedDateTime = rawLabels.ShippingLabel_CreatedDateTime,
                        ShippingLabel_EstimatedDeliveryDateTime = rawLabels.ShippingLabel_EstimatedDeliveryDateTime,
                        ShippingLabel_CustomID = rawLabels.ShippingLabel_CustomID,
                        ShippingLabel_ShippingOriginalTrackingNumber = rawLabels.ShippingLabel_ShippingOriginalTrackingNumber,
                        ShippingLabel_ShippingCollectedMoney = rawLabels.ShippingLabel_ShippingCollectedMoney,
                        ShippingLabel_ShippingRecipientAddress = rawLabels.ShippingLabel_ShippingRecipientAddress,
                        ShippingLabel_ShippingRecipientName = rawLabels.ShippingLabel_ShippingRecipientName,
                        ShippingLabel_ShippingRecipientPhoneNumber = rawLabels.ShippingLabel_ShippingRecipientPhoneNumber,
                        ShippingLabel_ShippingSenderAddress = rawLabels.ShippingLabel_ShippingSenderAddress,
                        ShippingLabel_ShippingSenderCompany = rawLabels.ShippingLabel_ShippingSenderCompany,
                        ShippingLabel_ShippingSenderName = rawLabels.ShippingLabel_ShippingSenderName,
                        ShippingLabel_ShippingSenderPhoneNumber = rawLabels.ShippingLabel_ShippingSenderPhoneNumber
                    }
                });
            }
            else
            {
                return BadRequest(new Response<GetShippingLabelResponseEntity>()
                { 
                    Data = new GetShippingLabelResponseEntity(),
                    Message = "查無對應資料!"
                });
            }
           
        }

        /// <summary>
        /// Create Task
        /// </summary>
        /// <param name="request">Task Data</param>
        [HttpPost("/createTask")]
        public async Task<ActionResult<Response<CreateTaskResponseEntitycs>>> Post(CreateTaskRequestEntity request)
        {
            var taskId = await _taskService.CreateTasks(request);
            if (taskId > 0)
            {
                return Ok(new Response<CreateTaskResponseEntitycs>()
                {
                    Data = new CreateTaskResponseEntitycs()
                    {
                        TaskId = taskId,
                        Status = 1
                    }
                });
            }
            else
            {
                return BadRequest(new Response<CreateTaskResponseEntitycs>()
                {
                    Data = new CreateTaskResponseEntitycs() { Status = 0 },                    
                    Message = "Task建立失敗"
                });
            }
        }

        /// <summary>
        /// Get TaskSlave 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/getTaskSlaveList/{taskId}")]
        public async Task<ActionResult<Response<IEnumerable<GetTaskSlaveListResponseEntity>>>> Get(long taskId)
        {
            if(taskId <= 0)
            {
                return BadRequest(new Response<IEnumerable<GetTaskSlaveListResponseEntity>>()
                {
                    Data = new List<GetTaskSlaveListResponseEntity>(),
                    Message = "TaskId不合法"
                });
            }

            var taskSlaves = await _repository.GetTaskSlave(taskId);

            return Ok(new Response<IEnumerable<GetTaskSlaveListResponseEntity>>()
            {
                Data = taskSlaves.Select(x => {
                    
                    var rawData = JsonConvert.DeserializeObject<TaskSlaveData>(x.TaskSlave_Data);
                    string trackingNumber = rawData.RowData[2];

                    return new GetTaskSlaveListResponseEntity
                    {
                        OriginalTrackingNumber = trackingNumber,
                        ErrorMessage = x.TaskSlave_ErrorMsg,
                        CreatedDateTime = x.TaskSlave_CreatedDateTime,
                        Status = x.TaskSlave_Status
                    };
                }).ToList()
            });

        }
    }
}
