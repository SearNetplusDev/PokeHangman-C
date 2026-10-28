using System.Text.Json;
using PokeHangman.Models;
using PokeHangman.Utils;

namespace PokeHangman.Services;

public class PokemonService
{
    public Task<Pokemon> GetRandomAsync() => GetByIdAsync(Random.Shared.Next(1, Config.MaxPokemonId + 1));

    public async Task<Pokemon> GetByIdAsync(int id)
    {
        string url = $"{Config.ApiBaseUrl}/pokemon/{id}";
        using var response = await Http.Client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStreamAsync());
        JsonElement root = doc.RootElement;

        string name = root.GetProperty("name").GetString()!;
        var types = root.GetProperty("types").EnumerateArray()
            .Select(t => t.GetProperty("type").GetProperty("name").GetString()!)
            .ToList();

        string? sprite = TryGetString(root, "sprites", "other", "home", "front_default");
        string? cry = TryGetString(root, "cries", "latest") ?? TryGetString(root, "cries", "legacy");

        return new Pokemon
        (
            Id: root.GetProperty("id").GetInt32(),
            Name: NormalizeName(name),
            DisplayName: name,
            Types: types,
            SpriteUrl: sprite,
            CryUrl: cry
        );
        
    }

    public static string NormalizeName(string name)
    {
        return new(name.ToLowerInvariant().Where(char.IsLetter).ToArray());
    }

    private static string? TryGetString(JsonElement element, params string[] path)
    {
        foreach(string key in path)
        {
            if(element.ValueKind != JsonValueKind.Object || !element.TryGetProperty(key, out element))
            {
                return null;
            }
        }

        return element.ValueKind == JsonValueKind.String ? element.GetString() : null;
    }
}