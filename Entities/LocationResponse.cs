using Newtonsoft.Json;
using System.Collections.Generic;

namespace MMC.Entities
{
    public class LocationResponse
    {

        [JsonProperty("post code")]
        public string PostCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("country abbreviation")]
        public string CountryAbbreviation { get; set; }

        [JsonProperty("places")]
        public List<Place> Places { get; set; }
    }

    public class Place
    {
        [JsonProperty("place name")]
        public string PlaceName { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("state abbreviation")]
        public string StateAbbreviation { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }
    }
}
