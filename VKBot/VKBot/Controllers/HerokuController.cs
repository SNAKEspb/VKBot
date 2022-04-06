using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VKBot.Controllers
{
    [Route("api/[controller]")]
    public class HerokuController : ControllerBase
    {
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        static VKBot.VkBotLogic.Services.AwakerService awakerService = new VKBot.VkBotLogic.Services.AwakerService(_logger);

        // GET: api/<controller>
        [HttpGet]
        public string Get()
        {
            return "Awake";
        }

        // GET api/<controller>/true
        [HttpGet("{switcher}")]
        public string Get(bool switcher)
        {
            awakerService.switchTimer(switcher);
            return switcher.ToString();
        }

        
    }
}