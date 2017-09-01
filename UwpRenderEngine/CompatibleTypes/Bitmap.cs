using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraViewer
{
    public class Bitmap : IDisposable
    {
        public double Width { get;  set; }
        public double Height { get;  set; }

        public void Dispose()
        {
        }
    }
}
