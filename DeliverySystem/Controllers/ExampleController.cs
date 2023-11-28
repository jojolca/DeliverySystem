using DeliverySystem.Interface;
using DeliverySystem.Module;
using DeliverySystem.Variables;
using DeliverySystem.Variables.Example;
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
        private ITaskService _taskService;

        //private readonly IHubContext<NotificationHub> _hubContext;

        public ExampleController(IHubContext<SignalRHub> hubContext)
        {
            _taskService = new TaskService(hubContext);
        }

        
        /// <summary>
        /// Get All Data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Create Task
        /// </summary>
        /// <param name="request">Task Data</param>
        [HttpPost("/CreateTask")]
        public async Task<CreateTaskResponseEntitycs> Post([FromBody] CreateTaskRequestEntity request)
        {
            
            return new CreateTaskResponseEntitycs();
        }

        /// <summary>
        /// Get All Data
        /// </summary>
        /// <returns></returns>
        [HttpPost("/SendMsg/{msg}")]
        public async Task Post(string msg)
        {
            await _taskService.SendMsg(msg, "server");
        }
    }
}
