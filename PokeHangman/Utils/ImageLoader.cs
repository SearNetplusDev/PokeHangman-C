using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace PokeHangman.Utils;

public static class ImageLoader
{
    public static async Task<Image?> LoadFromUrlAsync(string? url, int size = 160)
    {
        if (string.IsNullOrEmpty(url))
            return null;

        try
        {
            byte[] bytes = await Http.Client.GetByteArrayAsync(url);
            using var stream = new MemoryStream(bytes);
            using var original = Image.FromStream(stream);

            var resized = new Bitmap(size, size);
            using var g = Graphics.FromImage(resized);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(original, 0, 0, size, size);
            return resized;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading image from URL: {ex.Message}");
            return null;
        }
    }
}