using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public struct SerialNumbers
    {
        public static int PLANEID;
        public static int STATIONID;
    }
    public class State
    {
        public List<Station> Stations { get; set; }
        public List<Plane> Planes { get; set; }
        public State()
        {
            Planes = new List<Plane>();
            Stations = new List<Station>();
        }
    }
}
