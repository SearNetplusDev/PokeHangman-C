using System.Diagnostics;
using NAudio.Vorbis;
using NAudio.Wave;
using PokeHangman.Utils;

namespace PokeHangman.Services;

public sealed class AudioPlayer : IDisposable
{
    private readonly Dictionary<string, byte[]> _cache = new();
    private WaveOutEvent? _output;
    private VorbisWaveReader? _reader;

    public async void PlayFromUrl(string? url, Func<bool> isCurrent)
    {
        if (string.IsNullOrEmpty(url))
            return;

        try
        {
            if (!_cache.TryGetValue(url, out byte[]? data))
            {
                data = await Http.Client.GetByteArrayAsync(url);
                _cache[url] = data;
            }

            if (!isCurrent())
                return;

            Stop();

            _reader = new VorbisWaveReader(new MemoryStream(data));
            _output = new WaveOutEvent();
            _output.Init(_reader);
            _output.Play();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error playing audio from URL: {ex.Message}");
        }
    }

    private void Stop()
    {
        _output?.Stop();
        _output?.Dispose();
        _reader?.Dispose();
        _reader = null;
        _output = null;
    }

    public void Dispose() => Stop();
}