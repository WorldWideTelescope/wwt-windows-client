using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace TerraViewer
{
	/// <summary>
	/// Summary description for GoToLatLong.
	/// </summary>
	public class LocationSetup : System.Windows.Forms.Form
    {
		private System.Windows.Forms.TextBox txtLong;
		private System.Windows.Forms.ListBox PlacesList;
		private System.Windows.Forms.Button Remove;
		private System.Windows.Forms.Button Add;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.TextBox txtLat;
		private System.Windows.Forms.Label lblLatitude;
		private System.Windows.Forms.Label lblLongitude;
        private System.Windows.Forms.Label datasetLabel;
		private System.Windows.Forms.Label lblCatagory;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private WwtButton OK;
        private WwtButton Cancel;
        private WwtCombo DataSetList;
        private WwtCombo Categorys;
        private Label label2;
        private TextBox txtAltitude;
        private WwtButton FromEarthView;
        private bool sky = true;

        public bool Sky
        {
            get { return sky; }
            set { sky = value; }
        }
		public LocationSetup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
            SetUiStrings();
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

        private void SetUiStrings()
        {
            this.lblLatitude.Text = Language.GetLocalizedText(253, "Latitude (Dec. deg.)");
            this.lblLongitude.Text = Language.GetLocalizedText(254, "E. Longitude (Dec. deg.)");
            this.Remove.Text = Language.GetLocalizedText(129, "Remove");
            this.Add.Text = Language.GetLocalizedText(166, "Add");
            this.label1.Text = Language.GetLocalizedText(238, "Name");
            this.txtName.Text = Language.GetLocalizedText(255, "<Type name here>");
            this.datasetLabel.Text = Language.GetLocalizedText(256, "Data Set");
            this.lblCatagory.Text = Language.GetLocalizedText(257, "Region");
            this.label2.Text = Language.GetLocalizedText(258, "Elevation (Meters)");
            this.FromEarthView.Text = Language.GetLocalizedText(259, "Get From View");
            this.Categorys.Name = Language.GetLocalizedText(260, "Categorys");
            this.Cancel.Text = Language.GetLocalizedText(157, "Cancel");
            this.OK.Text = Language.GetLocalizedText(156, "OK");
            this.Text = Language.GetLocalizedText(261, "Observing Location Options");

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
            this.lblLatitude = new System.Windows.Forms.Label();
            this.lblLongitude = new System.Windows.Forms.Label();
            this.txtLong = new System.Windows.Forms.TextBox();
            this.txtLat = new System.Windows.Forms.TextBox();
            this.PlacesList = new System.Windows.Forms.ListBox();
            this.Remove = new System.Windows.Forms.Button();
            this.Add = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.datasetLabel = new System.Windows.Forms.Label();
            this.lblCatagory = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAltitude = new System.Windows.Forms.TextBox();
            this.FromEarthView = new TerraViewer.WwtButton();
            this.Categorys = new TerraViewer.WwtCombo();
            this.DataSetList = new TerraViewer.WwtCombo();
            this.Cancel = new TerraViewer.WwtButton();
            this.OK = new TerraViewer.WwtButton();
            this.SuspendLayout();
            // 
            // lblLatitude
            // 
            this.lblLatitude.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLatitude.ForeColor = System.Drawing.Color.White;
            this.lblLatitude.Location = new System.Drawing.Point(205, 107);
            this.lblLatitude.Name = "lblLatitude";
            this.lblLatitude.Size = new System.Drawing.Size(140, 16);
            this.lblLatitude.TabIndex = 9;
            this.lblLatitude.Text = "Latitude (Dec. deg.)";
            // 
            // lblLongitude
            // 
            this.lblLongitude.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLongitude.ForeColor = System.Drawing.Color.White;
            this.lblLongitude.Location = new System.Drawing.Point(205, 59);
            this.lblLongitude.Name = "lblLongitude";
            this.lblLongitude.Size = new System.Drawing.Size(144, 16);
            this.lblLongitude.TabIndex = 7;
            this.lblLongitude.Text = "E. Longitude (Dec. deg.)";
            // 
            // txtLong
            // 
            this.txtLong.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.txtLong.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLong.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLong.ForeColor = System.Drawing.Color.White;
            this.txtLong.Location = new System.Drawing.Point(208, 77);
            this.txtLong.Name = "txtLong";
            this.txtLong.Size = new System.Drawing.Size(136, 22);
            this.txtLong.TabIndex = 8;
            this.txtLong.Text = "-122.08580";
            // 
            // txtLat
            // 
            this.txtLat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.txtLat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLat.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLat.ForeColor = System.Drawing.Color.White;
            this.txtLat.Location = new System.Drawing.Point(208, 125);
            this.txtLat.Name = "txtLat";
            this.txtLat.Size = new System.Drawing.Size(136, 22);
            this.txtLat.TabIndex = 10;
            this.txtLat.Text = "47.717";
            // 
            // PlacesList
            // 
            this.PlacesList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.PlacesList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.PlacesList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PlacesList.ForeColor = System.Drawing.Color.White;
            this.PlacesList.Location = new System.Drawing.Point(10, 105);
            this.PlacesList.Name = "PlacesList";
            this.PlacesList.Size = new System.Drawing.Size(174, 156);
            this.PlacesList.TabIndex = 4;
            this.PlacesList.SelectedIndexChanged += new System.EventHandler(this.PlacesList_SelectedIndexChanged);
            // 
            // Remove
            // 
            this.Remove.ForeColor = System.Drawing.Color.White;
            this.Remove.Location = new System.Drawing.Point(208, 197);
            this.Remove.Name = "Remove";
            this.Remove.Size = new System.Drawing.Size(64, 23);
            this.Remove.TabIndex = 11;
            this.Remove.Text = "Remove";
            this.Remove.Visible = false;
            this.Remove.Click += new System.EventHandler(this.Remove_Click);
            // 
            // Add
            // 
            this.Add.ForeColor = System.Drawing.Color.White;
            this.Add.Location = new System.Drawing.Point(280, 197);
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(64, 23);
            this.Add.TabIndex = 12;
            this.Add.Text = "Add";
            this.Add.Visible = false;
            this.Add.Click += new System.EventHandler(this.Add_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(205, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.AcceptsReturn = true;
            this.txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.ForeColor = System.Drawing.Color.White;
            this.txtName.Location = new System.Drawing.Point(208, 32);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(136, 22);
            this.txtName.TabIndex = 6;
            this.txtName.Text = "<Type name here>";
            this.txtName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtName_KeyDown);
            // 
            // datasetLabel
            // 
            this.datasetLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.datasetLabel.ForeColor = System.Drawing.Color.White;
            this.datasetLabel.Location = new System.Drawing.Point(9, 6);
            this.datasetLabel.Name = "datasetLabel";
            this.datasetLabel.Size = new System.Drawing.Size(100, 16);
            this.datasetLabel.TabIndex = 0;
            this.datasetLabel.Text = "Data Set";
            // 
            // lblCatagory
            // 
            this.lblCatagory.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCatagory.ForeColor = System.Drawing.Color.White;
            this.lblCatagory.Location = new System.Drawing.Point(9, 50);
            this.lblCatagory.Name = "lblCatagory";
            this.lblCatagory.Size = new System.Drawing.Size(100, 23);
            this.lblCatagory.TabIndex = 2;
            this.lblCatagory.Text = "Region";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(205, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "Elevation (Meters)";
            // 
            // txtAltitude
            // 
            this.txtAltitude.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.txtAltitude.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAltitude.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAltitude.ForeColor = System.Drawing.Color.White;
            this.txtAltitude.Location = new System.Drawing.Point(208, 169);
            this.txtAltitude.Name = "txtAltitude";
            this.txtAltitude.Size = new System.Drawing.Size(136, 22);
            this.txtAltitude.TabIndex = 10;
            this.txtAltitude.Text = "100";
            this.txtAltitude.TextChanged += new System.EventHandler(this.txtAltitude_TextChanged);
            // 
            // FromEarthView
            // 
            this.FromEarthView.BackColor = System.Drawing.Color.Transparent;
            this.FromEarthView.DialogResult = System.Windows.Forms.DialogResult.None;
            this.FromEarthView.ImageDisabled = null;
            this.FromEarthView.ImageEnabled = null;
            this.FromEarthView.Location = new System.Drawing.Point(204, 197);
            this.FromEarthView.MaximumSize = new System.Drawing.Size(140, 33);
            this.FromEarthView.Name = "FromEarthView";
            this.FromEarthView.Selected = false;
            this.FromEarthView.Size = new System.Drawing.Size(140, 33);
            this.FromEarthView.TabIndex = 15;
            this.FromEarthView.Text = "Get From View";
            this.FromEarthView.Visible = false;
            this.FromEarthView.Click += new System.EventHandler(this.FromEarthView_Click);
            // 
            // Categorys
            // 
            this.Categorys.BackColor = System.Drawing.Color.Transparent;
            this.Categorys.DateTimeValue = new System.DateTime(2008, 1, 23, 20, 17, 47, 913);
            this.Categorys.Filter = TerraViewer.Classification.Unfiltered;
            this.Categorys.FilterStyle = false;
            this.Categorys.Location = new System.Drawing.Point(11, 64);
            this.Categorys.Margin = new System.Windows.Forms.Padding(0);
            this.Categorys.MaximumSize = new System.Drawing.Size(248, 33);
            this.Categorys.MinimumSize = new System.Drawing.Size(35, 33);
            this.Categorys.Name = "Categorys";
            this.Categorys.SelectedIndex = -1;
            this.Categorys.Size = new System.Drawing.Size(179, 33);
            this.Categorys.State = TerraViewer.State.Rest;
            this.Categorys.TabIndex = 3;
            this.Categorys.Type = TerraViewer.WwtCombo.ComboType.List;
            this.Categorys.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.Catagorys_SelectedIndexChanged);
            // 
            // DataSetList
            // 
            this.DataSetList.BackColor = System.Drawing.Color.Transparent;
            this.DataSetList.DateTimeValue = new System.DateTime(2008, 1, 23, 20, 17, 47, 933);
            this.DataSetList.Filter = TerraViewer.Classification.Unfiltered;
            this.DataSetList.FilterStyle = false;
            this.DataSetList.Location = new System.Drawing.Point(10, 19);
            this.DataSetList.Margin = new System.Windows.Forms.Padding(0);
            this.DataSetList.MaximumSize = new System.Drawing.Size(248, 33);
            this.DataSetList.MinimumSize = new System.Drawing.Size(35, 33);
            this.DataSetList.Name = "DataSetList";
            this.DataSetList.SelectedIndex = -1;
            this.DataSetList.Size = new System.Drawing.Size(180, 33);
            this.DataSetList.State = TerraViewer.State.Rest;
            this.DataSetList.TabIndex = 1;
            this.DataSetList.Type = TerraViewer.WwtCombo.ComboType.List;
            this.DataSetList.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.DataSetList_SelectedIndexChanged);
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.BackColor = System.Drawing.Color.Transparent;
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Cancel.ImageDisabled = null;
            this.Cancel.ImageEnabled = null;
            this.Cancel.Location = new System.Drawing.Point(277, 240);
            this.Cancel.MaximumSize = new System.Drawing.Size(140, 33);
            this.Cancel.Name = "Cancel";
            this.Cancel.Selected = false;
            this.Cancel.Size = new System.Drawing.Size(72, 33);
            this.Cancel.TabIndex = 14;
            this.Cancel.Text = "Cancel";
            this.Cancel.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.BackColor = System.Drawing.Color.Transparent;
            this.OK.DialogResult = System.Windows.Forms.DialogResult.None;
            this.OK.ImageDisabled = null;
            this.OK.ImageEnabled = null;
            this.OK.Location = new System.Drawing.Point(202, 240);
            this.OK.MaximumSize = new System.Drawing.Size(140, 33);
            this.OK.Name = "OK";
            this.OK.Selected = false;
            this.OK.Size = new System.Drawing.Size(73, 33);
            this.OK.TabIndex = 13;
            this.OK.Text = "OK";
            this.OK.Click += new System.EventHandler(this.goButton_Click);
            // 
            // LocationSetup
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(357, 285);
            this.Controls.Add(this.FromEarthView);
            this.Controls.Add(this.Categorys);
            this.Controls.Add(this.DataSetList);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.lblCatagory);
            this.Controls.Add(this.datasetLabel);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtAltitude);
            this.Controls.Add(this.txtLat);
            this.Controls.Add(this.txtLong);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Add);
            this.Controls.Add(this.Remove);
            this.Controls.Add(this.PlacesList);
            this.Controls.Add(this.lblLongitude);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblLatitude);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LocationSetup";
            this.ShowInTaskbar = false;
            this.Text = "Observing Location Options";
            this.Load += new System.EventHandler(this.GoToLatLong_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SearchDialog_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SearchDialog_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public static int lastSelectedDatasetIndex = 0;
		private void GoToLatLong_Load(object sender, System.EventArgs e)
		{
            Dictionary<string, DataSet> datasets = DataSetManager.GetDataSets();
            foreach (DataSet d in datasets.Values)
			{
                if (d.Sky == sky)
                {
                    this.DataSetList.Items.Add(d);
                }
			}
			this.DataSetList.SelectedIndex=lastSelectedDatasetIndex;
			
			this.Categorys.SelectedIndex = lastSelectedIndexCatagorys;
			this.txtLat.Text = Latitude.ToString();
			this.txtLong.Text = Longitude.ToString();
            this.txtName.Text = LocationName;
            this.txtAltitude.Text = Altitude.ToString();
            if (sky)
            {
                this.lblLatitude.Text = Language.GetLocalizedText(262, "Declination");
                this.lblLongitude.Text = Language.GetLocalizedText(263, "Right Ascension");

            }

            this.txtLat.Text = Coordinates.FormatDMS(Latitude);
            this.txtLong.Text = Coordinates.FormatDMS(Longitude);
            if (Earth3d.MainWindow.CurrentImageSet.DataSetType == ImageSetType.Earth)
            {
                FromEarthView.Visible = true;
            }
		}

        private void PlacesList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            TourPlace p = (TourPlace)this.PlacesList.SelectedItem;
            if (p != null)
            {
                this.txtName.Text = p.Name;

                this.txtLat.Text = Coordinates.FormatDMS(p.Lat);
                this.txtLong.Text = Coordinates.FormatDMS(p.Lng);
                this.txtAltitude.Text = p.Elevation.ToString();

            }
        }

		private void Add_Click(object sender, System.EventArgs e)
		{


		}

		private void Remove_Click(object sender, System.EventArgs e)
		{
			if (PlacesList.SelectedItem != null)
			{
				///cityList.Remove(PlacesList.SelectedItem);
			}
		}

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
            DialogResult = DialogResult.Cancel;
			this.Close();
		}
		public double Latitude;
		public double Longitude;
        public string LocationName;
        public double Altitude;
     

		private void goButton_Click(object sender, System.EventArgs e)
		{
			Latitude = Coordinates.Parse(txtLat.Text);
            Longitude = Coordinates.Parse(txtLong.Text);

            if (Latitude == 0)
            {
                Latitude = 1 / (360 * 60 * 60 * .5);
            }

            double alt = Altitude;
            try
            {
                alt = Convert.ToDouble(txtAltitude.Text);
            }
            catch
            {
            }
            Altitude = alt;
            LocationName = this.txtName.Text;
            DialogResult = DialogResult.OK;
			this.Close();
		}

		private void DataSetList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            try
            {
                lastSelectedDatasetIndex = this.DataSetList.SelectedIndex;
                DataSet ds = (DataSet)this.DataSetList.SelectedItem;
                if (ds != null)
                {
                    this.Categorys.Items.Clear();
                    Dictionary<string, Places> placesList = ds.GetPlaces();
                    foreach (Places places in placesList.Values)
                    {
                        this.Categorys.Items.Add(places);
                    }

                    if (Categorys.Items.Count > 0)
                    {
                        this.Categorys.SelectedIndex = 0;
                    }
                }
            }
            catch
            {
            }
		}
		public static int lastSelectedIndexCatagorys = 0;
		public static Places placesLastSelected = null;
		private void Catagorys_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            try
            {
                lastSelectedIndexCatagorys = this.Categorys.SelectedIndex;
                Places places = (Places)this.Categorys.SelectedItem;
                placesLastSelected = places;
                if (places != null)
                {
                    this.PlacesList.Items.Clear();
                    ArrayList placeList = places.GetPlaceList();
                    foreach (TourPlace place in placeList)
                    {
                        this.PlacesList.Items.Add(place);
                    }
                }
            }
            catch
            {
            }
		}

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void SearchDialog_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void SearchDialog_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                this.Close();
            }
            if (e.KeyCode == Keys.F1)
            {
                WebWindow.OpenUrl("http://www.worldwidetelescope.org/Learn", true);
            }
        }

        private void txtAltitude_TextChanged(object sender, EventArgs e)
        {
            string validationText = txtAltitude.Text;
            string outputString = "";

            foreach (char c in validationText)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    outputString += c;
                }
            }
            if (validationText != outputString)
            {
                txtAltitude.Text = outputString;
            }
         
        }

        private void FromEarthView_Click(object sender, EventArgs e)
        {
            txtLat.Text = Coordinates.FormatDMS(Earth3d.MainWindow.RenderEngine.ViewLat);
            txtLong.Text = Coordinates.FormatDMS(Earth3d.MainWindow.RenderEngine.ViewLong);
        }
	}
}
