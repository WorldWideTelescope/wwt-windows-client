using System;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class Object3dProperties : Form
    {
        public Object3dProperties()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            PitchLabel.Text = Language.GetLocalizedText(779, "Pitch");
            HeadingLabel.Text = Language.GetLocalizedText(780, "Heading");
            RollLabel.Text = Language.GetLocalizedText(781, "Roll");
            TranslationYLabel.Text = Language.GetLocalizedText(872, "Translation Y");
            TranslationZLabel.Text = Language.GetLocalizedText(873, "Translation Z");
            ScaleYLabel.Text = Language.GetLocalizedText(874, "Scale Y");
            TranslationXLabel.Text = Language.GetLocalizedText(875, "Translation X");
            ScaleZLabel.Text = Language.GetLocalizedText(876, "Scale Z");
            ScaleXLabel.Text = Language.GetLocalizedText(877, "Scale X");
            Smooth.Text = Language.GetLocalizedText(878, "Smooth Normals");
            FlipV.Text = Language.GetLocalizedText(879, "Flip Textures");
            UniformScaling.Text = Language.GetLocalizedText(880, "Uniform Scaling");
            Close.Text = Language.GetLocalizedText(212, "Close");
            Text = Language.GetLocalizedText(881, "3d Model Properties");
            TwoSidedGeometry.Text = Language.GetLocalizedText(1149, "Two-sided");
            rightHanded.Text = Language.GetLocalizedText(1315, "Right Handed");
        }

        public Object3dLayer layer;
        bool ignoreChanges = true;
        private void Object3dProperties_Load(object sender, EventArgs e)
        {
            Earth3d.MainWindow.UiController = layer.GetEditUI();

            layer.PropertiesChanged += layer_PropertiesChanged;

            SetParamaters();

            if (layer is ISSLayer)
            {
                rightHanded.Visible = false;
                Smooth.Visible = false;
                TwoSidedGeometry.Visible = false;
                FlipV.Visible = false;
            }

        }

        private void SetParamaters()
        {
            ignoreChanges = true;
            ScaleX.Text = layer.Scale.X.ToString();
            ScaleY.Text = layer.Scale.Y.ToString();
            ScaleZ.Text = layer.Scale.Z.ToString();
            TranslationX.Text = layer.Translate.X.ToString();
            TranslationY.Text = layer.Translate.Y.ToString();
            TranslationZ.Text = layer.Translate.Z.ToString();

            Heading.Text = layer.Heading.ToString();
            Pitch.Text = layer.Pitch.ToString();
            Roll.Text = layer.Roll.ToString();
            FlipV.Checked = layer.FlipV;
            rightHanded.Checked = layer.FlipHandedness;

            Smooth.Checked = layer.Smooth;
            TwoSidedGeometry.Checked = layer.TwoSidedGeometry;
            UniformScaling.Checked = (ScaleZ.Text == ScaleY.Text && ScaleY.Text == ScaleX.Text);
            ignoreChanges = false;
        }

        void layer_PropertiesChanged(object sender, EventArgs e)
        {
            SetParamaters();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            SyncChanges();
            Close();
        }

        private void SyncChanges()
        {
            try
            {
                layer.Scale = Vector3d.Parse(ScaleX.Text + "," + ScaleY.Text + "," + ScaleZ.Text);
                layer.Translate = Vector3d.Parse(TranslationX.Text + "," + TranslationY.Text + "," + TranslationZ.Text);
                layer.Heading = double.Parse(Heading.Text);
                layer.Pitch = double.Parse(Pitch.Text);
                layer.Roll = double.Parse(Roll.Text);
                layer.FlipV = FlipV.Checked;
                layer.FlipHandedness = rightHanded.Checked;
                layer.Smooth = Smooth.Checked;

            }
            catch
            {
            }
        }

        private void TextValueChanged(object sender, EventArgs e)
        {
            if (!ignoreChanges)
            {
                SyncChanges();
            }
        }

        private void FlipV_CheckedChanged(object sender, EventArgs e)
        {
            if (!ignoreChanges)
            {
                layer.FlipV = FlipV.Checked;
            }
        }

        private void Smooth_CheckedChanged(object sender, EventArgs e)
        {
            if (!ignoreChanges)
            {
                layer.Smooth = Smooth.Checked;
            }
        }

        private void TwoSidedGeometry_CheckedChanged(object sender, EventArgs e)
        {
            if (!ignoreChanges)
            {
                layer.TwoSidedGeometry = TwoSidedGeometry.Checked;
            }
        }

        private void ScaleX_TextChanged(object sender, EventArgs e)
        {
            TextValueChanged(sender, e);

            if (UniformScaling.Checked)
            {
                ScaleY.Text = layer.Scale.X.ToString();
                ScaleZ.Text = layer.Scale.X.ToString();
            }
        }

        private void UniformScaling_CheckedChanged(object sender, EventArgs e)
        {
            ScaleY.Enabled = ScaleZ.Enabled = !UniformScaling.Checked;

            if (UniformScaling.Checked)
            {
                ScaleY.Text = ScaleZ.Text = ScaleX.Text;
            }
        }

        private void rightHanded_CheckedChanged(object sender, EventArgs e)
        {
            if (!ignoreChanges)
            {
                layer.FlipHandedness = rightHanded.Checked;
            }
        }

        private void Object3dProperties_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Earth3d.MainWindow.UiController == layer.GetEditUI())
            {
                Earth3d.MainWindow.UiController = null;
            }

            layer.PropertiesChanged += layer_PropertiesChanged;
        }

    }
}
