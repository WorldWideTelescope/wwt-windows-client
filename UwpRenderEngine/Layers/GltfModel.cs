using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using glTFLoader;

namespace TerraViewer
{
    public class GltfModel
    {
        public void LoadModel(System.IO.Stream stream)
        {
            var model = Interface.LoadModel(stream);

        }
    }
}
