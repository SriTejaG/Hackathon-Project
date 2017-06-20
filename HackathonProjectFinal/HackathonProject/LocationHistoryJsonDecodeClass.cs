using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackathonProject
{
    public class Activity2
    {
        
        public string type { get; set; }
        public int confidence { get; set; }

        public Activity2(string newtype, int newconfidence)
        {
            type = newtype;
            confidence = newconfidence;
        }

        public Activity2() { }

    }

    public class Activity
    {
        public string timestampMs { get; set; }
        public List<Activity2> activity { get; set; }

        public Activity(string timeMS, List<Activity2> DefaultActivity2List)
        {
            timestampMs = timeMS;
            activity = DefaultActivity2List;
            
        }

        public Activity() { }

        
    }


    public class Location
    {
        public string timestampMs { get; set; }
        public long latitudeE7 { get; set; }
        public long longitudeE7 { get; set; }
        public int accuracy { get; set; }
        public List<Activity> activity { get; set; }



        public string ActivityMain { get; set; }
        public DateTime timeNormal { get; set; }
        public double latitudeDec { get; set; }
        public double longitudeDec { get; set; }
        public bool ActivityAvailable { get; set; }
    }

    public class LocationHistory
    {
        public List<Location> locations { get; set; }
    }
}
