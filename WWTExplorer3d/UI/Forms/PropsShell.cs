using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace TerraViewer
{
    public partial class PropsShell : Form
    {
        private WizardPropsBinding target;

        internal WizardPropsBinding Target
        {
            get { return target; }
            set { target = value; }
        }

        public PropsShell(WizardPropsBinding target)
        {
            this.target = target;
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.Finish.Text = Language.GetLocalizedText(930, "Done");
        }
        Tab[] tabs;
        private void WizardShell_Load(object sender, EventArgs e)
        {
            if (Target == null && Target.Pages.Count == 0)
            {
                DialogResult = DialogResult.Cancel;
                this.Close();
            }

            target.SendUpdateTab(this);
            BuildTabs();


            Contents.Visible = false;
            ShowTab(tabs[0].Tag as WizPropPageElement);
            this.Text = Target.WizardName + " Properties";
       //     target.UpdateTabs += new EventHandler(target_UpdateTabs);
            target.ReadyForNext += new RedayForNextDelegate(target_ReadyForNext);
        }
        bool showOk = true;
        bool target_ReadyForNext(object sender, bool ready)
        {
            showOk = ready;
            return true;
        }

        void target_UpdateTabs(object sender, EventArgs e)
        {
            BuildTabs();
        }

        private void BuildTabs()
        {
            int count = Controls.Count;
            int i = 0;
            // Remove any old controls
            for (i = 0; i < count; i++)
            {
                if (Controls[i] is Tab)
                {
                    Controls.RemoveAt(i);
                    i--;
                }
            }

            int tabCount = 0;
            foreach (WizPropPageElement page in Target.Pages)
            {
                if (!page.WizardOnly && page.Visible)
                {
                    tabCount++;
                }
            }

            tabs = new Tab[tabCount];
            i = 0;
            foreach (WizPropPageElement page in Target.Pages)
            {
                if (!page.WizardOnly && page.Visible)
                {
                    tabs[i] = new Tab();
                    tabs[i].Location = new Point(7 + i * 100, 7);
                    tabs[i].Title = page.Title;
                    tabs[i].Tag = page;
                    tabs[i].Click += new EventHandler(PropsShell_Click);
                    i++;
                }
            }
            this.Controls.AddRange(tabs);
            tabs[0].Selected = true;

            foreach (Tab tab in tabs)
            {
                tab.BringToFront();
            }
        }

        void PropsShell_Click(object sender, EventArgs e)
        {
            Tab tab = sender as Tab;

            if (tab == null)
            {
                return;
            }

            WizPropPageElement page = tab.Tag as WizPropPageElement;

            if (page == null)
            {
                return;
            }

            tab.Selected = true;

            foreach (Tab item in tabs)
            {
                if (item != sender)
                {
                    item.Selected = false;
                }
            }

            ShowTab(page);
        }

        PropPage currentPage = null;
        private void ShowTab(WizPropPageElement element)
        {

            if (currentPage != null)
            {
                currentPage.Save();
                this.Controls.Remove(currentPage);
                currentPage.Dispose();
            }


            PropPage page = (PropPage)Activator.CreateInstance(element.Page);
            page.Top = this.Contents.Top;
            page.Left = this.Contents.Left;
            page.Binding = target;
            page.SetData(Target.Data);
            this.Controls.Add(page);

            currentPage = page;
            UpdateButtonStates();
        }
       

        private void UpdateButtonStates()
        {

        }


        private void Finish_Click(object sender, EventArgs e)
        {
            if (currentPage != null)
            {
                currentPage.Save();
            }
            DialogResult = DialogResult.OK;
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush b = new LinearGradientBrush(new Point(0, 0), new Point(0, Height), Color.FromArgb(20, 30, 39), Color.FromArgb(41, 49, 73));
            Pen p = new Pen(Color.FromArgb(71, 84, 108));
            g.FillRectangle(b, this.ClientRectangle);
            g.DrawRectangle(p, new Rectangle(0, ClientSize.Height - 1, ClientSize.Width - 1, ClientSize.Height - 1));
            p.Dispose();
            b.Dispose();
        }
    }
}
