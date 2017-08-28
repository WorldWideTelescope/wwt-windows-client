using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerraViewer
{
    public interface IAnimatable
    {
        //Gets current Parameter set
        double[] GetParams();

        //Gets Descriptive Parameter Names
        string[] GetParamNames();

        //Gets Parameter base type. IE Position.X might be linear, Zoom might be Power
        BaseTweenType[] GetParamTypes();

        //Sets the parameter values
        void SetParams(double[] paramList);
        
        // Machine reabable for serialization
        string GetIndentifier();

        // Human readable name for the set
        string GetName();

        object GetEditUI();
    }

    public enum BaseTweenType { Linear, Power, Log, Boolean, Constant, PlanetID }; 
}
