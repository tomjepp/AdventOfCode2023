using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;

var stopwatch = new Stopwatch();

stopwatch.Start();

var regex = new Regex(@"^Card\s+(\d+): (.+?) \| (.+?)$", RegexOptions.Compiled);
var cards = new Dictionary<int, Card>();
var outstandingCards = new Queue<Card>();

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

        var card = new Card()
        {
            Id = cardId,
            WinningNumbers = winningNumbers,
            NumbersOnCard = numbersOnCard,
        };
        cards.Add(cardId, card);
        outstandingCards.Enqueue(card);
        //Console.WriteLine($"{line} - score: {card.Score}");
    }
}

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

int totalCards = 0;
// calculate & output your result here
while (outstandingCards.Count > 0)
{
    var card = outstandingCards.Dequeue();
    for (int i = card.Id + 1; i < card.Id + 1 + card.Score; i++)
    {
        outstandingCards.Enqueue(cards[i]);
    }

    totalCards++;
}
Console.WriteLine(totalCards);

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");

class Card
{
    public int Id { get; set; }
    public List<int> WinningNumbers { get; set; }
    public List<int> NumbersOnCard { get; set; }

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

                score++;
            }

            return score;
        }
    }
}
