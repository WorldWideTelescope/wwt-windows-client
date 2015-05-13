using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerraViewer
{
    enum StockScriptActions { GotoSun, GotoMercury, GotoVenus, GotoEarth, GotoMars, GotoJupiter, GotoUranus, GotoNeptune, GotoPluto, GotoMilkyWay, GotoSDSSGalaxies, GotoComaCluster };
    class ScriptActions : IScriptable
    {
    

        ScriptableProperty[] IScriptable.GetProperties()
        {
            return new ScriptableProperty[0];
        }

        string[] IScriptable.GetActions()
        {
            throw new NotImplementedException();
        }

        void IScriptable.InvokeAction(string name, string value)
        {
            throw new NotImplementedException();
        }

        void IScriptable.SetProperty(string name, string value)
        {
            return;
        }

        string IScriptable.GetProperty(string name)
        {
            return null;
        }

        bool IScriptable.ToggleProperty(string name)
        {
            return false;
        }
    }
}
