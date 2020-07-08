using DAL;
using Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IDataService
    {
        string GetDBState { get; set; }
        int MaxSavedStates { get; set; }
        int NumOfStates { get; }
        StateDO CreateStateDO(string recievedState);
        string GetStateById(int id);
        void AddOrUpdateState(StateDO recievedState);
        bool TryParseJson(string json, out JObject jObject);
    }
    public class DataService : IDataService
    {
        AirportContext _airportContext;
        public string GetDBState { get; set; }
        public int MaxSavedStates { get; set; }

        private int _numOfStates;
        public int NumOfStates
        {
            get { return _numOfStates; }
            set
            {
                if (value > MaxSavedStates)
                    value = 1;
                _numOfStates = value;
            }
        }
        public DataService(AirportContext airportContext)
        {
            _airportContext = airportContext;
            MaxSavedStates = 5;
        }

        public string GetStateById(int stateNumber)
        {
            //makes sure the  number is between 1-5
            NumOfStates = stateNumber;
            // checks if stateNumber is between 1-5, and if its more than 5 turns it to 1
            stateNumber = stateNumber == NumOfStates ? stateNumber : NumOfStates;
            var state = _airportContext.StateDOs.FirstOrDefault(s => s.Number == stateNumber);
            if (state == null)
            {
                Debug.WriteLine($"Didn't find state. Got exception:\n{new NullReferenceException().Message}");
                return $"Didn't find state. Got exception:\n{new NullReferenceException().Message}";
            }
            else return GetDBState = state.State;
        }

        public StateDO CreateStateDO(string recievedState)
        {
            return new StateDO { State = recievedState };
        }
        public void AddOrUpdateState(StateDO recievedState)
        {
            //makes sure the  number is between 1-5
            NumOfStates = recievedState.Number;
            // checks if recievedState.Number is between 1-5, and if its more than 5 turns it to 1
            recievedState.Number = recievedState.Number == NumOfStates? recievedState.Number : NumOfStates;
            var foundState = _airportContext.StateDOs.FirstOrDefault(s=>s.Number == recievedState.Number);
            if (foundState == null)
            {
                _airportContext.StateDOs.Add(recievedState);
                Debug.WriteLine($"Created new DB state {recievedState.Id}");
            }
            else
            {
                foundState.State = recievedState.State;
                _airportContext.StateDOs.Update(foundState);
                Debug.WriteLine($"Updated DB state {foundState.Id}");
            }
            _airportContext.SaveChanges();
        }

        public bool TryParseJson(string json, out JObject jObject)
        {
            try
            {
                jObject = JObject.Parse(json);
                return true;
            }
            catch
            {
                jObject = null;
                return false;
            }
        }
    }
}
