using System;
using System.Linq;
using System.Collections.Generic;

class Game
{
    public static readonly string Title = "Isometric Demo";
    public static readonly Vector2 Resolution = new Vector2(960, 540);

    public static readonly bool Debug = true;
    public static readonly bool DebugCollision = false;

    Screen CurrentScreen;

    public Game()
    {
        Assets.LoadAll();
        SetScreen(new BocageGeneratorScreen());
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
