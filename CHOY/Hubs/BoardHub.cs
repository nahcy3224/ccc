using CHOY.Models;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CHOY.Hubs
{
  public class BoardHub : Hub
  {
    public override async Task OnConnected()
    {
      Clients.All.onConnected(Context.ConnectionId);
      await base.OnConnected();
    }
    public void JoinRoom(string ProjectID)
    {
      Groups.Add(Context.ConnectionId, ProjectID);
    }
    public async Task NotificationUpdate(string ProjectID, string BoardID)
    {
      await Clients.OthersInGroup(ProjectID).getCanvasData(ProjectID, BoardID);
    }
  }
}