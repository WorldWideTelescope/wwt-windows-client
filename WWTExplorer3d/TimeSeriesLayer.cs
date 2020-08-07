using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Xml;

using Vector3 = SharpDX.Vector3;
using Matrix = SharpDX.Matrix;
namespace TerraViewer
{
    public class TimeSeriesLayer : Layer , ITimeSeriesDescription
    {
        protected TimeSeriesPointSpriteSet shapeFileVertex;
        protected IndexBuffer11 shapeFileIndex;
        protected bool isLongIndex = false;
        protected int shapeVertexCount;
        protected bool lines = false;
        protected int latColumn = -1;
        protected float fixedSize = 1;
        protected float decay = 16;
        protected bool timeSeries = false;
   
        private bool dynamicData = false;

        [LayerProperty]
        public bool DynamicData
        {
            get { return dynamicData; }
            set { dynamicData = value; }
        }

        private bool autoUpdate = false;

        [LayerProperty]
        public bool AutoUpdate
        {
            get { return autoUpdate; }
            set { autoUpdate = value; }
        }

        string dataSourceUrl = "";
        [LayerProperty]
        public string DataSourceUrl
        {
            get { return dataSourceUrl; }
            set { dataSourceUrl = value; }
        }

  
  

        [LayerProperty]
        public bool TimeSeries
        {
            get { return timeSeries; }
            set
            {
                if (timeSeries != value)
                {
                    version++;
                    timeSeries = value;
                }
            }
        }

        [LayerProperty]
        public virtual string[] Header
        {
            get
            {
                return null;
            }
        }

        DateTime beginRange = DateTime.MaxValue;

        [LayerProperty]
        public DateTime BeginRange
        {
            get { return beginRange; }
            set
            {
                if (beginRange != value)
                {
                    version++;
                    beginRange = value;
                }
            }
        }
        DateTime endRange = DateTime.MinValue;

        [LayerProperty]
        public DateTime EndRange
        {
            get { return endRange; }
            set
            {
                if (endRange != value)
                {
                    version++;
                    endRange = value;
                }
            }
        }

        public void MakeColorDomainValues()
        {
            string[] domainValues = GetDomainValues(ColorMapColumn);
            ColorDomainValues.Clear();
            int index = 0;
            foreach (string text in domainValues)
            {
                ColorDomainValues.Add(text, new DomainValue(text, UiTools.KnownColors[(index++) % 173].ToArgb()));
            }
        }

        public override void WriteLayerProperties(System.Xml.XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("TimeSeries", TimeSeries.ToString());
            xmlWriter.WriteAttributeString("BeginRange", BeginRange.ToString());
            xmlWriter.WriteAttributeString("EndRange", EndRange.ToString());
            xmlWriter.WriteAttributeString("Decay", Decay.ToString());
            xmlWriter.WriteAttributeString("CoordinatesType", CoordinatesType.ToString());
            xmlWriter.WriteAttributeString("LatColumn", LatColumn.ToString());
            xmlWriter.WriteAttributeString("LngColumn", LngColumn.ToString());
            xmlWriter.WriteAttributeString("GeometryColumn", GeometryColumn.ToString());
            xmlWriter.WriteAttributeString("AltType", AltType.ToString());
            xmlWriter.WriteAttributeString("MarkerMix", MarkerMix.ToString());
            xmlWriter.WriteAttributeString("ColorMap", ColorMap.ToString());
            xmlWriter.WriteAttributeString("MarkerColumn", MarkerColumn.ToString());
            xmlWriter.WriteAttributeString("ColorMapColumn", ColorMapColumn.ToString());
            xmlWriter.WriteAttributeString("PlotType", PlotType.ToString());
            xmlWriter.WriteAttributeString("MarkerIndex", MarkerIndex.ToString());
            xmlWriter.WriteAttributeString("MarkerScale", MarkerScale.ToString());
            xmlWriter.WriteAttributeString("AltUnit", AltUnit.ToString());
            xmlWriter.WriteAttributeString("AltColumn", AltColumn.ToString());
            xmlWriter.WriteAttributeString("StartDateColumn", StartDateColumn.ToString());
            xmlWriter.WriteAttributeString("EndDateColumn", EndDateColumn.ToString());
            xmlWriter.WriteAttributeString("SizeColumn", SizeColumn.ToString());
            xmlWriter.WriteAttributeString("HyperlinkFormat", HyperlinkFormat.ToString());
            xmlWriter.WriteAttributeString("HyperlinkColumn", HyperlinkColumn.ToString());
            xmlWriter.WriteAttributeString("ScaleFactor", ScaleFactor.ToString());
            xmlWriter.WriteAttributeString("PointScaleType", PointScaleType.ToString());
            xmlWriter.WriteAttributeString("ShowFarSide", ShowFarSide.ToString());
            xmlWriter.WriteAttributeString("RaUnits", RaUnits.ToString());
            xmlWriter.WriteAttributeString("HoverTextColumn", NameColumn.ToString());
            xmlWriter.WriteAttributeString("XAxisColumn", XAxisColumn.ToString());
            xmlWriter.WriteAttributeString("XAxisReverse", XAxisReverse.ToString());
            xmlWriter.WriteAttributeString("YAxisColumn", YAxisColumn.ToString());
            xmlWriter.WriteAttributeString("YAxisReverse", YAxisReverse.ToString());
            xmlWriter.WriteAttributeString("ZAxisColumn", ZAxisColumn.ToString());
            xmlWriter.WriteAttributeString("ZAxisReverse", ZAxisReverse.ToString());
            xmlWriter.WriteAttributeString("CartesianScale", CartesianScale.ToString());
            xmlWriter.WriteAttributeString("CartesianCustomScale", CartesianCustomScale.ToString());
            xmlWriter.WriteAttributeString("DynamicData", DynamicData.ToString());
            xmlWriter.WriteAttributeString("AutoUpdate", AutoUpdate.ToString());
            xmlWriter.WriteAttributeString("DataSourceUrl", DataSourceUrl.ToString());

        }

        public override void InitializeFromXml(System.Xml.XmlNode node)
        {
            TimeSeries = Convert.ToBoolean(node.Attributes["TimeSeries"].Value);
            BeginRange = Convert.ToDateTime(node.Attributes["BeginRange"].Value);
            EndRange = Convert.ToDateTime(node.Attributes["EndRange"].Value);
            Decay = Convert.ToSingle(node.Attributes["Decay"].Value);
            CoordinatesType = (CoordinatesTypes)Enum.Parse(typeof(CoordinatesTypes), node.Attributes["CoordinatesType"].Value);
            if ((int)CoordinatesType < 0)
            {
                CoordinatesType = CoordinatesTypes.Spherical;
            }
            LatColumn = Convert.ToInt32(node.Attributes["LatColumn"].Value);
            LngColumn = Convert.ToInt32(node.Attributes["LngColumn"].Value);
            if (node.Attributes["GeometryColumn"] != null)
            {
                GeometryColumn = Convert.ToInt32(node.Attributes["GeometryColumn"].Value);
            }
            AltType = (AltTypes)Enum.Parse(typeof(AltTypes), node.Attributes["AltType"].Value);
            MarkerMix = (MarkerMixes)Enum.Parse(typeof(MarkerMixes), node.Attributes["MarkerMix"].Value);
            ColorMap = (ColorMaps)Enum.Parse(typeof(ColorMaps), node.Attributes["ColorMap"].Value);
            MarkerColumn = Convert.ToInt32(node.Attributes["MarkerColumn"].Value);
            ColorMapColumn = Convert.ToInt32(node.Attributes["ColorMapColumn"].Value);
            PlotType = (PlotTypes)Enum.Parse(typeof(PlotTypes), node.Attributes["PlotType"].Value);
            MarkerIndex = Convert.ToInt32(node.Attributes["MarkerIndex"].Value);
            MarkerScale = (MarkerScales)Enum.Parse(typeof(MarkerScales), node.Attributes["MarkerScale"].Value);
            AltUnit = (AltUnits)Enum.Parse(typeof(AltUnits), node.Attributes["AltUnit"].Value);
            AltColumn = Convert.ToInt32(node.Attributes["AltColumn"].Value);
            StartDateColumn = Convert.ToInt32(node.Attributes["StartDateColumn"].Value);
            EndDateColumn = Convert.ToInt32(node.Attributes["EndDateColumn"].Value);
            SizeColumn = Convert.ToInt32(node.Attributes["SizeColumn"].Value);
            HyperlinkFormat = node.Attributes["HyperlinkFormat"].Value;
            HyperlinkColumn = Convert.ToInt32(node.Attributes["HyperlinkColumn"].Value);
            ScaleFactor = Convert.ToSingle(node.Attributes["ScaleFactor"].Value);
            PointScaleType = (PointScaleTypes)Enum.Parse(typeof(PointScaleTypes), node.Attributes["PointScaleType"].Value);
            if (node.Attributes["ShowFarSide"] != null)
            {
                ShowFarSide = Boolean.Parse(node.Attributes["ShowFarSide"].Value);
            }

            if (node.Attributes["RaUnits"] != null)
            {
                RaUnits = (RAUnits)Enum.Parse(typeof(RAUnits), node.Attributes["RaUnits"].Value);
            }

            if (node.Attributes["HoverTextColumn"] != null)
            {
                NameColumn = Convert.ToInt32(node.Attributes["HoverTextColumn"].Value);
            }

            if (node.Attributes["XAxisColumn"] != null)
            {
                XAxisColumn = Convert.ToInt32(node.Attributes["XAxisColumn"].Value);
                XAxisReverse = Convert.ToBoolean(node.Attributes["XAxisReverse"].Value);
                YAxisColumn = Convert.ToInt32(node.Attributes["YAxisColumn"].Value);
                YAxisReverse = Convert.ToBoolean(node.Attributes["YAxisReverse"].Value);
                ZAxisColumn = Convert.ToInt32(node.Attributes["ZAxisColumn"].Value);
                ZAxisReverse = Convert.ToBoolean(node.Attributes["ZAxisReverse"].Value);
                CartesianScale = (AltUnits)Enum.Parse(typeof(AltUnits), node.Attributes["CartesianScale"].Value);
                CartesianCustomScale = Convert.ToDouble(node.Attributes["CartesianCustomScale"].Value);
            }

            if (node.Attributes["DynamicData"] != null)
            {
                DynamicData = Convert.ToBoolean(node.Attributes["DynamicData"].Value);
                AutoUpdate = Convert.ToBoolean(node.Attributes["AutoUpdate"].Value);
                DataSourceUrl = node.Attributes["DataSourceUrl"].Value;
            }


        }


        public virtual void ComputeDateDomainRange(int columnStart, int columnEnd)
        {
        }
        public Dictionary<string, DomainValue> MarkerDomainValues = new Dictionary<string, DomainValue>();
        public Dictionary<string, DomainValue> ColorDomainValues = new Dictionary<string, DomainValue>();

        public virtual string[] GetDomainValues(int column)
        {
            return new string[0];
        }

        [LayerProperty]
        public float Decay
        {
            get { return decay; }
            set
            {
                if (decay != value)
                {
                    version++;
                    decay = value;
                }
            }
        }

        public enum CoordinatesTypes { Spherical, Rectangular, Orbital };

        private CoordinatesTypes coordinatesType = CoordinatesTypes.Spherical;

        [LayerProperty]
        public CoordinatesTypes CoordinatesType
        {
            get { return coordinatesType; }
            set
            {
                if (coordinatesType != value)
                {
                    version++;
                    coordinatesType = value;
                }
            }
        }


        public enum ReferenceFrames { Earth, Helocentric, Equatorial, Ecliptic, Galactic, Moon, Mercury, Venus, Mars, Jupiter, Saturn, Uranus, Neptune, Custom };


        [LayerProperty]
        public int LatColumn
        {
            get { return latColumn; }
            set
            {
                if (latColumn != value)
                {
                    version++;
                    latColumn = value;
                }
            }
        }
        protected int lngColumn = -1;

        [LayerProperty]
        public int LngColumn
        {
            get { return lngColumn; }
            set
            {
                if (lngColumn != value)
                {
                    version++;
                    lngColumn = value;
                }
            }
        }

        protected int geometryColumn = -1;

        [LayerProperty]
        public int GeometryColumn
        {
            get { return geometryColumn; }
            set
            {
                if (geometryColumn != value)
                {
                    version++;
                    geometryColumn = value;
                }
            }
        }

        private int xAxisColumn = -1;

        [LayerProperty]
        public int XAxisColumn
        {
            get { return xAxisColumn; }
            set
            {
                if (xAxisColumn != value)
                {
                    version++;
                    xAxisColumn = value;
                }
            }
        }
        private int yAxisColumn = -1;

        [LayerProperty]
        public int YAxisColumn
        {
            get { return yAxisColumn; }
            set
            {
                if (yAxisColumn != value)
                {
                    version++;
                    yAxisColumn = value;
                }
            }
        }
        private int zAxisColumn = -1;

        [LayerProperty]
        public int ZAxisColumn
        {
            get { return zAxisColumn; }
            set
            {
                if (zAxisColumn != value)
                {
                    version++;
                    zAxisColumn = value;
                }
            }
        }

        private bool xAxisReverse = false;

        [LayerProperty]
        public bool XAxisReverse
        {
            get { return xAxisReverse; }
            set
            {
                if (xAxisReverse != value)
                {
                    version++;
                    xAxisReverse = value;
                }
            }
        }
        private bool yAxisReverse = false;

        [LayerProperty]
        public bool YAxisReverse
        {
            get { return yAxisReverse; }
            set
            {
                if (yAxisReverse != value)
                {
                    version++;
                    yAxisReverse = value;
                }
            }
        }
        private bool zAxisReverse = false;

        [LayerProperty]
        public bool ZAxisReverse
        {
            get { return zAxisReverse; }
            set
            {
                if (zAxisReverse != value)
                {
                    version++;
                    zAxisReverse = value;
                }
            }
        }




        public enum AltTypes { Depth, Altitude, Distance, SeaLevel, Terrain };

        private AltTypes altType = AltTypes.SeaLevel;

        [LayerProperty]
        public AltTypes AltType
        {
            get { return altType; }
            set
            {
                if (altType != value)
                {
                    version++;
                    altType = value;
                }
            }
        }

        public enum MarkerMixes { Same_For_All, /*Group_by_Range, Group_by_Values */};
        public enum ColorMaps { Same_For_All, /*Group_by_Range, */Group_by_Values, Per_Column_Literal/*, Gradients_by_Range*/ };

        public enum PlotTypes { Gaussian, Point, Circle, Square, PushPin/*, Custom */, Target1, Target2, Column, Cylinder};

        public enum MarkerScales { Screen, World };
        public enum RAUnits { Hours, Degrees };

        private MarkerMixes markerMix = MarkerMixes.Same_For_All;

        [LayerProperty]
        public MarkerMixes MarkerMix
        {
            get { return markerMix; }
            set
            {
                if (markerMix != value)
                {
                    version++;
                    markerMix = value;
                }
            }
        }

        RAUnits raUnits = RAUnits.Hours;
        [LayerProperty]
        public RAUnits RaUnits
        {
            get { return raUnits; }
            set
            {

                if (raUnits != value)
                {
                    version++;
                    raUnits = value;
                }
            }
        }

        private ColorMaps colorMap = ColorMaps.Per_Column_Literal;

        [LayerProperty]
        internal ColorMaps ColorMap
        {
            get { return colorMap; }
            set
            {
                if (colorMap != value)
                {
                    version++;
                    colorMap = value;
                }
            }
        }


        private int markerColumn = -1;

        [LayerProperty]
        public int MarkerColumn
        {
            get { return markerColumn; }
            set
            {
                if (markerColumn != value)
                {
                    version++;
                    markerColumn = value;
                }
            }
        }

        private int colorMapColumn = -1;

        [LayerProperty]
        public int ColorMapColumn
        {
            get { return colorMapColumn; }
            set
            {
                if (colorMapColumn != value)
                {
                    version++;
                    colorMapColumn = value;
                }
            }
        }

        private PlotTypes plotType = PlotTypes.Gaussian;

        [LayerProperty]
        public PlotTypes PlotType
        {
            get { return plotType; }
            set
            {
                if (plotType != value)
                {
                    version++;
                    plotType = value;
                }

            }
        }

        private int markerIndex = 0;

        [LayerProperty]
        public int MarkerIndex
        {
            get { return markerIndex; }
            set
            {
                if (markerIndex != value)
                {
                    version++;
                    markerIndex = value;
                }
            }
        }

        private bool showFarSide = false;

        [LayerProperty]
        public bool ShowFarSide
        {
            get { return showFarSide; }
            set
            {
                if (showFarSide != value)
                {
                    version++;
                    showFarSide = value;
                }
            }
        }


        private MarkerScales markerScale = MarkerScales.World;

        [LayerProperty]
        public MarkerScales MarkerScale
        {
            get { return markerScale; }
            set
            {
                if (markerScale != value)
                {
                    version++;
                    markerScale = value;
                }
            }
        }


        private AltUnits altUnit = AltUnits.Meters;

        [LayerProperty]
        public AltUnits AltUnit
        {
            get { return altUnit; }
            set
            {
                if (altUnit != value)
                {
                    version++;
                    altUnit = value;
                }
            }
        }
        private AltUnits cartesianScale = AltUnits.Meters;

        [LayerProperty]
        public AltUnits CartesianScale
        {
            get { return cartesianScale; }
            set
            {
                if (cartesianScale != value)
                {
                    version++;
                    cartesianScale = value;
                }
            }
        }

        private double cartesianCustomScale = 1;

        [LayerProperty]
        public double CartesianCustomScale
        {
            get { return cartesianCustomScale; }
            set
            {
                if (cartesianCustomScale != value)
                {
                    version++;
                    cartesianCustomScale = value;
                }
            }
        }

        protected int altColumn = -1;
        [LayerProperty]
        public int AltColumn
        {
            get { return altColumn; }
            set
            {
                if (altColumn != value)
                {
                    version++;
                    altColumn = value;
                }
            }
        }

        protected int startDateColumn = -1;

        [LayerProperty]
        public int StartDateColumn
        {
            get { return startDateColumn; }
            set
            {
                if (startDateColumn != value)
                {
                    version++;
                    startDateColumn = value;
                }
            }
        }
        protected int endDateColumn = -1;

        [LayerProperty]
        public int EndDateColumn
        {
            get { return endDateColumn; }
            set
            {
                if (endDateColumn != value)
                {
                    version++;
                    endDateColumn = value;
                }
            }
        }

        protected int sizeColumn = -1;

        [LayerProperty]
        public int SizeColumn
        {
            get { return sizeColumn; }
            set
            {
                if (sizeColumn != value)
                {
                    version++;
                    sizeColumn = value;
                }
            }
        }
        protected int nameColumn = 0;

        [LayerProperty]
        public int NameColumn
        {
            get { return nameColumn; }
            set
            {
                if (nameColumn != value)
                {
                    version++;
                    nameColumn = value;
                }
            }
        }
        private string hyperlinkFormat = "";

        [LayerProperty]
        public string HyperlinkFormat
        {
            get { return hyperlinkFormat; }
            set {
                if (hyperlinkFormat != value)
                {
                    version++; hyperlinkFormat = value;
                }
            }
        }

        private int hyperlinkColumn = -1;

        [LayerProperty]
        public int HyperlinkColumn
        {
            get { return hyperlinkColumn; }
            set
            {
                if (hyperlinkColumn != value)
                {
                    version++;
                    hyperlinkColumn = value;
                }
            }
        }


        protected float scaleFactor = 1.0f;

        [LayerProperty]
        public float ScaleFactor
        {
            get { return scaleFactor; }
            set
            {
                if (scaleFactor != value)
                {
                    version++;

                    scaleFactor = value;
                }
            }
        }

        protected PointScaleTypes pointScaleType = PointScaleTypes.Power;

        [LayerProperty]
        public PointScaleTypes PointScaleType
        {
            get { return pointScaleType; }
            set
            {
                if (pointScaleType != value)
                {
                    version++;
                    pointScaleType = value;
                }
            }
        }

        protected List<Vector3> positions = new List<Vector3>();

        protected LineList lineList;
        protected LineList lineList2d;
        protected TriangleList triangleList;
        protected TriangleList triangleList2d;
        protected Text3dBatch textBatch;
        protected bool bufferIsFlat = false;

        static Texture11 circleTexture = null;
        static Texture11 squareTexture = null;
        static Texture11 pointTexture = null;
        static Texture11 target1Texture = null;
        static Texture11 target2Texture = null;

        public static Texture11 CircleTexture
        {
            get
            {
                if (circleTexture == null)
                {
                    circleTexture = Texture11.FromBitmap(Properties.Resources.circle);
                }

                return circleTexture;
            }
        }
        static Texture11 PointTexture
        {
            get
            {
                if (pointTexture == null)
                {
                    pointTexture = Texture11.FromBitmap(Properties.Resources.point);
                }

                return pointTexture;
            }
        }
        static Texture11 Target1Texture
        {
            get
            {
                if (target1Texture == null)
                {
                    target1Texture = Texture11.FromBitmap(Properties.Resources.target1);
                }

                return target1Texture;
            }
        }
        static Texture11 Target2Texture
        {
            get
            {
                if (target2Texture == null)
                {
                    target2Texture = Texture11.FromBitmap(Properties.Resources.target2);
                }

                return target2Texture;
            }
        }

        static Texture11 SquareTexture
        {
            get
            {
                if (squareTexture == null)
                {
                    Bitmap onePixel = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    onePixel.SetPixel(0, 0, Color.White);
                    squareTexture = Texture11.FromBitmap(onePixel);
                }

                return squareTexture;
            }
        }

        public override bool Draw(RenderContext11 renderContext, float opacity, bool flat)
        {
            if (bufferIsFlat != flat)
            {
                CleanUp();
                bufferIsFlat = flat;
            }

            if (!CleanAndReady)
            {
                PrepVertexBuffer( opacity);
            }

            Matrix3d oldWorld = renderContext.World;
            if (astronomical && !bufferIsFlat)
            {
                double ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;
                renderContext.World = Matrix3d.RotationX(ecliptic) * renderContext.World;
            }

            if (triangleList2d != null)
            {
                triangleList2d.Decay = decay;
                triangleList2d.Sky = this.Astronomical;
                triangleList2d.TimeSeries = timeSeries;
                triangleList2d.Draw(renderContext, opacity * Opacity, TerraViewer.TriangleList.CullMode.CounterClockwise);
            }

            if (triangleList != null)
            {

                triangleList.Decay = decay;
                triangleList.Sky = this.Astronomical;
                triangleList.TimeSeries = timeSeries;
                triangleList.Draw(renderContext, opacity * Opacity, TerraViewer.TriangleList.CullMode.CounterClockwise);
            }

            if (lineList != null)
            {
                lineList.Sky = this.Astronomical;
                lineList.Decay = decay;
                lineList.TimeSeries = timeSeries;

                lineList.DrawLines(renderContext, opacity * Opacity);
            }

            if (lineList2d != null)
            {
                lineList2d.Sky = !this.Astronomical;
                lineList2d.Decay = decay;
                lineList2d.TimeSeries = timeSeries;
                lineList2d.ShowFarSide = ShowFarSide;
                lineList2d.DrawLines(renderContext, opacity * Opacity);
            }

            if (textBatch != null)
            {
                DepthStencilMode mode = renderContext.DepthStencilMode;

                renderContext.DepthStencilMode = DepthStencilMode.Off;

                textBatch.Draw(renderContext, 1, Color.White);
                renderContext.DepthStencilMode = mode;
            }

            if (astronomical && !bufferIsFlat)
            {
                renderContext.DepthStencilMode = DepthStencilMode.ZReadWrite;
            }
            else
            {
                renderContext.DepthStencilMode = DepthStencilMode.Off;
            }

            DateTime baseDate = new DateTime(2010, 1, 1, 12, 00, 00);
            renderContext.setRasterizerState(TriangleCullMode.Off);
          
            Vector3 cam = Vector3d.TransformCoordinate(renderContext.CameraPosition, Matrix3d.Invert(renderContext.World)).Vector311;
            float adjustedScale = scaleFactor;

            if (flat && astronomical && (markerScale == MarkerScales.World))
            {
                adjustedScale = (float)(scaleFactor / (RenderEngine.Engine.ZoomFactor / 360));
            }             
            
            Matrix matrixWVP = (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
            matrixWVP.Transpose();
            switch (plotType)
            {
                default:
                case PlotTypes.Gaussian:
                    renderContext.BlendMode = BlendMode.Additive;
                    renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, Grids.StarProfile.ResourceView);
                    break;
                case PlotTypes.Circle:
                    renderContext.BlendMode = BlendMode.Alpha;
                    renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, CircleTexture.ResourceView);
                    break;
                case PlotTypes.Point:
                    renderContext.BlendMode = BlendMode.Alpha;
                    renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, PointTexture.ResourceView);
                    break;
                case PlotTypes.Square:
                    renderContext.BlendMode = BlendMode.Alpha;
                    renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, SquareTexture.ResourceView);
                    break;
                case PlotTypes.Target1:
                    renderContext.BlendMode = BlendMode.Alpha;
                    renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, Target1Texture.ResourceView);
                    break;
                case PlotTypes.Target2:
                    renderContext.BlendMode = BlendMode.Alpha;
                    renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, Target2Texture.ResourceView);
                    break;
                case PlotTypes.PushPin:
                    renderContext.BlendMode = BlendMode.Alpha;
                    renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, PushPin.GetPushPinTexture(markerIndex).ResourceView);
                    break;

            }

            columnChartsActivate.TargetState = plotType == PlotTypes.Column;

            if (shapeFileVertex != null)
            {
                if (plotType == PlotTypes.Column && !RenderContext11.Downlevel) // column chart mode
                {
                    columnChartsActivate.TargetState = true;

                    renderContext.BlendMode = BlendMode.Alpha;
                    renderContext.setRasterizerState(TriangleCullMode.CullClockwise);
                    renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, null);
                    renderContext.DepthStencilMode = DepthStencilMode.ZReadWrite;
                    TimeSeriesColumnChartShader11.Constants.CameraPosition = new SharpDX.Vector4(cam, 1);
                    TimeSeriesColumnChartShader11.Constants.JNow = (float)(SpaceTimeController.JNow - SpaceTimeController.UtcToJulian(baseDate));
                    TimeSeriesColumnChartShader11.Constants.Decay = timeSeries ? decay : 0f;
                    TimeSeriesColumnChartShader11.Constants.Scale = (float)(adjustedScale);
                    TimeSeriesColumnChartShader11.Constants.Sky = astronomical ? -1 : 1;
                    TimeSeriesColumnChartShader11.Constants.Opacity = opacity * this.Opacity;
                    TimeSeriesColumnChartShader11.Constants.ShowFarSide = ShowFarSide ? 1f : 0f;
                    TimeSeriesColumnChartShader11.Color = new SharpDX.Color4(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f);
                    TimeSeriesColumnChartShader11.Constants.WorldViewProjection = matrixWVP;
                    TimeSeriesColumnChartShader11.ViewportScale = new SharpDX.Vector2(2.0f / renderContext.ViewPort.Width, 2.0f / renderContext.ViewPort.Height);
                    TimeSeriesColumnChartShader11.Use(renderContext.devContext);
                }
                else if (plotType == PlotTypes.Cylinder && !RenderContext11.Downlevel)
                {
                    renderContext.BlendMode = BlendMode.Alpha;
                    renderContext.setRasterizerState(TriangleCullMode.CullClockwise);
                    renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, null);
                    renderContext.DepthStencilMode = DepthStencilMode.ZReadWrite;
                    TimeSeriesColumnChartShaderNGon11.Constants.CameraPosition = new SharpDX.Vector4(cam, 1);
                    TimeSeriesColumnChartShaderNGon11.Constants.JNow = (float)(SpaceTimeController.JNow - SpaceTimeController.UtcToJulian(baseDate));
                    TimeSeriesColumnChartShaderNGon11.Constants.Decay = timeSeries ? decay : 0f;
                    TimeSeriesColumnChartShaderNGon11.Constants.Scale = (float)(adjustedScale);
                    TimeSeriesColumnChartShaderNGon11.Constants.Sky = astronomical ? -1 : 1;
                    TimeSeriesColumnChartShaderNGon11.Constants.Opacity = opacity * this.Opacity;
                    TimeSeriesColumnChartShaderNGon11.Constants.ShowFarSide = ShowFarSide ? 1f : 0f;
                    TimeSeriesColumnChartShaderNGon11.Color = new SharpDX.Color4(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f);
                    TimeSeriesColumnChartShaderNGon11.Constants.WorldViewProjection = matrixWVP;
                    TimeSeriesColumnChartShaderNGon11.ViewportScale = new SharpDX.Vector2(2.0f / renderContext.ViewPort.Width, 2.0f / renderContext.ViewPort.Height);
                    TimeSeriesColumnChartShaderNGon11.Use(renderContext.devContext);
                }
                else
                {
                    TimeSeriesPointSpriteShader11.Constants.CameraPosition = new SharpDX.Vector4(cam, 1);
                    double jBase = SpaceTimeController.UtcToJulian(baseDate);
                    TimeSeriesPointSpriteShader11.Constants.JNow = (float)(SpaceTimeController.JNow - jBase);
                    TimeSeriesPointSpriteShader11.Constants.Decay = timeSeries ? decay : 0f;
                    TimeSeriesPointSpriteShader11.Constants.Scale = ((markerScale == MarkerScales.World) ? ((float)adjustedScale) : (-(float)adjustedScale));
                    TimeSeriesPointSpriteShader11.Constants.Sky = astronomical ? -1 : 1;
                    TimeSeriesPointSpriteShader11.Constants.Opacity = opacity * this.Opacity;
                    TimeSeriesPointSpriteShader11.Constants.ShowFarSide = ShowFarSide ? 1f : 0f;
                    TimeSeriesPointSpriteShader11.Color = new SharpDX.Color4(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f);
                    TimeSeriesPointSpriteShader11.Constants.WorldViewProjection = matrixWVP;
                    TimeSeriesPointSpriteShader11.ViewportScale = new SharpDX.Vector2(2.0f / renderContext.ViewPort.Width, 2.0f / renderContext.ViewPort.Height);
                    TimeSeriesPointSpriteShader11.PointScaleFactors = new SharpDX.Vector3(0.0f, 0.0f, 10.0f);
                    if (shapeFileVertex.Downlevel)
                    {
                        DownlevelTimeSeriesPointSpriteShader.Use(renderContext.devContext, shapeFileVertex.Instanced);
                    }
                    else
                    {
                        TimeSeriesPointSpriteShader11.Use(renderContext.devContext);
                    }
                }


                shapeFileVertex.Draw(renderContext, shapeFileVertex.Count, null, 1.0f);
            }
            renderContext.Device.ImmediateContext.GeometryShader.Set(null);
            renderContext.BlendMode = BlendMode.Alpha;


            renderContext.World = oldWorld;

            return true;
        }

        BlendState columnChartsActivate = new BlendState(false, 1000);

        virtual protected bool PrepVertexBuffer( float opacity)
        {
            throw new NotImplementedException();
        }

      
        public bool CleanAndReady = false;

        public override void CleanUp()
        {
            CleanAndReady = false;    
            
            if (shapeFileIndex != null)
            {
                shapeFileIndex.Dispose();
                GC.SuppressFinalize(shapeFileIndex);
                shapeFileIndex = null;
            }
            if (shapeFileVertex != null)
            {
                shapeFileVertex.Dispose();
                GC.SuppressFinalize(shapeFileVertex);

                shapeFileVertex = null;
            }


            if (lineList != null)
            {
                lineList.Clear();
            }
            if (lineList2d != null)
            {
                lineList2d.Clear();
            }

            if (triangleList2d != null)
            {
                triangleList2d.Clear();
            }

            if (triangleList != null)
            {
                triangleList.Clear();
            }
            if (textBatch != null)
            {
                textBatch.CleanUp();
            }
        }

        public virtual bool DynamicUpdate()
        {
            throw new NotImplementedException();
        }

        DateTime ITimeSeriesDescription.SeriesEndTime
        {
            get { return this.EndRange; }
        }

        DateTime ITimeSeriesDescription.SeriesStartTime
        {
            get { return this.BeginRange; }
        }

        TimeSpan ITimeSeriesDescription.TimeStep
        {
            get { return TimeSpan.FromSeconds(1); }
        }

        bool ITimeSeriesDescription.IsTimeSeries
        {
            get
            {
                return TimeSeries;
            }
            set
            {
                TimeSeries = value;
            }
        }
    }

}
