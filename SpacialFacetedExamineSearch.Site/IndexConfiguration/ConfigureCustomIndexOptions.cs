using Examine;
using Examine.Lucene;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Util;
using Microsoft.Extensions.Options;
using SpacialFacetedExamineSearch.Site.ValueTypeFactories;
using Umbraco.Cms.Core.Configuration.Models;

namespace SpacialFacetedExamineSearch.Site.IndexConfiguration
{
    public class ConfigureCustomIndexOptions :
    IConfigureNamedOptions<LuceneDirectoryIndexOptions>
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IOptions<IndexCreatorSettings> _settings;
        public ConfigureCustomIndexOptions(IOptions<IndexCreatorSettings> settings, ILoggerFactory loggerFactory)
        {
            _settings = settings;
            _loggerFactory = loggerFactory;
        }
        public void Configure(string name, LuceneDirectoryIndexOptions options)
        {
            if (name.Equals("LocationsIndex"))
            {
                var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
                options.Analyzer = analyzer;
                options.FieldDefinitions = 
                    new FieldDefinitionCollection(
                        new FieldDefinition("id", FieldDefinitionTypes.Integer),
                        new FieldDefinition("name", FieldDefinitionTypes.FullTextSortable),
                        new FieldDefinition("latitude", FieldDefinitionTypes.Double),
                        new FieldDefinition("longitude", FieldDefinitionTypes.Double),
                        new FieldDefinition("url", FieldDefinitionTypes.FullText),
                        new FieldDefinition("imageUrl", FieldDefinitionTypes.FullText),
                        new FieldDefinition("languages", FieldDefinitionTypes.FullTextSortable),
                        new FieldDefinition("locationItems", "shape"),
                        new FieldDefinition("latlng", FieldDefinitionTypes.Raw)
                );

                var defaults = ValueTypeFactoryCollection.GetDefaultValueTypes(_loggerFactory, analyzer);
                var valueTypeFactories = new Dictionary<string, IFieldValueTypeFactory>(defaults)
                {
                    { "shape", new ShapeValueTypeFactory(analyzer) }
                };

                options.IndexValueTypesFactory = valueTypeFactories;

                options.UnlockIndex = true;
                if (_settings.Value.LuceneDirectoryFactory ==
                LuceneDirectoryFactory.SyncedTempFileSystemDirectoryFactory)

                {
                    // if this directory factory is enabled then a snapshot deletion policy is required

                    options.IndexDeletionPolicy = new SnapshotDeletionPolicy(new

                    KeepOnlyLastCommitDeletionPolicy());

                }
            }
        }
        public void Configure(LuceneDirectoryIndexOptions options)
        {
            throw new NotImplementedException("This is never called and is just part of the interface");
        }
    }
}
