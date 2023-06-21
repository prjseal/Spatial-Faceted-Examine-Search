using Examine.Lucene;
using Examine.Lucene.Indexing;
using Lucene.Net.Analysis;
using SpacialFacetedExamineSearch.Site.FieldValueTypes;

namespace SpacialFacetedExamineSearch.Site.ValueTypeFactories
{
    public class ShapeValueTypeFactory : IFieldValueTypeFactory
    {
        private readonly Analyzer analyzer;

        public ShapeValueTypeFactory(Analyzer analyzer)
        {
            this.analyzer = analyzer;
        }

        public IIndexFieldValueType Create(string fieldName)
        {
            return new ShapeFieldValueType(fieldName, analyzer);
        }
    }
}
