using System;
using System.Windows.Forms;
using TerraViewer.Properties;

namespace TerraViewer
{
    public partial class Tab : UserControl
    {
        public Tab()
        {
            InitializeComponent();
        }
        bool selected;

        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                SetSelected();
            }
        }


        private string title = "";

        public string Title
        {
            get { return title; }
            set 
            {
                title = value;
                TitleLabel.Text = value;
            }
        }

        private void SetSelected()
        {
            if (selected)
            {
                BackgroundImage = Resources.tabSelected;
            }
            else
            {
                //this.BackgroundImage = global::TerraViewer.Properties.Resources.tabHover;
                BackgroundImage = Resources.tabPlain;
            }
        }


        private void TitleLabel_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        private void Tab_MouseEnter(object sender, EventArgs e)
        {
            if (selected)
            {
                BackgroundImage = Resources.tabSelectedHover;
            }
            else
            {
                BackgroundImage = Resources.tabHover;
            }

        }

        private void Tab_MouseLeave(object sender, EventArgs e)
        {
            SetSelected();
        }

        private void Tab_Load(object sender, EventArgs e)
        {

        }



    }
}
