using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackathonProject
{
                public class Geometry
            {
                public List<double> coordinates { get; set; }
                public string type { get; set; }
            }

            public class GeoCoordinates
            {
                public string Latitude { get; set; }
                public string Longitude { get; set; }
            }

            public class ReviewLocation
            {
                public string Address { get; set; }
                public string BusinessName { get; set; }
                public string CountryCode { get; set; }
                public GeoCoordinates LocationGeoCoordinates { get; set; }
            }

            public class PropertiesClass
            {
                public string GoogleMapsURL { get; set; }
                public ReviewLocation ReviewLocation { get; set; }
                public string Published { get; set; }
                public string ReviewComment { get; set; }
                public int StarRating { get; set; }
                public string Updated { get; set; }
            }

            public class Feature
            {
                public Geometry geometry { get; set; }
                public PropertiesClass properties { get; set; }
                public string type { get; set; }
            }

            public class Reviews
            {
                public string type { get; set; }
                public List<Feature> features { get; set; }
            }
}
