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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LocationsIndexPopulator(LocationsValueSetBuilder productValueSetBuilder,
            IWebHostEnvironment webHostEnvironment)
        {
            _todoValueSetBuilder = productValueSetBuilder;
            _webHostEnvironment = webHostEnvironment;
            //We're telling this populator that it's responsible for populating only our index

            RegisterIndex("LocationsIndex");
        }
        protected override void PopulateIndexes(IReadOnlyList<IIndex> indexes)
        {
            using (WebClient httpClient = new WebClient())
            {
                var domain = _webHostEnvironment.IsDevelopment()
                    ? "https://localhost:44380"
                    : "https://sfes.umbhost.dev";

                var jsonData = httpClient.DownloadString(domain + "/locations.json");

                var data = JsonConvert.DeserializeObject<IEnumerable<LocationItemModel>>(jsonData);

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
