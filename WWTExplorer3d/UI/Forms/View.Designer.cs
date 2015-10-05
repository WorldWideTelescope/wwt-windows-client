namespace TerraViewer
{
    partial class View
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.localHorizon = new TerraViewer.WWTCheckbox();
            this.ChooseLocation = new TerraViewer.WwtButton();
            this.lngText = new System.Windows.Forms.Label();
            this.Altitude = new System.Windows.Forms.Label();
            this.latText = new System.Windows.Forms.Label();
            this.locationName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.layerToggle = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.showUtcTime = new TerraViewer.WWTCheckbox();
            this.TimeNow = new TerraViewer.WwtButton();
            this.fastForward = new TerraViewer.WwtButton();
            this.fastBack = new TerraViewer.WwtButton();
            this.play = new TerraViewer.WwtButton();
            this.TimeMode = new System.Windows.Forms.Label();
            this.back = new TerraViewer.WwtButton();
            this.pause = new TerraViewer.WwtButton();
            this.timeDateControl = new TerraViewer.WwtCombo();
            this.timeDateTimer = new System.Windows.Forms.Timer(this.components);
            this.layerMgrMessage = new System.Windows.Forms.Label();
            this.showMinorPlanets = new TerraViewer.WWTCheckbox();
            this.CustomButtons = new TerraViewer.ButtonGroupControl();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layerToggle)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // pinUp
            // 
            this.pinUp.Enabled = false;
            this.pinUp.Location = new System.Drawing.Point(516, 94);
            this.pinUp.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.localHorizon);
            this.groupBox2.Controls.Add(this.ChooseLocation);
            this.groupBox2.Controls.Add(this.lngText);
            this.groupBox2.Controls.Add(this.Altitude);
            this.groupBox2.Controls.Add(this.latText);
            this.groupBox2.Controls.Add(this.locationName);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(138, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(219, 100);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Observing Location";
            // 
            // localHorizon
            // 
            this.localHorizon.BackColor = System.Drawing.Color.Transparent;
            this.localHorizon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.localHorizon.Checked = false;
            this.localHorizon.Location = new System.Drawing.Point(6, 70);
            this.localHorizon.Name = "localHorizon";
            this.localHorizon.Size = new System.Drawing.Size(146, 25);
            this.localHorizon.TabIndex = 6;
            this.localHorizon.Text = "View from this location";
            this.localHorizon.CheckedChanged += new System.EventHandler(this.localHorizon_CheckedChanged);
            // 
            // ChooseLocation
            // 
            this.ChooseLocation.BackColor = System.Drawing.Color.Transparent;
            this.ChooseLocation.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ChooseLocation.ImageDisabled = null;
            this.ChooseLocation.ImageEnabled = null;
            this.ChooseLocation.Location = new System.Drawing.Point(158, 61);
            this.ChooseLocation.MaximumSize = new System.Drawing.Size(140, 33);
            this.ChooseLocation.Name = "ChooseLocation";
            this.ChooseLocation.Selected = false;
            this.ChooseLocation.Size = new System.Drawing.Size(56, 33);
            this.ChooseLocation.TabIndex = 4;
            this.ChooseLocation.Text = "Setup";
            this.ChooseLocation.Click += new System.EventHandler(this.ChooseLocation_Click);
            // 
            // lngText
            // 
            this.lngText.BackColor = System.Drawing.Color.Transparent;
            this.lngText.ForeColor = System.Drawing.Color.White;
            this.lngText.Location = new System.Drawing.Point(35, 56);
            this.lngText.Name = "lngText";
            this.lngText.Size = new System.Drawing.Size(79, 13);
            this.lngText.TabIndex = 5;
            this.lngText.Text = "00 : 00 : 00";
            this.lngText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Altitude
            // 
            this.Altitude.BackColor = System.Drawing.Color.Transparent;
            this.Altitude.ForeColor = System.Drawing.Color.White;
            this.Altitude.Location = new System.Drawing.Point(171, 38);
            this.Altitude.Name = "Altitude";
            this.Altitude.Size = new System.Drawing.Size(42, 13);
            this.Altitude.TabIndex = 4;
            this.Altitude.Text = "500\'";
            this.Altitude.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Altitude.Visible = false;
            // 
            // latText
            // 
            this.latText.BackColor = System.Drawing.Color.Transparent;
            this.latText.ForeColor = System.Drawing.Color.White;
            this.latText.Location = new System.Drawing.Point(35, 38);
            this.latText.Name = "latText";
            this.latText.Size = new System.Drawing.Size(79, 13);
            this.latText.TabIndex = 4;
            this.latText.Text = "00 : 00 : 00";
            this.latText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // locationName
            // 
            this.locationName.AutoSize = true;
            this.locationName.Location = new System.Drawing.Point(50, 20);
            this.locationName.Name = "locationName";
            this.locationName.Size = new System.Drawing.Size(126, 13);
            this.locationName.TabIndex = 1;
            this.locationName.Text = "Please choose a location";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Long:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(120, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Altitude:";
            this.label5.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Lat:";
            // 
            // toolTips
            // 
            this.toolTips.ShowAlways = true;
            // 
            // layerToggle
            // 
            this.layerToggle.BackColor = System.Drawing.Color.Transparent;
            this.layerToggle.Image = global::TerraViewer.Properties.Resources.layersButton;
            this.layerToggle.InitialImage = null;
            this.layerToggle.Location = new System.Drawing.Point(26, 51);
            this.layerToggle.Name = "layerToggle";
            this.layerToggle.Size = new System.Drawing.Size(75, 39);
            this.layerToggle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.layerToggle.TabIndex = 55;
            this.layerToggle.TabStop = false;
            this.toolTips.SetToolTip(this.layerToggle, "Show/Hide Layer Manager");
            this.layerToggle.Click += new System.EventHandler(this.layerToggle_Click);
            this.layerToggle.MouseEnter += new System.EventHandler(this.layerToggle_MouseEnter);
            this.layerToggle.MouseLeave += new System.EventHandler(this.layerToggle_MouseLeave);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.showUtcTime);
            this.groupBox3.Controls.Add(this.TimeNow);
            this.groupBox3.Controls.Add(this.fastForward);
            this.groupBox3.Controls.Add(this.fastBack);
            this.groupBox3.Controls.Add(this.play);
            this.groupBox3.Controls.Add(this.TimeMode);
            this.groupBox3.Controls.Add(this.back);
            this.groupBox3.Controls.Add(this.pause);
            this.groupBox3.Controls.Add(this.timeDateControl);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(368, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(214, 100);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Observing Time";
            // 
            // showUtcTime
            // 
            this.showUtcTime.BackColor = System.Drawing.Color.Transparent;
            this.showUtcTime.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.showUtcTime.Checked = false;
            this.showUtcTime.Location = new System.Drawing.Point(154, 43);
            this.showUtcTime.Name = "showUtcTime";
            this.showUtcTime.Size = new System.Drawing.Size(54, 25);
            this.showUtcTime.TabIndex = 5;
            this.showUtcTime.Text = "UTC";
            this.showUtcTime.CheckedChanged += new System.EventHandler(this.showUtcTime_CheckedChanged);
            // 
            // TimeNow
            // 
            this.TimeNow.BackColor = System.Drawing.Color.Transparent;
            this.TimeNow.DialogResult = System.Windows.Forms.DialogResult.None;
            this.TimeNow.ImageDisabled = null;
            this.TimeNow.ImageEnabled = null;
            this.TimeNow.Location = new System.Drawing.Point(164, 65);
            this.TimeNow.MaximumSize = new System.Drawing.Size(140, 33);
            this.TimeNow.Name = "TimeNow";
            this.TimeNow.Selected = false;
            this.TimeNow.Size = new System.Drawing.Size(48, 33);
            this.TimeNow.TabIndex = 4;
            this.TimeNow.Text = "Now";
            this.TimeNow.Click += new System.EventHandler(this.TimeNow_Click);
            // 
            // fastForward
            // 
            this.fastForward.BackColor = System.Drawing.Color.Transparent;
            this.fastForward.DialogResult = System.Windows.Forms.DialogResult.None;
            this.fastForward.ImageDisabled = null;
            this.fastForward.ImageEnabled = global::TerraViewer.Properties.Resources.DoubleRightArrowHS;
            this.fastForward.Location = new System.Drawing.Point(126, 65);
            this.fastForward.MaximumSize = new System.Drawing.Size(140, 33);
            this.fastForward.Name = "fastForward";
            this.fastForward.Selected = false;
            this.fastForward.Size = new System.Drawing.Size(36, 33);
            this.fastForward.TabIndex = 4;
            this.fastForward.Click += new System.EventHandler(this.fastForward_Click);
            // 
            // fastBack
            // 
            this.fastBack.BackColor = System.Drawing.Color.Transparent;
            this.fastBack.DialogResult = System.Windows.Forms.DialogResult.None;
            this.fastBack.ImageDisabled = null;
            this.fastBack.ImageEnabled = global::TerraViewer.Properties.Resources.DoubleLeftArrowHS;
            this.fastBack.Location = new System.Drawing.Point(2, 65);
            this.fastBack.MaximumSize = new System.Drawing.Size(140, 33);
            this.fastBack.Name = "fastBack";
            this.fastBack.Selected = false;
            this.fastBack.Size = new System.Drawing.Size(36, 33);
            this.fastBack.TabIndex = 4;
            this.fastBack.Click += new System.EventHandler(this.fastBack_Click);
            // 
            // play
            // 
            this.play.BackColor = System.Drawing.Color.Transparent;
            this.play.DialogResult = System.Windows.Forms.DialogResult.None;
            this.play.ImageDisabled = null;
            this.play.ImageEnabled = global::TerraViewer.Properties.Resources.PlayHS;
            this.play.Location = new System.Drawing.Point(95, 65);
            this.play.MaximumSize = new System.Drawing.Size(140, 33);
            this.play.Name = "play";
            this.play.Selected = false;
            this.play.Size = new System.Drawing.Size(36, 33);
            this.play.TabIndex = 4;
            this.play.Click += new System.EventHandler(this.play_Click);
            // 
            // TimeMode
            // 
            this.TimeMode.AutoSize = true;
            this.TimeMode.Location = new System.Drawing.Point(6, 49);
            this.TimeMode.Name = "TimeMode";
            this.TimeMode.Size = new System.Drawing.Size(55, 13);
            this.TimeMode.TabIndex = 1;
            this.TimeMode.Text = "Real Time";
            // 
            // back
            // 
            this.back.BackColor = System.Drawing.Color.Transparent;
            this.back.DialogResult = System.Windows.Forms.DialogResult.None;
            this.back.ImageDisabled = null;
            this.back.ImageEnabled = global::TerraViewer.Properties.Resources.LeftArrowHS;
            this.back.Location = new System.Drawing.Point(32, 65);
            this.back.MaximumSize = new System.Drawing.Size(140, 33);
            this.back.Name = "back";
            this.back.Selected = false;
            this.back.Size = new System.Drawing.Size(36, 33);
            this.back.TabIndex = 4;
            this.back.Click += new System.EventHandler(this.back_Click);
            // 
            // pause
            // 
            this.pause.BackColor = System.Drawing.Color.Transparent;
            this.pause.DialogResult = System.Windows.Forms.DialogResult.None;
            this.pause.ImageDisabled = null;
            this.pause.ImageEnabled = global::TerraViewer.Properties.Resources.PauseHS;
            this.pause.Location = new System.Drawing.Point(64, 65);
            this.pause.MaximumSize = new System.Drawing.Size(140, 33);
            this.pause.Name = "pause";
            this.pause.Selected = false;
            this.pause.Size = new System.Drawing.Size(36, 33);
            this.pause.TabIndex = 4;
            this.pause.Click += new System.EventHandler(this.pause_Click);
            // 
            // timeDateControl
            // 
            this.timeDateControl.BackColor = System.Drawing.Color.Transparent;
            this.timeDateControl.DateTimeValue = new System.DateTime(2008, 1, 17, 18, 43, 57, 538);
            this.timeDateControl.Filter = TerraViewer.Classification.Unfiltered;
            this.timeDateControl.FilterStyle = false;
            this.timeDateControl.Location = new System.Drawing.Point(6, 11);
            this.timeDateControl.Margin = new System.Windows.Forms.Padding(0);
            this.timeDateControl.MasterTime = true;
            this.timeDateControl.MaximumSize = new System.Drawing.Size(248, 33);
            this.timeDateControl.MinimumSize = new System.Drawing.Size(35, 33);
            this.timeDateControl.Name = "timeDateControl";
            this.timeDateControl.SelectedIndex = -1;
            this.timeDateControl.SelectedItem = null;
            this.timeDateControl.Size = new System.Drawing.Size(205, 33);
            this.timeDateControl.State = TerraViewer.State.Rest;
            this.timeDateControl.TabIndex = 3;
            this.timeDateControl.Type = TerraViewer.WwtCombo.ComboType.DateTime;
            // 
            // timeDateTimer
            // 
            this.timeDateTimer.Enabled = true;
            this.timeDateTimer.Interval = 250;
            this.timeDateTimer.Tick += new System.EventHandler(this.timeDateTimer_Tick);
            // 
            // layerMgrMessage
            // 
            this.layerMgrMessage.BackColor = System.Drawing.Color.Transparent;
            this.layerMgrMessage.ForeColor = System.Drawing.Color.White;
            this.layerMgrMessage.Location = new System.Drawing.Point(12, 9);
            this.layerMgrMessage.MaximumSize = new System.Drawing.Size(500, 500);
            this.layerMgrMessage.Name = "layerMgrMessage";
            this.layerMgrMessage.Size = new System.Drawing.Size(120, 35);
            this.layerMgrMessage.TabIndex = 56;
            this.layerMgrMessage.Text = "Use Layer Manager to Control User Settings";
            // 
            // showMinorPlanets
            // 
            this.showMinorPlanets.BackColor = System.Drawing.Color.Transparent;
            this.showMinorPlanets.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.showMinorPlanets.Checked = false;
            this.showMinorPlanets.Location = new System.Drawing.Point(843, 29);
            this.showMinorPlanets.Name = "showMinorPlanets";
            this.showMinorPlanets.Size = new System.Drawing.Size(190, 25);
            this.showMinorPlanets.TabIndex = 57;
            this.showMinorPlanets.Text = "wwtCheckbox1";
            this.showMinorPlanets.Visible = false;
            this.showMinorPlanets.CheckedChanged += new System.EventHandler(this.showMinorPlanets_CheckedChanged);
            // 
            // CustomButtons
            // 
            this.CustomButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CustomButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(39)))));
            this.CustomButtons.ForeColor = System.Drawing.Color.White;
            this.CustomButtons.Location = new System.Drawing.Point(586, 2);
            this.CustomButtons.Name = "CustomButtons";
            this.CustomButtons.Size = new System.Drawing.Size(437, 107);
            this.CustomButtons.TabIndex = 58;
            // 
            // View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 110);
            this.Controls.Add(this.CustomButtons);
            this.Controls.Add(this.showMinorPlanets);
            this.Controls.Add(this.layerMgrMessage);
            this.Controls.Add(this.layerToggle);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "View";
            this.Opacity = 0D;
            this.Text = "View";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.View_FormClosing);
            this.Load += new System.EventHandler(this.View_Load);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.pinUp, 0);
            this.Controls.SetChildIndex(this.layerToggle, 0);
            this.Controls.SetChildIndex(this.layerMgrMessage, 0);
            this.Controls.SetChildIndex(this.showMinorPlanets, 0);
            this.Controls.SetChildIndex(this.CustomButtons, 0);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layerToggle)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lngText;
        private System.Windows.Forms.Label latText;
        private System.Windows.Forms.Label locationName;
        private System.Windows.Forms.Label Altitude;
        private System.Windows.Forms.Label label5;
        private WwtButton ChooseLocation;
        private System.Windows.Forms.ToolTip toolTips;
        private System.Windows.Forms.GroupBox groupBox3;
        private WwtCombo timeDateControl;
        private System.Windows.Forms.Timer timeDateTimer;
        private WwtButton play;
        private WwtButton pause;
        private WwtButton fastForward;
        private WwtButton fastBack;
        private WwtButton back;
        private WwtButton TimeNow;
        private System.Windows.Forms.Label TimeMode;
        private WWTCheckbox localHorizon;
        private WWTCheckbox showUtcTime;
        private System.Windows.Forms.PictureBox layerToggle;
        private System.Windows.Forms.Label layerMgrMessage;
        private WWTCheckbox showMinorPlanets;
        private ButtonGroupControl CustomButtons;
    }
}