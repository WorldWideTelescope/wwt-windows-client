using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TerraViewer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using D3D = SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WorldWideTelescope
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        RenderEngine engine = new RenderEngine();
        public MainPage()
        {
            this.InitializeComponent();
        
            CompositionTarget.Rendering += CompositionTarget_Rendering;

            engine.InitializeForUwp();
        }



    private void CompositionTarget_Rendering(object sender, object e)

    {
            engine.Render();

      //  this.swapChain.Present(1, DXGI.PresentFlags.None, new DXGI.PresentParameters());

    }
}
}
