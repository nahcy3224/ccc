using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using CHOY.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace CHOY.DAL
{
  public class ChoyContext : DbContext
  {
    public ChoyContext() : base("name=Choy")
    {
    }

    public DbSet<Board> Boards { get; set; }
    public DbSet<Bulletin> Bulletins { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<MemberJoinProject> MemberJoinProjects { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<VoteRecords> VoteRecords { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
      //modelBuilder.Entity<Group>().HasKey(group => new { group.GroupID, group.MemberID });
    }
  }
}
