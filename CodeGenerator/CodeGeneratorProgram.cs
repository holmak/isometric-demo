using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class CodeGeneratorProgram
{
    public static void Main(string[] args)
    {
        while (!Directory.Exists("Assets"))
        {
            Directory.SetCurrentDirectory("..");
        }

        Directory.SetCurrentDirectory("Assets");

        string[] fontPaths = Directory.GetFiles("Fonts");
        string[] texturePaths = Directory.GetFiles("Textures");

        StringBuilder output = new StringBuilder();
        output.AppendLine("using System;");
        output.AppendLine();
        output.AppendLine("static class Assets");
        output.AppendLine("{");

        foreach (string path in fontPaths)
        {
            string name = Path.GetFileNameWithoutExtension(path);

            output.AppendLine(string.Format("public static SpriteFont {0};", name));
        }

        foreach (string path in texturePaths)
        {
            string name = Path.GetFileNameWithoutExtension(path);

            output.AppendLine(string.Format("public static Texture {0};", name));
        }

        output.AppendLine();

        output.AppendLine("    public static void LoadAll()");
        output.AppendLine("    {");

        foreach (string path in fontPaths)
        {
            string filename = Path.GetFileName(path);
            string name = Path.GetFileNameWithoutExtension(path);

            output.AppendLine(string.Format("        {0} = new SpriteFont(\"{1}\");", name, filename));
        }

        foreach (string path in texturePaths)
        {
            string filename = Path.GetFileName(path);
            string name = Path.GetFileNameWithoutExtension(path);

            output.AppendLine(string.Format("        {0} = Engine.LoadTexture(\"{1}\");", name, filename));
        }

        output.AppendLine("    }");
        output.AppendLine("}");

        File.WriteAllText("../Game/Generated/Assets.cs", output.ToString());
    }
}
