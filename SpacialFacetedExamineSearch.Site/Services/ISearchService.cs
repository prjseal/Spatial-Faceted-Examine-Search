using Examine;
using SpacialFacetedExamineSearch.Site.Models;
using static SpacialFacetedExamineSearch.Site.Services.SearchService;

namespace SpacialFacetedExamineSearch.Site.Services
{
    public interface ISearchService
    {
        IEnumerable<ISearchResult>? AllResults();
        IEnumerable<SearchResultItem> Search(FacetedSearchModel searchModel);
    }
}
