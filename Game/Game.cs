using System;
using System.Linq;
using System.Collections.Generic;

static class Assets
{
    // Constants:
    public static Vector2 CellSize = new Vector2(32, 16);

    // Assets loaded from files:
    public static SpriteFont DefaultFont;
    public static Texture Tiles;
}

class Game
{
    public static readonly string Title = "Isometric Demo";
    public static readonly Vector2 Resolution = new Vector2(1280, 768);

    public static readonly bool Debug = true;
    public static readonly bool DebugCollision = false;

    Vector2 Origin = new Vector2(600, 140);

    public Game()
    {
        Load();
    }

    public void Load()
    {
        Assets.DefaultFont = new SpriteFont("font.png");
        Assets.Tiles = Engine.LoadTexture("tiles.png");
    }

    public void Update()
    {
        int hx, hy;
        {
            Vector2 query = Engine.MousePosition - Origin - new Vector2(16, 16);
            hx = (int)Math.Floor((query.X / 32) + (query.Y / 16));
            hy = (int)Math.Floor((-query.X / 32) + (query.Y / 16));
        }

        Assets.DefaultFont.Draw(string.Format("Hovered: {0}, {1}", hx, hy), new Vector2(20, 40), Color.White);

        for (int gy = 0; gy < 20; gy++)
        {
            for (int gx = 0; gx < 20; gx++)
            {
                Engine.DrawTexture(
                    Assets.Tiles,
                    Origin + new Vector2((gx - gy) * 16, (gy + gx) * 8),
                    source: new Bounds2(0, 0, 32, 32));
            }
        }
    }
}
