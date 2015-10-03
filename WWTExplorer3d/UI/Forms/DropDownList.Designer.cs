namespace TerraViewer
{
    partial class FilterDropDown
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Solar System");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Star");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Supernova");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Black Hole");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Neutron Star");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Double Star");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Multiple Stars");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Stellar", new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6,
            treeNode7});
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Constellation");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Asterism");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Open Cluster");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Globular Cluster");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Nebulous Cluster");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Stellar Groupings", new System.Windows.Forms.TreeNode[] {
            treeNode9,
            treeNode10,
            treeNode11,
            treeNode12,
            treeNode13});
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Nebula");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Planetary Nebula");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Supernova Remnant");
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Emission Nebula");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Reflection Nebula");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("Dark Nebula");
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("Giant Molecular Cloud");
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("Interstellar Dust");
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("Nebulae", new System.Windows.Forms.TreeNode[] {
            treeNode15,
            treeNode16,
            treeNode17,
            treeNode18,
            treeNode19,
            treeNode20,
            treeNode21,
            treeNode22});
            System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("Galaxy");
            System.Windows.Forms.TreeNode treeNode25 = new System.Windows.Forms.TreeNode("Spiral Galaxy");
            System.Windows.Forms.TreeNode treeNode26 = new System.Windows.Forms.TreeNode("Irregular Galaxy");
            System.Windows.Forms.TreeNode treeNode27 = new System.Windows.Forms.TreeNode("Elliptical Galaxy");
            System.Windows.Forms.TreeNode treeNode28 = new System.Windows.Forms.TreeNode("Knot");
            System.Windows.Forms.TreeNode treeNode29 = new System.Windows.Forms.TreeNode("Cluster of Galaxies");
            System.Windows.Forms.TreeNode treeNode30 = new System.Windows.Forms.TreeNode("Quasar");
            System.Windows.Forms.TreeNode treeNode31 = new System.Windows.Forms.TreeNode("Galactic", new System.Windows.Forms.TreeNode[] {
            treeNode24,
            treeNode25,
            treeNode26,
            treeNode27,
            treeNode28,
            treeNode29,
            treeNode30});
            System.Windows.Forms.TreeNode treeNode32 = new System.Windows.Forms.TreeNode("Unidentified");
            System.Windows.Forms.TreeNode treeNode33 = new System.Windows.Forms.TreeNode("Plate Defect");
            System.Windows.Forms.TreeNode treeNode34 = new System.Windows.Forms.TreeNode("Other NGC");
            System.Windows.Forms.TreeNode treeNode35 = new System.Windows.Forms.TreeNode("Other", new System.Windows.Forms.TreeNode[] {
            treeNode32,
            treeNode33,
            treeNode34});
            this.DropList = new System.Windows.Forms.TreeView();
            this.listBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // DropList
            // 
            this.DropList.CheckBoxes = true;
            this.DropList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DropList.Location = new System.Drawing.Point(0, 0);
            this.DropList.Name = "DropList";
            treeNode1.Checked = true;
            treeNode1.Name = "SolarSystem";
            treeNode1.Text = "Solar System";
            treeNode2.Name = "Star";
            treeNode2.Text = "Star";
            treeNode3.Name = "Supernova";
            treeNode3.Text = "Supernova";
            treeNode4.Name = "BlackHole";
            treeNode4.Text = "Black Hole";
            treeNode5.Name = "NeutronStar";
            treeNode5.Text = "Neutron Star";
            treeNode6.Name = "DoubleStar";
            treeNode6.Text = "Double Star";
            treeNode7.Name = "MultipleStars";
            treeNode7.Text = "Multiple Stars";
            treeNode8.Checked = true;
            treeNode8.Name = "Stellar";
            treeNode8.Text = "Stellar";
            treeNode9.Name = "Constellation";
            treeNode9.Text = "Constellation";
            treeNode10.Name = "Asterism";
            treeNode10.Text = "Asterism";
            treeNode11.Name = "OpenCluster";
            treeNode11.Text = "Open Cluster";
            treeNode12.Name = "GlobularCluster";
            treeNode12.Text = "Globular Cluster";
            treeNode13.Name = "NebulousCluster";
            treeNode13.Text = "Nebulous Cluster";
            treeNode14.Checked = true;
            treeNode14.Name = "StellarGroupings";
            treeNode14.Text = "Stellar Groupings";
            treeNode15.Name = "Nebula";
            treeNode15.Text = "Nebula";
            treeNode16.Name = "PlanetaryNebula";
            treeNode16.Text = "Planetary Nebula";
            treeNode17.Name = "SupernovaRemnant";
            treeNode17.Text = "Supernova Remnant";
            treeNode18.Name = "EmissionNebula";
            treeNode18.Text = "Emission Nebula";
            treeNode19.Name = "ReflectionNebula";
            treeNode19.Text = "Reflection Nebula";
            treeNode20.Name = "DarkNebula";
            treeNode20.Text = "Dark Nebula";
            treeNode21.Name = "GiantMolecularCloud";
            treeNode21.Text = "Giant Molecular Cloud";
            treeNode22.Name = "InterstellarDust";
            treeNode22.Text = "Interstellar Dust";
            treeNode23.Checked = true;
            treeNode23.Name = "Nebulae";
            treeNode23.Text = "Nebulae";
            treeNode24.Name = "Galaxy";
            treeNode24.Text = "Galaxy";
            treeNode25.Name = "SpiralGalaxy";
            treeNode25.Text = "Spiral Galaxy";
            treeNode26.Name = "IrregularGalaxy";
            treeNode26.Text = "Irregular Galaxy";
            treeNode27.Name = "EllipticalGalaxy";
            treeNode27.Text = "Elliptical Galaxy";
            treeNode28.Name = "Knot";
            treeNode28.Text = "Knot";
            treeNode29.Name = "ClusterOfGalaxies";
            treeNode29.Text = "Cluster of Galaxies";
            treeNode30.Name = "Quasar";
            treeNode30.Text = "Quasar";
            treeNode31.Checked = true;
            treeNode31.Name = "Galactic";
            treeNode31.Text = "Galactic";
            treeNode32.Name = "Unidentified";
            treeNode32.Text = "Unidentified";
            treeNode33.Name = "PlateDefect";
            treeNode33.Text = "Plate Defect";
            treeNode34.Name = "OtherNGC";
            treeNode34.Text = "Other NGC";
            treeNode35.Checked = true;
            treeNode35.Name = "Other";
            treeNode35.Text = "Other";
            this.DropList.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode8,
            treeNode14,
            treeNode23,
            treeNode31,
            treeNode35});
            this.DropList.Size = new System.Drawing.Size(230, 88);
            this.DropList.TabIndex = 1;
            this.DropList.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.DropList_AfterCheck);
            this.DropList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.DropList_AfterSelect);
            // 
            // listBox
            // 
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.FormattingEnabled = true;
            this.listBox.IntegralHeight = false;
            this.listBox.Location = new System.Drawing.Point(0, 0);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(230, 88);
            this.listBox.TabIndex = 2;
            this.listBox.Visible = false;
            this.listBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseClick);
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            this.listBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listBox_KeyUp);
            // 
            // FilterDropDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(230, 88);
            this.ControlBox = false;
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.DropList);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FilterDropDown";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "DropDownList";
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.FilterDropDown_Activated);
            this.Deactivate += new System.EventHandler(this.FilterDropDown_Deactivate);
            this.Load += new System.EventHandler(this.FilterDropDown_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView DropList;
        private System.Windows.Forms.ListBox listBox;
    }
}