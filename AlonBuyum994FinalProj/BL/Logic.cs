using AlonBuyum994FinalProj.Hubs;
using Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Timers;

namespace AlonBuyum994FinalProj.BL
{
    public class Logic
    {
        private int _numPlanes = 2;
        private int _numStations = 8;
        private int _numMilliSeconds = 3000;
        private readonly ISimulatorService _simulatorService;
        private readonly ILogicService _logicService;
        private readonly ITimerService _timerService;
        private readonly IHubContext<AirportHub> _hubContext;
        private readonly IDataService _dataService;

        public Plane SimPlane { get; set; }
        public Station SimStation { get; set; }
        public State SimState { get; set; }
        public Timer SimTimer { get; set; }

        public Logic(ISimulatorService simulatorService, ILogicService logicService, ITimerService timerService, IHubContext<AirportHub> hubcontext, IDataService dataService)
        {
            _logicService = logicService;
            _simulatorService = simulatorService;
            _timerService = timerService;
            _hubContext = hubcontext;
            _dataService = dataService;
            InitProps();
        }

        private void InitProps()
        {
            // generates initial data from the logic service. so the props wont ever be null
            SimPlane = _simulatorService.GetPlane;
            SimStation = _simulatorService.GetStation;
            SimState = _simulatorService.GetState;
            SimTimer = _timerService.GetTimer;
            SetTimers();
        }

        private void SetTimers()
        {
            SimTimer.Interval = _numMilliSeconds;
            SimTimer.Elapsed += SimTimer_Elapsed;
            SimTimer.AutoReset = true;
        }

        public void StopTimers()
        {
            SimTimer.Stop();
        }
        private async void SimTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine("===========> timer ticked");
            // generate new planes and look for routes 
            await Dispatcher.CreateDefault().InvokeAsync( () =>  /// makes sure the methods are being
            {                                                             /// executed in the same thread
                SearchForStations();
                MovePlanesBetweenStations();
                SendToHub();
            });
           
        }

        private void SendToHub()
        {
            var stateJson = JsonConvert.SerializeObject(SimState);
            _hubContext.Clients.All.SendAsync("ReceiveObject", stateJson);
        }

        private void SearchForStations()
        {
            // connects the planes and stations
            if (SimState.Stations != null && SimState.Stations.Count != 0
                && SimState.Planes != null && SimState.Planes.Count != 0)
            {
                foreach (var station in SimState.Stations)
                {
                    Debug.WriteLine($"inside station #{station.StationId} loop start");
                    if (station.isVacant == true)
                    {
                        Random r = new Random();
                        int i = r.Next(0, SimState.Planes.Count);
                        var somePlane = SimState.Planes[i];
                        Debug.WriteLine($"random plane selected is plane #{somePlane.PlaneId}");
                        somePlane.CurrentStation = station;
                        station.isVacant = false;
                        if (somePlane.PlaneRoute != null)
                        {
                            if (!somePlane.PlaneRoute.Any(s => s.StationId == station.StationId))
                            {
                                _logicService.AddToPlaneRoute(somePlane, station);
                                somePlane.Destination = somePlane.PlaneRoute.Last.Value;
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.WriteLine($"plane #{SimPlane.PlaneId} has no vacant stations, or the station list is empty");
            }
        }

        private async void MovePlanesBetweenStations()
        {
            if (SimState.Stations != null && SimState.Stations.Count != 0          /// check if the state and the lists (planes & stations)
                        && SimState.Planes != null && SimState.Planes.Count != 0)  /// are not null
            {
                Debug.WriteLine($"SimState.Stations SimState.Planes aren't null");
                foreach (var plane in SimState.Planes)              /// start synchtonous foreach
                {
                    if (plane == null) return;                  /// make sure plane insnt null
                    Debug.WriteLine($"inside plane #{plane.PlaneId} loop start");
                    if (plane.PlaneRoute != null)            /// check the route isnt null
                    {
                        Debug.WriteLine($"plane #{plane.PlaneId} route isn't null");
                        if (plane.CurrentStation != null)      /// makes sure the current
                        {                                                                                                   ///station isnt null
                            Debug.WriteLine($"plane #{plane.PlaneId} CurrentStation isn't null");
                            SimState.Stations.FirstOrDefault(s => s.StationId == plane.CurrentStation.StationId).isVacant = true;  ///makes current
                                                                                                                                   /// station vacant 
                                                                                                                                   /// so other planes 
                                                                                                                                   /// could move to it
                            //**** Move to next station **** 
                            if (plane.PlaneRoute.Find(plane.CurrentStation) != null) /// checks that the current station is in the route
                                                                                     /// may  occur  when loading a state from memory
                            {
                                if (plane.PlaneRoute.Find(plane.CurrentStation).Next != null)  /// checks next *node* is not null
                                {
                                    Debug.WriteLine($"plane #{plane.PlaneId} moved to next station");
                                    if (plane.PlaneRoute.Find(plane.CurrentStation).Next.Value != null) /// checks next node *value* is not null
                                    {
                                        var nextStation = plane.PlaneRoute.Find(plane.CurrentStation).Next.Value; ///  finds next station (node) value
                                        Debug.WriteLine($"plane #{plane.PlaneId} moved from station #{plane.CurrentStation.StationId} to station #{nextStation.StationId}");
                                        plane.CurrentStation = nextStation; /// ====> moves the current station
                                    }
                                    else plane.CurrentStation = plane.Destination; /// moves current station to the end of the route
                                }
                                else plane.CurrentStation = plane.Destination;
                            }
                            else await Task.Run(() => _logicService.AddToPlaneRoute(plane, plane.CurrentStation));
                                                                                                               
                            //**** Delete plane the finished route ****                                           
                            if (plane.PlaneRoute.Last != null)                                                    
                            {                                                                                     
                                Debug.WriteLine($"plane #{plane.PlaneId} last station isn't null");
                                if (plane.CurrentStation == plane.PlaneRoute.Last.Value) /// checks if the plane is in the last station
                                {
                                    Debug.WriteLine($"plane #{plane.PlaneId} reached last station");
                                    SimState.Stations.FirstOrDefault(s => s.StationId == plane.CurrentStation.StationId).isVacant = true; ///makes last station vacant
                                    plane.CurrentStation = default;                                                                       ///so other planes could move to it
                                    plane.Destination = default;    /// clears the stations                                                                       
                                    await Task.Run(() => PlaneRouteEnder(plane)); 
                                    return;                                    
                                }
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"plane #{plane.PlaneId} has no station in currentStation or in Destination");
                            FreeSomeStation();
                        }
                        _logicService.UpdatePlanes(plane, SimState);
                    }
                    else Debug.WriteLine($"plane #{plane.PlaneId} rout has no planes and is null");
                }
            }
            else
            {
                Debug.WriteLine($"plane #{SimPlane.PlaneId} has no vacant station to fly to, or the planes list is empty");
            }
        }
        private void FreeSomeStation()
        {
            Random r = new Random();
            int i = r.Next(0, SimState.Stations.Count);
            SimState.Stations[i].isVacant = true; // free up a random station
            Debug.WriteLine($"random station selected is station #{SimState.Stations[i].StationId}");
            // update the state
            SimState = _logicService.GetSimulatedState(_simulatorService.GetState);
        }
        private void PlaneRouteEnder(Plane plane)
        {
            SimState.Planes.Remove(plane);
            _simulatorService.NewPlane();
            _simulatorService.AddToPlanesList();
            SimState = _logicService.GetSimulatedState(_simulatorService.GetState);
        }

        public void Simulate()
        {
            // regenerats data. without worrying that the logic data is uninitialized and therefore null 

            //assign the current plane
            SimPlane = _logicService.GetSimulatedPlane(_simulatorService.GetPlane);

            if (SimState.Stations.Count < _numStations)
            {
                // create some  stations 
                for (int i = 0; i < _numStations; i++)
                {
                    _simulatorService.NewStation();
                    _simulatorService.AddToStationsList();
                }
            }
            if (SimState.Planes.Count < _numPlanes)
            {
                // create some planes 
                for (int i = 0; i < _numPlanes; i++)
                {
                    _simulatorService.NewPlane();
                    _simulatorService.AddToPlanesList();
                }
            }
            //assign current station and current state
            SimStation = _logicService.GetSimulatedStation(_simulatorService.GetStation);
            SimState = _logicService.GetSimulatedState(_simulatorService.GetState);

            // start data generation events
            SimTimer.Start();
        }
    }
}
