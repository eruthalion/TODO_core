using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using todo_gabor.Models;


namespace todo_gabor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskApiController : ControllerBase
    {
        private readonly Models.Model.ModelContainer _model;

        public TaskApiController(Models.Model.ModelContainer model)
        {
            _model = model;
        }

        [HttpGet]
        public string Get()
        {
            Management actMan = new Management(_model);
            List<Model.Task> actTasks = actMan.GetTasks();
                string retval= JsonConvert.SerializeObject(actTasks, Formatting.None, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                });

            return retval;
        }


        [HttpGet("{id}/{forUser}")]
        public string Get(int id, bool forUser)
        {
            Management actMan = new Management(_model);
            List<Model.Task> actTasks = actMan.GetTasks(id,forUser);
            string retval = JsonConvert.SerializeObject(actTasks, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });

            return retval;
        }

        [HttpPost]
        public void Post([FromBody] JObject value)
        {
            string taskOwner = value.GetValue("Users").ToString();
            value.Remove("Users");           
            Models.Model.Task task = value.ToObject<Models.Model.Task>();
            Management actMan = new Management(_model);
            bool retval= actMan.SaveTask(task, taskOwner);


        }
    }
}