using InteractiveNotifs.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InteractiveNotifs.Hub.Models
{
    public class DevicesViewModel
    {
        public IEnumerable<Device> Devices { get; set; }
    }
}
