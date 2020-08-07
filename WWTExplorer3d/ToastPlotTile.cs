using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;


namespace TerraViewer
{
    //todo11 Verify and test plot tile
    public class ToastPlotTile : ToastTile
    {
        public ToastPlotTile(int level, int x, int y, IImageSet dataset, Tile parent) : base(level, x, y, dataset, parent)
        {

        }
    }

}
