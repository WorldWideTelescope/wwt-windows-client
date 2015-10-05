using System.IO;
using System.Security.Permissions;
using System.Windows.Forms;

namespace TerraViewer
{
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class DataMessageFilter : IMessageFilter
    {
        public bool PreFilterMessage(ref Message m)
        {

            if (m.Msg == Earth3d.AlertMessage)
            {

                if (File.Exists(Properties.Settings.Default.CahceDirectory + @"\launchfile.txt"))
                {

                    Earth3d.launchTourFile = File.ReadAllText(Properties.Settings.Default.CahceDirectory + @"\launchfile.txt");

                    // This causes the welcome screen to go away if anther instance sends us data
                    Earth3d.closeWelcome = true;

                    Earth3d.MainWindow.StatupTimer.Enabled = true;
                    if (Earth3d.MainWindow.WindowState == FormWindowState.Minimized)
                    {
                        Earth3d.MainWindow.WindowState = FormWindowState.Maximized;
                    }
                    Earth3d.MainWindow.Focus();
                    return true;
                }
            }

            return false;
        }
    }
}
