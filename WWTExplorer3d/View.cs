using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;

namespace TerraViewer
{
    public partial class View : TabForm
    {
        public View()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
           
            this.groupBox2.Text = Language.GetLocalizedText(507, "Observing Location");
            this.localHorizon.Text = Language.GetLocalizedText(508, "View from this location");
            this.ChooseLocation.Text = Language.GetLocalizedText(379, "Setup");
            this.locationName.Text = Language.GetLocalizedText(509, "Please choose a location");
            this.label2.Text = Language.GetLocalizedText(7, "Name:");
            this.label3.Text = Language.GetLocalizedText(365, "Lng:   ");
            this.label5.Text = Language.GetLocalizedText(510, "Altitude:");
            this.label1.Text = Language.GetLocalizedText(367, "Lat :  ");
            this.groupBox3.Text = Language.GetLocalizedText(511, "Observing Time");
            this.showUtcTime.Text = Language.GetLocalizedText(194, "UTC");
            this.TimeNow.Text = Language.GetLocalizedText(512, "Now");
            this.TimeMode.Text = Language.GetLocalizedText(513, "Real Time");

            this.Text = Language.GetLocalizedText(140, "View");

            this.showMinorPlanets.Text = Language.GetLocalizedText(653, "Asteroids");

        }

        public static void ShowFovSetup()
        {
            if (RenderEngine.Engine.Fov == null)
            {
                RenderEngine.Engine.Fov = new FieldOfView(Properties.Settings.Default.FovTelescope, Properties.Settings.Default.FovCamera, Properties.Settings.Default.FovEyepiece);
            }

            FieldOfViewSetup.ShowFovSetup(Earth3d.MainWindow);
        }


        protected override void SetFocusedChild()
        {
            

        }



        private void View_Load(object sender, EventArgs e)
        {
            pinUp.Hide();
            UpdateProperties();
            Properties.Settings.Default.PropertyChanged += new PropertyChangedEventHandler(Default_PropertyChanged);

            CustomButtons.LoadButtons(Properties.Settings.Default.CahceDirectory + "buttons\\CustomView.wwtb");
        }

        void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
           
            MethodInvoker doIt = delegate
            {
                UpdateProperties();
            };

            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(doIt);
                }
                catch
                {
                }
            }
            else
            {
                doIt();
            }
        }


        bool ignoreChanges = true;
        private void UpdateProperties()
        {
            ignoreChanges = true;
            this.latText.Text = Coordinates.FormatDMS(Properties.Settings.Default.LocationLat);
            this.lngText.Text = Coordinates.FormatDMS(Properties.Settings.Default.LocationLng);
            this.locationName.Text = Properties.Settings.Default.LocationName;
            this.Altitude.Text = Properties.Settings.Default.LocationAltitude.ToString() + "m";
            localHorizon.Checked = Properties.Settings.Default.LocalHorizonMode;
            
            showUtcTime.Checked = Properties.Settings.Default.ShowUTCTime;

            showMinorPlanets.Checked = Properties.Settings.Default.SolarSystemMinorPlanets.TargetState;
            ignoreChanges = false;
        }






        private void ChooseLocation_Click(object sender, EventArgs e)
        {
            LocationSetup dialog = new LocationSetup();

            dialog.Sky = false;

            dialog.Latitude = Coordinates.Parse(this.latText.Text);
            dialog.Longitude = Coordinates.Parse(this.lngText.Text);
            dialog.LocationName = this.locationName.Text;
            dialog.Altitude = Convert.ToDouble(this.Altitude.Text.Replace("m","").Replace("'",""));

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {

                this.latText.Text = Coordinates.FormatDMS(Properties.Settings.Default.LocationLat = dialog.Latitude);
                this.lngText.Text = Coordinates.FormatDMS(Properties.Settings.Default.LocationLng = dialog.Longitude);
                this.locationName.Text = Properties.Settings.Default.LocationName = dialog.LocationName;
                this.Altitude.Text = (Properties.Settings.Default.LocationAltitude = dialog.Altitude).ToString() + "m";
                SpaceTimeController.Altitude = Properties.Settings.Default.LocationAltitude;
                SpaceTimeController.Location = Coordinates.FromLatLng(Properties.Settings.Default.LocationLat, Properties.Settings.Default.LocationLng);
            }
        }


        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void timeDateTimer_Tick(object sender, EventArgs e)
        {
            timeDateControl.DateTimeValue = SpaceTimeController.Now;
        }



        private void fastBack_Click(object sender, EventArgs e)
        {
            if (SpaceTimeController.TimeRate < -2 && SpaceTimeController.TimeRate >= -1000000000)
            {
                SpaceTimeController.TimeRate *= 10.00;
            }
            else
            {
                SpaceTimeController.TimeRate = -10.00;
            }
            SpaceTimeController.SyncToClock = true;
            UpdateSpeed();
        }

        private void back_Click(object sender, EventArgs e)
        {
            if (SpaceTimeController.TimeRate <= -10.0)
            {
                SpaceTimeController.TimeRate /= 10.00;
                SpaceTimeController.SyncToClock = true;
            }
            else
            {
                SpaceTimeController.TimeRate = -2.00;
                SpaceTimeController.SyncToClock = true;
            }
            if (SpaceTimeController.TimeRate == -1)
            {
                SpaceTimeController.TimeRate = -2.00;
            }

            if (Properties.Settings.Default.ShowUTCTime != showUtcTime.Checked)
            {
                showUtcTime.Checked = Properties.Settings.Default.ShowUTCTime;
            }
            UpdateSpeed();

        }

        private void pause_Click(object sender, EventArgs e)
        {
            SpaceTimeController.SyncToClock = !SpaceTimeController.SyncToClock;
            UpdateSpeed();

        }

        private void play_Click(object sender, EventArgs e)
        {
            if (SpaceTimeController.TimeRate >= 10.0)
            {
                SpaceTimeController.TimeRate /= 10.00;
            }
            else
            {
                SpaceTimeController.TimeRate = 1.00;
            }

            SpaceTimeController.SyncToClock = true;
            UpdateSpeed();

        }

        private void fastForward_Click(object sender, EventArgs e)
        {
            if (SpaceTimeController.TimeRate > 0 && SpaceTimeController.TimeRate <= 1000000000)
            {                                                                       
                SpaceTimeController.TimeRate *= 10.00;
            }
            else
            {
                SpaceTimeController.TimeRate = 10.00;
            }
            SpaceTimeController.SyncToClock = true;

            UpdateSpeed();
        }

        public void UpdateSpeed()
        {

            if (SpaceTimeController.TimeRate == 1.0)
            {
                TimeMode.Text = Language.GetLocalizedText(513, "Real Time");
            }
            else if (SpaceTimeController.TimeRate == -1.0)
            {
                TimeMode.Text = Language.GetLocalizedText(521, "Reverse Time");
            }
            else
            {
                TimeMode.Text = Language.GetLocalizedText(523, "X ") + SpaceTimeController.TimeRate.ToString();
            }
            if (!SpaceTimeController.SyncToClock)
            {
                TimeMode.Text = TimeMode.Text + Language.GetLocalizedText(522, " : Paused");

            }   
        }
        private void TimeNow_Click(object sender, EventArgs e)
        {
            SpaceTimeController.SyncToClock = true;
            SpaceTimeController.SyncTime();
            SpaceTimeController.TimeRate = 1.0;
            UpdateSpeed();
        }

        private void localHorizon_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
            {
                return;
            } 
            Properties.Settings.Default.LocalHorizonMode = localHorizon.Checked;
        }

        private void showUtcTime_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
            {
                return;
            }
            Properties.Settings.Default.ShowUTCTime = showUtcTime.Checked;
        }

 
        private void showMinorPlanets_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
            {
                return;
            }
            if (this.showMinorPlanets.Checked && !File.Exists(Properties.Settings.Default.CahceDirectory + "\\data\\mpc.bin"))
            {
                if (UiTools.ShowMessageBox("WorldWide Telescope needs to download the latest Minor Planet Center data file (about 12MB). Do you want to proceed?", "Minor Planet Center Data Download", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    this.showMinorPlanets.Checked = false;
                    return;
                }
                string filename = Properties.Settings.Default.CahceDirectory + "\\data\\mpc.bin";

                if (!FileDownload.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=mpcbin", filename, true))
                {
                    this.showMinorPlanets.Checked = false;
                    return;
                }
            }

            Properties.Settings.Default.SolarSystemMinorPlanets.TargetState = this.showMinorPlanets.Checked;

        }


        Bitmap layerButton = global::TerraViewer.Properties.Resources.layersButton;
        Bitmap layerButtonHover = global::TerraViewer.Properties.Resources.layersButtonHover;

        private void layerToggle_Click(object sender, EventArgs e)
        {
            Earth3d.MainWindow.ShowLayersWindows = !Earth3d.MainWindow.ShowLayersWindows;
        }

        private void layerToggle_MouseEnter(object sender, EventArgs e)
        {
            layerToggle.Image = layerButtonHover;
        }

        private void layerToggle_MouseLeave(object sender, EventArgs e)
        {
            layerToggle.Image = layerButton;

        }

        private void View_FormClosing(object sender, FormClosingEventArgs e)
        {
            CustomButtons.Save();
        }

    }
}
