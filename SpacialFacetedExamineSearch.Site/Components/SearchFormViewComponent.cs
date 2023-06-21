using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using SpacialFacetedExamineSearch.Site.Helpers;
using SpacialFacetedExamineSearch.Site.Models;
using SpacialFacetedExamineSearch.Site.Services;

namespace SpacialFacetedExamineSearch.Site.Components
{
    [ViewComponent(Name = "SearchForm")]
    public class SearchFormViewComponent : ViewComponent
    {

        private readonly ISearchService _searchService;

        public SearchFormViewComponent(ISearchService searchService)
        {
            _searchService = searchService;
        }

        public IViewComponentResult Invoke(string url) {

            var model = new FacetedSearchModel();
            var searchTerm = QueryStringHelper.GetValueFromQueryString("searchTerm", url);
            model.SearchQuery = new SearchService.SearchQuery() { Phrase = searchTerm};
            model.Latitude = QueryStringHelper.GetValueFromQueryString("lat", url);
            model.Longitude = QueryStringHelper.GetValueFromQueryString("long", url);
            model.RadiusInMiles = int.Parse(QueryStringHelper.GetValueFromQueryString("distance", url, "50"));
            model.PageResults = _searchService.Search(model);

            return View("~/Views/Partials/Components/SearchForm/SearchForm.cshtml", model);
        }
    }
}
