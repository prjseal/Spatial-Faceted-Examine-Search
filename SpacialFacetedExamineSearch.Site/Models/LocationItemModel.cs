﻿using Newtonsoft.Json.Linq;

namespace SpacialFacetedExamineSearch.Site.Models
{
    public class LocationItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public IEnumerable<JObject> Locations { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string Regions { get; set; }
    }
}
