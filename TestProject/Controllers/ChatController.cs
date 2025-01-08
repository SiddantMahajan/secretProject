using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TestProject.Hubs;

namespace TestProject.Controllers
{
    public class ChatController : Controller
    {
        private readonly IHubContext _hubContext;

        public ChatController()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
        }

        public async Task<ActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SendMessage(string user, string message)
        {
            if (user != null && message != null)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, message);
            }

            return RedirectToAction("Index");
        }
    }
}