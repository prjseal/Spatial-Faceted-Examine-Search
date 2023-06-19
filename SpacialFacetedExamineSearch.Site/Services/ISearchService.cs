using Examine;
using SpacialFacetedExamineSearch.Site.Models;

namespace SpacialFacetedExamineSearch.Site.Services
{
    public interface ISearchService
    {
        IEnumerable<ISearchResult> Search(FacetedSearchModel searchModel);
    }
}
