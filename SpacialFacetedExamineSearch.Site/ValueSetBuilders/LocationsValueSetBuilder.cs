using Examine;
using SpacialFacetedExamineSearch.Site.Models;
using Umbraco.Cms.Infrastructure.Examine;

namespace SpacialFacetedExamineSearch.Site.ValueSetBuilders
{
    public class LocationsValueSetBuilder : IValueSetBuilder<LocationItemModel>
    {
        public IEnumerable<ValueSet> GetValueSets(params LocationItemModel[] data)
        {
            foreach (var item in data)
            {
                var indexValues = new Dictionary<string, object>
                {
                    ["id"] = item.Id,
                    ["name"] = item.Name,
                    ["lat"] = item.Lat,
                    ["long"] = item.Long,
                    ["url"] = item.Url,
                    ["imageUrl"] = item.ImageUrl,
                    ["languages"] = item.Languages
                };
                var valueSet = new ValueSet(item.Id.ToString(), "locationItems", indexValues);
                yield return valueSet;
            }
        }
    }
}
