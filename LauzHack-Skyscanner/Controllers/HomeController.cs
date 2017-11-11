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

    public class Destination
    {
        //TODO: Add destination properties here

        public Destination() 
        {
            //TODO: Constructor to instantiate a destination
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

        public List<string> AutoComplete()
        {
            //TODO: Read request parameters to get input string for Origin Destination
            var origin = "";

            List<string> originList = new List<string>();
            originList.Add("Response1");
            originList.Add("Response2");
            originList.Add("Response3");

            //TODO: Method call to SkyScanner here

            return originList;
        }

        public IActionResult Destinations()
        {
            List<Friend> friendList = new List<Friend>();
            //TODO: Read list of friends from request parameters into List
            friendList.Add(new Friend());
            friendList.Add(new Friend());
            friendList.Add(new Friend());

            List<Destination> destinationList = new List<Destination>();
            destinationList.Add(new Destination());
            destinationList.Add(new Destination());
            destinationList.Add(new Destination());

            return View(new Tuple<IEnumerable<Friend>, IEnumerable<Destination>>(friendList, destinationList));    
        }

        public IActionResult TestRunPythonScript()
        {
            ViewData["Output"] = "Put output from python as a string here";
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
