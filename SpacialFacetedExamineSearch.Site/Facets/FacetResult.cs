using System.Collections;

namespace SpacialFacetedExamineSearch.Site.Facets;

public class FacetResult : IFacetResult
{
    private readonly IEnumerable<IFacetValue> _values;

    /// <inheritdoc/>
    public FacetResult(IEnumerable<IFacetValue> values)
    {
        _values = values;
    }

    /// <inheritdoc/>
    public IEnumerator<IFacetValue> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
