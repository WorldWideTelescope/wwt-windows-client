using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerraViewer
{
    class GridLayer : Layer
    {
        public override bool Draw(RenderContext11 renderContext, float opacity, bool flat)
        {
            Grids.DrawPlanetGrid(renderContext, opacity * Opacity, Color);
            Grids.DrawPlanetGridText(renderContext, opacity * Opacity, Color);
            return true;
        }
    }
}
