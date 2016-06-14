using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MakeDataCabinetFile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Microsoft\\WorldWideTelescope\\data";

            FileCabinet cab = new FileCabinet(textBox1.Text, path);

            InjestDirectory(cab, path);
            cab.Package();

        }

        private static void InjestDirectory(FileCabinet cab, string path)
        {
            foreach (string dir in Directory.GetDirectories(path))
            {
                InjestDirectory(cab, dir);

            }
            foreach (string file in Directory.GetFiles(path))
            {
                cab.AddFile(file);

            }            
        }
    }
}
