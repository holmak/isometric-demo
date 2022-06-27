using System;

static class Assets
{
public static SpriteFont DefaultFont;
public static Texture DotRed;
public static Texture PostGreen;
public static Texture PostRed;
public static Texture Tiles;

    public static void LoadAll()
    {
        DefaultFont = new SpriteFont("DefaultFont.png");
        DotRed = Engine.LoadTexture("DotRed.png");
        PostGreen = Engine.LoadTexture("PostGreen.png");
        PostRed = Engine.LoadTexture("PostRed.png");
        Tiles = Engine.LoadTexture("Tiles.png");
    }
}
