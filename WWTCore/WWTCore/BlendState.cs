using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
#if !WINDOWS_UWP
using System.Configuration;
#endif
namespace TerraViewer
{
#if !WINDOWS_UWP
    [TypeConverter(typeof(BlendStateConverter))]
    [SettingsSerializeAs(SettingsSerializeAs.String)]
#endif
    public class BlendState
    {
        bool state;

        public bool State
        {
            get
            {
                if (targetState != state && auto)
                {
                    TimeSpan ts = SpaceTimeController.MetaNow.Subtract(switchedTime);
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
                    
                    TimeSpan ts = SpaceTimeController.MetaNow.Subtract(switchedTime);
                    switchedTime = SpaceTimeController.MetaNow;
                    if (ts.TotalMilliseconds < delayTime )
                    {

                        switchedTime -= TimeSpan.FromMilliseconds(delayTime * (state ? 1f - opacity : 1f -opacity));
                        state = targetState;
                    }

                    if (!auto && opacity != 1f && opacity != 0)
                    {

                        switchedTime -= TimeSpan.FromMilliseconds(delayTime * (state ? 1f - opacity : 1f - opacity));
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
                    if (this == debugTarget)
                    {
                        if (opacity == 1)
                        {
                            int ttt = 0;
                        }
                    }

                    if (targetState != state)
                    {
                        TimeSpan ts = SpaceTimeController.MetaNow.Subtract(switchedTime);
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
                else
                {
                    return state ? opacity : 0f;
                }
            }
            set
            {

                if (opacity != value)
                {
                    if (this == debugTarget)
                    {
                        if (value == 1)
                        {
                            int ttt = 0;
                        }
                    }
                    bool current = targetState;

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
        double delayTime = 0;

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
            this.delayTime = 2000;
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
            this.state = initialState;
            this.opacity = opacity;
            this.targetState = initialState;
            this.delayTime = delayTime;
        }

        public static BlendState FromString(string value)
        {
            string[] parts = ((string)value).Split(new char[] { ',' });

            bool state = false;
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
            BlendState blendState = new BlendState(state, delay, opacity);
            blendState.SettingsOwned = true;
            return blendState;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", targetState, opacity, delayTime);
        }

        public static BlendState Parse(string val)
        {
            string[] parts = ((string)val).Split(new char[] { ',' });

            bool state = false;
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
            BlendState blendState = new BlendState(state, delay, opacity);
            return blendState;
        }
    }
#if !WINDOWS_UWP
    public class BlendStateConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }   
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] parts = ((string)value).Split(new char[] { ',' });

                bool state = false;
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
                BlendState blendState = new BlendState(state, delay, opacity);
                blendState.SettingsOwned = true;
                return blendState;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
#endif
}
