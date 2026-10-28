namespace PokeHangman.Models;

public class HangmanGame
{
    private readonly HashSet<char> _usedLetters = new();

    public HangmanGame(Pokemon pokemon, int maxAttempts)
    {
        Pokemon = pokemon;
        MaxAttempts = maxAttempts;
    }

    public Pokemon Pokemon { get; }
    public int MaxAttempts { get; }

    public int Errors => _usedLetters.Count(l => !Pokemon.Name.Contains(l));
    public int RemainingAttempts => MaxAttempts - Errors;

    public string HiddenWord => string.Join(" ", Pokemon.Name.Select(l => _usedLetters.Contains(l) ? char.ToUpperInvariant(l).ToString() : "_"));

    public bool HasWon => Pokemon.Name.All(_usedLetters.Contains);
    public bool HasLost => RemainingAttempts <= 0;
    public bool IsOver => HasWon || HasLost;

    public bool? Guess(char letter)
    {
        if (IsOver || _usedLetters.Contains(letter))
        {
            return null;
        }

        _usedLetters.Add(letter);

        return Pokemon.Name.Contains(letter);
    }
}