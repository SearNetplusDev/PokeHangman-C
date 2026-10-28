namespace PokeHangman.Utils;

public static class Http
{
    public static readonly HttpClient Client = new() { Timeout = Config.RequestTimeout };
}