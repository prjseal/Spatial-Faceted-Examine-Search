using Microsoft.AspNetCore.Mvc.Rendering;
using SpacialFacetedExamineSearch.Site.Enums;
using SpacialFacetedExamineSearch.Site.Facets;
using SpacialFacetedExamineSearch.Site.Models;
using static SpacialFacetedExamineSearch.Site.Services.SearchService;

namespace SpacialFacetedExamineSearch.Site.Helpers;

public static class FacetHelper
{
    public static List<FacetItem> GetFacetItemsFromResults(
        Dictionary<string, Tuple<DisplayType, char>> propertySettings, IEnumerable<SearchResultItem> results)
    {
        var facetPropertyAliases = propertySettings.Select(x => x.Key);

        List<FacetItem> facetItems = new List<FacetItem>();

        if (results == null || facetPropertyAliases == null) return facetItems;


        foreach (var result in results)
        {
            foreach (var field in result.Doc.Fields)
            {
                foreach (var propertyAlias in facetPropertyAliases)
                {
                    if (field.Name == propertyAlias)
                    {
                        var fieldValue = field.GetStringValue();
                        if(!string.IsNullOrWhiteSpace(fieldValue))
                        {
                            var splitValues = fieldValue.Split(propertySettings[propertyAlias].Item2);
                            foreach (var splitValue in splitValues)
                            {
                                facetItems.Add(new FacetItem()
                                {
                                    PropertyAlias = propertyAlias,
                                    FacetValue = splitValue
                                });
                            }
                        }
                        
                    }
                }
            }
        }

        return facetItems;
    }

    public static void SetFacetValues(FacetedSearchModel model, IOrderedEnumerable<FacetItem> distinctFacets)
    {
        if (model.FacetSets == null || !model.FacetSets.Any() || distinctFacets == null || !distinctFacets.Any()) return;

        foreach (var facetSet in model.FacetSets)
        {
            var items = new List<SelectListItem>() { };
            var facetValues = distinctFacets.Select(x => x.FacetValue).ToList();
            foreach (var item in facetValues)
            {
                items.Add(new SelectListItem() { Text = item.Replace('_', ' '), Value = item, Selected = facetSet.SelectedValues.Contains(item) });
            }
            facetSet.FacetValues = items;
        }
    }

    public static List<FacetSet> GetFacetSets(string url, Dictionary<string, Tuple<DisplayType, char>> propertyDisplayTypes)
    {
        var facetSets = new List<FacetSet>();
        var propertyAliases = propertyDisplayTypes.Select(x => x.Key);
        foreach (var alias in propertyAliases)
        {
            var selectedValues = QueryStringHelper.GetValueFromQueryString(alias, url)?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? new string[] { };

            facetSets.Add(new FacetSet()
            {
                PropertyAlias = alias,
                FacetValues = null,
                DisplayType = propertyDisplayTypes[alias].Item1,
                SelectedValues = selectedValues
            });
        }

        return facetSets;
    }
}