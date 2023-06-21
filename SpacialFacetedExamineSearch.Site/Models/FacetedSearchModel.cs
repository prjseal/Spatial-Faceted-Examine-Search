using Examine;
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
        public Dictionary<string, Tuple<string, IFacetResult>>? Facets { get; set; }
        public IEnumerable<SearchResultItem>? PageResults { get; set; }
        public long TotalItemCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public double PageCount { get; set; }
    }
}
