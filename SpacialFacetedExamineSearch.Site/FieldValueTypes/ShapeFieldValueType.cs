using Examine.Lucene.Indexing;
using Lucene.Net.Analysis;
using Lucene.Net.Spatial.Prefix.Tree;
using Lucene.Net.Spatial.Vector;
using Lucene.Net.Spatial;
using Newtonsoft.Json.Linq;
using Spatial4n.Context;
using Lucene.Net.Documents;
using Lucene.Net.Search;

namespace SpacialFacetedExamineSearch.Site.FieldValueTypes
{
    public class ShapeFieldValueType : IIndexFieldValueType
    {
        private readonly string fieldName;
        private readonly Analyzer analyzer;
        private SpatialStrategy strategy;
        private SpatialContext context;

        public ShapeFieldValueType(string fieldName, Analyzer analyzer)
        {
            this.fieldName = fieldName;
            this.analyzer = analyzer;

            const int maxLevels = 11;
            context = SpatialContext.Geo;
            var spatialGrid = new GeohashPrefixTree(context, maxLevels);
            strategy = new PointVectorStrategy(context, fieldName);
        }

        public void AddValue(Document doc, object value)
        {
            var locs = (IEnumerable<JObject>)value;
            var shapes = locs
                .Select(loc => context.MakePoint(loc.Value<double>("longitude"), loc.Value<double>("latitude")));
            var fields = shapes.SelectMany(shape => strategy.CreateIndexableFields(shape));
            foreach (var field in fields)
            {
                doc.Add(field);
            }
        }

        Query IIndexFieldValueType.GetQuery(string query)
        {
            throw null;
        }

        public string FieldName => fieldName;

        public string SortableFieldName => fieldName;

        public bool Store => true;

        public Analyzer Analyzer => analyzer;

        public SpatialStrategy Strategy => strategy;

        public SpatialContext Context => context;
    }
}
