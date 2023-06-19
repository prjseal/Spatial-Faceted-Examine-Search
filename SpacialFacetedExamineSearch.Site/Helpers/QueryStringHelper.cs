using Microsoft.AspNetCore.WebUtilities;

namespace SpacialFacetedExamineSearch.Site.Helpers
{
    public static class QueryStringHelper
    {
        public static string GetValueFromQueryString(string key, string url, string fallbackValue = "")
        {
            var uri = new Uri(url);
            var queryStringValues = QueryHelpers.ParseQuery(uri.Query);

            if(queryStringValues == null) return fallbackValue;

            if(!queryStringValues.ContainsKey(key)) return fallbackValue;

            var stringValue = queryStringValues[key].ToString();
            if (stringValue != null && !string.IsNullOrWhiteSpace(stringValue))
            {
                return stringValue;
            }
            return fallbackValue;
        }
    }
}
