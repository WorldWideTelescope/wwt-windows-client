using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public delegate void UpdateTabDelegate(object sender, Object e);
    public delegate bool RedayForNextDelegate(object sender, bool ready);
    public class WizardPropsBinding
    {
        public List<WizPropPageElement> Pages = new List<WizPropPageElement>();
            
        public object Data;

        public string WizardName;

        public event UpdateTabDelegate UpdateTabVisibility;
      //  public event EventHandler UpdateTabs;
        public event RedayForNextDelegate ReadyForNext;
        public void SendUpdateTab(object sender)
        {
            if (UpdateTabVisibility != null)
            {
                UpdateTabVisibility.Invoke(sender, this);
            }
        }

        public void SendReadyStatus(object sender, bool ready)
        {
            if (ReadyForNext != null)
            {
                ReadyForNext.Invoke(sender, ready);
            }
        }

        public DialogResult ShowModal(WizPropsStyle style)
        {
            WizardShell shell = new WizardShell(this);

            
            return shell.ShowDialog();
        }

        public void UpdateVisible(Type targetType, bool visible)
        {
            foreach (WizPropPageElement element in this.Pages)
            {
                if (element.Page == targetType)
                {
                    element.Visible = visible;
                    return;
                }
            }
        }


    }

    public enum WizPropsStyle { Wizard, Properties};

    public class WizPropPageElement
    {
        public WizPropPageElement()
        {
        }

        public WizPropPageElement(string title, Type page, bool wizardOnly, bool visible)
        {

            Visible = visible;
            Title = title;
            Page = page;
            WizardOnly = wizardOnly;
        }
        public string Title;
        public Type Page;
        public bool WizardOnly = false;
        public bool StepComplete = false;
        public bool Visible = true;
    }
}
