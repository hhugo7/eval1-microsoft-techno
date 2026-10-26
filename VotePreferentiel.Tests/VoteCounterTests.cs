using VotePreferentiel;

namespace VotePreferentiel.Tests;

public class VoteCounterTests
{
    private readonly VoteCounter _voteCounter = new();
    [Test]
    public async Task GetWinner_CasNominal_RetourneChloe()
    {
        var voteModel = CreateModel(
            ["Alice", "Bilal", "Chloé"],
            ["Alice", "Chloé", "Bilal"],
            ["Bilal", "Chloé", "Alice"],
            ["Chloé", "Bilal", "Alice"],
            ["Chloé", "Alice", "Bilal"]);
        var winner = _voteCounter.GetWinner(voteModel);
        await Assert.That(winner).IsEqualTo("Chloé");
    }

    [Test]
    public async Task GetWinner_UnSeulCandidat_RetourneAlice()
    {
        var voteModel = CreateModel(["Alice"]);
        var winner = _voteCounter.GetWinner(voteModel);
        await Assert.That(winner).IsEqualTo("Alice");
    }

    [Test]
    public async Task GetWinner_ReportDesPreferences_RetourneChloe()
    {
        var ballots = Repeat(4, "Alice", "Bilal", "Chloé", "Diane")
            .Concat(Repeat(3, "Bilal", "Chloé", "Alice", "Diane"))
            .Concat(Repeat(3, "Chloé", "Alice", "Bilal", "Diane"))
            .Concat(Repeat(1, "Diane", "Chloé", "Bilal", "Alice"))
            .ToArray();
        var voteModel = CreateModel(ballots);
        var winner = _voteCounter.GetWinner(voteModel);
        await Assert.That(winner).IsEqualTo("Chloé");
    }

    [Test]
    public async Task GetWinner_EgaliteEnDernierePosition_RetourneChloe()
    {
        var voteModel = CreateModel(
            ["Alice", "Chloé", "Bilal"],
            ["Bilal", "Chloé", "Alice"],
            ["Chloé", "Alice", "Bilal"],
            ["Chloé", "Bilal", "Alice"]);
        var winner = _voteCounter.GetWinner(voteModel);
        await Assert.That(winner).IsEqualTo("Chloé");
    }

    [Test]
    public async Task GetWinner_BulletinVide_LeveUneException()
    {
        var voteModel = new VoteModel
        {
            Votes = new List<Vote>
            {
                new Vote { Choices = new List<string>() }
            }
        };
        await Assert.That(() => _voteCounter.GetWinner(voteModel))
            .Throws<ArgumentException>();
    }

    private static VoteModel CreateModel(params string[][] ballots)
    {
        return new VoteModel
        {
            Votes = ballots
                .Select(choices => new Vote { Choices = choices.ToList() })
                .ToList()
        };
    }

    private static string[][] Repeat(int count, params string[] choices)
    {
        return Enumerable.Range(0, count)
            .Select(_ => choices)
            .ToArray();
    }
}