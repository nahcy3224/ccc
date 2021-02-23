using System;

namespace CHOY.Models.ModelBinders
{
  public class ApiVoteCreate
  {
    public string VoteName { get; set; }
    public string[] Choices { get; set; }
    public Nullable<int> VoteCount { get; set; }
  }
}