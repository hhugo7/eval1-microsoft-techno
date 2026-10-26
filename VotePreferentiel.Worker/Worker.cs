using VotePreferentiel;

public class Worker : BackgroundService
{
    private const string VotesUrl =
        "https://raw.githubusercontent.com/JLou/dotnet-course/refs/heads/main/eval-1/votes.json";

    private readonly VoteProvider _voteProvider;
    private readonly IVoteCounter _voteCounter;
    private readonly IHostApplicationLifetime _lifetime;

    public Worker(
        VoteProvider voteProvider,
        IVoteCounter voteCounter,
        IHostApplicationLifetime lifetime)
    {
        _voteProvider = voteProvider;
        _voteCounter = voteCounter;
        _lifetime = lifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var voteModel = await _voteProvider.GetVotesAsync(VotesUrl);

            if (voteModel == null)
            {
                Console.WriteLine("Impossible de récupérer les votes.");
            }
            else
            {
                Console.WriteLine($"Nombre de bulletins : {voteModel.Votes.Count}");

                var winner = _voteCounter.GetWinner(voteModel);
                Console.WriteLine($"Le gagnant est : {winner}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur : {ex.Message}");
        }

        _lifetime.StopApplication();
    }
}