﻿using System;
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
            //new AudioMessageHandler(),
            new CommandMessageHandler(),
            //new PhotoMessageHandler(),
            new TextMessageHandler(),
            new VoiceMessageHandler(),
            new WallMessageHandler(),
        };

        static List<IUpdatesHandler<IIncomingMessage>> responseHandler = new List<IUpdatesHandler<IIncomingMessage>>()
        {
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
                //todo: separate interface for Task<HandlerResult>
                //handle logic with response to vk
                foreach (var handler in responseHandler)
                {
                    if (handler.CanHandle(message, bot))
                    {
                        var result = await handler.HandleAsync(message, bot);
                        return Ok(result.message);
                    }
                }
                //todo: separate interface for Task 
                //handle bot requests
                foreach (var handler in updatesHandler)
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
