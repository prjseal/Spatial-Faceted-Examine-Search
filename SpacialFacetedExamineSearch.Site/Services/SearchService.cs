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

namespace SpacialFacetedExamineSearch.Site.Services
{
    public class SearchService : ISearchService
    {
        private readonly IExamineManager _examineManager;

        public SearchService(IExamineManager examineManager)
        {
            _examineManager = examineManager;
        }

        public IEnumerable<SearchResultItem>? Search(FacetedSearchModel searchModel)
        {
            var index = (LuceneIndex)_examineManager.GetIndex("LocationsIndex");
            var searcher = new IndexSearcher(index.IndexWriter.IndexWriter.GetReader(false));

            BooleanQuery geoQuery = null;
            Sort sort = Sort.RELEVANCE;

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

            var query = (LuceneSearchQueryBase)index.Searcher.CreateQuery(null, BooleanOperation.Or);

            if (geoQuery != null)
            {
                query.Query.Add(geoQuery, Occur.SHOULD);
            }

            var result = searcher.Search(query.Query, 10);
            //var result = searcher.Search(query.Query, searchModel.SearchQuery.MaxHits);

            var vals = result.ScoreDocs.Select(
                x =>
                {
                    var doc = searcher.Doc(x.Doc);
                    var location = doc.GetValues("latlng").FirstOrDefault().Split(';', StringSplitOptions.RemoveEmptyEntries).Select(Location.FromString).FirstOrDefault();
                    var distance = location.DistanceTo(new Location(float.Parse(searchModel.Latitude), float.Parse(searchModel.Longitude)));
                    return new SearchResultItem(doc, x.Score, distance);
                });

            return vals;
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
