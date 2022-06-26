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
    public static Texture PostRed;
    public static Texture PostGreen;
    public static Texture DotRed;
}

class Game
{
    public static readonly string Title = "Isometric Demo";
    public static readonly Vector2 Resolution = new Vector2(960, 540);

    public static readonly bool Debug = true;
    public static readonly bool DebugCollision = false;

    Screen CurrentScreen;

    public Game()
    {
        LoadAssets();
        SetScreen(new IsometricPathfindingScreen());
    }

    void LoadAssets()
    {
        Assets.DefaultFont = new SpriteFont("font.png");
        Assets.Tiles = Engine.LoadTexture("tiles.png");
        Assets.PostRed = Engine.LoadTexture("post_red.png");
        Assets.PostGreen = Engine.LoadTexture("post_green.png"); 
        Assets.DotRed = Engine.LoadTexture("dot_red.png");
    }

    void SetScreen(Screen screen)
    {
        CurrentScreen = screen;
        CurrentScreen.Start();
    }

    public void Update()
    {
        CurrentScreen.Update();
    }
}

interface Screen
{
    void Start();
    void Update();
}
