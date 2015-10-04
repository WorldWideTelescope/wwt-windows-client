using System.Windows.Forms;
using System.Drawing;

namespace TerraViewer
{
    public class PropPage : UserControl
    {
        public PropPage()
        {
            BackColor = Color.FromArgb(20, 22, 31);
            ForeColor = Color.White;
            MinimumSize = MinimumSize = new Size(690, 230);
        }
        public virtual bool Save()
        {
            return true;
        }
        public WizardPropsBinding Binding;

        public virtual void SetData(object data)
        {
        }

        public double ParseAndValidateDouble(TextBox input, double defValue, ref bool failed)
        {
            var sucsess = false;
            var result = defValue;
            sucsess = double.TryParse(input.Text, out result);

            if (sucsess)
            {
                input.BackColor = UiTools.TextBackground;
            }
            else
            {
                input.BackColor = Color.Red;
                failed = true;
            }

            return result;
        }

        public double ParseAndValidateCoordinate(TextBox input, double defValue, ref bool failed)
        {
            var sucsess = false;
            var result = defValue;
            sucsess = Coordinates.Validate(input.Text);

            

            if (sucsess)
            {
                result = Coordinates.Parse(input.Text);
                input.BackColor = UiTools.TextBackground;
            }
            else
            {
                input.BackColor = Color.Red;
                failed = true;
            }

            return result;
        }       
    }
}
