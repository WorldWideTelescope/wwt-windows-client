using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{

    enum LayerActions { Hide, Show };

    class LayerScripting : IScriptable
    {

        #region IScriptable Members
        ScriptableProperty[] IScriptable.GetProperties()
        {
            var props = new List<ScriptableProperty>();

            props.Add(new ScriptableProperty("Opacity", ScriptablePropertyTypes.BlendState, ScriptablePropertyScale.Linear, 0, 1, true));
            props.Add(new ScriptableProperty("Heading", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -180, +180, false));
            props.Add(new ScriptableProperty("Pitch", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -180, +180, false));
            props.Add(new ScriptableProperty("Roll", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -180, +180, false));
            props.Add(new ScriptableProperty("Scale", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Log, .1, 10, false));
            props.Add(new ScriptableProperty("Translate.X", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -10, +10, false));
            props.Add(new ScriptableProperty("Translate.Y", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -10, +10, false));
            props.Add(new ScriptableProperty("Translate.Z", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -10, +10, false));
            return props.ToArray();
        }

        string[] IScriptable.GetActions()
        {
            return Enum.GetNames(typeof(LayerActions));
        }

        void IScriptable.InvokeAction(string name, string value)
        {
                if (string.IsNullOrEmpty(name))
                {
                    return;
                }
                try
                {
                    var action = (LayerActions)Enum.Parse(typeof(LayerActions), name, true);

                    switch (action)
                    {
                        case LayerActions.Hide:
                            break;
                        case LayerActions.Show:
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                }

        }

        void IScriptable.SetProperty(string name, string value)
        {
            try
            {
                if (LayerManager.CurrentSelection is LayerMap)
                {
                    var map = LayerManager.CurrentSelection as LayerMap;
                    if (map.Frame.reference == ReferenceFrames.Custom)
                    {
                        var frame = map.Frame;
                        double val;
                        if (name.ToLower() == "scale")
                        {

                            val = double.Parse(value);

                            frame.Scale = val;
                            return;
                        }

                        if (name.ToLower().StartsWith("translate."))
                        {
                            val = double.Parse(value);
                            var translate = frame.translation;
                            switch (name.ToLower())
                            {
                                case "translate.x":
                                    translate.X = val;

                                    break;
                                case "translate.y":
                                    translate.Y = val;
                                    break;
                                case "translate.z":
                                    translate.Z = val;
                                    break;
                            }
                            frame.translation = translate;
                            return;
                        }

                        frame.SetProp(name, value);
                    }
                }
                if (LayerManager.CurrentSelection is Layer)
                {
                    
                    //MethodInvoker doIt = delegate
                    //{
                    //    Earth3d.MainWindow.Text = value;
                    //};

                    //Earth3d.MainWindow.Invoke(doIt);

                    var layer = LayerManager.CurrentSelection as Layer;
                    if ( layer is Object3dLayer)
                    {
                        var object3d = layer as Object3dLayer;
                        double val;
                        if (name.ToLower() == "scale")
                        {
                            
                            val = double.Parse(value);

                            object3d.Scale = new Vector3d(val,val,val);
                            return;
                        }

                        if (name.ToLower().StartsWith("translate."))
                        {
                            val = double.Parse(value);
                            var translate = object3d.Translate;
                            switch (name.ToLower())
                            {
                                case "translate.x":
                                    translate.X = val;

                                    break;
                                case "translate.y":
                                    translate.Y = val;
                                    break;
                                case "translate.z":
                                    translate.Z = val;
                                    break;
                            }
                            object3d.Translate = translate;
                        }

                        layer.SetProp(name, value);
                    }
                }
            }
            catch
            {
            }
        }

        string IScriptable.GetProperty(string name)
        {
            try
            {
                try
                {
                    if (LayerManager.CurrentSelection is Layer)
                    {
                        var layer = LayerManager.CurrentSelection as Layer;

                        return layer.GetProp(name);
                    }
                }
                catch
                {
                }
            }
            catch
            {

            }
            return null;     
        }

        bool IScriptable.ToggleProperty(string name)
        {

            if (LayerManager.CurrentSelection is Layer)
            {
                var layer = LayerManager.CurrentSelection as Layer;

                if (name == "Opacity")
                {
                    {
                        if (layer.Opacity > 0)
                        {
                            layer.Opacity = 100;
                            return true;
                        }
                        else
                        {
                            layer.Opacity = 0;
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        #endregion
    }
}
