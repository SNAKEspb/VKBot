using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VKBot.Controllers
{
    [Route("api/[controller]")]
    public class GoogleConfigController : Controller
    {

        // POST api/<controller>
        [HttpPost]
        async public Task<IActionResult> Post()
        {
            try
            {
                var message = await Common.Util.getRawBodyAsync(HttpContext.Request.Body);
                System.IO.File.WriteAllText("key.json", message);
                System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "key.json");
                return Ok("ok");
            }
            catch(Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        async Task<string> getRawBody()
        {
            using (System.IO.StreamReader reader = new System.IO.StreamReader(HttpContext.Request.Body, System.Text.Encoding.UTF8))
            {
                var result = await reader.ReadToEndAsync();
                return result;
            }
        }


    }
}
