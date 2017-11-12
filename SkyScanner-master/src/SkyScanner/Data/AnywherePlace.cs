using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyScanner.Data
{
    public class AnywherePlace
    {
        public List<Quote> Quotes { get; set; }
        public List<AnyPlace> Places { get; set; }
        public List<Carrier> Carriers { get; set; }
        public List<Currency> Currencies { get; set; }

        public Tuple<AnyPlace, AnyPlace> GetLocation()
        {
            int originId = Quotes.First().OutboundLeg.OriginId;
            int destinationId = Quotes.First().OutboundLeg.DestinationId;
            AnyPlace Origin = null;
            AnyPlace Destination = null;
            foreach(AnyPlace place in Places)
            {
                if (place.PlaceId == originId)
                    Origin = place;
                if (place.PlaceId == destinationId)
                    Destination = place;

                if (Origin != null && Destination != null)
                    break;
            }

            return new Tuple<AnyPlace, AnyPlace>(Origin, Destination);
        }
    }
}
