using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class BocageGeneratorScreen : Screen
{
    List<Area> Areas = new List<Area>();
    Random Random = new Random();

    public void Start()
    {
        Regenerate();
    }

    void Regenerate()
    {
        Areas.Clear();
        Areas.Add(new Area(new Bounds2(100, 50, 760, 440)));
    }

    public void Update()
    {
        if (Engine.GetKeyDown(Key.Space))
        {
            Area original = Areas[0];
            Areas.RemoveAt(0);

            float x0 = original.Bounds.Min.X;
            float x1 = original.Bounds.Max.X;
            float y0 = original.Bounds.Min.Y;
            float y1 = original.Bounds.Max.Y;
            float w = x1 - x0;
            float h = y1 - y0;
            float t = 0.3f + 0.4f * (float)Random.NextDouble();
            float wm = w * t;
            float hm = h * t;

            if (Random.Next(2) == 0)
            {
                // Split horizontally:
                Areas.Add(new Area(new Bounds2(x0, y0, wm, h)));
                Areas.Add(new Area(new Bounds2(x0 + wm, y0, w - wm, h)));
            }
            else
            {
                // Split vertically:
                Areas.Add(new Area(new Bounds2(x0, y0, w, hm)));
                Areas.Add(new Area(new Bounds2(x0, y0 + hm, w, h - hm)));
            }
        }

        Engine.Clear(new Color(0xE8, 0xE8, 0xE8));

        foreach (Area area in Areas)
        {
            Engine.DrawRectEmpty(area.Bounds.Expanded(-1), Color.Black);
        }
    }

    class Area
    {
        public Bounds2 Bounds;

        public Area(Bounds2 bounds)
        {
            Bounds = bounds;
        }
    }
}
