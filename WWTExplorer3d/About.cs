using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace TerraViewer
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class About : System.Windows.Forms.Form
    {
        private Label aboutText;
        private Timer timer1;
        private Button button1;
        private IContainer components;

		public About()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            this.aboutText.Visible = true;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.aboutText = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // aboutText
            // 
            this.aboutText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(222)))), ((int)(((byte)(227)))));
            this.aboutText.Location = new System.Drawing.Point(46, 279);
            this.aboutText.Name = "aboutText";
            this.aboutText.Size = new System.Drawing.Size(572, 271);
            this.aboutText.TabIndex = 0;
            this.aboutText.Text = resources.GetString("aboutText.Text");
            this.aboutText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.aboutText.MouseClick += new System.Windows.Forms.MouseEventHandler(this.aboutText_MouseClick);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(222)))), ((int)(((byte)(227)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.button1.Location = new System.Drawing.Point(413, 208);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(60, 24);
            this.button1.TabIndex = 2;
            this.button1.Text = "Close!";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.button1_MouseMove);
            // 
            // About
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(1)))), ((int)(((byte)(1)))));
            this.BackgroundImage = global::TerraViewer.Properties.Resources.betaSplash;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(618, 550);
            this.ControlBox = false;
            this.Controls.Add(this.aboutText);
            this.Controls.Add(this.button1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(1)))), ((int)(((byte)(1)))));
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.About_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.About_MouseDown);
            this.ResumeLayout(false);

		}
		#endregion

		private void Form1_Load(object sender, System.EventArgs e)
		{
            aboutText.Text = "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()+ "\r\n"+aboutText.Text;
 		}

	
        
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            aboutState = AboutState.FadingOut;
        }

        private void aboutText_MouseClick(object sender, MouseEventArgs e)
        {
            aboutState = AboutState.FadingOut;
        }

        enum AboutState { FadingIn, Showing, Running, FadingOut };

        AboutState aboutState = AboutState.FadingIn;

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (aboutState)
            {
                case AboutState.FadingIn:
                    this.Opacity += .20;
                    if (this.Opacity == 1.0)
                    {
                        aboutState = AboutState.Running;
                        timer1.Interval = 50;
                    }
                    break;
                case AboutState.Showing:
                    aboutState = AboutState.FadingOut;
                    timer1.Interval = 50;
                    break;
                case AboutState.FadingOut:
                    this.Opacity -= .10;
                    if (this.Opacity == 0.0)
                    {
                        this.Close();
                    }
                    break;
            }
        }

        private void About_KeyDown(object sender, KeyEventArgs e)
        {
            aboutState = AboutState.FadingOut;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you really sure you want to close this Daj?") == DialogResult.OK)
            {
                aboutState = AboutState.FadingOut;
            }
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > (button1.Width / 2))
            {
                button1.Left -= 4;
            }
            else
            {
                button1.Left += 4;
            }

            if (e.Y > (button1.Height / 2))
            {
                button1.Top -= 4;
            }
            else
            {
                button1.Top += 4;
            }
        }

        private void About_MouseDown(object sender, MouseEventArgs e)
        {
            aboutState = AboutState.FadingOut;

        }
	}
}
