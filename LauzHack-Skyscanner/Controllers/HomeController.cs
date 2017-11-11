using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LauzHack_Skyscanner.Controllers
{
    public class Friend
    {
        public string Name { get; set; }
        public string Origin { get; set; }
        public bool IsReturnJourney { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }

        public Friend() {
            Name = "Rufus";
            Origin = "Basel";
            IsReturnJourney = true;
            DepartureDate = new DateTime(2017, 10, 27);
            ReturnDate = new DateTime(2017, 10, 31);
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
            return View();    
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
