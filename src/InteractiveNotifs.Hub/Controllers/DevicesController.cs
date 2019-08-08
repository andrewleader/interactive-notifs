using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InteractiveNotifs.Hub.Models;
using Microsoft.AspNetCore.Mvc;

namespace InteractiveNotifs.Hub.Controllers
{
    public class DevicesController : Controller
    {
        public IActionResult Index()
        {
            return View(new DevicesViewModel()
            {
                Devices = Api.DevicesController.GetDevices()
            });
        }
    }
}