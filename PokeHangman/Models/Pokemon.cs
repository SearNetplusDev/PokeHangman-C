namespace PokeHangman.Models;

public record Pokemon(
    int Id,
    string Name,
    string DisplayName,
    List<string> Types,
    string? SpriteUrl,
    string? CryUrl
);