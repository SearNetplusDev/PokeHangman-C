namespace PokeHangman;

public static class Config
{   
    public const string AppTitle = "PokeHangman";
    public const int WindowWidth = 550;
    public const int WindowHeight = 800;
    public const int MaxAttempts = 6;
    public const int MaxPokemonId = 1025;
    public const string KeyboardLetters = "abcdefghijklmnopqrstuvwxyz";
    public const string ApiBaseUrl = "https://pokeapi.co/api/v2";
    public static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(5);
}

public static class Palette
{
    private static Color C(string hex) => ColorTranslator.FromHtml(hex);

    public static readonly Color Background = C("#1E1E2E");
    public static readonly Color Canvas = C("#181825");
    public static readonly Color Title = C("#F9E2AF");
    public static readonly Color Info = C("#89B4FA");
    public static readonly Color Success = C("#A6E3A1");
    public static readonly Color Button = C("#313244");
    public static readonly Color ButtonActive = C("#45475A");
    public static readonly Color ButtonText = C("#CDD6F4");
    public static readonly Color Error = C("#F38BA8");
    public static readonly Color Action = C("#FAB387");
    public static readonly Color DarkText = C("#11111B");

}
