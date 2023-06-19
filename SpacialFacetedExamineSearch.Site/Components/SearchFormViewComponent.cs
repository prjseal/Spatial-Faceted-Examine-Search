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
            model.SearchTerm = QueryStringHelper.GetValueFromQueryString("query", url);

            model.PageResults = _searchService.Search(model);

            return View("~/Views/Partials/Components/SearchForm.cshtml", model);
        }
    }
}
