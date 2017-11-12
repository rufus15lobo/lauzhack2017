using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyScanner.Data
{
    public class Quote
    {
        public string QuoteId { get; set; }
        public double MinPrice { get; set; }
        public bool Direct { get; set; }
        public QuoteLeg OutboundLeg { get; set; }
        public DateTime QuoteDateTime { get; set; }
    }
}
