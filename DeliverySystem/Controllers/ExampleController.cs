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
        public async Task<IEnumerable<ShippingLabel>> Get(string originalTrackingNumber)
        {
            return await _repository.GetShippingLabel(originalTrackingNumber);
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
