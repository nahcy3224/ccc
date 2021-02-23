using CHOY.DAL;
using CHOY.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace CHOY.Hubs
{
  public class VoteHub : Hub
  {
    ChoyContext db = new ChoyContext();
    //當前用戶數量
    private static List<OnlineUser> OnlineUsers = new List<OnlineUser>() { };

    public async Task UserConnected(string projectId, string memberId)
    {
      var data = OnlineUsers.
        Where(ou => ou.ProjectID == projectId && ou.MemberID == memberId)
        .FirstOrDefault();

      var user = new OnlineUser
      {
        ProjectID = projectId,
        MemberID = memberId,
        ConnectionId = Context.ConnectionId
      };

      if (data == null)
      {
        OnlineUsers.Add(user);
      }
      else
      {
        var index = OnlineUsers.IndexOf(data);
        OnlineUsers[index] = user;
      }

      await Groups.Add(Context.ConnectionId, projectId);
      var list = OnlineUsers.Where(ou => ou.ProjectID == projectId).ToList();
      await Clients.Group(projectId).updateOnlineUsersList(list);
    }
    public async Task SendVote(string projectId, int voteId)
    {
      await Clients.OthersInGroup(projectId).getVote(voteId);
    }
    public async Task NotificationUpdateVoting(string projectId, string str_JsFunction)
    {
      await Clients.OthersInGroup(projectId).updateVoting(str_JsFunction);
    }
    public async Task HandleOnbeforeunload(string projectId, int[] voteIds, string str_JsFunction)
    {
      var data = db.Votes.Where(v => voteIds.Contains(v.VoteID)).ToList();
     
      
      for (var i = 0; i < data.Count; i++)
      {
        data[i].VoteCount -= 1;
        db.Entry(data[i]).State = EntityState.Modified;
        db.SaveChanges();
      }
      
      
      
      await Clients.OthersInGroup(projectId).updateVoting(str_JsFunction);
    }
    public async Task GetOnlineUsersList(string projectId)
    {
      var list = OnlineUsers.Where(ou => ou.ProjectID == projectId).ToList();
      await Clients.Caller.updateOnlineUsersList(list);
    }

    public override Task OnDisconnected(bool stopCalled)
    {
      var user = OnlineUsers.Where(u => u.ConnectionId == Context.ConnectionId).FirstOrDefault();

      if (user != null)
      {
        OnlineUsers.Remove(user); //刪除

        var data = (
          from onlineUser in OnlineUsers
          where onlineUser.ProjectID == user.ProjectID
          select onlineUser
        ).ToList();

        Clients.Group(user.ProjectID).updateOnlineUsersList(data);
      }

      return base.OnDisconnected(stopCalled);
    }
  }
}