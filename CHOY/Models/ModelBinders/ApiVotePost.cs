namespace CHOY.Models.ModelBinders
{
  public class ApiVotePost
  {
    public string VoteName { get; set; }

    public string ProjectID { get; set; }

    public string ProjOwnerId { get; set; }

    public string[] Choices { get; set; }
  }
}