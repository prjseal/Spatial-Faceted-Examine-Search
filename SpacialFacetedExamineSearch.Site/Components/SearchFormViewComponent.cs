using Microsoft.AspNetCore.Mvc;
using SpacialFacetedExamineSearch.Site.Enums;
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

        public IViewComponentResult Invoke(string url)
        {

            var model = new FacetedSearchModel();
            var searchTerm = QueryStringHelper.GetValueFromQueryString("searchTerm", url);
            model.SearchQuery = new SearchService.SearchQuery() { Phrase = searchTerm };
            model.Latitude = QueryStringHelper.GetValueFromQueryString("lat", url);
            model.Longitude = QueryStringHelper.GetValueFromQueryString("long", url);
            model.RadiusInMiles = int.Parse(QueryStringHelper.GetValueFromQueryString("distance", url, "50"));

            Dictionary<string, Tuple<DisplayType, char>> propertySettings = GetPropertySettings();

            model.FacetSets = FacetHelper.GetFacetSets(url, propertySettings);


            var allResults = _searchService.Search(new FacetedSearchModel() { MaxResults = 1000 });
            var allFacets = FacetHelper.GetFacetItemsFromResults(propertySettings, allResults);

            var distinctFacets = allFacets.Distinct(new FacetItemComparer()).OrderBy(x => x.PropertyAlias).ThenBy(y => y.FacetValue);

            model.PageResults = _searchService.Search(model);

            FacetHelper.SetFacetValues(model, distinctFacets);

            return View("~/Views/Partials/Components/SearchForm/SearchForm.cshtml", model);
        }

        private static Dictionary<string, Tuple<DisplayType, char>> GetPropertySettings()
        {
            Dictionary<string, Tuple<DisplayType, char>> propertyDisplayTypes = new Dictionary<string, Tuple<DisplayType, char>>();
            propertyDisplayTypes.Add("regions", new Tuple<DisplayType, char>(DisplayType.CheckBoxList, ','));
            return propertyDisplayTypes;
        }
    }
}
