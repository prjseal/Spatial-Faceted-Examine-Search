using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpacialFacetedExamineSearch.Site.EqulaityComparers;
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

            var allResults = _searchService.Search(new FacetedSearchModel() { MaxResults = 1000 });
            var allFacets = FacetHelper.GetFacetItemsFromResults(new string[] { "languages" }, allResults);

            var distinctFacets = allFacets.Distinct(new FacetItemComparer());

            var language = QueryStringHelper.GetValueFromQueryString("language", url);
            model.SelectedLanguages = QueryStringHelper.GetValueFromQueryString("language", url)?.Split(',') ?? new string[] { } ;

            model.LanguageOptions = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "English", Value = "english", Selected = model.SelectedLanguages.Contains("english") },
                new SelectListItem() { Text = "German", Value = "german", Selected = model.SelectedLanguages.Contains("german") },
                new SelectListItem() { Text = "Spanish", Value = "spanish", Selected = model.SelectedLanguages.Contains("spanish") },
                new SelectListItem() { Text = "French", Value = "french", Selected = model.SelectedLanguages.Contains("french") }
            };

            return View("~/Views/Partials/Components/SearchForm/SearchForm.cshtml", model);
        }
    }
}
