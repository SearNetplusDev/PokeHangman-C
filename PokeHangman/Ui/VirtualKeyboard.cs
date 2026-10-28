namespace PokeHangman.Ui;

public class VirtualKeyboard : Panel 
{
    private const int ButtonWidth = 52;
    private const int ButtonHeight = 40;
    private const int Gap = 3;

    private readonly Dictionary<char, Button> _buttons = new();
    private readonly HashSet<char> _used = new();
    private bool _locked = true;

    public VirtualKeyboard(string letters, Action<char> onLetter, int columns = 9) 
    {
        BackColor = Palette.Background;
        int rows = (letters.Length + columns - 1) / columns;
        Size = new Size(columns * (ButtonWidth + Gap), rows * (ButtonHeight + Gap));

        for (int i = 0; i < letters.Length; i++)
        { 
            char letter = letters[i];
            var button = new Button
            {
                Text = char.ToUpperInvariant(letter).ToString(),
                Size = new Size(ButtonWidth, ButtonHeight),
                Location = new Point((i % columns) * (ButtonWidth + Gap), (i / columns) * (ButtonHeight + Gap)),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Palette.Button,
                ForeColor = Palette.ButtonText,
                FlatStyle = FlatStyle.Flat,
                TabStop = false,
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Palette.ButtonActive;
            button.Click += (_, _) =>
            {
                if (!_locked && !_used.Contains(letter))
                    onLetter(letter);
            };

            _buttons[letter] = button;
            Controls.Add(button);
        }
    }

    public void Mark(char letter, bool hit)
    { 
        if(!_buttons.TryGetValue(letter, out var button))
            return;

        _used.Add(letter);
        button.BackColor = hit ? Palette.Success : Palette.Error;
        button.ForeColor = Palette.DarkText;
        button.FlatAppearance.MouseOverBackColor = button.BackColor;
    }

    public void Enable() 
    { 
        _used.Clear();
        _locked = false;

        foreach (var button in _buttons.Values)
        {
            button.BackColor = Palette.Button;
            button.ForeColor = Palette.ButtonText;
            button.FlatAppearance.MouseOverBackColor = Palette.ButtonActive;
        }
    }

    public void Disable() => _locked = true;
    
}