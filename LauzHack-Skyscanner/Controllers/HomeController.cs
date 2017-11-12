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
            Destination = "Anywhere";
            IsReturnJourney = true;
            DepartureDate = new DateTime(2017, 10, 27).ToString();
            ReturnDate = new DateTime(2017, 10, 31).ToString();
        }

        public Friend(string name, string origin, string destination, bool isReturnJourney, string departureDate, string returnDate, string originId, string destinationId)
        {
            Name = name;
            Origin = origin;
            Destination = destination;
            IsReturnJourney = isReturnJourney;
            DepartureDate = departureDate;
            ReturnDate = returnDate;
            OriginId = originId;
            DestinationId = destinationId;
        }
    }

    public class Flight
    {
        public string FriendName { get; set; }
        public string DeepLink { get; set; }
        public string Price { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }

        public Flight() 
        {
        }

        public Flight(string friendName, string deepLink, string price, string origin, string destination)
        {
            FriendName = friendName;
            DeepLink = deepLink;
            Price = price;
            Origin = origin;
            Destination = destination;
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
        static int takeValue = 30;
        static int delayMs = 500;
        string apikey = "ha104652488387353485173355756299";


        public IActionResult Index()
        {
            List<Friend> friendList = new List<Friend>();
            return View(friendList);
        }

        private Dictionary<string, Dictionary<string, BestResult>> CollectBestOptions(List<Friend> friendList)
        {
            Dictionary<string, Dictionary<string, BestResult>> bestOptions = new Dictionary<string, Dictionary<string, BestResult>>();
            foreach (var friend in friendList)
            {
                if (friend.Destination == "everywhere" || friend.Destination == "anywhere")
                    try
                    {
                        bestOptions.Add(friend.Name, RufusAnywhere(friend, friend.Destination).Result);
                    }
                    catch (Exception)
                    {
                        Task.Delay(delayMs).Wait();
                        continue;
                    }
                else
                    try
                    {
                        bestOptions.Add(friend.Name, Rufus(friend, friend.Destination).Result);

                    }
                    catch (Exception)
                    {
                        Task.Delay(delayMs).Wait();
                        continue;
                    }
            }
            return bestOptions;
        }

        private Dictionary<string, BestResult> GetBestOption(List<string> bestPlacesForAll, List<Friend> friendsList, Dictionary<string, Dictionary<string, BestResult>> bestOptions)
        {
            Dictionary<string, Friend> friends = new Dictionary<string, Friend>();
            foreach (Friend fr in friendsList)
                friends[fr.Name] = fr;

            Dictionary<string, double> prices = new Dictionary<string, double>();
            // Acumulating best prices
            foreach (string place in bestPlacesForAll)
            {
                foreach (string name in bestOptions.Keys)
                {
                    if (prices.ContainsKey(place))
                        prices[place] += bestOptions[name][place].ActualPrice;
                    else
                        prices[place] = bestOptions[name][place].ActualPrice;
                }
            }
            // Find the minimum price in sum
            string placeWithMinTotalPrice = "";
            double minTotalPrice = 1000000.0;
            foreach (var place in prices)
            {
                if (place.Value < minTotalPrice)
                {
                    minTotalPrice = place.Value;
                    placeWithMinTotalPrice = place.Key;
                }
            }

            Dictionary<string, BestResult> result = new Dictionary<string, BestResult>();
            if (placeWithMinTotalPrice != "")
                foreach (string name in friends.Keys)
                {
                    result.Add(name, bestOptions[name][placeWithMinTotalPrice]);
                }

            return result;
        }

        private List<string> GetBestForAll(List<Friend> friendsList, Dictionary<string, Dictionary<string, BestResult>> bestOptions)
        {
            Dictionary<string, Friend> friends = new Dictionary<string, Friend>();
            foreach (Friend fr in friendsList)
                friends[fr.Name] = fr;

            List<string> commonBestPlaces = new List<string>();
            // Taking the first one in because we want all of the people to be included in the trip (no leftovers)
            foreach (string bo in bestOptions[bestOptions.Keys.First()].Keys)
            {
                int count = 0;
                foreach (string name in friends.Keys)
                {
                    if (bestOptions[name].ContainsKey(bo))
                        count += 1;
                }

                if (count == friends.Keys.Count())
                    commonBestPlaces.Add(bo);
            }

            return commonBestPlaces;
        }

        public async Task<Dictionary<string, BestResult>> Rufus(Friend friend, string destination)
        {
            var scanner = new Scanner(apikey);
            var fromPlace = (await scanner.QueryLocation(friend.Origin)).First();
            var toPlace = (await scanner.QueryLocation(destination)).First();
            DateTime dptDate = DateTime.ParseExact(friend.DepartureDate, "yyyy-MM-dd HH:mm:tt", null);
            LocalDate DepartureDate = new LocalDate(dptDate.Year, dptDate.Month, dptDate.Day);
            DateTime retDate = DateTime.ParseExact(friend.ReturnDate, "yyyy-MM-dd HH:mm:tt", null);
            LocalDate ReturnDate = new LocalDate(dptDate.Year, dptDate.Month, dptDate.Day);

            //Query flights
            var itineraries = await scanner.QueryFlight(
              new FlightQuerySettings(
                new FlightRequestSettings(
                  fromPlace, toPlace,
                  DepartureDate, ReturnDate),
                new FlightResponseSettings(SortType.Price, SortOrder.Ascending)));

            Dictionary<string, BestResult> rufuses = new Dictionary<string, BestResult>();
            if (itineraries.Count == 0)
                return rufuses;

            foreach (Itinerary itinerary in itineraries.Take(takeValue).ToList())
            {
                var estimatedPrice = itinerary.PricingOptions.Min(option => option.Price);

                //Query booking
                var booking = await scanner.QueryBooking(itinerary);
                double actualPrice = (double)booking.BookingOptions
                  .Select(option => option.BookingItems.Sum(item => item.Price))
                  .Min();
                Uri deepLink = new Uri(booking.BookingOptions.Select(option => option.BookingItems.Select(item => item.DeepLink)).First().First().ToString());
                var agent = booking.BookingOptions.Select(option => option.BookingItems.Select(item => item.Agent));
                LegSegment segment = booking.BookingOptions.Select(option => option.BookingItems.Select(item => item.Segments)).First().First().First();

                if (!rufuses.Keys.Contains(toPlace.PlaceName))
                    rufuses.Add(toPlace.PlaceName, new BestResult(actualPrice, deepLink, segment.OriginStation, segment.DestinationStation, segment.DepartureTime));

            }

            return rufuses;
        }

        public async Task<Dictionary<string, BestResult>> RufusAnywhere(Friend friend, string destination)
        {
            var scanner = new Scanner(apikey);
            DateTime dptDate = DateTime.ParseExact(friend.DepartureDate, "yyyy-MM-dd HH:mm:tt", null);
            LocalDate DepartureDate = new LocalDate(dptDate.Year, dptDate.Month, dptDate.Day);
            DateTime retDate = DateTime.ParseExact(friend.ReturnDate, "yyyy-MM-dd HH:mm:tt", null);
            LocalDate ReturnDate = new LocalDate(dptDate.Year, dptDate.Month, dptDate.Day);
            var query = (await scanner.QueryLocationById(friend.Origin, destination, DepartureDate.Year + "-" + DepartureDate.Month + "-" + DepartureDate.Day));

            Dictionary<string, BestResult> rufuses = new Dictionary<string, BestResult>();
            List<Tuple<AnyPlace, AnyPlace>> places = GetAnyPlaces(takeValue, query.First());
            foreach (var best in places)
            {
                var fromPlace = Location.FromString(best.Item1.SkyscannerCode);
                var toPlace = Location.FromString(best.Item2.SkyscannerCode);

                var itineraries = await scanner.QueryFlight(
                  new FlightQuerySettings(
                    new FlightRequestSettings(
                      fromPlace, toPlace,
                      DepartureDate, ReturnDate),
                    new FlightResponseSettings(SortType.Price, SortOrder.Ascending)));

                if (itineraries.Count == 0)
                    continue;

                Itinerary itinerary = itineraries.First();
                var estimatedPrice = itinerary.PricingOptions.Min(option => option.Price);

                //Query booking
                var booking = await scanner.QueryBooking(itinerary);
                double actualPrice = (double)booking.BookingOptions
                  .Select(option => option.BookingItems.Sum(item => item.Price))
                  .Min();
                Uri deepLink = new Uri(booking.BookingOptions.Select(option => option.BookingItems.Select(item => item.DeepLink)).First().First().ToString());
                var agent = booking.BookingOptions.Select(option => option.BookingItems.Select(item => item.Agent));
                LegSegment segment = booking.BookingOptions.Select(option => option.BookingItems.Select(item => item.Segments)).First().First().First();

                if (!rufuses.Keys.Contains(best.Item2.Name))
                    rufuses.Add(best.Item2.Name, new BestResult(actualPrice, deepLink, segment.OriginStation, segment.DestinationStation, segment.DepartureTime));

            }
            return rufuses;
        }

        private List<Tuple<AnyPlace, AnyPlace>> GetAnyPlaces(int numberOfPlaces, AnywherePlace query)
        {
            List<Tuple<AnyPlace, AnyPlace>> res = new List<Tuple<AnyPlace, AnyPlace>>();
            foreach (var qq in query.Quotes)
            {
                int originId = qq.OutboundLeg.OriginId;
                int destinationId = qq.OutboundLeg.DestinationId;
                AnyPlace Origin = null;
                AnyPlace Destination = null;
                foreach (AnyPlace place in query.Places)
                {
                    if (place.PlaceId == originId)
                        Origin = place;
                    if (place.PlaceId == destinationId)
                        Destination = place;

                    if (Origin != null && Destination != null)
                        break;
                }

                res.Add(new Tuple<AnyPlace, AnyPlace>(Origin, Destination));
            }
            return res.Take(numberOfPlaces).ToList();
        }

        private Tuple<AnyPlace, AnyPlace> GetFirstPlaceFromAnywhere(AnywherePlace firstPlace)
        {
            return firstPlace.GetLocation();
        }

        public async Task<JsonResult> AutoComplete(string term)
        {
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

            Dictionary<string, Dictionary<string, BestResult>> bestOptions = CollectBestOptions(friendList);
            // We found all the places where the lists of friends match
            List<string> bestPlacesForAll = GetBestForAll(friendList, bestOptions);

            // Now we pick the cheapest one
            Dictionary<string, BestResult> bestOfAll = GetBestOption(bestPlacesForAll, friendList, bestOptions);

            List<Flight> flightList = new List<Flight>();
            foreach(var instance in bestOfAll)
            {
                flightList.Add(new Flight(instance.Key, instance.Value.DeepLink.ToString(), instance.Value.ActualPrice.ToString(), instance.Value.OriginStation.ToString(), instance.Value.DestinationStation.ToString()));
            }

            return View(new Tuple<IEnumerable<Friend>, IEnumerable<Flight>>(friendList, flightList));    
        }

        public IActionResult Connections()
        {
            var friendListFromQueryString = Request.Query.ElementAt(0).Value.ElementAt(0);
            List<Friend> friendList = JsonConvert.DeserializeObject<List<Friend>>(friendListFromQueryString);

            //TODO: Patrick have your way here

            return View(friendList);
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
