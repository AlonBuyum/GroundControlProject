using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ISimulatorService
    {
         Plane GetPlane { get; set; }
         Station GetStation { get; set; }
        State GetState { get; set; }
        void NewStation();
        void NewPlane();
        void AddToStationsList();
        void AddToPlanesList();
    }
    public class SimulatorService : ISimulatorService
    {
        public Plane GetPlane   {get ; set; }

        public Station GetStation { get ; set; }

        public State GetState { get; set; }
        public SimulatorService()
        {
            GetPlane = new Plane ();
            GetStation = new Station ();
            GetState = new State ();
            GetState.Planes.Add(GetPlane);
            GetState.Stations.Add(GetStation);
        }

        public void NewStation()
        {
            GetStation = new Station ();
        }
        public void NewPlane()
        {
            GetPlane = new Plane() ;
        }

        public void AddToStationsList()
        {
            GetState.Stations.Add(GetStation);
        }

        public void AddToPlanesList()
        {
            GetState.Planes.Add(GetPlane);
        }
    }
}
