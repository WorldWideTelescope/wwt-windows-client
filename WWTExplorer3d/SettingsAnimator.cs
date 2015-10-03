using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerraViewer
{
    public class SettingsAnimator : IAnimatable
    {
        readonly StockSkyOverlayTypes settingsType = StockSkyOverlayTypes.AltAzGrid;

        double currentValue;

        public double CurrentValue
        {
            get { return currentValue; }
            set { currentValue = value; }
        }


        ConstellationFilter filter;

        public ConstellationFilter Filter
        {
            get { return filter; }
            set { filter = value; }
        }

        bool constant;

        public bool Constant
        {
            get
            {
                return constant;
            }

            set
            {
                constant = value;
            }
        }

        public SettingsAnimator(StockSkyOverlayTypes type)
        {
            settingsType = type;
            var sp = Settings.Ambient.GetSetting(settingsType);
            constant = sp.EdgeTrigger;

            if (sp.Filter != null)
            {
                filter = sp.Filter.Clone();
            }
        }

        public double[] GetParams()
        {
            var sp = Settings.Ambient.GetSetting(settingsType);
            if (sp.Filter == null)
            {
                return new double[] { sp.Opacity };
            }
            else
            {
                filter.SetBits(sp.Filter.GetBits());
                return new double[] { sp.Opacity, filter.Bits[0], filter.Bits[1], filter.Bits[2] };
            }
        }

        public string[] GetParamNames()
        {
            if (filter == null)
            {
                if (constant)
                {
                    return new string[] { "Visible" };
                }
                else
                {
                    return new string[] { "Opacity" };
                }
            }
            else
            {
                return new string[] { "Opacity", "Filter1", "Filter2", "Filter3" };
            }
        }

        public BaseTweenType[] GetParamTypes()
        {
            if (filter == null)
            {
                if (constant)
                {
                    return new BaseTweenType[] { BaseTweenType.Constant };
                }
                else
                {
                    return new BaseTweenType[] { BaseTweenType.Linear };
                }

            }
            else
            {
                return new BaseTweenType[] { BaseTweenType.Linear, BaseTweenType.Constant, BaseTweenType.Constant, BaseTweenType.Constant };
            }
        }

        public void SetParams(double[] paramList)
        {
            if (paramList.Length > 0)
            {
                currentValue = paramList[0];
            }
            if (filter != null && paramList.Length == 4)
            {
                filter.Bits[0] = (int)paramList[1];
                filter.Bits[1] = (int)paramList[2];
                filter.Bits[2] = (int)paramList[3];
            }
        }

        public string GetIndentifier()
        {
            return settingsType.ToString();
        }

        public string GetName()
        {
            return settingsType.ToString();
        }

        public IUiController GetEditUI()
        {
            return null;
        }
    }
}
