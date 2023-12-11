using DeliverySystem.Interface;
using DeliverySystem.Module;
using DeliverySystem.Variables;
using DeliverySystem.Variables.BaseObject;
using DeliverySystem.Variables.Example;
using DeliverySystem.Variables.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
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
        public async Task<IEnumerable<ShippingInformation>> Get()
        {
            return await _repository.GetShippingInformation();
        }

        /// <summary>
        /// Get ShippingLabel
        /// </summary>
        /// <returns></returns>
        [HttpGet("/shippinglabel/{originalTrackingNumber}")]
        public async Task<ActionResult<GetShippingLabelResponseEntity>> Get(string originalTrackingNumber)
        {
            var rawLabels = await _repository.GetShippingLabel(originalTrackingNumber);

            if(rawLabels != null)
            {
                return new GetShippingLabelResponseEntity()
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
                };
            }
            else
            {
                return BadRequest(new GetShippingLabelResponseEntity() { Message = "查無對應資料!"});
            }
           
        }

        /// <summary>
        /// Create Task
        /// </summary>
        /// <param name="request">Task Data</param>
        [HttpPost("/createTask")]
        public async Task<ActionResult<CreateTaskResponseEntitycs>> Post(CreateTaskRequestEntity request)
        {
            var taskId = await _taskService.CreateTasks(request);
            if (taskId > 0)
            {
                return Ok(new CreateTaskResponseEntitycs()
                {
                    TaskId = taskId,
                    Status = 1
                });
            }
            else
            {
                return BadRequest(new CreateTaskResponseEntitycs() { Status = 0, Message ="Task建立失敗" });
            }
        }

        /// <summary>
        /// Get TaskSlave 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/getTaskSlaveList/{taskId}")]
        public async Task<ActionResult<GetShippingLabelResponseEntity>> Get(long taskId)
        {

        }
    }
}
