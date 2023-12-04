using DeliverySystem.Interface;
using DeliverySystem.Module;
using DeliverySystem.Variables;
using DeliverySystem.Variables.Example;
using DeliverySystem.Variables.Repository;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IEnumerable<GetShippingLabelResponseEntity>> Get(string originalTrackingNumber)
        {
            var rawLabels = await _repository.GetShippingLabel(originalTrackingNumber);

            return rawLabels.Select(x => new GetShippingLabelResponseEntity()
            {
                ShippingLabel_Id = x.ShippingLabel_Id,
                ShippingLabel_BarCode = x.ShippingLabel_BarCode,
                ShippingLabel_SalesOffice = x.ShippingLabel_SalesOffice,
                ShippingLabel_ZipCode = x.ShippingLabel_ZipCode,
                ShippingLabel_ZipCodeVersion = x.ShippingLabel_ZipCodeVersion,
                ShippingLabel_PakageSize = x.ShippingLabel_PakageSize,
                ShippingLabel_CreatedDateTime = x.ShippingLabel_CreatedDateTime,
               ShippingLabel_EstimatedDeliveryDateTime = x.ShippingLabel_EstimatedDeliveryDateTime,
               ShippingLabel_CustomID = x.ShippingLabel_CustomID,
               ShippingLabel_ShippingOriginalTrackingNumber = x.ShippingLabel_ShippingOriginalTrackingNumber,
               ShippingLabel_ShippingCollectedMoney = x.ShippingLabel_ShippingCollectedMoney,
               ShippingLabel_ShippingRecipientAddress = x.ShippingLabel_ShippingRecipientAddress,
               ShippingLabel_ShippingRecipientName = x.ShippingLabel_ShippingRecipientName,
               ShippingLabel_ShippingRecipientPhoneNumber = x.ShippingLabel_ShippingRecipientPhoneNumber,
               ShippingLabel_ShippingSenderAddress =x.ShippingLabel_ShippingSenderAddress,
               ShippingLabel_ShippingSenderCompany = x.ShippingLabel_ShippingSenderCompany,
               ShippingLabel_ShippingSenderName = x.ShippingLabel_ShippingSenderName,
               ShippingLabel_ShippingSenderPhoneNumber = x.ShippingLabel_ShippingSenderPhoneNumber


            }).ToArray();
        }

        /// <summary>
        /// Create Task
        /// </summary>
        /// <param name="request">Task Data</param>
        [HttpPost("/createTask")]
        public async Task<ActionResult<CreateTaskResponseEntitycs>> Post( CreateTaskRequestEntity request)
        {
            var taskId = await _taskService.CreateTasks(request);
            if(taskId > 0)
            {
                return Ok(new CreateTaskResponseEntitycs()
                {
                    TaskId = taskId,
                    Status = 1
                });
            }
            else
            {
                return BadRequest(new CreateTaskResponseEntitycs() { Status = 0});
            }
        }

        ///// <summary>
        ///// Send Message
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost("/SendMsg/{msg}")]
        //public async Task Post(string msg)
        //{
        //    await _taskService.SendMsg(msg, "server");
        //}
    }
}
