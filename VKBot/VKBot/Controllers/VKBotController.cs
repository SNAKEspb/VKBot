using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VKBot.VkontakteBot.Models;

namespace VKBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VKBotController : ControllerBase
    {
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        static IVityaBot bot = VkBot.getinstanse(_logger);
        static List<IUpdatesHandler<IIncomingMessage>> updatesHandler = new List<IUpdatesHandler<IIncomingMessage>>()
        {
            new TextMessageHandler(),
            //new PhotoMessageHandler(),
            //new AudioMessageHandler(),
            //new WallMessageHandler(),
            new ConfirmationHandler(),
        };

        //// GET: api/<controller>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<controller>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get()
        {
            return Forbid();
        }

        // POST api/values
        [HttpPost]
        async public Task<IActionResult> Post()
        {
            var message = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateMessage>(await getRawBody());
            return await ProcessMessagesAsync(bot, message);
        }

        async Task<IActionResult> ProcessMessagesAsync(IVityaBot bot, IIncomingMessage message)
        {
            try
            {
                foreach (var handler in updatesHandler)
                {
                    if (handler.CanHandle(message, bot))
                    {
                        var result = await handler.HandleAsync(message, bot);
                        return Ok(result.message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during bot process");
                Console.WriteLine(ex);
            }
            return Ok("ok");
        }

        async Task<string> getRawBody()
        {
            using (StreamReader reader = new StreamReader(HttpContext.Request.Body, System.Text.Encoding.UTF8))
            {
                var result = await reader.ReadToEndAsync();
                return result;
            }
        }
    }
}
