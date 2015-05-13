using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class ButtonProperties : Form
    {
        public ControlMap ButtonMap = new ControlMap();
         
        public ButtonProperties()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.Cancel.Text = Language.GetLocalizedText(157, "Cancel");
            this.OK.Text = Language.GetLocalizedText(156, "OK");
            this.txtName.Text = Language.GetLocalizedText(255, "<Type name here>");
            this.label1.Text = Language.GetLocalizedText(238, "Name");
            this.Property.Text = Language.GetLocalizedText(1166, "Property");
            this.BindTypeLabel.Text = Language.GetLocalizedText(1165, "Bind Type");
            this.label4.Text = Language.GetLocalizedText(1164, "Binding Target Type");
            this.buttonTypeLabel.Text = Language.GetLocalizedText(1316, "Button Type");
            this.Text = Language.GetLocalizedText(1317, "Button Properties");
        }

        private void TargetTypeCombo_SelectionChanged(object sender, EventArgs e)
        {
            UpdatePropertyCombo();
        }

        private void UpdatePropertyCombo()
        {
            BindingTargetType tt = (BindingTargetType)TargetTypeCombo.SelectedIndex;

            TargetPropertyCombo.Items.Clear();
            TargetPropertyCombo.ClearText();

            filterLabel.Visible = false;
            filterList.Visible = false;

            ControlBinding binding = ButtonMap.BindingA;

            binding.TargetType = tt;

            IScriptable scriptInterface = null;
            bool comboVisible = true;
            switch (tt)
            {
                case BindingTargetType.Setting:
                    scriptInterface = Settings.Active as IScriptable;
                    break;
                case BindingTargetType.SpaceTimeController:
                    scriptInterface = SpaceTimeController.ScriptInterface;
                    break;
                case BindingTargetType.Goto:
                    comboVisible = false;
                    break;

                case BindingTargetType.Layer:
                    scriptInterface = LayerManager.ScriptInterface;
                    break;
                case BindingTargetType.Navigation:
                    scriptInterface = Earth3d.MainWindow as IScriptable;
                    break;
                //case BindingTargetType.Actions:
                //    break;
                //case BindingTargetType.Key:
                //    break;
                //case BindingTargetType.Mouse:
                //    break;
                default:
                    break;
            }

            if (comboVisible)
            {
                TargetPropertyCombo.Visible = true;
                PropertyNameText.Visible = false;
                if (scriptInterface != null)
                {
                    switch (binding.BindingType)
                    {
                        case BindingType.Action:
                            TargetPropertyCombo.Items.Clear();
                            TargetPropertyCombo.Items.AddRange(scriptInterface.GetActions());
                            break;
                        case BindingType.Toggle:
                            TargetPropertyCombo.Items.Clear();
                            TargetPropertyCombo.Items.AddRange(UiTools.GetFilteredProperties(scriptInterface.GetProperties(), binding.BindingType));
                            break;
                        case BindingType.SyncValue:
                            TargetPropertyCombo.Items.Clear();
                            TargetPropertyCombo.Items.AddRange(scriptInterface.GetProperties());
                            break;
                        case BindingType.SetValue:
                            TargetPropertyCombo.Items.Clear();
                            TargetPropertyCombo.Items.AddRange(scriptInterface.GetProperties());
                            break;
                        default:
                            break;
                    }
                }
                TargetPropertyCombo.SelectedItem = binding.PropertyName;
            }
            else
            {
                PropertyNameText.Visible = true;
                TargetPropertyCombo.Visible = false;
                PropertyNameText.Text = binding.PropertyName;
            }

            BindTypeLabel.Visible = comboVisible;
            BindTypeCombo.Visible = comboVisible;
        }

        private void BindTypeCombo_SelectionChanged(object sender, EventArgs e)
        {
            ButtonMap.BindingA.BindingType = (BindingType)BindTypeCombo.SelectedIndex;
            UpdatePropertyCombo();
        }

        private void TargetPropertyCombo_SelectionChanged(object sender, EventArgs e)
        {
            ScriptableProperty prop = TargetPropertyCombo.SelectedItem as ScriptableProperty;


            filterLabel.Visible = false;
            filterList.Visible = false;

            if (prop != null)
            {
                ButtonMap.BindingA.PropertyName = prop.Name;
                ButtonMap.BindingA.Max = prop.Max;
                ButtonMap.BindingA.Min = prop.Min;
                ButtonMap.BindingA.Integer = prop.Type == ScriptablePropertyTypes.Integer;

                if (ButtonMap.BindingA.BindingType == BindingType.SetValue && prop.Type == ScriptablePropertyTypes.ConstellationFilter)
                {
                    filterLabel.Visible = true;
                    filterList.Visible = true;
                    filterList.Items.Clear();
                    int index = 0;
                    int selectedIndex = 0;
                    foreach (string name in ConstellationFilter.Families.Keys)
                    {
                        filterList.Items.Add(name);
                        if (name == ButtonMap.BindingA.Value)
                        {
                            selectedIndex = index;
                        }
                        index++;
                    }
                    filterList.SelectedIndex = selectedIndex;
                }
            }
            else
            {
                if (TargetPropertyCombo.SelectedItem is string)
                {
                    ButtonMap.BindingA.PropertyName = TargetPropertyCombo.SelectedItem.ToString();
                }
            }


        }

        private void buttonTypeCombo_SelectionChanged(object sender, EventArgs e)
        {
            ButtonMap.ButtonType = (ButtonType)buttonTypeCombo.SelectedIndex;
        }

        private void PropertyNameText_TextChanged(object sender, EventArgs e)
        {
            ButtonMap.BindingA.PropertyName = PropertyNameText.Text;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            ButtonMap.Name = txtName.Text;
        }

        private void ButtonProperties_Load(object sender, EventArgs e)
        {
            txtName.Text = ButtonMap.Name;
            SetupBidingCombos();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            ButtonMap.BindingB.HadnlerType = HandlerType.None;
            ButtonMap.BindingA.HadnlerType = HandlerType.KeyDown;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void SetupBidingCombos()
        {
            TargetTypeCombo.Items.Clear();
            TargetTypeCombo.Items.AddRange(UiTools.GetBindingTargetTypeList());
            TargetTypeCombo.SelectedIndex = (int)ButtonMap.BindingA.TargetType;

            BindTypeCombo.Items.Clear();
            BindTypeCombo.Items.AddRange(Enum.GetNames(typeof(BindingType)));
            BindTypeCombo.SelectedIndex = (int)ButtonMap.BindingA.BindingType;

            buttonTypeCombo.Items.Clear();
            buttonTypeCombo.Items.AddRange(Enum.GetNames(typeof(ButtonType)));
            buttonTypeCombo.SelectedIndex = (int)ButtonMap.ButtonType;

            UpdatePropertyCombo();
        }

        private void filterList_SelectionChanged(object sender, EventArgs e)
        {
            ControlBinding binding = (ControlBinding)ButtonMap.BindingA;

            ScriptableProperty prop = TargetPropertyCombo.SelectedItem as ScriptableProperty;

            if (prop != null)
            {
                if (binding.BindingType == BindingType.SetValue && prop.Type == ScriptablePropertyTypes.ConstellationFilter)
                {
                    binding.Value = filterList.SelectedItem.ToString();
                }
            }
        }
    }
}
