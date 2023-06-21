using Examine;
using SpacialFacetedExamineSearch.Site.CustomIndex;
using SpacialFacetedExamineSearch.Site.IndexConfiguration;
using SpacialFacetedExamineSearch.Site.IndexPopulators;
using SpacialFacetedExamineSearch.Site.Services;
using SpacialFacetedExamineSearch.Site.ValueSetBuilders;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Infrastructure.Examine;

namespace SpacialFacetedExamineSearch.Site.Composers
{
    public class ExamineComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<ISearchService, SearchService>();
            builder.Services.AddUnique<LocationsValueSetBuilder>();

            builder.Services.AddSingleton<IIndexPopulator, LocationsIndexPopulator>();
            builder.Services.AddExamineLuceneIndex<CustomLocationsIndex, ConfigurationEnabledDirectoryFactory>("LocationsIndex");
            builder.Services.ConfigureOptions<ConfigureCustomIndexOptions>();
        }
    }
}
