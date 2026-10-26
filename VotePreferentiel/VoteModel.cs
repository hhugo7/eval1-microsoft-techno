namespace VotePreferentiel;

public class VoteModel
{
    public List<Vote> Votes { get; set; } = new();
}

public class Vote
{
    public List<string> Choices { get; set; } = new();
}