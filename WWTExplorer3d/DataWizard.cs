using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    class DataWizard
    {
        static public DialogResult ShowWizard(TimeSeriesLayer layer)
        {
            WizardPropsBinding props = GetPropsObject();
            props.Data = layer;

            props.UpdateTabVisibility += new UpdateTabDelegate(props_UpdateTabVisibility);

            WizardShell shell = new WizardShell(props);

            return shell.ShowDialog();
        }

        static void props_UpdateTabVisibility(object sender, object e)
        {
            WizardPropsBinding props = e as WizardPropsBinding;
            TimeSeriesLayer layer = props.Data as TimeSeriesLayer;

            switch (layer.CoordinatesType)
            {
                case TimeSeriesLayer.CoordinatesTypes.Spherical:
                    props.UpdateVisible(typeof(DataWizardCoordinates), true);
                    props.UpdateVisible(typeof(DataWizardCartesian), false);
                    props.UpdateVisible(typeof(DataWizardOrbits), false);

                    break;
                case TimeSeriesLayer.CoordinatesTypes.Rectangular:
                    props.UpdateVisible(typeof(DataWizardCoordinates), false);
                    props.UpdateVisible(typeof(DataWizardCartesian), true);
                    props.UpdateVisible(typeof(DataWizardOrbits), false);
                    break;
                case TimeSeriesLayer.CoordinatesTypes.Orbital:
                    props.UpdateVisible(typeof(DataWizardCoordinates), false);
                    props.UpdateVisible(typeof(DataWizardCartesian), false);
                    props.UpdateVisible(typeof(DataWizardOrbits), true);
                    break;
                default:
                    break;
            }

        }


        
        static public DialogResult ShowPropertiesSheet(TimeSeriesLayer layer)
        {
            WizardPropsBinding props = GetPropsObject();
            props.Data = layer;

            PropsShell shell = new PropsShell(props);
            props.UpdateTabVisibility += new UpdateTabDelegate(props_UpdateTabVisibility);

            return shell.ShowDialog();
        }

        private static WizardPropsBinding GetPropsObject()
        {
            WizardPropsBinding props;
            props = new WizardPropsBinding();
            props.WizardName = Language.GetLocalizedText(917, "Data Visualization Layer");
            props.Pages.Add(new WizPropPageElement(Language.GetLocalizedText(918, "Welcome"), typeof(DataWizardWelcome), true, true));
            props.Pages.Add(new WizPropPageElement(Language.GetLocalizedText(919, "Position"), typeof(DataWizardCoordinates), false, true));
            props.Pages.Add(new WizPropPageElement(Language.GetLocalizedText(919, "Position"), typeof(DataWizardCartesian), false, true));
            props.Pages.Add(new WizPropPageElement(Language.GetLocalizedText(919, "Position"), typeof(DataWizardOrbits), false, true));
            props.Pages.Add(new WizPropPageElement(Language.GetLocalizedText(920, "Scale"), typeof(DataWizardSize), false, true));
            props.Pages.Add(new WizPropPageElement(Language.GetLocalizedText(921, "Markers"), typeof(DataWizardMarkers), false, true));
            props.Pages.Add(new WizPropPageElement(Language.GetLocalizedText(922, "Color Map"), typeof(DataWizardColorMap), false, true));
            props.Pages.Add(new WizPropPageElement(Language.GetLocalizedText(923, "Date Time"), typeof(DataWizardDateTime), false, true));
            props.Pages.Add(new WizPropPageElement(Language.GetLocalizedText(924, "Hover Text"), typeof(DataWizardInfoPopup), false, true));
            props.Pages.Add(new WizPropPageElement(Language.GetLocalizedText(925, "Your data is ready to view"), typeof(DataWizardFinal), true, true));

            return props;
        }
    }
}
