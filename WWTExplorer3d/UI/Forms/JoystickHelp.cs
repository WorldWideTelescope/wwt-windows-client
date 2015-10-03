using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class JoystickHelp : Form
    {
        public JoystickHelp()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            this.label1.Text = "Joystick or Game Contoller Found";
            this.richTextBox1.Text = Language.GetLocalizedText(250, "WorldWide Telescope can use a XBOX 360 Game controller or other compatible device to navigate the 3d viewport. Some controllers are not properly center calibrated and can cause the viewport to zoom or spin out of control. Having more than one type of controller can cause conflicts. If you have problems with the viewport, either disable the joystick control or unplug conflicting devices before launching WorldWide Telescope.");
            this.UserController.Text = Language.GetLocalizedText(156, "OK");
            this.close.Text = Language.GetLocalizedText(252, "Ignore the controller");
        }
        private void close_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowJoystickHelp = !dontShowAgain.Checked;
            DialogResult = DialogResult.OK;
            Properties.Settings.Default.UseJoystick = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // This causes the welcome screen to go away if anther instance sends us data
            if (Earth3d.MainWindow.CloseWelcome)
            {
                DialogResult = DialogResult.OK;
            }
        }

        private void c(object sender, EventArgs e)
        {

        }

        private void Welcome_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Enabled = false;
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {

        }

        private void UserController_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowJoystickHelp = !dontShowAgain.Checked;
            DialogResult = DialogResult.OK;
            Properties.Settings.Default.UseJoystick = true;

        }

        private void JoystickHelp_Load(object sender, EventArgs e)
        {

        }


    }
}