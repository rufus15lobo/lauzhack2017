using Newtonsoft.Json;
using SkyScanner.Data;
using SkyScanner.Services.Base;
using SkyScanner.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkyScanner.Services
{
    class LocationById : Requester<List<AnywherePlace>>
    {
        private readonly LocationByIdSettings _settings;


        public LocationById(string apiKey, LocationByIdSettings settings)
            : base(apiKey)
        {
            this._settings = settings;
        }

        protected override Func<HttpClient, CancellationToken, Task<HttpResponseMessage>> HttpMethod
        {
            get
            {
                return (client, token) => client.GetAsync(
                    string.Format(
                        "http://partners.api.skyscanner.net/apiservices/browsequotes/v1.0/{0}/{1}/{2}/{3}/{4}/{5}?apiKey={6}",
                        _settings.Market, _settings.Currency, _settings.Locale, _settings.Origin, _settings.Destination, _settings.OutboundPartialDate, ApiKey),
                    token);
            }
        }

        protected override List<AnywherePlace> CreateResponse(string content)
        {
            List<AnywherePlace> places = new List<AnywherePlace>();
            //places.Add(JsonConvert.DeserializeObject<Response>(content, Scanner.JsonSerializerSettings).AnywherePlaces);
            places.Add(JsonConvert.DeserializeObject<AnywherePlace>(content));
            return places;
        }

        internal class Response
        {
            internal AnywherePlace AnywherePlaces { get; set; }
        }
    }
}
