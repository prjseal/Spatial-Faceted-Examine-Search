using Microsoft.AspNetCore.Mvc.Rendering;
using SpacialFacetedExamineSearch.Site.Enums;

namespace SpacialFacetedExamineSearch.Site.Facets
{
    public class FacetSet
    {
        public string PropertyAlias { get; set; }
        public DisplayType DisplayType { get; set; }
        public List<SelectListItem> FacetValues { get; set; }
        public string[] SelectedValues { get; set; }
    }
}
