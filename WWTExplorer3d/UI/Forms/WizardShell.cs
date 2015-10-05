using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace TerraViewer
{
    public partial class WizardShell : Form
    {
        private WizardPropsBinding target;

        internal WizardPropsBinding Target
        {
            get { return target; }
            set { target = value; }
        }

        public WizardShell(WizardPropsBinding target)
        {
            this.target = target;
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            Finish.Text = Language.GetLocalizedText(911, "Finish");
            Next.Text = Language.GetLocalizedText(912, "Next");
            Back.Text = Language.GetLocalizedText(913, "Back");
        }

        private void WizardShell_Load(object sender, EventArgs e)
        {
            if (Target == null && Target.Pages.Count == 0)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }

            Contents.Visible = false;
            target.ReadyForNext += target_ReadyForNext;
            ShowNextPage();
            Text = Target.WizardName;

        }
        bool readyForNext = true;
        bool target_ReadyForNext(object sender, bool ready)
        {
            readyForNext = ready;
            UpdateButtonStates();
            return false;
        }

        int currentPageIndex = -1;
        PropPage currentPage;
        private void ShowNextPage()
        {
            target.SendUpdateTab(this);

            if (currentPageIndex >= (Target.Pages.Count-1))
            {
                return;
            }

            var checkPageIndex = currentPageIndex+1;
            var foundVisiblePage = false;
            while (!(checkPageIndex >= (Target.Pages.Count - 1)))
            {
                if (Target.Pages[checkPageIndex].Visible)
                {
                    foundVisiblePage = true;
                    break;
                }
                checkPageIndex++;
            }

            if (!foundVisiblePage)
            {
                return;
            }


            if (currentPage != null)
            {
                if (!currentPage.Save())
                {
                    return;
                }
                Controls.Remove(currentPage);
                currentPage.Dispose();
            }

            currentPageIndex = checkPageIndex;

            var page = (PropPage)Activator.CreateInstance(Target.Pages[currentPageIndex].Page);
            page.Top = Contents.Top;
            page.Left = Contents.Left;
            page.Binding = target;
            StepTitle.Text = Target.Pages[currentPageIndex].Title;
            page.SetData(Target.Data);
            Controls.Add(page);

            currentPage = page;

            UpdateButtonStates();
        }
        private void ShowPreviousPage()
        {
            target.SendUpdateTab(this);

            if (currentPageIndex < 1 )
            {
                return;
            }

            var checkPageIndex = currentPageIndex - 1;
            var foundVisiblePage = false;
            while (checkPageIndex >= 0)
            {
                if (Target.Pages[checkPageIndex].Visible)
                {
                    foundVisiblePage = true;
                    break;
                }
                checkPageIndex--;
            }

            if (!foundVisiblePage)
            {
                return;
            }

            if (currentPage != null)
            {
                if (!currentPage.Save())
                {
                    return;
                }
                Controls.Remove(currentPage);
                currentPage.Dispose();
            }
            
            currentPageIndex = checkPageIndex;

            var page = (PropPage)Activator.CreateInstance(Target.Pages[currentPageIndex].Page);
            page.Top = Contents.Top;
            page.Left = Contents.Left;
            page.Binding = target;
            StepTitle.Text = Target.Pages[currentPageIndex].Title;
            page.SetData(Target.Data);

            Controls.Add(page);

            currentPage = page;
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            if (currentPageIndex >= (Target.Pages.Count - 1))
            {
                Next.Enabled = false;
            }
            else
            {
                Next.Enabled = readyForNext && IsNextVisible();
            }
            if (currentPageIndex > 0)
            {
                Back.Enabled = true;
            }
            else
            {
                Back.Enabled = false;
            }
            Finish.Enabled = readyForNext;
        }

        private bool IsNextVisible()
        {
            var checkPageIndex = currentPageIndex + 1;
            var foundVisiblePage = false;
            while (!(checkPageIndex >= (Target.Pages.Count - 1)))
            {
                if (Target.Pages[checkPageIndex].Visible)
                {
                    foundVisiblePage = true;
                    break;
                }
                checkPageIndex++;
            }

            return foundVisiblePage;
        }

        private void Back_Click(object sender, EventArgs e)
        {
            ShowPreviousPage();
        }

        private void Next_Click(object sender, EventArgs e)
        {
            ShowNextPage();
        }

        private void Finish_Click(object sender, EventArgs e)
        {
            if (currentPage != null)
            {
                currentPage.Save();
            }
            DialogResult = DialogResult.OK;
            //Close();
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var g = e.Graphics;
            Brush b = new LinearGradientBrush(new Point(0, 0), new Point(0, Height), Color.FromArgb(20, 30, 39), Color.FromArgb(41, 49, 73));
            var p = new Pen(Color.FromArgb(71, 84, 108));
            g.FillRectangle(b, ClientRectangle);
            g.DrawRectangle(p, new Rectangle(0, ClientSize.Height - 1, ClientSize.Width - 1, ClientSize.Height - 1));
            p.Dispose();
            b.Dispose();
        }
    }
}
