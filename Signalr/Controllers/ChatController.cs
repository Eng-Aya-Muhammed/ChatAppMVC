using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SignalR.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Group()
        {
            return View();
        }
        [Authorize]
        public IActionResult chat()
        {
            return View();
        }
    }


}
