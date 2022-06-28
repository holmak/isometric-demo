using System;

static class Assets
{
public static SpriteFont DefaultFont;
public static Texture DotRed;
public static Texture PostGreen;
public static Texture PostRed;
public static Texture Tiles;
public static Texture TreeMedium;

    public static void LoadAll()
    {
        DefaultFont = new SpriteFont("Fonts/DefaultFont.png");
        DotRed = Engine.LoadTexture("Textures/DotRed.png");
        PostGreen = Engine.LoadTexture("Textures/PostGreen.png");
        PostRed = Engine.LoadTexture("Textures/PostRed.png");
        Tiles = Engine.LoadTexture("Textures/Tiles.png");
        TreeMedium = Engine.LoadTexture("Textures/TreeMedium.png");
    }
}
