using System;
using System.Collections.Generic;

namespace CHOY.Models.ApiModels
{
  public class VoteForm
  {
    public int VoteID { get; set; }
    public string VoteName { get; set; }
    public List<VoteChoices> Choices { get; set; }
    public string Result { get; set; }
  }
  public class VoteChoices
  {
      public int ChoiceID { get; set; }
      public string Choice { get; set; }
      public Nullable<int> VoteCounts { get; set; }
  }
}