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
        Assets.DefaultFont.Draw("This is an isometric grid:", new Vector2(20, 20), Color.White);

        for (int gy = 0; gy < 6; gy++)
        {
            for (int gx = 0; gx < 6; gx++)
            {
                Engine.DrawTexture(
                    Assets.Tiles,
                    new Vector2(100 + (gx + gy) * 16, 100 - (gx - gy) * 8),
                    source: new Bounds2(0, 0, 32, 32));
            }
        }
    }
}
