using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using TerraViewer.Properties;

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
           
            groupBox2.Text = Language.GetLocalizedText(507, "Observing Location");
            localHorizon.Text = Language.GetLocalizedText(508, "View from this location");
            ChooseLocation.Text = Language.GetLocalizedText(379, "Setup");
            locationName.Text = Language.GetLocalizedText(509, "Please choose a location");
            label2.Text = Language.GetLocalizedText(7, "Name:");
            label3.Text = Language.GetLocalizedText(365, "Lng:   ");
            label5.Text = Language.GetLocalizedText(510, "Altitude:");
            label1.Text = Language.GetLocalizedText(367, "Lat :  ");
            groupBox3.Text = Language.GetLocalizedText(511, "Observing Time");
            showUtcTime.Text = Language.GetLocalizedText(194, "UTC");
            TimeNow.Text = Language.GetLocalizedText(512, "Now");
            TimeMode.Text = Language.GetLocalizedText(513, "Real Time");

            Text = Language.GetLocalizedText(140, "View");

            showMinorPlanets.Text = Language.GetLocalizedText(653, "Asteroids");

        }

        public static void ShowFovSetup()
        {
            if (Earth3d.MainWindow.Fov == null)
            {
                Earth3d.MainWindow.Fov = new FieldOfView(Properties.Settings.Default.FovTelescope, Properties.Settings.Default.FovCamera, Properties.Settings.Default.FovEyepiece);
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
            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;

            CustomButtons.LoadButtons(Properties.Settings.Default.CahceDirectory + "buttons\\CustomView.wwtb");
        }

        void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
           
            MethodInvoker doIt = delegate
            {
                UpdateProperties();
            };

            if (InvokeRequired)
            {
                try
                {
                    Invoke(doIt);
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
            latText.Text = Coordinates.FormatDMS(Properties.Settings.Default.LocationLat);
            lngText.Text = Coordinates.FormatDMS(Properties.Settings.Default.LocationLng);
            locationName.Text = Properties.Settings.Default.LocationName;
            Altitude.Text = Properties.Settings.Default.LocationAltitude + "m";
            localHorizon.Checked = Properties.Settings.Default.LocalHorizonMode;
            
            showUtcTime.Checked = Properties.Settings.Default.ShowUTCTime;

            showMinorPlanets.Checked = Properties.Settings.Default.SolarSystemMinorPlanets.TargetState;
            ignoreChanges = false;
        }






        private void ChooseLocation_Click(object sender, EventArgs e)
        {
            var dialog = new LocationSetup();

            dialog.Sky = false;

            dialog.Latitude = Coordinates.Parse(latText.Text);
            dialog.Longitude = Coordinates.Parse(lngText.Text);
            dialog.LocationName = locationName.Text;
            dialog.Altitude = Convert.ToDouble(Altitude.Text.Replace("m","").Replace("'",""));

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {

                latText.Text = Coordinates.FormatDMS(Properties.Settings.Default.LocationLat = dialog.Latitude);
                lngText.Text = Coordinates.FormatDMS(Properties.Settings.Default.LocationLng = dialog.Longitude);
                locationName.Text = Properties.Settings.Default.LocationName = dialog.LocationName;
                Altitude.Text = (Properties.Settings.Default.LocationAltitude = dialog.Altitude) + "m";
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

        private void UpdateSpeed()
        {

            if (SpaceTimeController.TimeRate == 1.0)
            {
                TimeMode.Text = Language.GetLocalizedText(513, "Real Time");
            }
            else if (SpaceTimeController.TimeRate == -2.0)
            {
                TimeMode.Text = Language.GetLocalizedText(521, "Reverse Time");
            }
            else
            {
                TimeMode.Text = Language.GetLocalizedText(523, "X ") + SpaceTimeController.TimeRate;
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
            if (showMinorPlanets.Checked && !File.Exists(Properties.Settings.Default.CahceDirectory + "\\data\\mpc.bin"))
            {
                if (UiTools.ShowMessageBox("WorldWide Telescope needs to download the latest Minor Planet Center data file (about 12MB). Do you want to proceed?", "Minor Planet Center Data Download", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    showMinorPlanets.Checked = false;
                    return;
                }
                var filename = Properties.Settings.Default.CahceDirectory + "\\data\\mpc.bin";

                if (!FileDownload.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=mpcbin", filename, true))
                {
                    showMinorPlanets.Checked = false;
                    return;
                }
            }

            Properties.Settings.Default.SolarSystemMinorPlanets.TargetState = showMinorPlanets.Checked;

        }


        readonly Bitmap layerButton = Resources.layersButton;
        readonly Bitmap layerButtonHover = Resources.layersButtonHover;

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
