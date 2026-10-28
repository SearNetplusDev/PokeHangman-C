using PokeHangman.Ui;

namespace PokeHangman;

internal class Program()
{
    [STAThread]

    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}