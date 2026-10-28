using PokeHangman.Models;
using PokeHangman.Services;

namespace PokeHangman.Ui;

public class MainForm : Form
{
    private readonly PokemonService _pokemonService = new();
    private readonly AudioPlayer _audio = new();

    private HangmanGame? _game;
    private int _gameId;

    private readonly HangmanCanvas _canvas = new();
    private readonly Label _lblHint;
    private readonly Label _lblWord;
    private readonly SpritePanel _sprite = new();
    private readonly Button _btnSound;
    private readonly Button _btnRestart;
    private readonly VirtualKeyboard _keyboard;


    public MainForm()
    {
        // ------ VENTANA ------
        Text = Config.AppTitle;
        ClientSize = new Size(Config.WindowWidth, Config.WindowHeight);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Palette.Background;
        KeyPreview = true;
        KeyPress += OnKeyPress;
        FormClosed += (_, _) => _audio.Dispose();

        // ------ WIDGETS ------
        var lblTitle = new Label
        {
            Text = Config.AppTitle,
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Palette.Title,
            Size = new Size(520, 45),
            TextAlign = ContentAlignment.MiddleCenter,
        };

        _lblHint = new Label
        {
            Font = new Font("Segoe UI", 11, FontStyle.Italic),
            ForeColor = Palette.Info,
            Size = new Size(520, 28),
            TextAlign = ContentAlignment.MiddleCenter,
        };

        _lblWord = new Label
        {
            Font = new Font("Consolas", 20, FontStyle.Bold),
            ForeColor = Palette.Success,
            Size = new Size(520, 40),
            TextAlign = ContentAlignment.MiddleCenter,
        };

        _btnSound = MakeButton("Grito", Palette.Button, Palette.ButtonText, 10, new Size(100, 30));
        _btnSound.Click += (_, _) => PlayCry();

        _keyboard = new VirtualKeyboard(Config.KeyboardLetters, PressLetter);

        _btnRestart = MakeButton("Siguiente Pokémon", Palette.Action, Palette.DarkText, 10, new Size(200, 40));
        _btnRestart.Click += (_, _) => StartNewGame();

        // ------ LAYOUT  ------
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            BackColor = Palette.Background,
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));


        void Add(Control control, int verticalPadding)
        {
            control.Anchor = AnchorStyles.None;
            control.Margin = new Padding(0, verticalPadding, 0, verticalPadding);
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(control);
        }

        Add(lblTitle, 5);
        Add(_canvas, 5);
        Add(_lblHint, 5);
        Add(_lblWord, 5);
        Add(_sprite, 5);
        Add(_btnSound, 5);
        Add(_keyboard, 8);
        Add(_btnRestart, 8);

        Controls.Add(layout);
        StartNewGame();
    }


    private static Button MakeButton(string text, Color backColor, Color foreColor, float fontSize, Size size)
    {
        var button = new Button
        {
            Text = text,
            BackColor = backColor,
            ForeColor = foreColor,
            Font = new Font("Segoe UI", fontSize, FontStyle.Bold),
            Size = size,
            FlatStyle = FlatStyle.Flat,
            TabStop = false,
        };
        button.FlatAppearance.BorderSize = 0;
        button.Enter += (_, _) => button.Parent?.Focus();
        return button;
    }


    //-------------------------------
    //  TECLADO FÍSICO
    //-------------------------------
    private void OnKeyPress(object? sender, KeyPressEventArgs e)
    {
        char letter = char.ToLowerInvariant(e.KeyChar);

        if (Config.KeyboardLetters.Contains(letter))
        {
            PressLetter(letter);
            e.Handled = true;
        }
    }

    //-------------------------------
    //  NUEVA PARTIDA
    //-------------------------------
    private async void StartNewGame()
    {
        int gameId = ++_gameId;
        _game = null;

        _lblHint.Text = "Cargando Pokémon desde PokeAPI...";
        _lblWord.Text = "";
        _sprite.Clear();
        _canvas.Draw(0);
        _btnRestart.Enabled = false;
        _btnSound.Enabled = false;
        _keyboard.Disable();
        ActiveControl = null;
        Focus();

        try
        {
            Pokemon pokemon = await _pokemonService.GetRandomAsync();
            ConfigureGame(pokemon, gameId);
        }
        catch (Exception error)
        {
            ShowError(error);
        }
    }


    private void ConfigureGame(Pokemon pokemon, int gameId)
    {
        if (gameId != _gameId)
            return;
        
        _game = new HangmanGame(pokemon, Config.MaxAttempts);
        
        string types = string.Join(", ", pokemon.Types.Select(Capitalize));
        _lblHint.Text = $"Pokémon # {pokemon.Id} | Tipo: {types}";

        _keyboard.Enable();
        _btnRestart.Enabled = true;
        _btnSound.Enabled = pokemon.CryUrl is not null;
        RefreshScreen();

        ActiveControl = null;
        Focus();
    }

    private static string Capitalize(string s) => s.Length == 0 ? s : char.ToUpperInvariant(s[0]) + s[1..];

    //-------------------------------
    //  JUGADA
    //-------------------------------
    private void PressLetter(char letter)
    {
        if (_game is null)
            return;

        bool? hit = _game.Guess(letter);

        if (hit is null)
            return;

        _keyboard.Mark(letter, hit.Value);
        RefreshScreen();

        if (_game.IsOver)
            FinishGame();
    }

    private void RefreshScreen()
    {
        if (_game is null)
            return;
        _lblWord.Text = _game.HiddenWord;
        _canvas.Draw(_game.Errors);
    }


    //-------------------------------
    //  SONIDO
    //-------------------------------
    private void PlayCry()
    {
        if (_game?.Pokemon.CryUrl is not { } cryUrl)
            return;
        
        int gameId = _gameId;
        _audio.PlayFromUrl(cryUrl, () => gameId == _gameId);
    }


    //-------------------------------
    //  FIN DE PARTIDA
    //-------------------------------
    private void FinishGame() 
    {
        if (_game is null)
            return;

        Pokemon pokemon = _game.Pokemon;
        int gameId = _gameId;

        _keyboard.Disable();
        _lblWord.Text = pokemon.DisplayName.ToUpperInvariant();
        _sprite.Load(pokemon.SpriteUrl, () => gameId == _gameId);
        PlayCry();

        if (_game.HasWon)
        {
            MessageBox.Show(this, $"¡Excelente era {pokemon.DisplayName}!", "¡Victoria!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else { 
            MessageBox.Show(this, $"¡Sin intentos disponibles! \n\nEra {pokemon.DisplayName}.", "¡Derrota!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }

    //-------------------------------
    //  ERRORES
    //-------------------------------
    private void ShowError(Exception error)
    {
        _lblHint.Text = "Error al cargar Pokémon.";
        _btnRestart.Enabled = true;
        MessageBox.Show(this, $"Ocurrió un error al cargar el Pokémon\n\n{error.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}