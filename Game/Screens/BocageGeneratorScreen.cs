using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class BocageGeneratorScreen : Screen
{
    public void Start()
    {
    }

    public void Update()
    {
        Engine.DrawRectEmpty(new Bounds2(100, 100, 400, 300), Color.Green);
    }
}
