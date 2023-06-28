using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpacialFacetedExamineSearch.Site.Enums;
using SpacialFacetedExamineSearch.Site.EqulaityComparers;
using SpacialFacetedExamineSearch.Site.Facets;
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

            Dictionary<string, DisplayType> propertyDisplayTypes = GetPropertyDisplayTypes();

            model.FacetSets = GetFacetSets(url, propertyDisplayTypes);


            var allResults = _searchService.Search(new FacetedSearchModel() { MaxResults = 1000 });
            var allFacets = FacetHelper.GetFacetItemsFromResults(propertyDisplayTypes.Select(x => x.Key), allResults);

            var distinctFacets = allFacets.Distinct(new FacetItemComparer()).OrderBy(x => x.PropertyAlias).ThenBy(y => y.FacetValue);

            model.PageResults = _searchService.Search(model);

            SetFacetValues(model, distinctFacets);

            return View("~/Views/Partials/Components/SearchForm/SearchForm.cshtml", model);
        }

        private static Dictionary<string, DisplayType> GetPropertyDisplayTypes()
        {
            Dictionary<string, DisplayType> propertyDisplayTypes = new Dictionary<string, DisplayType>();
            propertyDisplayTypes.Add("languages", DisplayType.CheckBoxList);
            return propertyDisplayTypes;
        }

        private static void SetFacetValues(FacetedSearchModel model, IOrderedEnumerable<FacetItem> distinctFacets)
        {
            if (model.FacetSets == null || !model.FacetSets.Any() || distinctFacets == null || !distinctFacets.Any()) return;

            foreach (var facetSet in model.FacetSets)
            {
                var items = new List<SelectListItem>() { };
                var facetValues = distinctFacets.Select(x => x.FacetValue).ToList();
                foreach (var item in facetValues)
                {
                    items.Add(new SelectListItem() { Text = item, Value = item, Selected = facetSet.SelectedValues.Contains(item) });
                }
                facetSet.FacetValues = items;
            }
        }

        private static List<FacetSet> GetFacetSets(string url, Dictionary<string, DisplayType> propertyDisplayTypes)
        {
            var facetSets = new List<FacetSet>();
            var propertyAliases = propertyDisplayTypes.Select(x => x.Key);
            foreach (var alias in propertyAliases)
            {
                var selectedValues = QueryStringHelper.GetValueFromQueryString(alias, url)?.Split(',') ?? new string[] { };

                facetSets.Add(new FacetSet()
                {
                    PropertyAlias = alias,
                    FacetValues = null,
                    DisplayType = propertyDisplayTypes[alias],
                    SelectedValues = selectedValues
                });
            }

            return facetSets;
        }
    }
}
