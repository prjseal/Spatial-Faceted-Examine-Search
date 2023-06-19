namespace SpacialFacetedExamineSearch.Site.Facets
{
    public interface IFacetValue
    {
        object Value { get; }
        int Hits { get; }
    }
}
