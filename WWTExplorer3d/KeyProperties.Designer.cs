namespace TerraViewer
{
    partial class KeyProperties
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
            this.KeyTypeLabel = new System.Windows.Forms.Label();
            this.TimeLabel = new System.Windows.Forms.Label();
            this.ValueLabel = new System.Windows.Forms.Label();
            this.Time = new System.Windows.Forms.TextBox();
            this.CurrentValue = new System.Windows.Forms.TextBox();
            this.curveEditor1 = new TerraViewer.CurveEditor();
            this.keyType = new TerraViewer.WwtCombo();
            this.SuspendLayout();
            // 
            // KeyTypeLabel
            // 
            this.KeyTypeLabel.AutoSize = true;
            this.KeyTypeLabel.Location = new System.Drawing.Point(12, 53);
            this.KeyTypeLabel.Name = "KeyTypeLabel";
            this.KeyTypeLabel.Size = new System.Drawing.Size(97, 13);
            this.KeyTypeLabel.TabIndex = 0;
            this.KeyTypeLabel.Text = "Transition Function";
            // 
            // TimeLabel
            // 
            this.TimeLabel.AutoSize = true;
            this.TimeLabel.Location = new System.Drawing.Point(9, 5);
            this.TimeLabel.Name = "TimeLabel";
            this.TimeLabel.Size = new System.Drawing.Size(30, 13);
            this.TimeLabel.TabIndex = 2;
            this.TimeLabel.Text = "Time";
            // 
            // ValueLabel
            // 
            this.ValueLabel.AutoSize = true;
            this.ValueLabel.Location = new System.Drawing.Point(128, 5);
            this.ValueLabel.Name = "ValueLabel";
            this.ValueLabel.Size = new System.Drawing.Size(34, 13);
            this.ValueLabel.TabIndex = 4;
            this.ValueLabel.Text = "Value";
            // 
            // Time
            // 
            this.Time.AcceptsReturn = true;
            this.Time.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Time.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Time.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Time.ForeColor = System.Drawing.Color.White;
            this.Time.Location = new System.Drawing.Point(12, 21);
            this.Time.MaxLength = 64;
            this.Time.Name = "Time";
            this.Time.Size = new System.Drawing.Size(102, 20);
            this.Time.TabIndex = 3;
            this.Time.Text = "0";
            this.Time.TextChanged += new System.EventHandler(this.Time_TextChanged);
            this.Time.Validating += new System.ComponentModel.CancelEventHandler(this.Time_Validating);
            // 
            // CurrentValue
            // 
            this.CurrentValue.AcceptsReturn = true;
            this.CurrentValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.CurrentValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CurrentValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentValue.ForeColor = System.Drawing.Color.White;
            this.CurrentValue.Location = new System.Drawing.Point(131, 21);
            this.CurrentValue.MaxLength = 64;
            this.CurrentValue.Name = "CurrentValue";
            this.CurrentValue.Size = new System.Drawing.Size(102, 20);
            this.CurrentValue.TabIndex = 5;
            this.CurrentValue.Text = "0";
            this.CurrentValue.TextChanged += new System.EventHandler(this.CurrentValue_TextChanged);
            this.CurrentValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CurrentValue_KeyDown);
            this.CurrentValue.Validating += new System.ComponentModel.CancelEventHandler(this.CurrentValue_Validating);
            // 
            // curveEditor1
            // 
            this.curveEditor1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(60)))));
            this.curveEditor1.CurveType = TerraViewer.Key.KeyType.Custom;
            this.curveEditor1.Location = new System.Drawing.Point(12, 106);
            this.curveEditor1.Name = "curveEditor1";
            this.curveEditor1.P1 = 0D;
            this.curveEditor1.P2 = 1D;
            this.curveEditor1.P3 = 1D;
            this.curveEditor1.P4 = 0D;
            this.curveEditor1.Size = new System.Drawing.Size(221, 221);
            this.curveEditor1.TabIndex = 6;
            this.curveEditor1.ValueChanged += new System.EventHandler(this.curveEditor1_ValueChanged);
            // 
            // keyType
            // 
            this.keyType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.keyType.DateTimeValue = new System.DateTime(2013, 11, 14, 13, 53, 30, 651);
            this.keyType.Filter = TerraViewer.Classification.Unfiltered;
            this.keyType.FilterStyle = false;
            this.keyType.Location = new System.Drawing.Point(12, 69);
            this.keyType.Margin = new System.Windows.Forms.Padding(0);
            this.keyType.MasterTime = true;
            this.keyType.MaximumSize = new System.Drawing.Size(248, 33);
            this.keyType.MinimumSize = new System.Drawing.Size(35, 33);
            this.keyType.Name = "keyType";
            this.keyType.SelectedIndex = -1;
            this.keyType.SelectedItem = null;
            this.keyType.Size = new System.Drawing.Size(226, 33);
            this.keyType.State = TerraViewer.State.Rest;
            this.keyType.TabIndex = 1;
            this.keyType.Type = TerraViewer.WwtCombo.ComboType.List;
            this.keyType.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.keyType_SelectionChanged);
            // 
            // KeyProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(243, 340);
            this.Controls.Add(this.CurrentValue);
            this.Controls.Add(this.Time);
            this.Controls.Add(this.ValueLabel);
            this.Controls.Add(this.TimeLabel);
            this.Controls.Add(this.curveEditor1);
            this.Controls.Add(this.KeyTypeLabel);
            this.Controls.Add(this.keyType);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::TerraViewer.Properties.Settings.Default, "KeyPropsLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Location = global::TerraViewer.Properties.Settings.Default.KeyPropsLocation;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KeyProperties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Key Properties";
            this.Deactivate += new System.EventHandler(this.KeyProperties_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.KeyProperties_FormClosed);
            this.Load += new System.EventHandler(this.KeyProperties_Load);
            this.Leave += new System.EventHandler(this.KeyProperties_Leave);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtCombo keyType;
        private System.Windows.Forms.Label KeyTypeLabel;
        private CurveEditor curveEditor1;
        private System.Windows.Forms.Label TimeLabel;
        private System.Windows.Forms.Label ValueLabel;
        private System.Windows.Forms.TextBox Time;
        private System.Windows.Forms.TextBox CurrentValue;
    }
}