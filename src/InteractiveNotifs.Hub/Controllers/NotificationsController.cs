using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InteractiveNotifs.Api;
using InteractiveNotifs.Hub.Models;
using Microsoft.AspNetCore.Mvc;

namespace InteractiveNotifs.Hub.Controllers
{
    public class NotificationsController : Controller
    {
        public IActionResult Index()
        {
            return View(new NotificationViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(NotificationViewModel value)
        {
            await Api.NotificationsController.SendNotification(new Notification()
            {
                AdaptiveBlock = value.AdaptiveBlock
            });
            return RedirectToAction(nameof(Index));
        }
    }
}