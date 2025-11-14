using MemoryGame.Models;

namespace MemoryGame.Services;

public sealed class GameService
{
    private readonly LocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private readonly Random _random = new();
    private GameStatistics? _statistics;
    private List<GameWord>? _availableWords;
    
    public GameService(LocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }
    
    public async Task InitializeAsync()
    {
        await LoadImagesFromFolderAsync();
        
        _statistics = await _localStorage.LoadStatisticsAsync();
        
        if (_statistics == null)
        {
            _statistics = new GameStatistics { Words = new List<GameWord>(_availableWords ?? []) };
        }
        else
        {
            if (_availableWords != null)
            {
                foreach (var availableWord in _availableWords)
                {
                    var existingWord = _statistics.Words.FirstOrDefault(w => w.Word == availableWord.Word);
                    if (existingWord == null)
                    {
                        _statistics.Words.Add(availableWord);
                    }
                }
            }
        }
    }
    
    private async Task LoadImagesFromFolderAsync()
    {
        _availableWords = [];
        
        var commonWords = new[]
        {
            "ball", "bed", "book", "car", "cat", "chair", "clock", "cup",
            "dog", "door", "fork", "hat", "house", "knife", "phone",
            "shoe", "spoon", "table", "tree", "window"
        };
        
        var loadTasks = commonWords.Select(async word =>
        {
            var imagePath = $"images/{word}.svg";
            try
            {
                var response = await _httpClient.GetAsync(imagePath, HttpCompletionOption.ResponseHeadersRead);
                if (response.IsSuccessStatusCode)
                {
                    var germanWord = ConvertToGermanWord(word);
                    return new GameWord
                    {
                        Word = germanWord,
                        ImagePath = imagePath,
                        CorrectCount = 0,
                        IncorrectCount = 0
                    };
                }
            }
            catch
            {
            }
            return null;
        });
        
        var results = await Task.WhenAll(loadTasks);
        _availableWords = results.Where(w => w != null).Cast<GameWord>().ToList();
    }
    
    private static string ConvertToGermanWord(string englishWord)
    {
        return englishWord.ToLower() switch
        {
            "fork" => "Gabel",
            "knife" => "Messer",
            "spoon" => "Löffel",
            "door" => "Tür",
            "window" => "Fenster",
            "ball" => "Ball",
            "house" => "Haus",
            "tree" => "Baum",
            "car" => "Auto",
            "cat" => "Katze",
            "dog" => "Hund",
            "chair" => "Stuhl",
            "table" => "Tisch",
            "cup" => "Tasse",
            "book" => "Buch",
            "clock" => "Uhr",
            "bed" => "Bett",
            "phone" => "Telefon",
            "shoe" => "Schuh",
            "hat" => "Hut",
            _ => char.ToUpper(englishWord[0]) + englishWord[1..].ToLower()
        };
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
