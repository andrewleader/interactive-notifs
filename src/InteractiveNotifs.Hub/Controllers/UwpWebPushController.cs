using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InteractiveNotifs.Hub.Helpers;
using InteractiveNotifs.Hub.Models;
using Microsoft.AspNetCore.Mvc;

namespace InteractiveNotifs.Hub.Controllers
{
    public class UwpWebPushController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UwpWebPushViewModel value)
        {
            await PushNotificationsWeb.SendAsync(new PushNotificationsWeb.Subscription()
            {
                Endpoint = value.EndpointUrl
            }, payload: "Hello");

            return RedirectToAction(nameof(Index));
        }
    }
}