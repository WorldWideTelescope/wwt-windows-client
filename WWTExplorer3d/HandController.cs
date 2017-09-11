using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraViewer
{
    public class HandController
    {
        public HandController(HandType hand)
        {
            Active = false;
            Hand = hand;
        }

        public Vector3d Position;
        public Vector3d Forward;
        public Vector3d Up;
        public double ThumbX;
        public double ThumbY;
        public double Trigger;
        public double TouchX;
        public double TouchY;
        public double Grip;
        public bool Menu;
        public bool ThumbDown;
        public bool TouchDown;
        public bool Touched;
        public HandType Hand = HandType.Either;

        public bool Active { get; set; }
    }

    public enum HandType { Left, Right, Either};
}
