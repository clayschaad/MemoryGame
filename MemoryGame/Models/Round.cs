namespace MemoryGame.Models;

public sealed class Round
{
    public required RoundType Type { get; init; }
    public required GameWord CorrectAnswer { get; init; }
    public required List<GameWord> Options { get; init; }
    public GameWord? SelectedAnswer { get; set; }
    public bool IsCorrect => SelectedAnswer?.Word == CorrectAnswer.Word;
}
