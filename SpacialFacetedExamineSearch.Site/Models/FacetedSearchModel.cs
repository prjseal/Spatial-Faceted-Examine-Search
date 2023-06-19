using Examine;
using SpacialFacetedExamineSearch.Site.Facets;

namespace SpacialFacetedExamineSearch.Site.Models
{
    public class FacetedSearchModel
    {
        public string SearchTerm { get; set; }
        public Dictionary<string, Tuple<string, IFacetResult>>? Facets { get; set; }
        public IEnumerable<ISearchResult>? PageResults { get; set; }
        public long TotalItemCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public double PageCount { get; set; }
    }
}
