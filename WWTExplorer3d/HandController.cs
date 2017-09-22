using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraViewer
{
    public class HandController
    {
        public HandController(HandType hand, bool last = false)
        {
            Active = false;
            Hand = hand;
            isLastState = last;
        }

        private bool isLastState = false;
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

        const double low = .2;
        const double high = .8;

        public bool Active { get; set; }
        public HandController LastState
        {
            get => lastState;
            set => lastState = value;
        }

        public HandControllerStatus Events = HandControllerStatus.None;
        public HandControllerStatus Status = HandControllerStatus.None;

        private HandController lastState = null;

        public void UpdateEvents()
        {
            if (isLastState)
            {
                throw new InvalidOperationException("Cant update Events on Last State");
            }

            Events = HandControllerStatus.None;
            Status = HandControllerStatus.None;
            if (lastState == null)
            {
                lastState = new HandController(Hand, true);
                // Skip updating since last == this
            }
            else
            {
                // Update events with state changes

                //Stick Left and Right
                if (lastState.Status.HasFlag(HandControllerStatus.StickLeft))
                {
                    if (ThumbX < -low)
                    {
                        Status = Status | HandControllerStatus.StickLeft;
                    }
                }
                else
                {
                    if (ThumbX < -high)
                    {
                        Status = Status | HandControllerStatus.StickLeft;
                        Events = Events | HandControllerStatus.StickLeft;
                    }
                }

                if (lastState.Status.HasFlag(HandControllerStatus.StickRight))
                {
                    if (ThumbX > low)
                    {
                        Status = Status | HandControllerStatus.StickRight;
                    }
                }
                else
                {
                    if (ThumbX > high)
                    {
                        Status = Status | HandControllerStatus.StickRight;
                        Events = Events | HandControllerStatus.StickRight;
                    }
                }

                // up and down
                if (lastState.Status.HasFlag(HandControllerStatus.StickDown))
                {
                    if (ThumbY < -low)
                    {
                        Status = Status | HandControllerStatus.StickDown;
                    }
                }
                else
                {
                    if (ThumbY < -high)
                    {
                        Status = Status | HandControllerStatus.StickDown;
                        Events = Events | HandControllerStatus.StickDown;
                    }
                }

                if (lastState.Status.HasFlag(HandControllerStatus.StickDown))
                {
                    if (ThumbY > low)
                    {
                        Status = Status | HandControllerStatus.StickDown;
                    }
                }
                else
                {
                    if (ThumbY > high)
                    {
                        Status = Status | HandControllerStatus.StickDown;
                        Events = Events | HandControllerStatus.StickDown;
                    }
                }

                if (!lastState.TouchDown && TouchDown )
                {
                    Events = Events | HandControllerStatus.TouchDown;
                }

                if (TouchDown)
                {
                    Status = Status | HandControllerStatus.TouchDown;
                }

                if (!lastState.Touched && Touched)
                {
                    Events = Events | HandControllerStatus.Touched;
                }

                if (lastState.Touched && !Touched)
                {
                    Events = Events | HandControllerStatus.UnTouch;
                }

                // up and down
                if (lastState.Status.HasFlag(HandControllerStatus.GripDown))
                {
                    if (Grip > low)
                    {
                        Status = Status | HandControllerStatus.GripDown;
                    }
                }
                else
                {
                    if (Grip > high)
                    {
                        Status = Status | HandControllerStatus.GripDown;
                        Events = Events | HandControllerStatus.GripDown;
                    }
                }

            }

            lastState.CopyState(this);
        }

        public void CopyState(HandController state)
        {
            Position = state.Position;
            Forward = state.Forward;
            Up = state.Up;
            ThumbX = state.ThumbX;
            ThumbY = state.ThumbY;
            Trigger = state.Trigger;
            TouchX = state.TouchX;
            TouchY = state.TouchY;
            Grip = state.Grip;
            Menu = state.Menu;
            ThumbDown = state.ThumbDown;
            TouchDown = state.TouchDown;
            Touched = state.Touched;
            Hand = state.Hand;
            Active = state.Active;
            Events = state.Events;
            Status = state.Status;
        }
    }
    public enum HandControllerStatus
    {
        None            =   0,
        StickLeft       =   1,
        StickRight       =   2,
        StickUp         =   4,
        StickDown       =   8,
        StickCenter     =   16,
        TriggerDown     =   32,
        TriggerUp       =   64,
        GripDown        =   128,
        GripUp          =   256,
        MenuUp          =   512,
        MenuDown        =   1024,
        TouchUp         =   2048,
        TouchDown       =   4096,
        Touched         =   8192,
        UnTouch         =   16384,
    };

    public enum HandType { Left, Right, Either};
}
