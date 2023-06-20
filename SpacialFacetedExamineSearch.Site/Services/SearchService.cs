using Examine;
using Lucene.Net.QueryParsers.Surround.Query;
using SpacialFacetedExamineSearch.Site.Models;

namespace SpacialFacetedExamineSearch.Site.Services
{
    public class SearchService : ISearchService
    {
        private readonly IExamineManager _examineManager;

        public SearchService(IExamineManager examineManager)
        {
            _examineManager = examineManager;
        }

        public IEnumerable<ISearchResult>? Search(FacetedSearchModel searchModel)
        {
            ISearchResults? results = null;
            if (_examineManager.TryGetIndex("LocationsIndex", out IIndex? index))
            {
                var searchFields = "name";

                var query = index
                    .Searcher
                    .CreateQuery()
                    .NativeQuery($"+__IndexType:locationItems");

                if(!string.IsNullOrWhiteSpace(searchModel?.SearchTerm ?? ""))
                {
                    query.And()
                    .GroupedAnd(searchFields.Split(','), searchModel!.SearchTerm);
                }

                results = query.Execute();
            }

            return results != null && results.Any() ? results.ToList() : Enumerable.Empty<ISearchResult>();
        }
    }
}
