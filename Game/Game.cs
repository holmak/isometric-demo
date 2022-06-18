using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

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

    static readonly string MapFilePath = "tile.map";
    static readonly int MapFileVersion = 1;

    Vector2 Origin = new Vector2(600, 140);
    bool[,] Map;
    bool MustSaveMap = false;

    public Game()
    {
        LoadAssets();

        // Load map:
        if (File.Exists(MapFilePath))
        {
            using (BinaryReader r = new BinaryReader(File.OpenRead(MapFilePath)))
            {
                int version = r.ReadInt32();
                int width = r.ReadInt32();
                int height = r.ReadInt32();
                Map = new bool[width, height];

                for (int gy = 0; gy < height; gy++)
                {
                    for (int gx = 0; gx < width; gx++)
                    {
                        Map[gx, gy] = r.ReadBoolean();
                    }
                }
            }
        }
        else
        {
            int width = 20;
            int height = 20;
            Map = new bool[width, height];

            for (int gy = 0; gy < height; gy++)
            {
                for (int gx = 0; gx < width; gx++)
                {
                    Map[gx, gy] = true;
                }
            }
        }
    }

    public void LoadAssets()
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

        if (Engine.GetMouseButtonDown(MouseButton.Left) &&
            hx >= 0 && hy < Map.GetLength(0) && hy >= 0 && hy < Map.GetLength(1))
        {
            Map[hx, hy] = !Map[hx, hy];
            MustSaveMap = true;
        }

        for (int gy = 0; gy < 20; gy++)
        {
            for (int gx = 0; gx < 20; gx++)
            {
                if (Map[gx, gy])
                {
                    Engine.DrawTexture(
                        Assets.Tiles,
                        Origin + new Vector2((gx - gy) * 16, (gy + gx) * 8),
                        source: new Bounds2(0, 0, 32, 32));
                }
            }
        }

        if (MustSaveMap)
        {
            MustSaveMap = false;

            using (BinaryWriter w = new BinaryWriter(File.OpenWrite(MapFilePath)))
            {
                w.Write(MapFileVersion);
                w.Write(Map.GetLength(0));
                w.Write(Map.GetLength(1));

                for (int gy = 0; gy < Map.GetLength(1); gy++)
                {
                    for (int gx = 0; gx < Map.GetLength(0); gx++)
                    {
                        w.Write(Map[gx, gy]);
                    }
                }
            }
        }
    }
}
