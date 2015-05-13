using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerraViewer
{
    public interface IScriptable
    {
        ScriptableProperty[] GetProperties();
        string[] GetActions();
        void InvokeAction(string name, string value);
        void SetProperty(string name, string value);
        string GetProperty(string name);
        bool ToggleProperty(string name);
    }

    public enum ScriptablePropertyTypes { Bool, BlendState, Integer, Double, Float, Enum, Color, String, ConstellationFilter };

    public enum ScriptablePropertyScale { None, Linear, Log, Power };

    public class ScriptableProperty
    {
        public ScriptableProperty(string name, ScriptablePropertyTypes type)
        {
            Name = name;
            Type = type;
            Togglable = (Type == ScriptablePropertyTypes.Bool || Type == ScriptablePropertyTypes.BlendState);
        }

        public ScriptableProperty(string name, ScriptablePropertyTypes type, ScriptablePropertyScale scale, double min, double max, bool togglable)
        {
            Name = name;
            Type = type;
            Min = min;
            Max = max;
            Scale = scale;
            Togglable = togglable;
        }

        public String Name;
        public ScriptablePropertyTypes Type = ScriptablePropertyTypes.BlendState;
        public ScriptablePropertyScale Scale = ScriptablePropertyScale.Linear;
        public double Min = 0;
        public double Max = 1;
        public bool Togglable = false;
        public override string ToString()
        {

            return Name;
        }
    }
}
