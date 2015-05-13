namespace TerraViewer
{
    partial class DatePopup
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
            this.hours = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.min = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.sec = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.year = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.month = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.day = new System.Windows.Forms.TextBox();
            this.PushPin = new System.Windows.Forms.PictureBox();
            this.showUtcTime = new TerraViewer.WWTCheckbox();
            this.apply = new TerraViewer.WwtButton();
            this.OK = new TerraViewer.WwtButton();
            this.secUpDown = new TerraViewer.WwtUpDown();
            this.dayUpDown = new TerraViewer.WwtUpDown();
            this.minUpDown = new TerraViewer.WwtUpDown();
            this.hourUpDown = new TerraViewer.WwtUpDown();
            this.monthUpDown = new TerraViewer.WwtUpDown();
            this.yearUpDown = new TerraViewer.WwtUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.PushPin)).BeginInit();
            this.SuspendLayout();
            // 
            // hours
            // 
            this.hours.AcceptsReturn = true;
            this.hours.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.hours.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hours.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hours.ForeColor = System.Drawing.Color.White;
            this.hours.Location = new System.Drawing.Point(14, 74);
            this.hours.Name = "hours";
            this.hours.Size = new System.Drawing.Size(25, 22);
            this.hours.TabIndex = 7;
            this.hours.TextChanged += new System.EventHandler(this.Date_TextChanged);
            this.hours.Validating += new System.ComponentModel.CancelEventHandler(this.hours_Validating);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(11, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "Hrs";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(76, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "Min";
            // 
            // min
            // 
            this.min.AcceptsReturn = true;
            this.min.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.min.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.min.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.min.ForeColor = System.Drawing.Color.White;
            this.min.Location = new System.Drawing.Point(78, 74);
            this.min.Name = "min";
            this.min.Size = new System.Drawing.Size(25, 22);
            this.min.TabIndex = 9;
            this.min.TextChanged += new System.EventHandler(this.Date_TextChanged);
            this.min.Validating += new System.ComponentModel.CancelEventHandler(this.min_Validating);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(144, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 16);
            this.label4.TabIndex = 10;
            this.label4.Text = "Sec";
            // 
            // sec
            // 
            this.sec.AcceptsReturn = true;
            this.sec.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.sec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sec.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sec.ForeColor = System.Drawing.Color.White;
            this.sec.Location = new System.Drawing.Point(147, 74);
            this.sec.Name = "sec";
            this.sec.Size = new System.Drawing.Size(25, 22);
            this.sec.TabIndex = 11;
            this.sec.TextChanged += new System.EventHandler(this.Date_TextChanged);
            this.sec.Validating += new System.ComponentModel.CancelEventHandler(this.sec_Validating);
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(4, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 16);
            this.label8.TabIndex = 0;
            this.label8.Text = "Year";
            // 
            // year
            // 
            this.year.AcceptsReturn = true;
            this.year.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.year.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.year.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.year.ForeColor = System.Drawing.Color.White;
            this.year.Location = new System.Drawing.Point(7, 25);
            this.year.Name = "year";
            this.year.Size = new System.Drawing.Size(32, 22);
            this.year.TabIndex = 1;
            this.year.TextChanged += new System.EventHandler(this.Date_TextChanged);
            this.year.Validating += new System.ComponentModel.CancelEventHandler(this.year_Validating);
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(71, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 16);
            this.label9.TabIndex = 2;
            this.label9.Text = "Month";
            // 
            // month
            // 
            this.month.AcceptsReturn = true;
            this.month.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.month.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.month.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.month.ForeColor = System.Drawing.Color.White;
            this.month.Location = new System.Drawing.Point(78, 25);
            this.month.Name = "month";
            this.month.Size = new System.Drawing.Size(25, 22);
            this.month.TabIndex = 3;
            this.month.TextChanged += new System.EventHandler(this.Date_TextChanged);
            this.month.Validating += new System.ComponentModel.CancelEventHandler(this.month_Validating);
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(144, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(38, 16);
            this.label10.TabIndex = 4;
            this.label10.Text = "Day";
            // 
            // day
            // 
            this.day.AcceptsReturn = true;
            this.day.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.day.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.day.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.day.ForeColor = System.Drawing.Color.White;
            this.day.Location = new System.Drawing.Point(147, 25);
            this.day.Name = "day";
            this.day.Size = new System.Drawing.Size(25, 22);
            this.day.TabIndex = 5;
            this.day.TextChanged += new System.EventHandler(this.Date_TextChanged);
            this.day.Validating += new System.ComponentModel.CancelEventHandler(this.day_Validating);
            // 
            // PushPin
            // 
            this.PushPin.Image = global::TerraViewer.Properties.Resources.pushpin;
            this.PushPin.Location = new System.Drawing.Point(198, 2);
            this.PushPin.Name = "PushPin";
            this.PushPin.Size = new System.Drawing.Size(11, 14);
            this.PushPin.TabIndex = 14;
            this.PushPin.TabStop = false;
            this.PushPin.Click += new System.EventHandler(this.PushPin_Click);
            // 
            // showUtcTime
            // 
            this.showUtcTime.Checked = false;
            this.showUtcTime.Location = new System.Drawing.Point(5, 109);
            this.showUtcTime.Name = "showUtcTime";
            this.showUtcTime.Size = new System.Drawing.Size(54, 25);
            this.showUtcTime.TabIndex = 18;
            this.showUtcTime.Text = "UTC";
            this.showUtcTime.CheckedChanged += new System.EventHandler(this.showUtcTime_CheckedChanged);
            // 
            // apply
            // 
            this.apply.BackColor = System.Drawing.Color.Transparent;
            this.apply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.apply.ImageDisabled = null;
            this.apply.ImageEnabled = null;
            this.apply.Location = new System.Drawing.Point(73, 108);
            this.apply.MaximumSize = new System.Drawing.Size(140, 33);
            this.apply.Name = "apply";
            this.apply.Selected = false;
            this.apply.Size = new System.Drawing.Size(69, 33);
            this.apply.TabIndex = 19;
            this.apply.Text = "Apply";
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // OK
            // 
            this.OK.BackColor = System.Drawing.Color.Transparent;
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.ImageDisabled = null;
            this.OK.ImageEnabled = null;
            this.OK.Location = new System.Drawing.Point(140, 108);
            this.OK.MaximumSize = new System.Drawing.Size(140, 33);
            this.OK.Name = "OK";
            this.OK.Selected = false;
            this.OK.Size = new System.Drawing.Size(69, 33);
            this.OK.TabIndex = 20;
            this.OK.Text = "OK";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // secUpDown
            // 
            this.secUpDown.BackColor = System.Drawing.Color.Transparent;
            this.secUpDown.Location = new System.Drawing.Point(172, 70);
            this.secUpDown.MaximumSize = new System.Drawing.Size(33, 30);
            this.secUpDown.MinimumSize = new System.Drawing.Size(33, 30);
            this.secUpDown.Name = "secUpDown";
            this.secUpDown.Size = new System.Drawing.Size(33, 30);
            this.secUpDown.TabIndex = 17;
            this.secUpDown.Down += new System.EventHandler(this.secDown_Pushed);
            this.secUpDown.Up += new System.EventHandler(this.secUp_Pushed);
            // 
            // dayUpDown
            // 
            this.dayUpDown.BackColor = System.Drawing.Color.Transparent;
            this.dayUpDown.Location = new System.Drawing.Point(172, 21);
            this.dayUpDown.MaximumSize = new System.Drawing.Size(33, 30);
            this.dayUpDown.MinimumSize = new System.Drawing.Size(33, 30);
            this.dayUpDown.Name = "dayUpDown";
            this.dayUpDown.Size = new System.Drawing.Size(33, 30);
            this.dayUpDown.TabIndex = 14;
            this.dayUpDown.Down += new System.EventHandler(this.dayDown_Pushed);
            this.dayUpDown.Up += new System.EventHandler(this.dayUp_Pushed);
            // 
            // minUpDown
            // 
            this.minUpDown.BackColor = System.Drawing.Color.Transparent;
            this.minUpDown.Location = new System.Drawing.Point(103, 70);
            this.minUpDown.MaximumSize = new System.Drawing.Size(33, 30);
            this.minUpDown.MinimumSize = new System.Drawing.Size(33, 30);
            this.minUpDown.Name = "minUpDown";
            this.minUpDown.Size = new System.Drawing.Size(33, 30);
            this.minUpDown.TabIndex = 16;
            this.minUpDown.Down += new System.EventHandler(this.minDown_Pushed);
            this.minUpDown.Up += new System.EventHandler(this.minUp_Pushed);
            // 
            // hourUpDown
            // 
            this.hourUpDown.BackColor = System.Drawing.Color.Transparent;
            this.hourUpDown.Location = new System.Drawing.Point(39, 70);
            this.hourUpDown.MaximumSize = new System.Drawing.Size(33, 30);
            this.hourUpDown.MinimumSize = new System.Drawing.Size(33, 30);
            this.hourUpDown.Name = "hourUpDown";
            this.hourUpDown.Size = new System.Drawing.Size(33, 30);
            this.hourUpDown.TabIndex = 15;
            this.hourUpDown.Down += new System.EventHandler(this.hourDown_Pushed);
            this.hourUpDown.Up += new System.EventHandler(this.hourUp_Pushed);
            // 
            // monthUpDown
            // 
            this.monthUpDown.BackColor = System.Drawing.Color.Transparent;
            this.monthUpDown.Location = new System.Drawing.Point(103, 21);
            this.monthUpDown.MaximumSize = new System.Drawing.Size(33, 30);
            this.monthUpDown.MinimumSize = new System.Drawing.Size(33, 30);
            this.monthUpDown.Name = "monthUpDown";
            this.monthUpDown.Size = new System.Drawing.Size(33, 30);
            this.monthUpDown.TabIndex = 13;
            this.monthUpDown.Down += new System.EventHandler(this.monthDown_Pushed);
            this.monthUpDown.Up += new System.EventHandler(this.monthUp_Pushed);
            // 
            // yearUpDown
            // 
            this.yearUpDown.BackColor = System.Drawing.Color.Transparent;
            this.yearUpDown.Location = new System.Drawing.Point(39, 21);
            this.yearUpDown.MaximumSize = new System.Drawing.Size(33, 30);
            this.yearUpDown.MinimumSize = new System.Drawing.Size(33, 30);
            this.yearUpDown.Name = "yearUpDown";
            this.yearUpDown.Size = new System.Drawing.Size(33, 30);
            this.yearUpDown.TabIndex = 12;
            this.yearUpDown.Down += new System.EventHandler(this.yearDown_Pushed);
            this.yearUpDown.Up += new System.EventHandler(this.yearUp_Pushed);
            // 
            // DatePopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(212, 146);
            this.Controls.Add(this.PushPin);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.showUtcTime);
            this.Controls.Add(this.apply);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.day);
            this.Controls.Add(this.sec);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.month);
            this.Controls.Add(this.min);
            this.Controls.Add(this.year);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.hours);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.secUpDown);
            this.Controls.Add(this.dayUpDown);
            this.Controls.Add(this.minUpDown);
            this.Controls.Add(this.hourUpDown);
            this.Controls.Add(this.monthUpDown);
            this.Controls.Add(this.yearUpDown);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DatePopup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Date Time Selection";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DatePopup_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DatePopup_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.PushPin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox hours;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox min;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox sec;
        private WwtButton OK;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox year;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox month;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox day;
        private WWTCheckbox showUtcTime;
        private WwtButton apply;
        private System.Windows.Forms.PictureBox PushPin;
        private WwtUpDown yearUpDown;
        private WwtUpDown monthUpDown;
        private WwtUpDown dayUpDown;
        private WwtUpDown hourUpDown;
        private WwtUpDown minUpDown;
        private WwtUpDown secUpDown;

    }
}