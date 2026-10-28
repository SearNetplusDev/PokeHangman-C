using System.Drawing.Drawing2D;

namespace PokeHangman.Ui;

public class HangmanCanvas : Panel
{
    private static readonly (int X1, int Y1, int X2, int Y2)[] Structure = { 
        (20, 160, 180, 160),
        (50, 160, 50, 20),
        (50, 20, 130, 20),
        (130, 20, 130, 40),
    };


    private static readonly (int X1, int Y1, int X2, int Y2)[] Limbs = {
        (130, 70, 130, 110),  // tronco
        (130, 80, 110, 100),  // brazo izquierdo
        (130, 80, 150, 100),  // brazo derecho
        (130, 110, 110, 135), // pierna izquierda
        (130, 110, 150, 135), // pierna derecha
    };

    private int _errors;

    public HangmanCanvas()
    { 
        DoubleBuffered = true;
        Size = new Size(200, 180);
        BackColor = Palette.Canvas;
    }

    public void Draw(int errors)
    {
        _errors = Math.Clamp(errors, 0, Limbs.Length + 1);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using var structurePen = new Pen(Palette.ButtonText, 3);

        foreach(var (x1, y1, x2, y2) in Structure)
            g.DrawLine(structurePen, x1, y1, x2, y2);

        using var bodyPen = new Pen(Palette.Error, 3);
        using var bodyBrush = new SolidBrush(Palette.Error);

        for(int i = 0; i < _errors; i++)
        {
            if (i == 0) { 
                g.FillEllipse(bodyBrush, 115, 40, 30, 30);
            }
            else
            {
                var (x1, y1, x2, y2) = Limbs[i - 1];
                g.DrawLine(bodyPen, x1, y1, x2, y2);
            }
        }
    }
}