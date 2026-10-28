using PokeHangman.Utils;

namespace PokeHangman.Ui;

public class SpritePanel : PictureBox
{
    public SpritePanel()
    {
        Size = new Size(160, 160);
        BackColor = Palette.Background;
        SizeMode = PictureBoxSizeMode.Zoom;
    }

    public void Clear()
    {
        var old = Image;
        Image = null;
        old?.Dispose();
    }


    public async void Load(string? url, Func<bool> isCurrent)
    {
        if (string.IsNullOrEmpty(url))
            return;

        var image = await ImageLoader.LoadFromUrlAsync(url);

        if (image is null)
            return;

        if (IsDisposed || !isCurrent())
        {
            image.Dispose();
            return;
        }

        Clear();
        Image = image;
    }
    
}