using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InteractiveNotifs.Hub.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostBackController : ControllerBase
    {
        // POST: api/PostBack
        [HttpPost]
        public ActionResult Post()
        {
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                string reqBody = reader.ReadToEnd();
                return Ok(reqBody);
            }
        }
    }
}
