using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    class FrameWizard
    {
        static public DialogResult ShowWizard(ReferenceFrame frame)
        {
            WizardPropsBinding props = GetPropsObject();
            props.Data = frame;

            props.UpdateTabVisibility += new UpdateTabDelegate(props_UpdateTabVisibility);

            WizardShell shell = new WizardShell(props);

            return shell.ShowDialog();
        }

        static void props_UpdateTabVisibility(object sender, object e)
        {
            WizardPropsBinding props = e as WizardPropsBinding;
            ReferenceFrame frame = props.Data as ReferenceFrame;
            switch (frame.ReferenceFrameType)
            {
                case ReferenceFrameTypes.FixedSherical:
                    props.UpdateVisible(typeof(FrameWizardPosition), true);
                    props.UpdateVisible(typeof(FrameWizardTrajectory), false);
                    props.UpdateVisible(typeof(FrameWizardOrbital), false);
                    break;
                //case ReferenceFrameTypes.FixedRectangular:
                //    props.UpdateVisible(typeof(FrameWizardPosition), false);
                //    props.UpdateVisible(typeof(FrameWizardCartisian), true);
                //    props.UpdateVisible(typeof(FrameWizardOrbital), false);
                //    break;
                case ReferenceFrameTypes.Orbital:
                    props.UpdateVisible(typeof(FrameWizardPosition), false);
                    props.UpdateVisible(typeof(FrameWizardTrajectory), false);
                    props.UpdateVisible(typeof(FrameWizardOrbital), true);
                    break;
                case ReferenceFrameTypes.Trajectory:
                    props.UpdateVisible(typeof(FrameWizardPosition), false);
                    props.UpdateVisible(typeof(FrameWizardTrajectory), true);
                    props.UpdateVisible(typeof(FrameWizardOrbital), false);
                    break;
                default:
                    break;
            }
        }



        static public DialogResult ShowPropertiesSheet(ReferenceFrame frame)
        {
            WizardPropsBinding props = GetPropsObject();
            props.Data = frame;

            PropsShell shell = new PropsShell(props);
            props.UpdateTabVisibility += new UpdateTabDelegate(props_UpdateTabVisibility);

            return shell.ShowDialog();
        }

        private static WizardPropsBinding GetPropsObject()
        {
            WizardPropsBinding props;
            props = new WizardPropsBinding();
            props.WizardName = Language.GetLocalizedText(926, "Reference Frame");
            props.Pages.Add(new WizPropPageElement(Language.GetLocalizedText(918, "Welcome"), typeof(FrameWizardWelcome), true, true));
            props.Pages.Add(new WizPropPageElement(Language.GetLocalizedText(928, "General Options"), typeof(FrameWizardMain), false, true));
            props.Pages.Add(new WizPropPageElement(Language.GetLocalizedText(919, "Position"), typeof(FrameWizardPosition), false, true));
            props.Pages.Add(new WizPropPageElement(Language.GetLocalizedText(927, "Trajectory"), typeof(FrameWizardTrajectory), false, true));
            props.Pages.Add(new WizPropPageElement(Language.GetLocalizedText(919, "Position"), typeof(FrameWizardOrbital), false, true));
            props.Pages.Add(new WizPropPageElement(Language.GetLocalizedText(929, "Your Reference Frame is Complete"), typeof(FrameWizardFinal), true, true));

            return props;
        }
    }
}
