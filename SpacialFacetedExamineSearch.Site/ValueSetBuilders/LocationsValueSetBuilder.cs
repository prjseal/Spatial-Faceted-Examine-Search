using Examine;
using SpacialFacetedExamineSearch.Site.Models;
using System.Globalization;
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
                    ["latitude"] = item.Latitude,
                    ["longitude"] = item.Longitude,
                    ["latlng"] = item.Latitude.ToString(CultureInfo.InvariantCulture) + "," + item.Longitude.ToString(CultureInfo.InvariantCulture),
                    ["locations"] = item.Locations,
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
