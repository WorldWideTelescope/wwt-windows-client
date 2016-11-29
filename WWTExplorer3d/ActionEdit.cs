using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class ActionEdit : Form
    {
        public ActionEdit()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.OK.Text = Language.GetLocalizedText(156, "OK");
            this.Cancel.Text = Language.GetLocalizedText(157, "Cancel");
            this.FilenameLabel.Text = Language.GetLocalizedText(1376, "Filename");
            this.IdLabel.Text = "Id";
            this.PointsLabel.Text = Language.GetLocalizedText(1377, "Points");
        }

        public Action Action = null;
        public Overlay Overlay = null;


        private void ActionEdit_Load(object sender, EventArgs e)
        {
            if (Overlay.Action != null)
            {
                Action = Overlay.Action;
                this.Filename.Text = Overlay.Action.Filename;
                this.Id.Text = Overlay.Action.Id;
                this.Points.Text = Overlay.Action.Points.ToString();
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Action = new Action(this.Id.Text, ActionType.Quiz, this.Filename.Text, Int32.Parse(this.Points.Text));
            Overlay.Action = Action;
            this.Close();
        }

        

        private void Filename_TextChanged(object sender, EventArgs e)
        {
            if (Action != null)
            {
                Overlay.Action.Filename = Filename.Text;
            }
        }

        private void Id_TextChanged(object sender, EventArgs e)
        {
            if (Action != null)
            {
                Action.Id = Id.Text;
            }
        }

        private void Points_TextChanged(object sender, EventArgs e)
        {
            if (Action != null)
            {
                Action.Points = Int32.Parse(Points.Text);
            }
        }

        private void ActionEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OK_Click(this, new EventArgs());
            }

            if (e.KeyCode == Keys.Escape)
            {
                Cancel_Click(this, new EventArgs());
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
