using InteractiveNotifs.Hub.Helpers;
using InteractiveNotifs.Hub.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InteractiveNotifs.Hub.Controllers
{
    public class WebPushController : Controller
    {
        public IActionResult Index()
        {
            return View(new WebPushViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Send(WebPushViewModel value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value.SubscriptionJson))
                {
                    throw new ArgumentException("SubscriptionJson must be provided");
                }

                await PushNotificationsWeb.SendAsync(value.SubscriptionJson, payload: value.Message, value.PublicServerKey, value.PrivateServerKey);

                base.ViewData["Response"] = "Successfully sent!";
            }
            catch (Exception ex)
            {
                base.ViewData["Response"] = ex.ToString();
            }

            return View("Index", value);
        }
    }
}
