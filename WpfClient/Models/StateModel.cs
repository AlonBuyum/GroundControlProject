using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfClient.Models
{
    public class StateModel
    {
        public ViewState GetState { get; set; }
       
        public class ViewState
        {
            public ViewStation[] Stations { get; set; }
            public ViewPlane[] Planes { get; set; }
        }

        public class ViewStation
        {
            public int StationId { get; set; }
            public ViewPlane CurrentPlane { get; set; }
            public bool isVacant { get; set; }
        }

        public class ViewPlane : ViewModelBase
        {
            public int PlaneId { get; set; }
            public ViewStation Destination { get; set; }
            public ViewStation CurrentStation { get; set; }

            private ViewStation[] _planeRoute;
            public ViewStation[] PlaneRoute
            {
                get { return _planeRoute; }
                set
                {
                    if (_planeRoute != value)
                    {
                        _planeRoute = value;
                        ObRoute = new ObservableCollection<ViewStation>(_planeRoute.ToList());
                    }
                }
            }

            private ObservableCollection<ViewStation> _obRoute;
            public ObservableCollection<ViewStation> ObRoute
            {
                get { return _obRoute; }
                set
                {
                    if (_obRoute != value)
                    {
                        _obRoute = value;
                        RaisePropertyChanged();
                    }
                }
            }
        }

    }
}
