using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Data
{
    public class Plane
    {
#nullable enable
        public int PlaneId { get; set; }
        public Station? Destination { get; set; }
        public Station? CurrentStation { get; set; }
        public LinkedList<Station> PlaneRoute { get; set; }

        public Plane()
        {
            PlaneId = SerialNumbers.PLANEID++;
            PlaneRoute = new LinkedList<Station>();
        }
    }
}
