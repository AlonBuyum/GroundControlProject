using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AlonBuyum994FinalProj.BL;
using Repositories;
using AlonBuyum994FinalProj.Hubs;
using Microsoft.AspNetCore.SignalR;
using Data;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;

namespace AlonBuyum994FinalProj.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("[controller]")]
    public class StateManagerController : ControllerBase
    {

        private readonly ILogger<StateManagerController> _logger;
        private readonly ISimulatorService _simulatorService;
        private readonly ITimerService _timerService;
        private readonly IHubContext<AirportHub> _hubContext;
        private readonly IDataService _dataService;
        private readonly ILogicService _logicService;
        Logic _logic;
        HttpClient client = new HttpClient();



        public StateManagerController(ILogger<StateManagerController> logger, ILogicService logicService,
                                     ISimulatorService simulatorService, ITimerService timerService,
                                     IHubContext<AirportHub> hubcontext, IDataService dataService)
        {
            _logger = logger;
            _simulatorService = simulatorService;
            _logicService = logicService;
            _timerService = timerService;
            _hubContext = hubcontext;
            _dataService = dataService;
            _logic = new Logic(simulatorService, logicService, timerService, hubcontext, dataService);
        }

        [HttpGet]
        public IActionResult GetDataAsync()
        {
            _logic.Simulate();
            var stateJson = JsonConvert.SerializeObject(_logic.SimState);
            _hubContext.Clients.All.SendAsync("ReceiveObject", stateJson);
            return Ok(stateJson);
        }

        [HttpGet("{id}")]
        public IActionResult SaveDataAsync(int id)
        {
            var stateJson = JsonConvert.SerializeObject(_logic.SimState);

            // update the DB:
            // create data object from stateJson
            var stateDO = _dataService.CreateStateDO(stateJson);
            //checks if number is under 0. if so send 1
            stateDO.Number = id <= 0 ? 1 : id;

            _dataService.AddOrUpdateState(stateDO);
            _hubContext.Clients.All.SendAsync("ReceiveObject", stateJson);
            return Ok(stateJson);
        }

        [HttpGet("load{id}")]
        public async Task<IActionResult> LoadDataAsync(int id)
        {
            // load from DB
            //checks if number is under 0. if so send 1
            id = id <= 0 ? 1 : id;
            var stateJson = _dataService.GetStateById(id);
            if (_dataService.TryParseJson(stateJson, out JObject jObject))
            {
                _simulatorService.GetState = JsonConvert.DeserializeObject<State>(stateJson);
            }
            await Dispatcher.CreateDefault().InvokeAsync(() =>
            {
                _logic.SimState = _logicService.GetSimulatedState(_simulatorService.GetState);
            });
            await _hubContext.Clients.All.SendAsync("ReceiveObject", stateJson);
            return Ok(stateJson);
        }
    }
}
