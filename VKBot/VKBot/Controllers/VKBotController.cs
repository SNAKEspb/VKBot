using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VKBot.VkontakteBot.Models;
using VKBot.Common;
using System.Runtime.Caching;


namespace VKBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VKBotController : ControllerBase
    {
        public VKBotController(IVityaBot bot, List<IUpdatesHandler<IIncomingMessage>> updatesHandlers, List<IUpdatesResultHandler<IIncomingMessage>> responseHandlers)
        {
            _bot = bot;
            _updatesHandlers = updatesHandlers;
            _responseHandlers = responseHandlers;
        }
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private IVityaBot _bot;

        private List<IUpdatesHandler<IIncomingMessage>> _updatesHandlers;
        private List<IUpdatesResultHandler<IIncomingMessage>> _responseHandlers;

        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get()
        {
            return Forbid();
        }

        // POST api/values
        [HttpPost]
        public Task<IActionResult> Post([FromBody]object messageBody)
        {
            _logger.Log(NLog.LogLevel.Info, $"Start bot process {messageBody}");
            var process = ProcessMessagesAsync(_bot, messageBody.ToString());
            _logger.Log(NLog.LogLevel.Info, $"End bot process {process}");
            return process;
        }

        async Task<IActionResult> ProcessMessagesAsync(IVityaBot bot, string messageBody)
        {
            try
            {
                var message = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateMessage>(messageBody.ToString());
                //var message = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateMessage>(messageBody);
                //check chache
                //if cache already contains the message, then return ok result, else proceed
                ObjectCache cache = MemoryCache.Default;
                var cacheKey = message.peer_id + message.MessageType + message.from_id + message.date;
                if (cache[cacheKey] != null)
                {
                    _logger.Log(NLog.LogLevel.Info, $"cache key found: {cacheKey}");
                    return Ok("ok");
                }
                cache.Add(cacheKey, message, DateTime.Now.AddMinutes(5));
                //todo: separate interface for Task<HandlerResult>
                //handle logic with response to vk
                foreach (var handler in _responseHandlers)
                {
                    if (handler.CanHandle(message, bot))
                    {
                        var result = await handler.HandleAsync(message, bot);
                        return Ok(result.message);
                    }
                }
                //todo: separate interface for Task 
                //handle bot requests
                foreach (var handler in _updatesHandlers)
                {
                    if (handler.CanHandle(message, bot))
                    {
                        handler.HandleAsync(message, bot);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(NLog.LogLevel.Error, ex, "Error during bot process");
                return Ok(ex.ToString());
            }
            return Ok("ok");
        }

        string getRawBody()
        {
            using (StreamReader reader = new StreamReader(HttpContext.Request.Body, System.Text.Encoding.UTF8))
            {
                var result = reader.ReadToEnd();
                return result;
            }
        }
    }
}
