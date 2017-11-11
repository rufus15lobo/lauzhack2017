using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LauzHack_Skyscanner.Controllers
{
    public class Friend
    {
        string Name { get; set; }

        public Friend() {
            Name = "Rufus";
        }
    }

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            List<Friend> friendList = new List<Friend>();
            friendList.Add(new Friend());

            return View(friendList);
        }

        public IActionResult AddFriend()
        {
            return RedirectToAction("Index");    
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
    }
}
