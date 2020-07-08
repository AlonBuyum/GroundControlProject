using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlonBuyum994FinalProj.Hubs
{
    public class AirportHub:Hub
    {
        public async Task SendObject(string recievedState)
        {
            await Clients.All.SendAsync("ReceiveObject", recievedState);
        }
    }
}
