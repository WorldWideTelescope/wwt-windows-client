using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class FlipbookSetup : Form
    {
        public FlipbookSetup()
        {
            InitializeComponent();
        }

        private void loopCheckbox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void endFrameLabel_Click(object sender, EventArgs e)
        {

        }

        private void frameSequenceText_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string values = frameSequenceText.Text;
                List<int> framesList = new List<int>();

                if (!String.IsNullOrEmpty(values))
                {
                    string[] parts = values.Split(new char[] { ',' });
                    foreach (string part in parts)
                    {
                        int x = Convert.ToInt32(part.Trim());
                        framesList.Add(x);
                    }
                    StringBuilder sb = new StringBuilder();
                    string delim = "";
                    foreach (int frame in framesList)
                    {
                        sb.Append(delim);
                        sb.Append(frame.ToString());
                        delim = ", ";
                    }
                    frameSequenceText.Text = sb.ToString();
                }
                frameSequenceText.BackColor = Color.FromArgb(68, 88, 105);
            }
            catch
            {
                frameSequenceText.BackColor = Color.Red;
            }
        }

        private void FlipbookSetup_Load(object sender, EventArgs e)
        {
            string[] values = Enum.GetNames(typeof(LoopTypes));
            int index = 0;
            foreach (string value in values)
            {
                int temp = flipbookStyle.Items.Add(value);
                if (value == LoopType.ToString())
                {
                    index = temp;
                }            
            }
            flipbookStyle.SelectedIndex = index;

            frameSequenceText.Text = FrameSequence;
            endFrame.Text = Frames.ToString();
            startFrame.Text = StartFrame.ToString();
            gridXText.Text = FramesX.ToString();
            gridYText.Text = FramesY.ToString();
        }
 
        public int StartFrame = 0;
        public int Frames = 64;
        public int FramesX = 8;
        public int FramesY = 8;
        public LoopTypes LoopType = LoopTypes.UpDown;
        public string FrameSequence = "";
        
        private void OK_Click(object sender, EventArgs e)
        {
            FrameSequence = frameSequenceText.Text;
            Frames = Convert.ToInt32(endFrame.Text);
            StartFrame = Convert.ToInt32(startFrame.Text);
            FramesX = Convert.ToInt32(gridXText.Text);
            FramesY = Convert.ToInt32(gridYText.Text);
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void flipbookStyle_SelectionChanged(object sender, EventArgs e)
        {
            LoopType = (LoopTypes)Enum.Parse(typeof(LoopTypes), flipbookStyle.SelectedItem.ToString());
        }
    }
}