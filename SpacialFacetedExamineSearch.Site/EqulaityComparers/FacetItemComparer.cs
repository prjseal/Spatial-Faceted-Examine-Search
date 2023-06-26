using SpacialFacetedExamineSearch.Site.Facets;

namespace SpacialFacetedExamineSearch.Site.EqulaityComparers;

public class FacetItemComparer : IEqualityComparer<FacetItem>
{
    public bool Equals(FacetItem x, FacetItem y)
    {
        // Compare the desired properties for equality
        return x.FacetValue == y.FacetValue && x.PropertyAlias == y.PropertyAlias;
    }

    public int GetHashCode(FacetItem obj)
    {
        // Compute a hash code based on the desired properties
        int hashFacetValue = obj.FacetValue == null ? 0 : obj.FacetValue.GetHashCode();
        int hashPropertyAlias = obj.PropertyAlias == null ? 0 : obj.PropertyAlias.GetHashCode();
        return hashFacetValue ^ hashPropertyAlias;
    }
}