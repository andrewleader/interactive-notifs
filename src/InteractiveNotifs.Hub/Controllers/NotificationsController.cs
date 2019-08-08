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
            return View();
        }

        [HttpPost]
        public IActionResult Create(NotificationViewModel value)
        {
            Api.NotificationsController.SendNotification(new Notification()
            {
                Title = value.Title
            });
            return RedirectToAction(nameof(Index));
        }
    }
}