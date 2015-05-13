namespace TerraViewer
{
    partial class TimeLine
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.End = new TerraViewer.WwtButton();
            this.DelKey = new TerraViewer.WwtButton();
            this.AddKey = new TerraViewer.WwtButton();
            this.Start = new TerraViewer.WwtButton();
            this.Forward = new TerraViewer.WwtButton();
            this.Back = new TerraViewer.WwtButton();
            this.Pause = new TerraViewer.WwtButton();
            this.hoverTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.ShowAlways = true;
            // 
            // End
            // 
            this.End.BackColor = System.Drawing.Color.Transparent;
            this.End.DialogResult = System.Windows.Forms.DialogResult.None;
            this.End.ImageDisabled = null;
            this.End.ImageEnabled = global::TerraViewer.Properties.Resources.DataContainer_MoveLastHS;
            this.End.Location = new System.Drawing.Point(214, -2);
            this.End.MaximumSize = new System.Drawing.Size(140, 33);
            this.End.Name = "End";
            this.End.Selected = false;
            this.End.Size = new System.Drawing.Size(36, 33);
            this.End.TabIndex = 8;
            this.toolTip.SetToolTip(this.End, "Jump to End");
            this.End.Click += new System.EventHandler(this.End_Click);
            // 
            // DelKey
            // 
            this.DelKey.BackColor = System.Drawing.Color.Transparent;
            this.DelKey.DialogResult = System.Windows.Forms.DialogResult.None;
            this.DelKey.ImageDisabled = null;
            this.DelKey.ImageEnabled = global::TerraViewer.Properties.Resources.DelKey;
            this.DelKey.Location = new System.Drawing.Point(33, -2);
            this.DelKey.MaximumSize = new System.Drawing.Size(140, 33);
            this.DelKey.Name = "DelKey";
            this.DelKey.Selected = false;
            this.DelKey.Size = new System.Drawing.Size(36, 33);
            this.DelKey.TabIndex = 9;
            this.toolTip.SetToolTip(this.DelKey, "Delete Key");
            this.DelKey.Click += new System.EventHandler(this.DelKey_Click);
            // 
            // AddKey
            // 
            this.AddKey.BackColor = System.Drawing.Color.Transparent;
            this.AddKey.DialogResult = System.Windows.Forms.DialogResult.None;
            this.AddKey.ImageDisabled = null;
            this.AddKey.ImageEnabled = global::TerraViewer.Properties.Resources.Key;
            this.AddKey.Location = new System.Drawing.Point(1, -2);
            this.AddKey.MaximumSize = new System.Drawing.Size(140, 33);
            this.AddKey.Name = "AddKey";
            this.AddKey.Selected = false;
            this.AddKey.Size = new System.Drawing.Size(36, 33);
            this.AddKey.TabIndex = 9;
            this.toolTip.SetToolTip(this.AddKey, "Add Key");
            this.AddKey.Click += new System.EventHandler(this.AddKey_Click);
            // 
            // Start
            // 
            this.Start.BackColor = System.Drawing.Color.Transparent;
            this.Start.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Start.ImageDisabled = null;
            this.Start.ImageEnabled = global::TerraViewer.Properties.Resources.DataContainer_MoveFirstHS;
            this.Start.Location = new System.Drawing.Point(90, -2);
            this.Start.MaximumSize = new System.Drawing.Size(140, 33);
            this.Start.Name = "Start";
            this.Start.Selected = false;
            this.Start.Size = new System.Drawing.Size(36, 33);
            this.Start.TabIndex = 9;
            this.toolTip.SetToolTip(this.Start, "Jump to Begin");
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // Forward
            // 
            this.Forward.BackColor = System.Drawing.Color.Transparent;
            this.Forward.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Forward.ImageDisabled = null;
            this.Forward.ImageEnabled = global::TerraViewer.Properties.Resources.DataContainer_MoveNextHS;
            this.Forward.Location = new System.Drawing.Point(183, -2);
            this.Forward.MaximumSize = new System.Drawing.Size(140, 33);
            this.Forward.Name = "Forward";
            this.Forward.Selected = false;
            this.Forward.Size = new System.Drawing.Size(36, 33);
            this.Forward.TabIndex = 7;
            this.toolTip.SetToolTip(this.Forward, "Play");
            this.Forward.Click += new System.EventHandler(this.Forward_Click);
            // 
            // Back
            // 
            this.Back.BackColor = System.Drawing.Color.Transparent;
            this.Back.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Back.ImageDisabled = null;
            this.Back.ImageEnabled = global::TerraViewer.Properties.Resources.DataContainer_MovePreviousHS;
            this.Back.Location = new System.Drawing.Point(120, -2);
            this.Back.MaximumSize = new System.Drawing.Size(140, 33);
            this.Back.Name = "Back";
            this.Back.Selected = false;
            this.Back.Size = new System.Drawing.Size(36, 33);
            this.Back.TabIndex = 5;
            this.toolTip.SetToolTip(this.Back, "Play Backward");
            this.Back.Click += new System.EventHandler(this.Back_Click);
            // 
            // Pause
            // 
            this.Pause.BackColor = System.Drawing.Color.Transparent;
            this.Pause.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Pause.ImageDisabled = null;
            this.Pause.ImageEnabled = global::TerraViewer.Properties.Resources.PauseHS;
            this.Pause.Location = new System.Drawing.Point(152, -2);
            this.Pause.MaximumSize = new System.Drawing.Size(140, 33);
            this.Pause.Name = "Pause";
            this.Pause.Selected = false;
            this.Pause.Size = new System.Drawing.Size(36, 33);
            this.Pause.TabIndex = 6;
            this.toolTip.SetToolTip(this.Pause, "Pause");
            this.Pause.Click += new System.EventHandler(this.Pause_Click);
            // 
            // hoverTimer
            // 
            this.hoverTimer.Enabled = true;
            this.hoverTimer.Tick += new System.EventHandler(this.hoverTimer_Tick);
            // 
            // TimeLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.Controls.Add(this.End);
            this.Controls.Add(this.DelKey);
            this.Controls.Add(this.AddKey);
            this.Controls.Add(this.Start);
            this.Controls.Add(this.Forward);
            this.Controls.Add(this.Back);
            this.Controls.Add(this.Pause);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(300, 90);
            this.Name = "TimeLine";
            this.Size = new System.Drawing.Size(786, 90);
            this.Load += new System.EventHandler(this.TimeLine_Load);
            this.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.TimeLine_ControlAdded);
            this.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.TimeLine_ControlRemoved);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TimeLine_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TimeLine_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TimeLine_MouseDown);
            this.MouseHover += new System.EventHandler(this.TimeLine_MouseHover);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TimeLine_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TimeLine_MouseUp);
            this.Resize += new System.EventHandler(this.TimeLine_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private WwtButton End;
        private WwtButton Start;
        private WwtButton Forward;
        private WwtButton Back;
        private WwtButton Pause;
        private WwtButton AddKey;
        private WwtButton DelKey;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Timer hoverTimer;

    }
}
