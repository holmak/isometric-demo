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
    public static Texture DotRed;
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
        Assets.DotRed = Engine.LoadTexture("dot_red.png");
    }

    public void Update()
    {
        // Pathfinding
        List<Index2> path = new List<Index2>();
        {
            Index2 start = CellAtPoint(StartPoint);
            Index2 end = CellAtPoint(EndPoint);

            Queue<Index2> frontier = new Queue<Index2>();
            Dictionary<Index2, Index2> backlinks = new Dictionary<Index2, Index2>();
            frontier.Enqueue(start);

            while (frontier.Count > 0)
            {
                Index2 here = frontier.Dequeue();

                if (here == end)
                {
                    while (here != start)
                    {
                        path.Add(here);
                        here = backlinks[here];
                    }

                    path.Reverse();
                    break;
                }

                foreach (Index2 offset in Index2.NeigborOffsets)
                {
                    Index2 neighbor = here + offset;
                    if (!backlinks.ContainsKey(neighbor) && GetCell(neighbor))
                    {
                        frontier.Enqueue(neighbor);
                        backlinks.Add(neighbor, here);
                    }
                }
            }
        }

        Index2 hoveredCell = CellAtPoint(Engine.MousePosition);

        Assets.DefaultFont.Draw(string.Format("Hovered: {0}", hoveredCell), new Vector2(20, 40), Color.White);

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
                else if (CellInBounds(hoveredCell))
                {
                    SetCell(hoveredCell, !GetCell(hoveredCell));
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
                Index2 cell = new Index2(gx, gy);

                if (GetCell(cell))
                {
                    Engine.DrawTexture(
                        Assets.Tiles,
                        GridCellCorner(cell),
                        source: new Bounds2(0, 0, 32, 32));
                }
            }
        }

        foreach (Index2 cell in path)
        {
            DrawIsoSprite(Assets.DotRed, GridCellCenter(cell));
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

    Index2 CellAtPoint(Vector2 point)
    {
        point = point - Origin - new Vector2(16, 16);
        return new Index2(
            (int)Math.Floor((point.X / 32) + (point.Y / 16)),
            (int)Math.Floor((-point.X / 32) + (point.Y / 16)));
    }

    bool CellInBounds(Index2 cell)
    {
        return cell.X >= 0 && cell.X < Map.GetLength(0) && cell.Y >= 0 && cell.Y < Map.GetLength(1);
    }

    bool GetCell(Index2 cell)
    {
        if (CellInBounds(cell))
        {
            return Map[cell.X, cell.Y];
        }
        else
        {
            return false;
        }
    }

    void SetCell(Index2 cell, bool value)
    {
        Map[cell.X, cell.Y] = value;
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
