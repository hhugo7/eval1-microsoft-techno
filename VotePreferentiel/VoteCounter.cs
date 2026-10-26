namespace VotePreferentiel;

public class VoteCounter : IVoteCounter
{
    public HashSet<string> GetCandidates(VoteModel voteModel)
    {
        var candidates = new HashSet<string>();
        foreach (var vote in voteModel.Votes)
        {
            foreach (var choice in vote.Choices)
            {
                candidates.Add(choice);
            }
        }
        return candidates;
    }

    public string GetWinner(VoteModel voteModel)
    {
        if (voteModel.Votes.Count == 0)
        {
            throw new ArgumentException("Aucun bulletin à dépouiller.");
        }
        if (voteModel.Votes.Any(vote => vote.Choices.Count == 0))
        {
            throw new ArgumentException("Un bulletin ne contient aucun choix.");
        }
        var remainingCandidates = GetCandidates(voteModel);
        var workingModel = new VoteModel
        {
            Votes = voteModel.Votes
                .Select(vote => new Vote { Choices = new List<string>(vote.Choices) })
                .ToList()
        };
        while (remainingCandidates.Count > 1)
        {
            var firstChoices = GetAllFirstChoices(workingModel);
            var counts = CountFirstChoices(firstChoices);
            foreach (var candidate in remainingCandidates)
            {
                if (!counts.ContainsKey(candidate))
                {
                    counts[candidate] = 0;
                }
            }
            var toEliminate = GetCandidatesWithLeastVotes(counts);
            if (toEliminate.Length == remainingCandidates.Count)
            {
                throw new InvalidOperationException(
                    "Égalité parfaite : impossible de désigner un gagnant.");
            }
            foreach (var candidate in toEliminate)
            {
                remainingCandidates.Remove(candidate);
            }
            foreach (var vote in workingModel.Votes)
            {
                vote.Choices.RemoveAll(choice => !remainingCandidates.Contains(choice));
            }
        }
        return remainingCandidates.First();
    }
    
    public List<string> GetAllFirstChoices(VoteModel voteModel)
    {
        var firstChoices = new List<string>();
        foreach (var vote in voteModel.Votes)
        {
            if (vote.Choices.Count > 0)
            {
                firstChoices.Add(vote.Choices[0]);
            }
        }
        return firstChoices;
    }
    
    public IDictionary<string, int> CountFirstChoices(List<string> firstChoices)
    {
        var counts = new Dictionary<string, int>();
        foreach (var choice in firstChoices)
        {
            if (counts.ContainsKey(choice))
            {
                counts[choice]++;
            }
            else
            {
                counts[choice] = 1;
            }
        }
        return counts;
    }

    public string[] GetCandidatesWithLeastVotes(IDictionary<string, int> firstChoiceCounts)
    {
        var minVotes = firstChoiceCounts.Values.Min();
        return firstChoiceCounts
            .Where(pair => pair.Value == minVotes)
            .Select(pair => pair.Key)
            .ToArray();
    }
}