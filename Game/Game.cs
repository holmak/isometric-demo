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
    public static Texture PostRed;
    public static Texture PostGreen;
}

class Game
{
    public static readonly string Title = "Isometric Demo";
    public static readonly Vector2 Resolution = new Vector2(1280, 768);

    public static readonly bool Debug = true;
    public static readonly bool DebugCollision = false;

    static readonly string MapFilePath = "tile.map";
    static readonly int MapFileVersion = 1;
    static readonly float ClickToSelectRadius = 8;

    Vector2 Origin = new Vector2(600, 140);
    bool[,] Map;
    bool MustSaveMap = false;
    Vector2 StartPoint, EndPoint;
    InputState Input = new InputState();

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

        StartPoint = GridCellCenter(new Index2(2, 1));
        EndPoint = GridCellCenter(new Index2(15, 5));
    }

    public void LoadAssets()
    {
        Assets.DefaultFont = new SpriteFont("font.png");
        Assets.Tiles = Engine.LoadTexture("tiles.png");
        Assets.PostRed = Engine.LoadTexture("post_red.png");
        Assets.PostGreen = Engine.LoadTexture("post_green.png");
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

        if (Input.Mode == InputMode.Default)
        {
            if (Engine.GetMouseButtonDown(MouseButton.Left))
            {
                if (StartPoint.DistanceTo(Engine.MousePosition) < ClickToSelectRadius)
                {
                    Input.Mode = InputMode.MovingStart;
                }
                else if (EndPoint.DistanceTo(Engine.MousePosition) < ClickToSelectRadius)
                {
                    Input.Mode = InputMode.MovingEnd;
                }
                else if (hx >= 0 && hx < Map.GetLength(0) && hy >= 0 && hy < Map.GetLength(1))
                {
                    Map[hx, hy] = !Map[hx, hy];
                    MustSaveMap = true;
                }
            }
        }
        else if (Input.Mode == InputMode.MovingStart)
        {
            StartPoint = Engine.MousePosition;

            if (Engine.GetMouseButtonUp(MouseButton.Left))
            {
                Input.Mode = InputMode.Default;
            }
        }
        else if (Input.Mode == InputMode.MovingEnd)
        {
            EndPoint = Engine.MousePosition;

            if (Engine.GetMouseButtonUp(MouseButton.Left))
            {
                Input.Mode = InputMode.Default;
            }
        }

        for (int gy = 0; gy < 20; gy++)
        {
            for (int gx = 0; gx < 20; gx++)
            {
                if (Map[gx, gy])
                {
                    Index2 cell = new Index2(gx, gy);

                    Engine.DrawTexture(
                        Assets.Tiles,
                        GridCellCorner(cell),
                        source: new Bounds2(0, 0, 32, 32));
                }
            }
        }

        DrawIsoSprite(Assets.PostGreen, StartPoint);
        DrawIsoSprite(Assets.PostRed, EndPoint);

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

    Vector2 GridCellCorner(Index2 cell)
    {
        return Origin + new Vector2((cell.X - cell.Y) * 16, (cell.X + cell.Y) * 8);
    }

    Vector2 GridCellCenter(Index2 cell)
    {
        return GridCellCorner(cell) + new Vector2(16, 24);
    }

    void DrawIsoSprite(Texture texture, Vector2 center)
    {
        Engine.DrawTexture(texture, center - texture.Size * new Vector2(0.5f, 0.75f));
    }
}

class InputState
{
    public InputMode Mode;
    public int State;
}

enum InputMode
{
    Default,
    EditingTiles,
    MovingStart,
    MovingEnd,
}
