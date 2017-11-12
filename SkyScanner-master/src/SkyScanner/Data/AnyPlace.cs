using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyScanner.Data
{
    public class AnyPlace
    {
        public int PlaceId { get; set; }
        public string IataCode { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string SkyscannerCode { get; set; }
        public string CityName { get; set; }
        public string CityId { get; set; }
        public string CountryName { get; set; }
    }
}
