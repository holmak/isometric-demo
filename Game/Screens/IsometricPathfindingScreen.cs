using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

class IsometricPathfindingScreen : Screen
{
    static string MapFilePath => Engine.GetAssetPath("tile.map");
    static readonly int MapFileVersion = 1;
    static readonly int MaxAgents = 10;
    static readonly float AgentSpeed = 5.0f;
    static readonly float WaypointRadius = 0.1f;

    Vector2 Origin = new Vector2(480, 40);
    bool[,] Map;
    bool MustSaveMap = false;
    InputState Input = new InputState();
    List<Agent> Agents = new List<Agent>();
    Random Random = new Random();

    public void Start()
    {
        // Initialize the default map:
        {
            int width = 30;
            int height = 30;
            Map = new bool[width, height];

            for (int gy = 0; gy < height; gy++)
            {
                for (int gx = 0; gx < width; gx++)
                {
                    Map[gx, gy] = true;
                }
            }
        }

        // Load saved map data:
        if (File.Exists(MapFilePath))
        {
            using (BinaryReader r = new BinaryReader(File.OpenRead(MapFilePath)))
            {
                int version = r.ReadInt32();
                int width = r.ReadInt32();
                int height = r.ReadInt32();

                width = Math.Min(width, Map.GetLength(0));
                height = Math.Min(height, Map.GetLength(1));

                for (int gy = 0; gy < height; gy++)
                {
                    for (int gx = 0; gx < width; gx++)
                    {
                        Map[gx, gy] = r.ReadBoolean();
                    }
                }
            }
        }

        Reset();
    }

    void Reset()
    {
        Agents.Clear();

        for (int i = 0; i < MaxAgents; i++)
        {
            Vector2 pos = CellCenter(new Index2(i, 0));
            Agents.Add(new Agent
            {
                Position = pos,
                GoalShown = pos,
            });
        }
    }

    public void Update()
    {
        //=========================================================================
        // Reset
        //=========================================================================

        if (Engine.GetKeyDown(Key.F2))
        {
            Reset();
        }

        //=========================================================================
        // Agent behavior
        //=========================================================================

        foreach (Agent self in Agents)
        {
            if (self.Path.Count == 0)
            {
                Index2 goal = new Index2(Random.Next(Map.GetLength(0)), Random.Next(Map.GetLength(1)));
                self.Path = FindPath(RoundToGridCell(self.Position), goal);
                self.GoalShown = CellCenter(goal);
            }

            Vector2 immediateTarget = self.Position;

            while (self.Path.Count > 0)
            {
                Index2 next = self.Path[0];
                immediateTarget = CellCenter(next);

                if (self.Position.DistanceTo(CellCenter(next)) < WaypointRadius)
                {
                    self.Path.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }

            Vector2 motion = immediateTarget - self.Position;
            float distance = motion.Length();
            if (distance > 0)
            {
                float limit = Math.Min(distance, AgentSpeed * Engine.TimeDelta);
                motion *= (limit / distance);
            }

            self.Position += motion;
        }

        //=========================================================================
        // Input processing
        //=========================================================================

        Index2 hoveredCell = RoundToGridCell(ScreenToGrid(Engine.MousePosition));
        Vector2 hovered = ScreenToGrid(Engine.MousePosition);

        Assets.DefaultFont.Draw(string.Format("Hovered cell: {0}", hoveredCell), new Vector2(20, 40), Color.White);
        Assets.DefaultFont.Draw(string.Format("Hovered coordinate: {0}", hovered), new Vector2(20, 60), Color.White);

        if (Input.Mode == InputMode.Default)
        {
            if (Engine.GetMouseButtonDown(MouseButton.Left))
            {
                if (CellInBounds(hoveredCell))
                {
                    SetCell(hoveredCell, !GetCell(hoveredCell));
                    MustSaveMap = true;
                }
            }
        }

        if (Engine.GetMouseButtonDown(MouseButton.Right))
        {
            Vector2 target = ScreenToGrid(Engine.MousePosition);

            foreach (Agent agent in Agents)
            {
                agent.Path = FindPath(RoundToGridCell(agent.Position), RoundToGridCell(target));
                agent.GoalShown = target;
                target.X += 1;
            }
        }

        //=========================================================================
        // Draw
        //=========================================================================

        for (int gy = 0; gy < Map.GetLength(1); gy++)
        {
            for (int gx = 0; gx < Map.GetLength(0); gx++)
            {
                Index2 cell = new Index2(gx, gy);

                if (GetCell(cell))
                {
                    Vector2 pos = Origin + new Vector2((cell.X - cell.Y) * 16 - 16, (cell.X + cell.Y) * 8 - 16);
                    Engine.DrawTexture(
                        Assets.Tiles,
                        pos,
                        source: new Bounds2(0, 0, 32, 32));
                }
            }
        }

        foreach (Agent agent in Agents)
        {
            foreach (Index2 waypoint in agent.Path)
            {
                DrawIsoSprite(Assets.DotRed, CellCenter(waypoint));
            }

            DrawIsoSprite(Assets.PostRed, agent.GoalShown);
            DrawIsoSprite(Assets.PostGreen, agent.Position);
        }

        //=========================================================================
        // End of frame
        //=========================================================================

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

    Vector2 GridToScreen(Vector2 grid)
    {
        return Origin + new Vector2((grid.X - grid.Y) * 16, (grid.X + grid.Y) * 8);
    }

    /// <summary>
    /// Returns the fractional grid coordinate corresponding to the screen position.
    /// </summary>
    Vector2 ScreenToGrid(Vector2 point)
    {
        point = point - Origin;
        return new Vector2(
            (point.X / 32) + (point.Y / 16),
            (-point.X / 32) + (point.Y / 16));
    }

    Index2 RoundToGridCell(Vector2 grid)
    {
        return new Index2((int)Math.Floor(grid.X), (int)Math.Floor(grid.Y));
    }

    Vector2 CellCenter(Index2 cell)
    {
        return new Vector2(cell.X + 0.5f, cell.Y + 0.5f);
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
        Engine.DrawTexture(texture, GridToScreen(center) - texture.Size * new Vector2(0.5f, 0.75f));
    }

    List<Index2> FindPath(Index2 start, Index2 end)
    {
        Queue<Index2> frontier = new Queue<Index2>();
        Dictionary<Index2, Index2> backlinks = new Dictionary<Index2, Index2>();
        frontier.Enqueue(start);

        while (frontier.Count > 0)
        {
            Index2 here = frontier.Dequeue();

            if (here == end)
            {
                List<Index2> path = new List<Index2>();
                while (here != start)
                {
                    path.Add(here);
                    here = backlinks[here];
                }

                path.Reverse();
                return path;
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

        return new List<Index2>();
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
}
