using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using Properties = TerraViewer.Properties;
//using Settings = UwpRenderEngine.Settings;

namespace WorldWideTelescope
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        RenderEngine RenderEngine = null;
        public MainPage()
        {
            this.InitializeComponent();

        }

        double Width = 1;
        double Height = 1;


        private void SwapChainPanel_Loaded(object sender, RoutedEventArgs e)
        {
            float pixelScale = Windows.Graphics.Display.DisplayInformation.GetForCurrentView().LogicalDpi / 96.0f;
            Width = (SwapChainPanel.RenderSize.Width * pixelScale);

            Height = (SwapChainPanel.RenderSize.Height * pixelScale);
            int width = (int)(Width);
            int height = (int)(Height);

            var t = Task.Run(() =>
            {
                RenderEngine = new RenderEngine();
                RenderEngine.InitializeForUwp(null, new SharpDX.WIC.ImagingFactory2(), width, height);

                // Obtain a reference to the native COM object of the SwapChainPanel.
                
            });
            CompositionTarget.Rendering += CompositionTarget_Rendering;

            this.SwapChainPanel.PointerPressed += SwapChainPanel_PointerPressed;
            this.SwapChainPanel.PointerReleased += SwapChainPanel_PointerReleased;
            this.SwapChainPanel.PointerMoved += SwapChainPanel_PointerMoved;
            this.SwapChainPanel.PointerWheelChanged += SwapChainPanel_PointerWheelChanged;
            this.KeyDown += SwapChainPanel_KeyDown;
            this.Focus(FocusState.Keyboard);
        }

        private void SwapChainPanel_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (RenderEngine != null)
            {
                if (e.Key == Windows.System.VirtualKey.S)
                {
                    RenderEngine.currentImageSetfield = RenderEngine.GetDefaultImageset(ImageSetType.Sky, BandPass.Visible);
                }
                if (e.Key == Windows.System.VirtualKey.E)
                {
                    RenderEngine.currentImageSetfield = RenderEngine.GetDefaultImageset(ImageSetType.Earth, BandPass.Visible);
                }
            }
        }

        private void SwapChainPanel_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            int delta = e.GetCurrentPoint(SwapChainPanel).Properties.MouseWheelDelta;

            if (delta != 0)
            {
                //if (Properties.Settings.Default.FollowMouseOnZoom && !RenderEngine.PlanetLike)
                //{
                //    Coordinates point = GetCoordinatesForScreenPoint(e.X, e.Y);
                //    if (RenderEngine.Space && Settings.Active.LocalHorizonMode && !RenderEngine.tracking)
                //    {
                //        Coordinates currentAltAz = Coordinates.EquitorialToHorizon(point, SpaceTimeController.Location, SpaceTimeController.Now);

                //        RenderEngine.targetAlt = currentAltAz.Alt;
                //        RenderEngine.targetAz = currentAltAz.Az;

                //    }
                //    else
                //    {
                //        RenderEngine.TargetLong = RAtoViewLng(point.RA);
                //        RenderEngine.TargetLat = point.Lat;
                //    }
                //}
                RenderEngine.ZoomSpeed = (ZoomSpeeds)Properties.Settings.Default.ZoomSpeed;

                if (Math.Abs(delta) == 120)
                {
                    if (delta < 0)
                    {
                        ZoomOut();
                    }
                    else
                    {
                        ZoomIn();
                    }
                }
                else
                {
                    if (delta < 0)
                    {
                        ZoomOut((double)Math.Abs(delta) / 120.0);
                    }
                    else
                    {
                        ZoomIn((double)Math.Abs(delta) / 120.0);
                    }
                }

            }
        }
        bool smoothZoom = true;
        public double NetZoomFactor
        {
            get
            {
                double net = 1;
                switch (RenderEngine.ZoomSpeed)
                {
                    case ZoomSpeeds.SLOW:
                        net = .1;
                        break;
                    case ZoomSpeeds.MEDIUM:
                        net = .25;
                        break;
                    case ZoomSpeeds.FAST:
                        net = 1;
                        break;
                    default:
                        break;
                }

                var shift = Window.Current.CoreWindow.GetKeyState(Windows.System.VirtualKey.Shift);


                if (shift.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down))
                {
                    net /= 5;
                }

                return 1 + net;
            }

        }

        public double GetNetzoom(double amount)
        {
            double net = 1;
            switch (RenderEngine.ZoomSpeed)
            {
                case ZoomSpeeds.SLOW:
                    net = .1;
                    break;
                case ZoomSpeeds.MEDIUM:
                    net = .25;
                    break;
                case ZoomSpeeds.FAST:
                    net = 1;
                    break;
                default:
                    break;
            }
            var shift = Window.Current.CoreWindow.GetKeyState(Windows.System.VirtualKey.Shift);


            if (shift.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down))
            { 
                net /= 5;
            }

            return 1 + (net * amount / 4);
        }

        public void ZoomIn(double amount)
        {
            if (RenderEngine.TargetZoom > RenderEngine.ZoomFactor)
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomFactor;
                return;
            }

            if (RenderEngine.TargetZoom > RenderEngine.ZoomMin)
            {
                RenderEngine.TargetZoom /= 1 + GetNetzoom(amount);

                if (!smoothZoom)
                {
                    RenderEngine.ZoomFactor = RenderEngine.TargetZoom;
                }
                RenderEngine.ComputeViewParameters(CurrentImageSet);
            }
            else
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomMin;
            }

        }

        public void ZoomOut(double amount)
        {
            if (RenderEngine.TargetZoom < RenderEngine.ZoomFactor)
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomFactor;
                return;
            }

            if ((RenderEngine.TargetZoom * GetNetzoom(amount)) <= RenderEngine.ZoomMax)
            {
                RenderEngine.TargetZoom *= GetNetzoom(amount);
                if (!smoothZoom)
                {
                    RenderEngine.ZoomFactor = RenderEngine.TargetZoom;
                }
                RenderEngine.ComputeViewParameters(CurrentImageSet);
            }
            else
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomMax;
            }
        }


        public void ZoomIn()
        {
            if (RenderEngine.TargetZoom > RenderEngine.ZoomFactor)
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomFactor;
                return;
            }

            if (RenderEngine.TargetZoom > RenderEngine.ZoomMin)
            {
                RenderEngine.TargetZoom /= NetZoomFactor;

                if (!smoothZoom)
                {
                    RenderEngine.ZoomFactor = RenderEngine.TargetZoom;
                }
                RenderEngine.ComputeViewParameters(CurrentImageSet);
            }
            else
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomMin;
            }

        }

        public void ZoomOut()
        {
            if (RenderEngine.TargetZoom < RenderEngine.ZoomFactor)
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomFactor;
                return;
            }

            if ((RenderEngine.TargetZoom * NetZoomFactor) <= RenderEngine.ZoomMax)
            {
                RenderEngine.TargetZoom *= NetZoomFactor;
                if (!smoothZoom)
                {
                    RenderEngine.ZoomFactor = RenderEngine.TargetZoom;
                }
                RenderEngine.ComputeViewParameters(CurrentImageSet);
            }
            else
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomMax;
            }
        }


        bool dragging = false;
        double mouseDownX = 0;
        double mouseDownY = 0;
        private void SwapChainPanel_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (dragging)
            {
                var point = e.GetCurrentPoint(SwapChainPanel);
                var pos = point.Position;


                MoveView(-(pos.X - this.mouseDownX), (pos.Y - this.mouseDownY), true);
                if (!TerraViewer.Properties.Settings.Default.SmoothPan)
                {
                    RenderEngine.ViewLat = RenderEngine.TargetLat;
                    RenderEngine.ViewLong = RenderEngine.TargetLong;
                    //NotifyMoveComplete();
                }
                mouseDownX = pos.X;
                mouseDownY = pos.Y;
            }
        }
        private void SwapChainPanel_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
                dragging = false;
        }

        private void SwapChainPanel_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            dragging = true;
            var point = e.GetCurrentPoint(SwapChainPanel);
            var pos = point.Position;
            mouseDownX = pos.X;
            mouseDownY = pos.Y;
        }

        const double RC = (double)(3.1415927 / 180);
        public bool SandboxMode
        {
            get
            {
                if (CurrentImageSet == null)
                {
                    return false;
                }

                return CurrentImageSet.DataSetType == ImageSetType.Sandbox;
            }
        }

        public bool SolarSystemMode
        {
            get
            {
                if (CurrentImageSet == null)
                {
                    return false;
                }

                return CurrentImageSet.DataSetType == ImageSetType.SolarSystem;
            }
        }

        public IImageSet CurrentImageSet
        {
            get { return RenderEngine.currentImageSetfield; }
            //set
            //{
            //    if (RenderEngine.currentImageSetfield != value)
            //    {
            //        bool solarSytemOld = (RenderEngine.currentImageSetfield != null && RenderEngine.currentImageSetfield.DataSetType == ImageSetType.SolarSystem);
            //        RenderEngine.currentImageSetfield = value;

            //        if (RenderEngine.currentImageSetfield.DataSetType == ImageSetType.SolarSystem && !solarSytemOld)
            //        {
            //            if (contextPanel != null)
            //            {
            //                contextPanel.Constellation = "Error";
            //            }
            //        }
            //        if (ImageSetChanged != null)
            //        {
            //            ImageSetChanged.Invoke(this, new EventArgs());
            //            if (imageStackVisible)
            //            {
            //                stack.UpdateList();
            //            }
            //        }
            //        if (value != null)
            //        {
            //            int hash = value.GetHash();
            //            RenderEngine.AddImageSetToTable(hash, value);
            //        }
            //    }
            //}
        }

        public double GetPixelScaleX(bool mouseRelative)
        {
            double lat = RenderEngine.ViewLat;

            //if (mouseRelative)
            //{
            //    if (RenderEngine.Space && Settings.Active.GalacticMode)
            //    {
            //        Point cursor = renderWindow.PointToClient(Cursor.Position);
            //        Coordinates result = GetCoordinatesForScreenPoint(cursor.X, cursor.Y);

            //        double[] gPoint = Coordinates.J2000toGalactic(result.RA * 15, result.Dec);

            //        lat = gPoint[1];
            //    }
            //    else if (RenderEngine.Space && Settings.Active.LocalHorizonMode)
            //    {
            //        Point cursor = renderWindow.PointToClient(Cursor.Position);
            //        Coordinates currentAltAz = Coordinates.EquitorialToHorizon(GetCoordinatesForScreenPoint(cursor.X, cursor.Y), SpaceTimeController.Location, SpaceTimeController.Now);

            //        lat = currentAltAz.Alt;
            //    }
            //    else
            //    {
            //        Point cursor = renderWindow.PointToClient(Cursor.Position);
            //        Coordinates result = GetCoordinatesForScreenPoint(cursor.X, cursor.Y);
            //        lat = result.Lat;
            //    }
            //}

            if (RenderEngine.currentImageSetfield != null && (RenderEngine.currentImageSetfield.DataSetType == ImageSetType.Sky || RenderEngine.currentImageSetfield.DataSetType == ImageSetType.Panorama || SandboxMode || SolarSystemMode || RenderEngine.currentImageSetfield.DataSetType == ImageSetType.Earth || RenderEngine.currentImageSetfield.DataSetType == ImageSetType.Planet))
            {
                double cosLat = 1;
                if (RenderEngine.ViewLat > 89.9999)
                {
                    cosLat = Math.Cos(89.9999 * RC);
                }
                else
                {
                    cosLat = Math.Cos(lat * RC);

                }

                double zz = (90 - RenderEngine.ZoomFactor / 6);
                double zcos = Math.Cos(zz * RC);

                return GetPixelScaleY() / Math.Max(zcos, cosLat);
            }
            else
            {
                return (((RenderEngine.baseTileDegrees / ((double)Math.Pow(2, RenderEngine.viewTileLevel))) / RenderEngine.tileSizeX) / 5) / Math.Max(.2, Math.Cos(RenderEngine.TargetLat));
            }

        }

        public double GetPixelScaleY()
        {
            if (SolarSystemMode)
            {
                if ((int)RenderEngine.SolarSystemTrack < (int)SolarSystemObjects.Custom)
                {
                    return Math.Min(.06, 545000 * Math.Tan(Math.PI / 4) * RenderEngine.ZoomFactor / Height);
                }
                else
                {

                    return .06;
                }
            }
            else if (RenderEngine.currentImageSetfield != null && (RenderEngine.currentImageSetfield.DataSetType == ImageSetType.Sky || RenderEngine.currentImageSetfield.DataSetType == ImageSetType.Panorama))
            {
                double val = RenderEngine.FovAngle / Height;

                return val;
            }
            else if (SandboxMode)
            {
                return .06;
            }
            else
            {
                return ((RenderEngine.baseTileDegrees / ((double)Math.Pow(2, RenderEngine.viewTileLevel))) / (double)RenderEngine.tileSizeY) / 5;
            }
        }

        public void MoveView(double amountX, double amountY, bool mouseDrag)
        {
            if (RenderEngine.currentImageSetfield == null)
            {
                return;
            }
            RenderEngine.Tracking = false;
            double angle = Math.Atan2(amountY, amountX);
            double distance = Math.Sqrt(amountY * amountY + amountX * amountX);
            if (SolarSystemMode)
            {
                amountX = Math.Cos(angle - RenderEngine.CameraRotate) * distance;
                amountY = Math.Sin(angle - RenderEngine.CameraRotate) * distance;
            }
            else if (!RenderEngine.PlanetLike)
            {
                amountX = Math.Cos(angle + RenderEngine.CameraRotate) * distance;
                amountY = Math.Sin(angle + RenderEngine.CameraRotate) * distance;
            }
            else
            {
                amountX = Math.Cos(angle - RenderEngine.CameraRotate) * distance;
                amountY = Math.Sin(angle - RenderEngine.CameraRotate) * distance;
            }

            MoveViewNative(amountX, amountY, mouseDrag);
        }



        public void MoveViewNative(double amountX, double amountY, bool mouseDrag)
        {
            double scaleY = GetPixelScaleY();
            double scaleX = GetPixelScaleX(mouseDrag);


            if (RenderEngine.currentImageSetfield.DataSetType == ImageSetType.SolarSystem || SandboxMode)
            {
                if (scaleY > .05999)
                {
                    scaleX = scaleY;
                }
            }

            if (RenderEngine.Space && Settings.Active.GalacticMode)
            {
                amountX = -amountX;
            }

            if (RenderEngine.Space && (Settings.Active.LocalHorizonMode || Settings.Active.GalacticMode))
            {
                RenderEngine.targetAlt += (amountY) * scaleY;
                if (RenderEngine.targetAlt > Properties.Settings.Default.MaxLatLimit)
                {
                    RenderEngine.targetAlt = Properties.Settings.Default.MaxLatLimit;
                }
                if (RenderEngine.targetAlt < -Properties.Settings.Default.MaxLatLimit)
                {
                    RenderEngine.targetAlt = -Properties.Settings.Default.MaxLatLimit;
                }

            }
            else
            {
                RenderEngine.TargetLat += (amountY) * scaleY;

                if (RenderEngine.TargetLat > Properties.Settings.Default.MaxLatLimit)
                {
                    RenderEngine.TargetLat = Properties.Settings.Default.MaxLatLimit;
                }
                if (RenderEngine.TargetLat < -Properties.Settings.Default.MaxLatLimit)
                {
                    RenderEngine.TargetLat = -Properties.Settings.Default.MaxLatLimit;
                }
            }
            if (RenderEngine.Space && (Settings.Active.LocalHorizonMode || Settings.Active.GalacticMode))
            {
                RenderEngine.targetAz = ((RenderEngine.targetAz + amountX * scaleX) + 720) % 360;
            }
            else
            {
                RenderEngine.TargetLong += (amountX) * scaleX;

                RenderEngine.TargetLong = ((RenderEngine.TargetLong + 900.0) % 360.0) - 180.0;
            }
        }


        bool firstTime = true;

        private void CompositionTarget_Rendering(object sender, object e)
        {
            if (RenderEngine != null && RenderEngine.ReadyToRender)
            {
                if (firstTime)
                {
                    using (ISwapChainPanelNative nativeObject = SharpDX.ComObject.As<ISwapChainPanelNative>(this.SwapChainPanel))
                    {
                        // Set its swap chain.
                        nativeObject.SwapChain = RenderEngine.RenderContext11.swapChain;
                    }
                    firstTime = false;
                }

                RenderEngine.Render();

                RenderEngine.RenderContext11.swapChain.Present(1, DXGI.PresentFlags.None, new DXGI.PresentParameters());
            }
        }
    }
}
