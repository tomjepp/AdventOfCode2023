using System.Diagnostics;
using System.Text.RegularExpressions;

var stopwatch = new Stopwatch();

stopwatch.Start();

var regex = new Regex(@"^Card\s+(\d+): (.+?) \| (.+?)$", RegexOptions.Compiled);
var cards = new List<Card>();

using (var reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();
        if (line == null || string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        // parse your input here
        var match = regex.Match(line);
        if (!match.Success)
        {
            continue;
        }

        int cardId = int.Parse(match.Groups[1].Value);
        var winningNumberStrings = match.Groups[2].Value
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var winningNumbers = new List<int>();
        foreach (var winningNumberString in winningNumberStrings)
        {
            winningNumbers.Add(int.Parse(winningNumberString));
        }
        var numberOnCardStrings = match.Groups[3].Value
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var numbersOnCard = new List<int>();
        foreach (var numberOnCardString in numberOnCardStrings)
        {
            numbersOnCard.Add(int.Parse(numberOnCardString));
        }

        var card = new Card(cardId, winningNumbers, numbersOnCard);
        cards.Add(card);
    }
}

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

// calculate & output your result here
var totalScore = cards.Sum(x => x.Score);
Console.WriteLine(totalScore);

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");

class Card(int id, List<int> winningNumbers, List<int> numbersOnCard)
{
    public int Id { get; } = id;
    public List<int> WinningNumbers { get; } = winningNumbers;
    public List<int> NumbersOnCard { get; } = numbersOnCard;

    public long Score
    {
        get
        {
            long score = 0;
            foreach (var numberOnCard in NumbersOnCard)
            {
                if (!WinningNumbers.Contains(numberOnCard))
                {
                    continue;
                }

                if (score == 0)
                {
                    score = 1;
                }
                else
                {
                    score *= 2;
                }
            }

            return score;
        }
    }
}
