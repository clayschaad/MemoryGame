using MemoryGame.Models;

namespace MemoryGame.Services;

public sealed class GameService
{
    private readonly LocalStorageService _localStorage;
    private readonly Random _random = new();
    private GameStatistics? _statistics;
    
    private readonly List<GameWord> _defaultWords =
    [
        new() { Word = "fork", ImagePath = "images/fork.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "knife", ImagePath = "images/knife.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "spoon", ImagePath = "images/spoon.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "door", ImagePath = "images/door.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "window", ImagePath = "images/window.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "ball", ImagePath = "images/ball.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "house", ImagePath = "images/house.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "tree", ImagePath = "images/tree.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "car", ImagePath = "images/car.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "cat", ImagePath = "images/cat.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "dog", ImagePath = "images/dog.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "chair", ImagePath = "images/chair.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "table", ImagePath = "images/table.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "cup", ImagePath = "images/cup.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "book", ImagePath = "images/book.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "clock", ImagePath = "images/clock.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "bed", ImagePath = "images/bed.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "phone", ImagePath = "images/phone.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "shoe", ImagePath = "images/shoe.svg", CorrectCount = 0, IncorrectCount = 0 },
        new() { Word = "hat", ImagePath = "images/hat.svg", CorrectCount = 0, IncorrectCount = 0 }
    ];
    
    public GameService(LocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }
    
    public async Task InitializeAsync()
    {
        _statistics = await _localStorage.LoadStatisticsAsync();
        
        if (_statistics == null)
        {
            _statistics = new GameStatistics { Words = new List<GameWord>(_defaultWords) };
        }
        else
        {
            foreach (var defaultWord in _defaultWords)
            {
                var existingWord = _statistics.Words.FirstOrDefault(w => w.Word == defaultWord.Word);
                if (existingWord == null)
                {
                    _statistics.Words.Add(defaultWord);
                }
            }
        }
    }
    
    public GameSession CreateNewGame()
    {
        var rounds = new List<Round>();
        
        for (var i = 0; i < 10; i++)
        {
            var roundType = _random.Next(2) == 0 ? RoundType.ImageWithWords : RoundType.WordWithImages;
            var round = CreateRound(roundType);
            rounds.Add(round);
        }
        
        return new GameSession
        {
            Rounds = rounds,
            CurrentRoundIndex = 0,
            StartTime = DateTimeOffset.UtcNow
        };
    }
    
    private Round CreateRound(RoundType type)
    {
        var correctAnswer = SelectWeightedRandomWord();
        var incorrectAnswers = SelectRandomWords(3, correctAnswer);
        var options = new List<GameWord> { correctAnswer };
        options.AddRange(incorrectAnswers);
        
        options = [.. options.OrderBy(_ => _random.Next())];
        
        return new Round
        {
            Type = type,
            CorrectAnswer = correctAnswer,
            Options = options
        };
    }
    
    private GameWord SelectWeightedRandomWord()
    {
        if (_statistics == null || _statistics.Words.Count == 0)
            throw new InvalidOperationException("No words available");
        
        var totalPriority = _statistics.Words.Sum(w => w.Priority);
        var randomValue = _random.NextDouble() * totalPriority;
        
        var cumulativePriority = 0.0;
        foreach (var word in _statistics.Words)
        {
            cumulativePriority += word.Priority;
            if (randomValue <= cumulativePriority)
                return word;
        }
        
        return _statistics.Words[^1];
    }
    
    private List<GameWord> SelectRandomWords(int count, GameWord exclude)
    {
        if (_statistics == null)
            throw new InvalidOperationException("Statistics not initialized");
        
        var availableWords = _statistics.Words.Where(w => w.Word != exclude.Word).ToList();
        return [.. availableWords.OrderBy(_ => _random.Next()).Take(count)];
    }
    
    public async Task UpdateStatisticsAsync(Round round)
    {
        if (_statistics == null || round.SelectedAnswer == null)
            return;
        
        var word = _statistics.Words.FirstOrDefault(w => w.Word == round.CorrectAnswer.Word);
        if (word == null)
            return;
        
        if (round.IsCorrect)
        {
            word.CorrectCount++;
        }
        else
        {
            word.IncorrectCount++;
        }
        
        await _localStorage.SaveStatisticsAsync(_statistics);
    }
}
