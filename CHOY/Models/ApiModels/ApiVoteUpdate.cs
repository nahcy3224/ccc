using System;

namespace CHOY.Models.ModelBinders
{
  public class ApiVoteUpdate
  {
    public Nullable<int> VoteCount { get; set; } = null;
    public Nullable<int> VoteCounts { get; set; }
  }
}