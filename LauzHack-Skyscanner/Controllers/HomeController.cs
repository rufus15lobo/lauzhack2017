using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using SkyScanner.Services;
using SkyScanner.Data;
using SkyScanner.Settings;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Primitives;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace LauzHack_Skyscanner.Controllers
{
    [DataContract]
    public class Friend
    {
        [DataMember]
        [JsonProperty]
        public string SerialNo { get; set; }
        [DataMember]
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        [DataMember]
        public string Origin { get; set; }
        [DataMember]
        [JsonProperty]
        public string Destination { get; set; }
        [DataMember]
        [JsonProperty]
        public bool IsReturnJourney { get; set; }
        [DataMember]
        [JsonProperty]
        public string DepartureDate { get; set; }
        [DataMember]
        [JsonProperty]
        public string ReturnDate { get; set; }
        [DataMember]
        [JsonProperty]
        public string OriginId { get; set; }
        [DataMember]
        [JsonProperty]
        public string DestinationId { get; set; }

        public Friend() {
            Name = "Rufus";
            Origin = "Basel";
            IsReturnJourney = true;
            DepartureDate = new DateTime(2017, 10, 27).ToString();
            ReturnDate = new DateTime(2017, 10, 31).ToString();
        }
    }

    public class Flight
    {
        //TODO: Add destination properties here

        public Flight() 
        {
            //TODO: Constructor to instantiate a destination
        }
    }

    public class FromPlace
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }

        public FromPlace(string id, string label, string value)
        {
            Id = id;
            Label = label;
            Value = value;
        }
    }

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            List<Friend> friendList = new List<Friend>();
            // friendList.Add(new Friend());

            return View(friendList);
        }

        public async Task<JsonResult> AutoComplete(string term)
        {
            var apikey = "ha104652488387353485173355756299";
            var scanner = new Scanner(apikey);
            List<SkyScanner.Data.Location> fromPlace = (await scanner.QueryLocation(term));

            List<FromPlace> fromPlaceJson = new List<FromPlace>();

            foreach (var location in fromPlace) {
                var displayNameExtn = location.PlaceId.Split("-").First();
                var displayName = location.PlaceName + " (" + displayNameExtn + ") - " + location.CountryName;
                FromPlace fromPlaceObj = new FromPlace(location.PlaceId, displayName, displayName);
                fromPlaceJson.Add(fromPlaceObj);
            }

            return Json(fromPlaceJson);
        }

        public IActionResult Flights()
        {
            var friendListFromQueryString = Request.Query.ElementAt(0).Value.ElementAt(0);
            List<Friend> friendList = JsonConvert.DeserializeObject<List<Friend>>(friendListFromQueryString);

            List<Flight> flightList = new List<Flight>();

            return View(new Tuple<IEnumerable<Friend>, IEnumerable<Flight>>(friendList, flightList));    
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
