using Examine;
using Newtonsoft.Json;
using SpacialFacetedExamineSearch.Site.Models;
using SpacialFacetedExamineSearch.Site.ValueSetBuilders;
using System.Net;
using Umbraco.Cms.Infrastructure.Examine;

namespace SpacialFacetedExamineSearch.Site.IndexPopulators
{
    public class LocationsIndexPopulator : IndexPopulator
    {
        private readonly LocationsValueSetBuilder _todoValueSetBuilder;
        public LocationsIndexPopulator(LocationsValueSetBuilder productValueSetBuilder)
        {
            _todoValueSetBuilder = productValueSetBuilder;

            //We're telling this populator that it's responsible for populating only our index

            RegisterIndex("LocationsIndex");
        }
        protected override void PopulateIndexes(IReadOnlyList<IIndex> indexes)
        {
            using (WebClient httpClient = new WebClient())
            {
                var jsonData =

                httpClient.DownloadString("https://localhost:44380/locations.json");

                var data =

                JsonConvert.DeserializeObject<IEnumerable<LocationItemModel>>(jsonData);

                if (data != null)
                {
                    foreach (var item in indexes)
                    {
                        item.IndexItems(_todoValueSetBuilder.GetValueSets(data.ToArray()));
                    }
                }
            }
        }
    }
}
