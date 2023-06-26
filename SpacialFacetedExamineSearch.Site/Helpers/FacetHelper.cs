using Examine;

using SpacialFacetedExamineSearch.Site.Facets;
using Umbraco.Cms.Core.Models.PublishedContent;
using static SpacialFacetedExamineSearch.Site.Services.SearchService;

namespace SpacialFacetedExamineSearch.Site.Helpers;

public static class FacetHelper
{
    public static List<FacetItem> GetFacetItemsFromResults(
        IEnumerable<string> facetPropertyAliases, IEnumerable<SearchResultItem> results)
    {
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
                            var splitValues = fieldValue.Split(' ');
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
}