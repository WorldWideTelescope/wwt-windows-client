using System.Data;

namespace ShapefileTools
{
    public abstract class Shape
    {
        internal DataRow props;
        public DataRow Attributes
        {
            get { return props; }
           
        }
    }


    #region building block geometries

    public class Point : Shape
    {
        public double X;
        public double Y;
    }

    public class PointM : Point
    {
        public double M;
    }

    public class PointZ : PointM
    {
        public double Z;
    }
    /// <summary>
    /// A ring of a polygon of an unspecified type
    /// </summary>
    public class Ring : Shape
    {
        public Point[] Points;
    }

    public class RingZ : Shape
    {
        public PointZ[] Points;
    }

    public class RingM : Shape
    {
        public PointM[] Points;
    }

    public class Line
    {
        public Point[] Points;
    }

    public class LineZ
    {
        public PointZ[] Points;
    }

    public class LineM
    {
        public PointM[] Points;
    }

    #endregion 

    # region complex types

    public abstract class ComplexShape : Shape {

        private double[] bbox;
/// <summary>
/// Contents are in the order Xmin, Ymin, Xmax, Ymax
/// </summary>
        public double[] BoundingBox
        {
            get { return bbox; }
            set { bbox = value; }
        }
    }

    public abstract class ComplexShapeM : ComplexShape {
        private readonly double[] mRange = new double[2];

        public double MMin
        {
            get { return mRange[0]; }
            set { mRange[0] = value; }
        }

        public double MMax
        {
            get { return mRange[1]; }
            set { mRange[1] = value; }
        }
    }

    public abstract class ComplexShapeZ : ComplexShapeM
    {
        private readonly double[] zRange = new double[2];

        public double ZMin
        {
            get { return zRange[0]; }
            set { zRange[0] = value; }
        }

        public double ZMax
        {
            get { return zRange[1]; }
            set { zRange[1] = value; }
        }
    }

    /// <summary>
    /// A PolyLine is an ordered set of vertices that consists of one or more parts. A part is a
    /// connected sequence of two or more points. Parts may or may not be connected to one
    /// another. Parts may or may not intersect one another.
    /// </summary>
    public class PolyLine: ComplexShape
    {
        public Line[] Lines;

    }

    public class PolyLineM : ComplexShapeM
    {
        public LineM[] Lines;
    }

    public class PolyLineZ : ComplexShapeZ
    {
        public LineZ[] Lines;
    }

    public class MultiPoint : ComplexShape
    {
        public Point[] Points;
    }

    public class MultiPointZ : ComplexShapeZ
    {
        public PointZ[] Points;
    }

    public class MultiPointM : ComplexShapeM
    {
        public PointM[] Points;
    }

    /// <summary>
    /// A polygon consists of one or more rings. A ring is a connected sequence of four or more
    /// points that form a closed, non-self-intersecting loop. A polygon may contain multiple
    /// outer rings. The order of vertices or orientation for a ring indicates which side of the ring
    /// is the interior of the polygon. The neighborhood to the right of an observer walking along
    /// the ring in vertex order is the neighborhood inside the polygon. Vertices of rings defining
    /// holes in polygons are in a counterclockwise direction. Vertices for a single, ringed
    /// polygon are, therefore, always in clockwise order.
    /// </summary>
    public class Polygon : ComplexShape
    {
        public Ring[] Rings;
    }

    public class PolygonM: ComplexShapeM
    {
        public RingM[] Rings;
    }

    public class PolygonZ : ComplexShapeZ
    {
        public RingZ[] Rings;
    }

    public class MultiPatch : ComplexShapeZ {

        public MultiPatchElement [] Parts;

    }

    public abstract class MultiPatchElement : RingZ { 
    
    
    }

    /// <summary>
    /// A linked strip of triangles, where every vertex (after the first two)
    /// completes a new triangle. A new triangle is always formed by
    /// connecting the new vertex with its two immediate predecessors.
    /// </summary>
    public class TriangleStrip : MultiPatchElement
    {

    }
    /// <summary>
    /// A linked fan of triangles, where every vertex (after the first two)
    /// completes a new triangle. A new triangle is always formed by
    /// connecting the new vertex with its immediate predecessor and the
    /// first vertex of the part.
    /// </summary>
    public class TriangleFan : MultiPatchElement
    {

    }
    /// <summary>
    /// The outer ring of a polygon
    /// </summary>
    public class OuterRing : MultiPatchElement
    {

    }
    /// <summary>
    ///  A hole of a polygon
    /// </summary>
    public class InnerRing : MultiPatchElement
    {

    }

    public class UndefinedRing : MultiPatchElement
    {

    }



    #endregion





}
