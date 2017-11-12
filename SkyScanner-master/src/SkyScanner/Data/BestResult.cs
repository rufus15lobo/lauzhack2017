using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyScanner.Data
{
    public class BestResult
    {
        public double ActualPrice { get; set; }
        public Uri DeepLink { get; set; }
        public int OriginStation { get; set; }
        public int DestinationStation { get; set; }
        LocalDateTime DepartureTime { get; set; }
        public BestResult(double actualPrice, Uri deepLink, int originStation, int destinationStation, LocalDateTime departureTime)
        {
            ActualPrice = actualPrice;
            DeepLink = deepLink;
            OriginStation = originStation;
            DestinationStation = destinationStation;
            DepartureTime = departureTime;
        }

        public string ToStringNice()
        {
            return "Price: " + ActualPrice + " from: " + OriginStation + " to: " + DestinationStation + " at: " + DepartureTime.ToString();
        }

        public override string ToString()
        {
            return "Price: " + ActualPrice + " from: " + OriginStation + " to: " + DestinationStation + " at: " + DepartureTime.ToString() + " Link: " + DeepLink.ToString();
        }
    }
}
