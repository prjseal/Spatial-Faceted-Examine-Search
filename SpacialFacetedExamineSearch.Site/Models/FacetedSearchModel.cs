﻿using Examine;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpacialFacetedExamineSearch.Site.Facets;
using static SpacialFacetedExamineSearch.Site.Services.SearchService;

namespace SpacialFacetedExamineSearch.Site.Models
{
    public class FacetedSearchModel
    {
        public SearchQuery SearchQuery { get; set; }
        public string SearchTerm { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public List<FacetSet>? FacetSets { get; set; }
        public IEnumerable<SearchResultItem>? PageResults { get; set; }
        public long TotalItemCount { get; set; }
        public int MaxResults { get; set; }
        public int RadiusInMiles { get; set; }
        public bool RestrictResultsToDistance { get; set; }
    }
}
