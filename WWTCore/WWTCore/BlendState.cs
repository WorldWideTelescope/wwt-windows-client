using System;
using System.Globalization;
using System.ComponentModel;
using System.Configuration;

namespace TerraViewer
{
    [TypeConverter(typeof(BlendStateConverter))]
    [SettingsSerializeAs(SettingsSerializeAs.String)]
    public class BlendState
    {
        bool state;

        public bool State
        {
            get
            {
                if (targetState != state && auto)
                {
                    var ts = SpaceTimeController.MetaNow.Subtract(switchedTime);
                    if (ts.TotalMilliseconds > delayTime)
                    {
                        state = targetState;
                    }
                    if (targetState != state)
                    {
                        return true;
                    }
                }

                return state;
            }
            set
            {
                switchedTime = SpaceTimeController.MetaNow.Subtract(new TimeSpan(1, 0, 0));
                state = value;
                targetState = state;
            }
        }
       
        public bool SettingsOwned = false;
        bool targetState;
        bool auto = true;

        public bool Auto
        {
            get { return auto; }
            set { auto = value; }
        }
        public bool TargetState
        {
            get
            {
                return targetState;
            }
            set
            {
                if (targetState != value)
                {
                    
                    var ts = SpaceTimeController.MetaNow.Subtract(switchedTime);
                    switchedTime = SpaceTimeController.MetaNow;
                    if (ts.TotalMilliseconds < delayTime )
                    {

                        switchedTime -= TimeSpan.FromMilliseconds(delayTime * (1f - opacity));
                        state = targetState;
                    }

                    if (!auto && opacity != 1f && opacity != 0)
                    {

                        switchedTime -= TimeSpan.FromMilliseconds(delayTime * (1f - opacity));
                        state = targetState;
                    }

                    auto = true;
                    
                    targetState = value;
                    FireChanged();
                   
                }
                
            }
        }

        private void FireChanged()
        {
            if (SettingsOwned)
            {
                PulseMe.PulseForSettingsUpdate();
            }
        }

        public static BlendState debugTarget = null;

        private float opacity = 1f;
        public float Opacity
        {
            get
            {
                if (auto)
                {
                    if (targetState != state)
                    {
                        var ts = SpaceTimeController.MetaNow.Subtract(switchedTime);
                        if (ts.TotalMilliseconds > delayTime)
                        {
                            state = targetState;
                            opacity = state ? 1f : 0f;
                        }
                        else
                        {
                            opacity = (float)(Math.Min(1, Math.Max(0, (ts.TotalMilliseconds / delayTime))));


                            return targetState ? opacity : 1f - opacity;
                        }
                    }
                    return state ? 1f : 0f;
                }
                return state ? opacity : 0f;
            }
            set
            {

                if (opacity != value)
                {
                    var current = targetState;

                    opacity = value;
                    auto = false;
                    if (opacity == 0)
                    {
                        targetState = state = false;
                    }
                    else
                    {
                        targetState = state = true;
                    }

                    if (current != targetState)
                    {
                        FireChanged();
                    }

                    switchedTime = SpaceTimeController.MetaNow.Subtract(new TimeSpan(1, 0, 0));

                }
            }
        }

        DateTime switchedTime;
        double delayTime;

        public double DelayTime
        {
            get { return delayTime; }
            set { delayTime = value; }
        }

        public BlendState()
        {
            switchedTime = SpaceTimeController.MetaNow.Subtract(new TimeSpan(1, 0, 0));
            state = false;
            targetState = state;
            delayTime = 2000;
        }

        public BlendState(bool initialState, double delayTime)
        {
            switchedTime = SpaceTimeController.MetaNow.Subtract(new TimeSpan(1, 0, 0));
            state = initialState;
            opacity = state ? 1f : 0f;
            targetState = state;
            this.delayTime = delayTime;
        }

        public BlendState(bool initialState, double delayTime, float opacity)
        {
            switchedTime = SpaceTimeController.MetaNow.Subtract(new TimeSpan(1, 0, 0));
            state = initialState;
            this.opacity = opacity;
            targetState = initialState;
            this.delayTime = delayTime;
        }



        public override string ToString()
        {
            return string.Format("{0},{1},{2}", targetState, opacity, delayTime);
        }

        public static BlendState Parse(string val)
        {
            var parts = val.Split(new[] { ',' });

            var state = false;
            float opacity = 0;
            double delay = 2000;
            try
            {
                state = bool.Parse(parts[0]);
                if (parts.Length > 1)
                {
                    opacity = float.Parse(parts[1]);
                }

                if (parts.Length > 2)
                {
                    delay = int.Parse(parts[2]);
                }
            }
            catch
            {
            }
            var blendState = new BlendState(state, delay, opacity);
            return blendState;
        }
    }

    public class BlendStateConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }   
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var parts = ((string)value).Split(new[] { ',' });

                var state = false;
                float opacity = 0;
                double delay = 2000;
                try
                {
                    state = bool.Parse(parts[0]);
                    if (parts.Length > 1)
                    {
                        opacity = float.Parse(parts[1]);
                    }

                    if (parts.Length > 2)
                    {
                        delay = int.Parse(parts[2]);
                    }
                }
                catch
                {
                }
                var blendState = new BlendState(state, delay, opacity) {SettingsOwned = true};
                return blendState;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
