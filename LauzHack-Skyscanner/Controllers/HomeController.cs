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

        public Friend()
        {
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

    public class Destination
    {
        //TODO: Add destination properties here
        public SkyScanner.Data.Location Dest { get; set; }
        public Destination(SkyScanner.Data.Location dest)
        {
            Dest = dest;
            //TODO: Constructor to instantiate a destination
        }
    }
    public class SegmentCheck
    {
        public SkyScanner.Booking.BookingResponse SegBooking { get; set; }
        public List<LegSegment> SegSegments { get; set; }
        public Friend SegFriend { get; set; }
        public SegmentCheck(SkyScanner.Booking.BookingResponse booking, List<LegSegment> segments, Friend friend)
        {
            SegBooking = booking;
            SegSegments = segments;
            SegFriend = friend;
        }
        public SegmentCheck()
        {
        }
    }
    public class Flight1
    {
        //TODO: Add flights properties here
        public Friend flightFriend { get; set; }
        public Destination flightDestination { get; set; }
        List<LocalDate> departure { get; set; }
        List<LocalDate> arrival { get; set; }
        public Flight1(Friend friend, Destination destination)
        {
            flightFriend = friend;
            flightDestination = destination;
            //TODO: Constructor to instantiate a flights


        }
        static string apikey = "ha104652488387353485173355756299";


        public async Task<List<SegmentCheck>> scan()
        {
            var scanner = new Scanner(apikey);
            //Query flights
            DateTime dptDate = DateTime.ParseExact(flightFriend.DepartureDate, "yyyy-MM-dd", null);
            LocalDate DepartureDate = new LocalDate(dptDate.Year, dptDate.Month, dptDate.Day);

            var itineraries = await scanner.QueryFlight(
              new FlightQuerySettings(
                new FlightRequestSettings(
                  SkyScanner.Data.Location.FromString(flightFriend.OriginId), flightDestination.Dest,
                  DepartureDate),
                new FlightResponseSettings(SortType.Price, SortOrder.Ascending)));

            List<SegmentCheck> returnSegments = new List<SegmentCheck>();
            foreach (var itinerary in itineraries)
            {
                var booking = await scanner.QueryBooking(itinerary);

                SegmentCheck segments = new SegmentCheck(booking, itinerary.OutboundLeg.Segments, flightFriend);
                returnSegments.Add(segments);
            }
            return returnSegments;
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
        static int takeValue = 5;
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
                        bestOptions.Add(friend.Name, GoAnywhere(friend, friend.Destination).Result);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                        Task.Delay(delayMs).Wait();
                        continue;
                    }
                else
                    try
                    {
                        bestOptions.Add(friend.Name, Go(friend, friend.DestinationId.Split('-')[0]).Result);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
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

        public async Task<Dictionary<string, BestResult>> Go(Friend friend, string destination)
        {
            var scanner = new Scanner(apikey);
            var fromPlace = (await scanner.QueryLocation(friend.OriginId.Split('-')[0])).First();
            var toPlace = (await scanner.QueryLocation(destination)).First();
            DateTime dptDate = DateTime.ParseExact(friend.DepartureDate, "yyyy-MM-dd", null);
            LocalDate DepartureDate = new LocalDate(dptDate.Year, dptDate.Month, dptDate.Day);
            DateTime retDate = DateTime.ParseExact(friend.ReturnDate, "yyyy-MM-dd", null);
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

        public async Task<Dictionary<string, BestResult>> GoAnywhere(Friend friend, string destination)
        {
            var scanner = new Scanner(apikey);
            DateTime dptDate = DateTime.ParseExact(friend.DepartureDate, "yyyy-MM-dd", null);
            LocalDate DepartureDate = new LocalDate(dptDate.Year, dptDate.Month, dptDate.Day);
            DateTime retDate = DateTime.ParseExact(friend.ReturnDate, "yyyy-MM-dd", null);
            LocalDate ReturnDate = new LocalDate(dptDate.Year, dptDate.Month, dptDate.Day);
            var query = (await scanner.QueryLocationById(friend.OriginId.Split('-')[0], destination, DepartureDate.Year + "-" + DepartureDate.Month + "-" + DepartureDate.Day));

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

            foreach (var location in fromPlace)
            {
                var displayNameExtn = location.PlaceId.Split("-").First();
                var displayName = location.PlaceName + " (" + displayNameExtn + ") - " + location.CountryName;
                FromPlace fromPlaceObj = new FromPlace(location.PlaceId, displayName, displayName);
                fromPlaceJson.Add(fromPlaceObj);
            }

            return Json(fromPlaceJson);
        }

        public JsonResult Flights()
        {
            var friendListFromQueryString = Request.Query.ElementAt(0).Value.ElementAt(0);
            List<Friend> friendList = JsonConvert.DeserializeObject<List<Friend>>(friendListFromQueryString);

            Dictionary<string, Dictionary<string, BestResult>> bestOptions = CollectBestOptions(friendList);
            // We found all the places where the lists of friends match
            List<string> bestPlacesForAll = GetBestForAll(friendList, bestOptions);

            // Now we pick the cheapest one
            Dictionary<string, BestResult> bestOfAll = GetBestOption(bestPlacesForAll, friendList, bestOptions);

            List<Flight> flightList = new List<Flight>();
            foreach (var instance in bestOfAll)
            {
                flightList.Add(new Flight(instance.Key, instance.Value.DeepLink.ToString(), instance.Value.ActualPrice.ToString(), instance.Value.OriginStation.ToString(), instance.Value.DestinationStation.ToString()));
            }

            //List<Flight> flightList = new List<Flight>();
            //flightList.Add(new Flight("123", "shit", "123", "so", "os"));

            return Json(flightList);
        }

        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        public JsonResult Connections()
        {
            var friendListFromQueryString = Request.Query.ElementAt(0).Value.ElementAt(0);
            List<Friend> friendList = JsonConvert.DeserializeObject<List<Friend>>(friendListFromQueryString);

            bool three = true;
            bool four = true;
            //List<Friend> friendList = new List<Friend>();
            int numDudes = friendList.Count;
            if (numDudes > 4) numDudes = 4;

            for (int i = 0; i < 4; i++)
            {
                friendList.Add(new Friend());
            }
            //TODO: Read list of friends from request parameters into List

            Destination dest = new Destination(SkyScanner.Data.Location.FromString("LED"));
            Destination dest2 = new Destination(SkyScanner.Data.Location.FromString("HKG"));
            //TODO: Logic to get flight details
            List<Flight1> flightsList = new List<Flight1>();
            for (int i = 0; i < 4; i++)
            {
                flightsList.Add(new Flight1(friendList[0], dest));
            }
            //foreach (Friend friend in friendList)
            //{
            flightsList[0] = (new Flight1(friendList[0], dest));
            flightsList[1] = (new Flight1(friendList[1], dest));
            flightsList[2] = (new Flight1(friendList[2], dest));
            flightsList[3] = (new Flight1(friendList[3], dest));
            //}
            List<List<SegmentCheck>> allSegments = new List<List<SegmentCheck>>();
            for (int i = 0; i < 4; i++)
            {
                allSegments.Add(new List<SegmentCheck>());
            }
            Parallel.For(0, numDudes, index =>
            {
                List<SegmentCheck> segments = flightsList[index].scan().Result;
                for (int i = 0; i < numDudes; i++)
                {
                    if (segments.First().SegFriend.Name == friendList[i].Name)
                    {
                        allSegments[i] = segments;
                    }
                }


            });

            SegmentCheck dummy = new SegmentCheck();
            List<Tuple<SegmentCheck, SegmentCheck, SegmentCheck, SegmentCheck>> tuples = new List<Tuple<SegmentCheck, SegmentCheck, SegmentCheck, SegmentCheck>>();
            foreach (var x in allSegments[0])
            {
                foreach (var y in allSegments[1])
                {
                    if (!allSegments[2].Any())
                    {
                        three = false;
                        four = false;
                        foreach (var seg1 in x.SegSegments)
                        {
                            foreach (var seg2 in y.SegSegments)
                            {
                                if (seg1.Flight.FlightNumber == seg2.Flight.FlightNumber)
                                {
                                    tuples.Add(Tuple.Create(x, y, dummy, dummy));
                                }
                            }
                        }
                    }
                    foreach (var z in allSegments[2])
                    {
                        if (!allSegments[3].Any())
                        {
                            four = false;
                            foreach (var seg1 in x.SegSegments)
                            {
                                foreach (var seg2 in y.SegSegments)
                                {
                                    foreach (var seg3 in z.SegSegments)
                                    {
                                        if (seg1.Flight.FlightNumber == seg2.Flight.FlightNumber)
                                        {
                                            if (seg2.Flight.FlightNumber == seg3.Flight.FlightNumber)
                                            {
                                                tuples.Add(Tuple.Create(x, y, z, dummy));
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        foreach (var w in allSegments[3])
                        {
                            foreach (var seg1 in x.SegSegments)
                            {
                                foreach (var seg2 in y.SegSegments)
                                {
                                    if (!z.SegSegments.Any())
                                    {
                                        if (seg1.Flight.FlightNumber == seg2.Flight.FlightNumber)
                                        {
                                            tuples.Add(Tuple.Create(x, y, dummy, dummy));
                                        }
                                    }
                                    foreach (var seg3 in z.SegSegments)
                                    {
                                        if (!w.SegSegments.Any())
                                        {
                                            if (seg1.Flight.FlightNumber == seg2.Flight.FlightNumber)
                                            {
                                                if (seg2.Flight.FlightNumber == seg3.Flight.FlightNumber)
                                                {
                                                    tuples.Add(Tuple.Create(x, y, z, dummy));
                                                }
                                            }
                                        }
                                        foreach (var seg4 in w.SegSegments)
                                        {
                                            if (seg1.Flight.FlightNumber == seg2.Flight.FlightNumber)
                                            {
                                                if (seg2.Flight.FlightNumber == seg3.Flight.FlightNumber)
                                                {
                                                    if (seg3.Flight.FlightNumber == seg4.Flight.FlightNumber)
                                                    {
                                                        tuples.Add(Tuple.Create(x, y, z, w));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            Tuple<SegmentCheck, SegmentCheck, SegmentCheck, SegmentCheck> finalTuple = tuples[0];
            double currSum = 10000000000;
            double lowest1 = 10000000000;
            double lowest2 = 10000000000;
            double lowest3 = 10000000000;
            double lowest4 = 10000000000;
            foreach (Tuple<SegmentCheck, SegmentCheck, SegmentCheck, SegmentCheck> tup in tuples)
            {
                double price1 = (double)tup.Item1.SegBooking.Itinerary.PricingOptions.First().Price;
                double price2 = (double)tup.Item2.SegBooking.Itinerary.PricingOptions.First().Price;
                double price3 = 0;
                double price4 = 0;
                if (three)
                {
                    price3 = (double)tup.Item3.SegBooking.Itinerary.PricingOptions.First().Price;
                }
                if (four)
                {
                    price4 = (double)tup.Item4.SegBooking.Itinerary.PricingOptions.First().Price;
                }

                double sum = price1 + price2 + price3 + price4;
                if (sum < currSum)
                {
                    currSum = sum;
                    finalTuple = tup;
                }
                if (price1 < lowest1)
                {
                    lowest1 = price1;
                }
                if (price2 < lowest2)
                {
                    lowest2 = price2;
                }
                if (price3 < lowest3)
                {
                    lowest3 = price3;
                }
                if (price4 < lowest4)
                {
                    lowest4 = price4;
                }
            }
            List<double> priceList = new List<double>();
            priceList.Add(lowest1);
            priceList.Add(lowest2);
            priceList.Add(lowest3);
            priceList.Add(lowest4);
            /*Console.WriteLine("Friend 1 lowest price: " + lowest1 + " Friend 2 lowest price: " + lowest2 + " Friend 3 lowest price: " + lowest3 + " Friend 4 lowest price: " + lowest4 + "lowest sum: " + currSum);
            Console.WriteLine(finalTuple.Item1.SegFriend.Name + ": ");
            Console.WriteLine(finalTuple.Item1.SegBooking.BookingOptions.First().BookingItems.First().DeepLink);
            Console.WriteLine(finalTuple.Item2.SegFriend.Name + ": ");
            Console.WriteLine(finalTuple.Item2.SegBooking.BookingOptions.First().BookingItems.First().DeepLink);
            */
            List<string> linkList = new List<string>();
            List<string> nameList = new List<string>();
            List<string> origList = new List<string>();
            linkList.Add(finalTuple.Item1.SegBooking.BookingOptions.First().BookingItems.First().DeepLink.ToString());
            linkList.Add(finalTuple.Item2.SegBooking.BookingOptions.First().BookingItems.First().DeepLink.ToString());
            nameList.Add(finalTuple.Item1.SegFriend.Name);
            nameList.Add(finalTuple.Item2.SegFriend.Name);
            origList.Add(finalTuple.Item1.SegFriend.Origin);
            origList.Add(finalTuple.Item2.SegFriend.Origin);
            if (three)
            {
                linkList.Add(finalTuple.Item3.SegBooking.BookingOptions.First().BookingItems.First().DeepLink.ToString());
                nameList.Add(finalTuple.Item3.SegFriend.Name);
                origList.Add(finalTuple.Item3.SegFriend.Origin);
            }
            if (four)
            {
                linkList.Add(finalTuple.Item4.SegBooking.BookingOptions.First().BookingItems.First().DeepLink.ToString());
                nameList.Add(finalTuple.Item4.SegFriend.Name);
                origList.Add(finalTuple.Item4.SegFriend.Origin);
            }


            List<Flight> connectionList = new List<Flight>();
            for (int i = 0; i < numDudes; i++)
            {
                Flight reFlight = new Flight();
                reFlight.DeepLink = linkList[i];
                reFlight.Destination = finalTuple.Item1.SegFriend.Destination;
                reFlight.FriendName = nameList[i];
                reFlight.Origin = origList[i];
                reFlight.Price = priceList[i].ToString();
                connectionList.Add(reFlight);
            }

            return Json(connectionList);
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
