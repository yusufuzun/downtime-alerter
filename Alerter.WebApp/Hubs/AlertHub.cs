using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alerter.WebApp.Hubs
{
    [Authorize]
    public class AlertHub : Hub
    {
    }
}
