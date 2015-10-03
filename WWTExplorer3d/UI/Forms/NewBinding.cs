using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MIDI;

namespace TerraViewer
{
    public partial class NewBinding : Form
    {
        public NewBinding()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            this.StatusText.Text = Language.GetLocalizedText(1173, "Listening for unmapped controls: Activate Control now.");
            this.Ok.Text = Language.GetLocalizedText(156, "OK");
            this.controlTypeLabel.Text = Language.GetLocalizedText(1174, "Control Type");
            this.Cancel.Text = Language.GetLocalizedText(157, "Cancel");  
            this.Text = Language.GetLocalizedText(1175, "New Control");
        }

        public MidiMap MidiMap = null;
        public ControlMap ControlMap = null;

        private void NewBinding_Load(object sender, EventArgs e)
        {
            if (MidiMap != null)
            {
                MidiMap.UnhandledMessageReceived += new MIDI.MidiMessageReceived(Owner_MessageReceived);
            }
            ControlTypeCombo.Items.Clear();
            ControlTypeCombo.Items.AddRange(Enum.GetNames(typeof(ControlType)));
            ControlTypeCombo.SelectedIndex = 0;

        }
        bool firstMessageReceived;
        MidiMessage firstMessage = MidiMessage.None;
        int firstValue;
        void Owner_MessageReceived(object sender, MidiMessage message, int channel, int key, int value)
        {
            if (ControlMap != null)
            {
                if (message == MidiMessage.AfterTouch || message == MidiMessage.None)
                {
                    return;
                }
                if (firstMessageReceived)
                {

                    if (ControlMap.ID != key || ControlMap.Channel != channel)
                    {
                        return;
                    }

                    if ( (firstMessage == MidiMessage.NoteOn && message == MidiMessage.NoteOff) || (firstMessage == MidiMessage.NoteOff && message == MidiMessage.NoteOn))
                    {
                        ControlMap.ControlType = ControlType.KeyPress;
                    }
                    else if (firstMessage == MidiMessage.Control && firstValue == 127 && message == MidiMessage.Control && value == 0)
                    {
                        ControlMap.ControlType = ControlType.KeyPress;
                    }
                    else if ((firstMessage == MidiMessage.Control && firstValue == 127 && message == MidiMessage.Control && value == 1)
                            || (firstMessage == MidiMessage.Control && firstValue == 1 && message == MidiMessage.Control && value == 127)
                        || (firstMessage == MidiMessage.Control && firstValue == 1 && message == MidiMessage.Control && value == 1)
                        || (firstMessage == MidiMessage.Control && firstValue == 127 && message == MidiMessage.Control && value == 127)
                        )
                    {

                        ControlMap.ControlType = ControlType.Jog;
                    }
                    else if (message == MidiMessage.Control && ((firstValue < 2 && value < 2)  || (firstValue == 127 && value == 126) || (firstValue > 0 && firstValue < 127 && value > 1 && value < 127)))
                    {
                        ControlMap.ControlType = ControlType.Slider;
                    }

                }
                else
                {
                    firstMessageReceived = true;
                    firstMessage = message;
                    firstValue = value;
                    ControlMap.ID = key;
                    ControlMap.Channel = channel;
                    ControlMap.Name = key.ToString();
                    return;
                }
            }

            MethodInvoker doIt = delegate
            {
                MidiMap.UnhandledMessageReceived -= new MIDI.MidiMessageReceived(Owner_MessageReceived);
                EnableControlUi();
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

        private void EnableControlUi()
        {
            ControlTypeCombo.Visible = true;
            controlTypeLabel.Visible = true;
            Ok.Visible = true;
            ControlTypeCombo.SelectedIndex = (int)ControlMap.ControlType;

            StatusText.Text = string.Format("Select Control Type for Chan {0}, ID {1}", ControlMap.Channel, ControlMap.ID);
        }

        private void NewBinding_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (MidiMap != null)
            {
                MidiMap.UnhandledMessageReceived -= new MIDI.MidiMessageReceived(Owner_MessageReceived);
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
           
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            if (ControlMap != null)
            {

                ControlMap.ControlType = (ControlType)ControlTypeCombo.SelectedIndex;
                switch (ControlMap.ControlType)
                {
                    case ControlType.KeyPress:
                        ControlMap.BindingA.HadnlerType = HandlerType.KeyPress;
                        ControlMap.BindingB.HadnlerType = HandlerType.None;
                        break;
                    case ControlType.KeyUpDown:
                        ControlMap.BindingA.HadnlerType = HandlerType.KeyUp;
                        ControlMap.BindingB.HadnlerType = HandlerType.KeyDown;
                        break;
                    case ControlType.Slider:
                    case ControlType.Knob:
                        ControlMap.BindingA.HadnlerType = HandlerType.ValueChange;
                        ControlMap.BindingA.BindingType = BindingType.SyncValue;
                        ControlMap.BindingB.HadnlerType = HandlerType.None;
                        break;

                    case ControlType.Jog:
                        ControlMap.BindingA.HadnlerType = HandlerType.ClockWise;
                        ControlMap.BindingB.HadnlerType = HandlerType.CounterClockwise;
                        break;
                    default:
                        break;
                }
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

    }
}
