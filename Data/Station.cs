using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{

    public class Station
#nullable  enable
    {
        public int StationId { get; set; }
        public Plane? CurrentPlane { get; set; }
        public bool isVacant { get; set; }

        public Station()
        {
            StationId= SerialNumbers.STATIONID++;
            isVacant = true;
        }
    }
}
