namespace TerraViewer
{
    partial class DataWizardCoordinates
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataWizardCoordinates));
            this.longRALable = new System.Windows.Forms.Label();
            this.latDecLabel = new System.Windows.Forms.Label();
            this.lngColumnCombo = new TerraViewer.WwtCombo();
            this.latColumnCombo = new TerraViewer.WwtCombo();
            this.AltitudeDepthLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.AltitudeDepthTypeCombo = new TerraViewer.WwtCombo();
            this.altColumnCombo = new TerraViewer.WwtCombo();
            this.altDepthLabel = new System.Windows.Forms.Label();
            this.altDepthTypeLabel = new System.Windows.Forms.Label();
            this.AltDepthUnitsCombo = new TerraViewer.WwtCombo();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.raUnits = new TerraViewer.WwtCombo();
            this.raUnitsLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // longRALable
            // 
            this.longRALable.AutoSize = true;
            this.longRALable.Location = new System.Drawing.Point(21, 149);
            this.longRALable.Name = "longRALable";
            this.longRALable.Size = new System.Drawing.Size(54, 13);
            this.longRALable.TabIndex = 4;
            this.longRALable.Text = "Longitude";
            // 
            // latDecLabel
            // 
            this.latDecLabel.AutoSize = true;
            this.latDecLabel.Location = new System.Drawing.Point(21, 93);
            this.latDecLabel.Name = "latDecLabel";
            this.latDecLabel.Size = new System.Drawing.Size(45, 13);
            this.latDecLabel.TabIndex = 2;
            this.latDecLabel.Text = "Latitude";
            // 
            // lngColumnCombo
            // 
            this.lngColumnCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.lngColumnCombo.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.lngColumnCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.lngColumnCombo.FilterStyle = false;
            this.lngColumnCombo.Location = new System.Drawing.Point(24, 165);
            this.lngColumnCombo.Margin = new System.Windows.Forms.Padding(0);
            this.lngColumnCombo.MasterTime = true;
            this.lngColumnCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.lngColumnCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.lngColumnCombo.Name = "lngColumnCombo";
            this.lngColumnCombo.SelectedIndex = -1;
            this.lngColumnCombo.SelectedItem = null;
            this.lngColumnCombo.Size = new System.Drawing.Size(126, 33);
            this.lngColumnCombo.State = TerraViewer.State.Rest;
            this.lngColumnCombo.TabIndex = 5;
            this.lngColumnCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            // 
            // latColumnCombo
            // 
            this.latColumnCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.latColumnCombo.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.latColumnCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.latColumnCombo.FilterStyle = false;
            this.latColumnCombo.Location = new System.Drawing.Point(24, 109);
            this.latColumnCombo.Margin = new System.Windows.Forms.Padding(0);
            this.latColumnCombo.MasterTime = true;
            this.latColumnCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.latColumnCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.latColumnCombo.Name = "latColumnCombo";
            this.latColumnCombo.SelectedIndex = -1;
            this.latColumnCombo.SelectedItem = null;
            this.latColumnCombo.Size = new System.Drawing.Size(126, 33);
            this.latColumnCombo.State = TerraViewer.State.Rest;
            this.latColumnCombo.TabIndex = 3;
            this.latColumnCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.latColumnCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.latColumnCombo_SelectionChanged);
            // 
            // AltitudeDepthLabel
            // 
            this.AltitudeDepthLabel.AutoSize = true;
            this.AltitudeDepthLabel.Location = new System.Drawing.Point(302, 70);
            this.AltitudeDepthLabel.Name = "AltitudeDepthLabel";
            this.AltitudeDepthLabel.Size = new System.Drawing.Size(135, 13);
            this.AltitudeDepthLabel.TabIndex = 6;
            this.AltitudeDepthLabel.Text = "Altitude / Depth / Distance";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Spatial Coordinates";
            // 
            // AltitudeDepthTypeCombo
            // 
            this.AltitudeDepthTypeCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.AltitudeDepthTypeCombo.DateTimeValue = new System.DateTime(2010, 5, 7, 15, 2, 27, 816);
            this.AltitudeDepthTypeCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.AltitudeDepthTypeCombo.FilterStyle = false;
            this.AltitudeDepthTypeCombo.Location = new System.Drawing.Point(305, 109);
            this.AltitudeDepthTypeCombo.Margin = new System.Windows.Forms.Padding(0);
            this.AltitudeDepthTypeCombo.MasterTime = true;
            this.AltitudeDepthTypeCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.AltitudeDepthTypeCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.AltitudeDepthTypeCombo.Name = "AltitudeDepthTypeCombo";
            this.AltitudeDepthTypeCombo.SelectedIndex = -1;
            this.AltitudeDepthTypeCombo.SelectedItem = null;
            this.AltitudeDepthTypeCombo.Size = new System.Drawing.Size(126, 33);
            this.AltitudeDepthTypeCombo.State = TerraViewer.State.Rest;
            this.AltitudeDepthTypeCombo.TabIndex = 8;
            this.AltitudeDepthTypeCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.AltitudeDepthTypeCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.AltitudeDepthTypeCombo_SelectionChanged);
            // 
            // altColumnCombo
            // 
            this.altColumnCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.altColumnCombo.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.altColumnCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.altColumnCombo.FilterStyle = false;
            this.altColumnCombo.Location = new System.Drawing.Point(305, 165);
            this.altColumnCombo.Margin = new System.Windows.Forms.Padding(0);
            this.altColumnCombo.MasterTime = true;
            this.altColumnCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.altColumnCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.altColumnCombo.Name = "altColumnCombo";
            this.altColumnCombo.SelectedIndex = -1;
            this.altColumnCombo.SelectedItem = null;
            this.altColumnCombo.Size = new System.Drawing.Size(126, 33);
            this.altColumnCombo.State = TerraViewer.State.Rest;
            this.altColumnCombo.TabIndex = 10;
            this.altColumnCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            // 
            // altDepthLabel
            // 
            this.altDepthLabel.AutoSize = true;
            this.altDepthLabel.Location = new System.Drawing.Point(302, 149);
            this.altDepthLabel.Name = "altDepthLabel";
            this.altDepthLabel.Size = new System.Drawing.Size(120, 13);
            this.altDepthLabel.TabIndex = 9;
            this.altDepthLabel.Text = "Altitude / Depth Column";
            // 
            // altDepthTypeLabel
            // 
            this.altDepthTypeLabel.AutoSize = true;
            this.altDepthTypeLabel.Location = new System.Drawing.Point(302, 93);
            this.altDepthTypeLabel.Name = "altDepthTypeLabel";
            this.altDepthTypeLabel.Size = new System.Drawing.Size(31, 13);
            this.altDepthTypeLabel.TabIndex = 7;
            this.altDepthTypeLabel.Text = "Type";
            // 
            // AltDepthUnitsCombo
            // 
            this.AltDepthUnitsCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.AltDepthUnitsCombo.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.AltDepthUnitsCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.AltDepthUnitsCombo.FilterStyle = false;
            this.AltDepthUnitsCombo.Location = new System.Drawing.Point(442, 109);
            this.AltDepthUnitsCombo.Margin = new System.Windows.Forms.Padding(0);
            this.AltDepthUnitsCombo.MasterTime = true;
            this.AltDepthUnitsCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.AltDepthUnitsCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.AltDepthUnitsCombo.Name = "AltDepthUnitsCombo";
            this.AltDepthUnitsCombo.SelectedIndex = -1;
            this.AltDepthUnitsCombo.SelectedItem = null;
            this.AltDepthUnitsCombo.Size = new System.Drawing.Size(126, 33);
            this.AltDepthUnitsCombo.State = TerraViewer.State.Rest;
            this.AltDepthUnitsCombo.TabIndex = 12;
            this.AltDepthUnitsCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(439, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Units";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(21, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(604, 47);
            this.label3.TabIndex = 0;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // raUnits
            // 
            this.raUnits.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.raUnits.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.raUnits.Filter = TerraViewer.Classification.Unfiltered;
            this.raUnits.FilterStyle = false;
            this.raUnits.Location = new System.Drawing.Point(163, 109);
            this.raUnits.Margin = new System.Windows.Forms.Padding(0);
            this.raUnits.MasterTime = true;
            this.raUnits.MaximumSize = new System.Drawing.Size(248, 33);
            this.raUnits.MinimumSize = new System.Drawing.Size(35, 33);
            this.raUnits.Name = "raUnits";
            this.raUnits.SelectedIndex = -1;
            this.raUnits.SelectedItem = null;
            this.raUnits.Size = new System.Drawing.Size(126, 33);
            this.raUnits.State = TerraViewer.State.Rest;
            this.raUnits.TabIndex = 12;
            this.raUnits.Type = TerraViewer.WwtCombo.ComboType.List;
            this.raUnits.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.raUnits_SelectionChanged);
            // 
            // raUnitsLabel
            // 
            this.raUnitsLabel.AutoSize = true;
            this.raUnitsLabel.Location = new System.Drawing.Point(160, 93);
            this.raUnitsLabel.Name = "raUnitsLabel";
            this.raUnitsLabel.Size = new System.Drawing.Size(31, 13);
            this.raUnitsLabel.TabIndex = 11;
            this.raUnitsLabel.Text = "Units";
            // 
            // DataWizardCoordinates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.AltitudeDepthTypeCombo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AltitudeDepthLabel);
            this.Controls.Add(this.raUnitsLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.altDepthLabel);
            this.Controls.Add(this.longRALable);
            this.Controls.Add(this.altDepthTypeLabel);
            this.Controls.Add(this.latDecLabel);
            this.Controls.Add(this.raUnits);
            this.Controls.Add(this.AltDepthUnitsCombo);
            this.Controls.Add(this.altColumnCombo);
            this.Controls.Add(this.lngColumnCombo);
            this.Controls.Add(this.latColumnCombo);
            this.Name = "DataWizardCoordinates";
            this.Load += new System.EventHandler(this.DataWizardCoordinates_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label longRALable;
        private System.Windows.Forms.Label latDecLabel;
        private WwtCombo lngColumnCombo;
        private WwtCombo latColumnCombo;
        private System.Windows.Forms.Label AltitudeDepthLabel;
        private System.Windows.Forms.Label label1;
        private WwtCombo AltitudeDepthTypeCombo;
        private WwtCombo altColumnCombo;
        private System.Windows.Forms.Label altDepthLabel;
        private System.Windows.Forms.Label altDepthTypeLabel;
        private WwtCombo AltDepthUnitsCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private WwtCombo raUnits;
        private System.Windows.Forms.Label raUnitsLabel;
    }
}
