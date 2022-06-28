using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class U
{
    public static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
    {
        return a + (b - a) * t;
    }
}
