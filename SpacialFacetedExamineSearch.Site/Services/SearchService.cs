using Examine;
using Microsoft.AspNetCore.Mvc;
using SpacialFacetedExamineSearch.Site.Models;
using System.Collections.Specialized;
using System.Net;
using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.Examine;
using System.Net;

using Examine;

using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.Examine;

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
            var model = new FacetedSearchModel();

            SpatialContext ctx = SpatialContext.GEO;

            ISearchResults? results = null;
            if (_examineManager.TryGetIndex("LocationsIndex", out IIndex? index))
            {
                var query = index
                    .Searcher
                    .CreateQuery()
                    .NativeQuery($"+__IndexType:locationItems");

                if(!string.IsNullOrWhiteSpace(searchModel?.SearchTerm ?? ""))
                {
                    query.And()
                    .GroupedAnd("name".Split(','), searchModel!.SearchTerm);
                }

                results = query.Execute();
            }

            return results != null && results.Any() ? results.ToList() : Enumerable.Empty<ISearchResult>();
        }
    }
}
