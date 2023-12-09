using System.Diagnostics;

var stopwatch = new Stopwatch();

stopwatch.Start();

var hands = new List<Hand>();

using (var reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();
        if (line == null || string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        var linePieces = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var hand = new Hand(linePieces[0], long.Parse(linePieces[1]));
        hands.Add(hand);
    }
}

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

// calculate & output your result here
var orderedHands = hands.OrderBy(x => x.Score);
long totalWinnings = 0;

int rank = 1;
foreach (var hand in orderedHands)
{
    totalWinnings += hand.Bid * rank;
    rank++;
}
Console.WriteLine(totalWinnings);

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");

enum HandType: long
{
    HighCard = 0x1000000000000000,
    OnePair = 0x2000000000000000,
    TwoPair = 0x3000000000000000,
    ThreeOfAKind = 0x4000000000000000,
    FullHouse = 0x5000000000000000,
    FourOfAKind = 0x6000000000000000,
    FiveOfAKind = 0x7000000000000000,
}

enum CardType: byte
{
    Joker = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    //Jack = 11,
    Queen = 12,
    King = 13,
    Ace = 14,
}

class Hand
{
    public List<CardType> Cards { get; }

    public HandType HandType { get; }
    public long Score { get; }
    public long Bid { get; }

    public Hand(string cards, long bid)
    {
        Cards = new List<CardType>();
        foreach (var c in cards)
        {
            Cards.Add(ParseCard(c));
        }

        Bid = bid;

        HandType = CalculateHandType();
        Score = CalculateScore();
    }

    private CardType ParseCard(char c)
    {
        return c switch
        {
            'J' => CardType.Joker,
            '2' => CardType.Two,
            '3' => CardType.Three,
            '4' => CardType.Four,
            '5' => CardType.Five,
            '6' => CardType.Six,
            '7' => CardType.Seven,
            '8' => CardType.Eight,
            '9' => CardType.Nine,
            'T' => CardType.Ten,
            'Q' => CardType.Queen,
            'K' => CardType.King,
            'A' => CardType.Ace,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private long CalculateScore()
    {
        return (long)HandType | ((long)Cards[0] << 32) | ((long)Cards[1] << 24) | ((long)Cards[2] << 16) |
               ((long)Cards[3] << 8) | (long)Cards[4];
    }

    private HandType CalculateHandType()
    {
        var cardCounts = new Dictionary<CardType, int>();
        foreach (var c in Cards)
        {
            cardCounts.TryAdd(c, 0);
            cardCounts[c] += 1;
        }

        if (cardCounts.ContainsKey(CardType.Joker))
        {
            // Make sure we don't only contain jokers
            if (cardCounts.Count != 1)
            {
                var orderedCardCounts = cardCounts.OrderByDescending(x => x.Value);
                cardCounts[orderedCardCounts.First(x => x.Key != CardType.Joker).Key] += cardCounts[CardType.Joker];
                cardCounts.Remove(CardType.Joker);
            }
        }

        switch (cardCounts.Count)
        {
            case 1:
                // Has to be five of a kind
                return HandType.FiveOfAKind;
            case 2:
            {
                // four of a kind or full house
                foreach (var cardCount in cardCounts)
                {
                    switch (cardCount.Value)
                    {
                        case 4:
                            return HandType.FourOfAKind;
                        case 3:
                        case 2:
                            return HandType.FullHouse;
                    }
                }

                break;
            }
            case 3:
            {
                // three of a kind or two pair
                foreach (var cardCount in cardCounts)
                {
                    switch (cardCount.Value)
                    {
                        case 3:
                            return HandType.ThreeOfAKind;
                        case 2:
                            return HandType.TwoPair;
                    }
                }

                break;
            }
            case 4:
                // has to be one pair
                return HandType.OnePair;
            case 5:
                // has to be high card
                return HandType.HighCard;
        }

        throw new Exception("Should never get here...");
    }
}
