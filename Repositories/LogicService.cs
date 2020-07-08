using Data;
using System;

namespace Repositories
{
    public interface ILogicService
    {
        Plane GetSimulatedPlane(Plane plane);
        Station GetSimulatedStation(Station station);
        State GetSimulatedState(State state);
        void AddToPlaneRoute(Plane plane, Station station);
        void UpdatePlanes(Plane plane, State state);
    }
    public class LogicService : ILogicService
    {
        public Plane GetSimulatedPlane(Plane plane)
        {
            return plane;
        }

        public void AddToPlaneRoute(Plane plane, Station station)
        {
            plane.PlaneRoute.AddFirst(station);
        }
        public Station GetSimulatedStation(Station station)
        {
            return station;
        }
        public State GetSimulatedState(State state)
        {
            return state;
        }

        public void UpdatePlanes(Plane plane, State state)
        {
            state.Planes[state.Planes.IndexOf(plane)] = plane;
        }
    }
}
