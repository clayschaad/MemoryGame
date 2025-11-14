namespace MemoryGame.Models;

public sealed class GameSession
{
    public required List<Round> Rounds { get; init; }
    public int CurrentRoundIndex { get; set; }
    public DateTimeOffset StartTime { get; init; }
    public DateTimeOffset? EndTime { get; set; }
    
    public Round? CurrentRound => CurrentRoundIndex < Rounds.Count ? Rounds[CurrentRoundIndex] : null;
    public bool IsComplete => CurrentRoundIndex >= Rounds.Count;
    public int CorrectAnswers => Rounds.Count(r => r.SelectedAnswer != null && r.IsCorrect);
    public int IncorrectAnswers => Rounds.Count(r => r.SelectedAnswer != null && !r.IsCorrect);
}
