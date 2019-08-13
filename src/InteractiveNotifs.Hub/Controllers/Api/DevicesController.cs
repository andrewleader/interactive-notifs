using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InteractiveNotifs.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InteractiveNotifs.Hub.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private static readonly List<Device> _devices = new List<Device>();

        public static IEnumerable<Device> GetDevices()
        {
            lock (_devices)
            {
                return _devices.ToArray();
            }
        }

        // POST: api/Devices
        [HttpPost]
        public void Post([FromBody] Device value)
        {
            lock(_devices)
            {
                var samePlatform = _devices.FirstOrDefault(i => i.Type == value.Type);
                if (samePlatform != null)
                {
                    samePlatform.Identifier = value.Identifier;
                }
                else
                {
                    _devices.Add(value);
                }
            }
        }

        [HttpGet]
        public IEnumerable<Device> Get()
        {
            return GetDevices();
        }
    }
}
