using Examine;
using SpacialFacetedExamineSearch.Site.Models;
using System.Globalization;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Infrastructure.Examine;

namespace SpacialFacetedExamineSearch.Site.ValueSetBuilders
{
    public class LocationsValueSetBuilder : IValueSetBuilder<LocationItemModel>
    {

        private readonly IAppPolicyCache _runtimeCache;

        public LocationsValueSetBuilder(IAppPolicyCache runtimeCache)
        {
            _runtimeCache = runtimeCache;
        }

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
                    ["regions"] = item.Regions.Replace(' ', '_')
                };
                var valueSet = new ValueSet(item.Id.ToString(), "locationItems", indexValues);
                yield return valueSet;
            }

            _runtimeCache.ClearByKey("DistinctFacets");
        }
    }
}
