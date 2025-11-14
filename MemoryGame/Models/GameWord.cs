namespace MemoryGame.Models;

public sealed class GameWord
{
    public required string Word { get; init; }
    public required string ImagePath { get; init; }
    public int CorrectCount { get; set; }
    public int IncorrectCount { get; set; }
    
    public double Priority
    {
        get
        {
            if (CorrectCount == 0 && IncorrectCount == 0)
                return 1.0;
            
            var totalAttempts = CorrectCount + IncorrectCount;
            var incorrectRatio = (double)IncorrectCount / totalAttempts;
            
            return incorrectRatio * 2 + 0.5;
        }
    }
}
