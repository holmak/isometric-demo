using System;
using System.Collections.Generic;
using System.Linq;

class SpriteFont
{
    public readonly Texture Atlas;
    public readonly Vector2 CharSize;

    public SpriteFont(string filename)
    {
        Atlas = Engine.LoadTexture(filename);
        CharSize = Atlas.Size / 16f;
    }

    public void DrawSprite(int col, int row, Vector2 position, Color color)
    {
        Engine.DrawTexture(
            Atlas,
            position: position,
            color: color,
            source: new Bounds2(new Vector2(col, row) * CharSize, CharSize));
    }

    public void Draw(string text, Vector2 position, Color color)
    {
        float step = CharSize.X;
        float left = position.X;
        foreach (char c in text)
        {
            if (c == '\n')
            {
                position.X = left;
                position.Y += step;
            }
            else
            {
                DrawSprite(c % 16, c / 16, position, color);
                position.X += step;
            }
        }
    }
}
