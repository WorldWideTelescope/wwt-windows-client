﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class PushPinPickerPopup : Form
    {
        public PushPinPickerPopup()
        {
            InitializeComponent();
        }
        public int SelectedIndex = -1;
        private void PushPinPickerPopup_Load(object sender, EventArgs e)
        {
            vertScroll.Maximum = (PushPin.PinCount / 40)*40;
        }

        int startIndex = 0;

        private void PushPinPickerPopup_Paint(object sender, PaintEventArgs e)
        {
            
            int index = startIndex;
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (SelectedIndex == index)
                    {
                        e.Graphics.DrawRectangle(Pens.Yellow, new Rectangle(x * 36, y * 36, 36, 36));
                    }
                    PushPin.DrawAt(e.Graphics, index, x * 36+2, y * 36+2);
                    index++;
                }
            }
            

        }

        private void vertScroll_Scroll(object sender, ScrollEventArgs e)
        {
            startIndex = vertScroll.Value;
            Invalidate();
        }

        private void PushPinPickerPopup_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void PushPinPickerPopup_MouseUp(object sender, MouseEventArgs e)
        {
            SelectedIndex = startIndex + (e.X / 36) + (e.Y / 36) * 8;
            Invalidate();
            mouseDown = false;

        }
        bool mouseDown = false;
        private void PushPinPickerPopup_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            SelectedIndex = startIndex + (e.X / 36) + (e.Y / 36) * 8;
            Invalidate();

        }

        private void PushPinPickerPopup_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                SelectedIndex = startIndex + (e.X / 36) + (e.Y / 36) * 8;
                Invalidate();
            }

        }
    }
}
