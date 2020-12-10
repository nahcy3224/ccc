using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.IO;
using CHOY.Models;
using CHOY.App_Code.Auth;
using CHOY.App_Code.Common;

namespace CHOY.DAL
{
  public class ChoyInitializer : DropCreateDatabaseAlways<ChoyContext>
  {
    protected override void Seed(ChoyContext context)
    {
      base.Seed(context);
      var now = DateTime.Now;
      List<Member> members = new List<Member>
      {
        new Member
        {
          MemberID="M0001",
          Email="nahcy3224@gmail.com",
          Psw=ChoyPassword.Hash("000000", TimeConverter.ToTimestamp(now)),
          NickName="Mei",
          Gender=false,
          Bday=new DateTime(1993,05,22),
          ContactEmail ="nahcy3224@yahoo.com.tw",
          CreateAt=now,
          ProfilePic=getFileBytes("\\Images\\img4.jpg"),
          ImageMimeType = "image/jpeg",
          PerCode=Permissions.Download | Permissions.General,
          //IsSuspended=false,
          //LastLogInTime=null
        },
        new Member
        {
          MemberID="M0002",
          Email="ncyuae104@gmail.com",
          Psw=ChoyPassword.Hash("000000", TimeConverter.ToTimestamp(now)),
          NickName="Chun",
          Gender=false,
          Bday=new DateTime(1993,06,08),
          ContactEmail ="ncyuae104@gmail.com",
           CreateAt=now,
          ProfilePic=getFileBytes("\\Images\\img4.jpg"),
          ImageMimeType = "image/jpeg",
          PerCode=Permissions.Download,
          //IsSuspended=false,
          // LastLogInTime=null
        },
        new Member
        {
          MemberID="M0003",
          Email="yn28572@gmail.com",
          Psw=ChoyPassword.Hash("000000", TimeConverter.ToTimestamp(now)),
          NickName="Fong",
          Gender=true,
          Bday=new DateTime(1993,09,20),
          ContactEmail ="yn28572@gmail.com",
          CreateAt=now,
          ProfilePic=getFileBytes("\\Images\\img4.jpg"),
          ImageMimeType = "image/jpeg",
          PerCode=Permissions.Download | Permissions.General ,
          IsSuspended=false,
          LastLogInTime=null
        }
        ,
        new Member
        {
          MemberID="M0004",
          Email="jhs030509@gmail.com",
          Psw=ChoyPassword.Hash("000000", TimeConverter.ToTimestamp(now)),
          NickName="Andy",
          Gender=true,
          Bday=new DateTime(1996,12,18),
          ContactEmail ="nahcy3224@yahoo.com.tw",
          CreateAt=now,
          ProfilePic=getFileBytes("\\Images\\img4.jpg"),
          ImageMimeType = "image/jpeg",
          PerCode=Permissions.Download | Permissions.General,
          IsSuspended=false,
          LastLogInTime=null
        }
      };
      members.ForEach(s => context.Members.Add(s));
      context.SaveChanges();

      List<Project> projects = new List<Project>
      {
        new Project
        {
          ProjectID="P0001",
          ProjectName="資料庫",
          CreateAt=DateTime.Now,
          DeleteAt=null,
          MemberID="M0001"
        }
      };
      projects.ForEach(s => context.Projects.Add(s));
      context.SaveChanges();

      List<Board> boards = new List<Board>
      {
        new Board
        {
          BoardID="B0001",
          ProjectID="P0001",
          MemberIDOwner="M0001",
          DeleteAt=null
        },
          new Board
        {
          BoardID="B0002",
          ProjectID="P0001",
          MemberIDOwner="M0001",
          DeleteAt=null
        },
            new Board
        {
          BoardID="B0003",
          ProjectID="P0001",
          MemberIDOwner="M0001",
          DeleteAt=null
        }
      };
      boards.ForEach(s => context.Boards.Add(s));
      context.SaveChanges();

      List<Bulletin> bulletins = new List<Bulletin>
      {
        new Bulletin
        {
          BulletinID = "N0001",
          PublishStart = new DateTime(2020,12,11,0,0,0),
          PublishEnd =  new DateTime(2020,12,18,0,0,0),
          EditTime=DateTime.Now,
          Content=""
        }
      };

      bulletins.ForEach(s => context.Bulletins.Add(s));
      context.SaveChanges();

      List<Group> groups = new List<Group>
      {
        new Group
        {
          GroupID="G0001",
          GroupName="小圈圈",
          MemberID="M0001"
        },
        new Group
        {
          GroupID="G0002",
          GroupName="Java們",
          MemberID="M0001"
        }
          ,
        new Group
        {
          GroupID="G0003",
          GroupName="我的英文好夥伴",
          MemberID="M0002"
        }
            ,
        new Group
        {
          GroupID="G0004",
          GroupName="我的中文好夥伴",
          MemberID="M0003"
        }
      };
      groups.ForEach(s => context.Groups.Add(s));
      context.SaveChanges();

      List<GroupMember> groupMembers = new List<GroupMember>
      {
        new GroupMember
        {
          MemberIDOwner="M0001",
          GroupID="G0001",
          MemberIDInGroup="M0002"
        },
        new GroupMember
        {
          MemberIDOwner="M0001",
          GroupID="G0001",
          MemberIDInGroup="M0003"
        }
        ,
        new GroupMember
        {
          MemberIDOwner="M0001",
          GroupID="G0002",
          MemberIDInGroup="M0003"
        }
          ,
        new GroupMember
        {
          MemberIDOwner="M0002",
          GroupID="G0003",
          MemberIDInGroup="M0001"
        }
          ,
        new GroupMember
        {
          MemberIDOwner="M0003",
          GroupID="G0004",
          MemberIDInGroup="M0001"
        }
      };
      groupMembers.ForEach(s => context.GroupMembers.Add(s));
      context.SaveChanges();

      List<MemberJoinProject> memberJoinProjects = new List<MemberJoinProject>
      {
        new MemberJoinProject
        {
          MemberIDOwner="M0001",
          ProjectID="P0001",
          SharePerID=Share.Edit,
          MemberIDJoin="M0002"
        },
        new MemberJoinProject
        {
          MemberIDOwner="M0001",
          ProjectID="P0001",
          SharePerID= Share.Edit | Share.Share,
          MemberIDJoin="M0003"
        }
      };
      memberJoinProjects.ForEach(s => context.MemberJoinProjects.Add(s));
      context.SaveChanges();

      List<Vote> votes = new List<Vote>
      {
        new Vote
        {
          VoteID="V0001",
          VoteName="你今天晚上要吃什麼?",
          Result="鍋燒意麵",
          VoteCount=2,
          ProjectID="P0001",
          MemberIDOwner="M0001"
        }
      };
      votes.ForEach(s => context.Votes.Add(s));
      context.SaveChanges();

      List<VoteRecords> voteRecords = new List<VoteRecords>
      {
        new VoteRecords
        {
          VoteID="V0001",
          ChoiceID="C0001",
          Choice="鍋燒意麵",
          VoteCounts=1
        },
        new VoteRecords
        {
          VoteID="V0001",
          ChoiceID="C0002",
          Choice="義大利麵",
          VoteCounts=1
        },
        new VoteRecords
        {
          VoteID="V0001",
          ChoiceID="C0003",
          Choice="日式拉麵",
          VoteCounts=3
        }
      };
      voteRecords.ForEach(s => context.VoteRecords.Add(s));
      context.SaveChanges();

    }

    private byte[] getFileBytes(string path)
    {
      FileStream fileOnDisk = new FileStream(HttpRuntime.AppDomainAppPath + path, FileMode.Open);
      byte[] fileBytes;

      using (BinaryReader br = new BinaryReader(fileOnDisk))
      {
        fileBytes = br.ReadBytes((int)fileOnDisk.Length);
      }
      return fileBytes;

    }
  }
}