// Copyright (c) 2015-2016 Tamas Vajk. All Rights Reserved. Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using Newtonsoft.Json;
using NodaTime;
using SkyScanner.Data.Base;

namespace SkyScanner.Data
{
    public class LegSegment
    {
        internal int Id { get; set; }
        public int OriginStation { get; set; }
        public int DestinationStation { get; set; }
        [JsonProperty("DepartureDateTime")]
        public LocalDateTime DepartureTime { get; set; }
        [JsonProperty("ArrivalDateTime")]
        public LocalDateTime ArrivalTime { get; internal set; }
        /// <summary>
        /// The duration in minutes
        /// </summary>
        public int Duration { get; internal set; }
        public JourneyMode JourneyMode { get; internal set; }
        public Directionality Directionality { get; internal set; }

        [JsonIgnore]
        public FlightInfo Flight => new FlightInfo
        {
            CarrierId = CarrierId,
            FlightNumber = FlightNumber,
            ContainerResponse = ContainerResponse
        };

        internal string FlightNumber { get; set; }
        [JsonProperty("Carrier")]
        internal int CarrierId { get; set; }
        [JsonIgnore]
        internal IContainerResponse ContainerResponse { get; set; }
        [JsonIgnore]
        public Place Origin
        {
            get { return ContainerResponse.Places.FirstOrDefault(place => place.Id == OriginStation); }
        }
        [JsonIgnore]
        public Place Destination
        {
            get { return ContainerResponse.Places.FirstOrDefault(place => place.Id == DestinationStation); }
        }
    }
}