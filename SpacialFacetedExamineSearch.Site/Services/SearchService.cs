using Examine;
using Examine.Lucene.Providers;
using Examine.Lucene.Search;
using Examine.Search;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.Spatial.Queries;
using SpacialFacetedExamineSearch.Site.FieldValueTypes;
using SpacialFacetedExamineSearch.Site.Models;
using Spatial4n.Distance;
using System.Text.RegularExpressions;
using Umbraco.Cms.Core;

namespace SpacialFacetedExamineSearch.Site.Services
{
    public class SearchService : ISearchService
    {
        private readonly IExamineManager _examineManager;

        public SearchService(IExamineManager examineManager)
        {
            _examineManager = examineManager;
        }

        public IEnumerable<ISearchResult>? AllResults()
        {
            if (_examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out IIndex? index))
            {

                var query = index
                    .Searcher
                    .CreateQuery()
                .NativeQuery($"+__IndexType:locationItems");

                var results = query.Execute();

                return results;
            }

            return Enumerable.Empty<ISearchResult>();
        }

        public IEnumerable<SearchResultItem>? Search(FacetedSearchModel searchModel)
        {
            var index = (LuceneIndex)_examineManager.GetIndex("LocationsIndex");
            var query = (LuceneSearchQueryBase)index.Searcher.CreateQuery(null, BooleanOperation.Or);

            var searcher = new IndexSearcher(index.IndexWriter.IndexWriter.GetReader(false));

            var filteredQuery = index
                .Searcher
                .CreateQuery()
                .NativeQuery($"+__IndexType:locationItems");

            if (!string.IsNullOrWhiteSpace(searchModel?.SearchQuery?.Phrase ?? ""))
            {
                filteredQuery
                    .And()
                    .GroupedAnd(SearchFields, searchModel.SearchQuery.Terms);
            }

            if(searchModel?.FacetSets != null && searchModel.FacetSets.Any())
            {
                foreach(var facetSet in searchModel.FacetSets)
                {
                    if (facetSet.SelectedValues != null && facetSet.SelectedValues.Any())
                    {
                        filteredQuery.And()
                        .GroupedOr(new[] { facetSet.PropertyAlias }, facetSet.SelectedValues);
                    }
                }
            }

            Match match = Regex.Match(filteredQuery.ToString(), @"LuceneQuery:(.*?)\}");
            if (match.Success)
            {
                string luceneQuery = match.Groups[1].Value.Trim();
                var filterLuceneQuery = query.QueryParser.Parse(luceneQuery);
                query.Query.Add(filterLuceneQuery, Occur.MUST);
            }

            Sort sort = Sort.RELEVANCE;

            var hasLocationPoint = !string.IsNullOrWhiteSpace(searchModel.Longitude) && !string.IsNullOrWhiteSpace(searchModel.Latitude);
            if (hasLocationPoint)
            {
                BooleanQuery geoQuery = null;

                var valueType = (ShapeFieldValueType)index.FieldValueTypeCollection.ValueTypes
                    .FirstOrDefault(x => x.FieldName == "locations");
                var circleQuery = valueType.Strategy.MakeQuery(
                                new SpatialArgs(
                                    SpatialOperation.Intersects,
                                    valueType.Context.MakeCircle(
                                        double.Parse(searchModel.Longitude),
                                        double.Parse(searchModel.Latitude),
                                        DistanceUtils.Dist2Degrees(searchModel.RadiusInMiles, DistanceUtils.EarthMeanRadiusMiles))
                                )
                            );

                geoQuery = new BooleanQuery();
                geoQuery.Add(circleQuery, Occur.SHOULD);


                if (geoQuery != null)
                {
                    query.Query.Add(geoQuery, Occur.SHOULD);
                }
            }

            var result = searcher.Search(query.Query, searchModel.MaxResults, sort);
            //var result = searcher.Search(query.Query, searchModel.SearchQuery.MaxHits);

            var vals = result.ScoreDocs.Select(
                x =>
                {
                    var doc = searcher.Doc(x.Doc);
                    var location = hasLocationPoint ? doc.GetValues("latlng").FirstOrDefault().Split(';', StringSplitOptions.RemoveEmptyEntries).Select(Location.FromString).FirstOrDefault() : new Location();
                    var distance = hasLocationPoint ? location.DistanceTo(new Location(float.Parse(searchModel.Latitude), float.Parse(searchModel.Longitude))) : 0f;
                    return new SearchResultItem(doc, x.Score, distance);
                });

            return vals.OrderBy(x => x.Distance);
        }

        public class SearchQuery
        {
            private string phrase = "";
            private string[] terms;

            public string Phrase
            {
                get => phrase;
                set
                {
                    phrase = value;
                    terms = Phrase
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Where(x => x.Length >= 3)
                        .ToArray();
                }
            }

            public int MaxHits { get; set; } = 10;

            public string[] Terms
            {
                get { return terms; }
            }
        }

        public struct Location
        {
            public float Latitude { get; set; }
            public float Longitude { get; set; }

            public Location(float latitude, float longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }

            public static Location FromString(string value)
            {
                var parts = value.Split(',');
                var point = new Location(Convert.ToSingle(parts[0], System.Globalization.CultureInfo.InvariantCulture), Convert.ToSingle(parts[1], System.Globalization.CultureInfo.InvariantCulture));
                return point;
            }

            public float DistanceTo(Location b)
            {
                //var x = MathF.Abs(Longitude - b.Longitude);
                //var y = MathF.Abs(Latitude - b.Latitude);
                //return MathF.Sqrt(x * x + y * y);

                var dLat = ToRadians(b.Latitude - Latitude);
                var dLon = ToRadians(b.Longitude - Longitude);

                var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(b.Latitude)) *
                        Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

                var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

                var distance = DistanceUtils.EarthMeanRadiusMiles * c;

                return (float)distance;
            }

            private static double ToRadians(double degrees)
            {
                return degrees * Math.PI / 180;
            }

            public override string ToString()
            {
                return $"{Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            }
        }

        public class SearchResultItem
        {
            public Document Doc { get; set; }
            public float Score { get; set; }
            public float Distance { get; set; }

            public SearchResultItem(Document doc, float score, float distance)
            {
                Doc = doc;
                Score = score;
                Distance = distance;
            }
        }

        public static readonly string[] SearchFields = new[]
        {
            "name"
        };
    }
}
