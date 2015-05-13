using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TerraViewer
{
    public class VisibleKey
    {
        public VisibleKey(AnimationTarget target, int parameterIndex, string propertyName, double time, Point point)
        {
            Target = target;
            ParameterIndex = parameterIndex;
            Time = time;
            Point = point;
            PropertyName = propertyName;
        }

        public AnimationTarget Target = null;
        public int ParameterIndex = 0;
        public double Time;
        public Point Point;
        public string PropertyName;

        public string IndexKey
        {
            get
            {
                return Target.TargetID + "\t" + ParameterIndex.ToString() + "\t" + KeyGroup.Quant(Time).ToString();
            }
        }
        public static string GetIndexKey(AnimationTarget target, int parameterIndex, double time)
        {
            return target.TargetID + "\t" + parameterIndex.ToString() + "\t" + KeyGroup.Quant(time).ToString();
        }
    }
}
