using Examine;
using Examine.Lucene;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Util;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;

namespace SpacialFacetedExamineSearch.Site.IndexConfiguration
{
    public class ConfigureCustomIndexOptions :
    IConfigureNamedOptions<LuceneDirectoryIndexOptions>
    {
        private readonly IOptions<IndexCreatorSettings> _settings;
        public ConfigureCustomIndexOptions(IOptions<IndexCreatorSettings> settings)
        {
            _settings = settings;
        }
        public void Configure(string name, LuceneDirectoryIndexOptions options)
        {
            if (name.Equals("LocationsIndex"))
            {
                options.Analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
                options.FieldDefinitions = new FieldDefinitionCollection(
                new FieldDefinition("id", FieldDefinitionTypes.Integer),
                new FieldDefinition("name", FieldDefinitionTypes.FullTextSortable),
                new FieldDefinition("lat", FieldDefinitionTypes.Double),
                new FieldDefinition("long", FieldDefinitionTypes.Double),
                new FieldDefinition("url", FieldDefinitionTypes.FullText),
                new FieldDefinition("imageUrl", FieldDefinitionTypes.FullText),
                new FieldDefinition("languages", FieldDefinitionTypes.FullTextSortable)

                );
;
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
