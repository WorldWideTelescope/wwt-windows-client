using System;

namespace TerraViewer
{
    public class LayerInfo
    {
        public Guid ID = Guid.Empty;
        public float StartOpacity = 1;
        public float EndOpacity = 1;
        public float FrameOpacity = 1;
        public double[] StartParams = new double[0];
        public double[] EndParams = new double[0];
        public double[] FrameParams = new double[0];
    }
}
