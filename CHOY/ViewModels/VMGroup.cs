using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CHOY.Models;

namespace CHOY.ViewModels
{
    public class VMGroup
    {
            //public List<Member> members { get; set; }
            public List<Group> groups { get; set; }
            public List<GroupMember> groupmembers { get; set; }

    }
}