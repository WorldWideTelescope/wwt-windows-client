using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
namespace TerraViewer
{
    public class Glu
    {
        public const string GLU_DLL = "glu32";
        public const uint GLU_VERSION_1_1 = 1;
        public const uint GLU_VERSION_1_2 = 1;

        public const uint GLU_INVALID_ENUM = 100900;
        public const uint GLU_INVALID_VALUE = 100901;
        public const uint GLU_OUT_OF_MEMORY = 100902;
        public const uint GLU_INCOMPATIBLE_GL_VERSION = 100903;

        public const uint GLU_VERSION = 100800;
        public const uint GLU_EXTENSIONS = 100801;
        public const uint GLU_TRUE = 1;
        public const uint GLU_FALSE = 0;

        public const double GLU_TESS_MAX_COORD = 1.0e150;


        public const uint GLU_TESS_BEGIN = 100100;
        public const uint GLU_TESS_VERTEX = 100101;
        public const uint GLU_TESS_END = 100102;
        public const uint GLU_TESS_ERROR = 100103;
        public const uint GLU_TESS_EDGE_FLAG = 100104;
        public const uint GLU_TESS_COMBINE = 100105;
        public const uint GLU_TESS_BEGIN_DATA = 100106;
        public const uint GLU_TESS_VERTEX_DATA = 100107;
        public const uint GLU_TESS_END_DATA = 100108;
        public const uint GLU_TESS_ERROR_DATA = 100109;
        public const uint GLU_TESS_EDGE_FLAG_DATA = 100110;
        public const uint GLU_TESS_COMBINE_DATA = 100111;

        public const uint GLU_TESS_ERROR1 = 100151;
        public const uint GLU_TESS_ERROR2 = 100152;
        public const uint GLU_TESS_ERROR3 = 100153;
        public const uint GLU_TESS_ERROR4 = 100154;
        public const uint GLU_TESS_ERROR5 = 100155;
        public const uint GLU_TESS_ERROR6 = 100156;
        public const uint GLU_TESS_ERROR7 = 100157;
        public const uint GLU_TESS_ERROR8 = 100158;
        public const uint GLU_TESS_MISSING_BEGIN_POLYGON = GLU_TESS_ERROR1;
        public const uint GLU_TESS_MISSING_BEGIN_CONTOUR = GLU_TESS_ERROR2;
        public const uint GLU_TESS_MISSING_END_POLYGON = GLU_TESS_ERROR3;
        public const uint GLU_TESS_MISSING_END_CONTOUR = GLU_TESS_ERROR4;
        public const uint GLU_TESS_COORD_TOO_LARGE = GLU_TESS_ERROR5;
        public const uint GLU_TESS_NEED_COMBINE_CALLBACK = GLU_TESS_ERROR6;
        public const uint GLU_BEGIN = GLU_TESS_BEGIN;
        public const uint GLU_VERTEX = GLU_TESS_VERTEX;
        public const uint GLU_END = GLU_TESS_END;
        public const uint GLU_ERROR = GLU_TESS_ERROR;
        public const uint GLU_EDGE_FLAG = GLU_TESS_EDGE_FLAG;


        [DllImport(GLU_DLL)]
        public unsafe static extern byte* gluErrorString(uint errCode);
        [DllImport(GLU_DLL)]
        public unsafe static extern byte* gluGetString(uint name);
        [DllImport(GLU_DLL)]
        public unsafe static extern IntPtr gluNewTess();
        [DllImport(GLU_DLL)]
        public unsafe static extern void gluDeleteTess(IntPtr tess);
        [DllImport(GLU_DLL)]
        public unsafe static extern void gluTessBeginPolygon(IntPtr tess, IntPtr polygon_data);
        [DllImport(GLU_DLL)]
        public unsafe static extern void gluTessBeginContour(IntPtr tess);
        [DllImport(GLU_DLL)]
        public unsafe static extern void gluTessVertex(IntPtr tess, double[] coords, int data);
        [DllImport(GLU_DLL)]
        public unsafe static extern void gluTessEndContour(IntPtr tess);
        [DllImport(GLU_DLL)]
        public unsafe static extern void gluTessEndPolygon(IntPtr tess);
        [DllImport(GLU_DLL)]
        public unsafe static extern void gluTessProperty(IntPtr tess, uint which, double valuex);
        [DllImport(GLU_DLL)]
        public unsafe static extern void gluTessNormal(IntPtr tess, double x, double y, double z);
        [DllImport(GLU_DLL)]
        public unsafe static extern void gluTessCallback(IntPtr tess, uint which, Delegate fn);
        [DllImport(GLU_DLL)]
        public unsafe static extern void gluGetTessProperty(IntPtr tess, uint which, double* valuex);

        public delegate void TessCallback(uint flag, int data);

        public delegate void TessEndData(IntPtr data);
        public unsafe delegate void TessCombine(double* coords, uint[] vertexData, float[] weight, int* outData, int data);

        public static event TessCallback BeginDataEvent;
        public static event TessCallback EdgeFlagEvent;
        public static event TessCallback VertexEvent;
        public static event TessEndData EndDataEvent;
        public static event TessCombine CombineEvent;
        public static event TessCallback ErrorEvent;

        static List<Vector3d> VertexList = new List<Vector3d>();
        static List<int> TriangleListOut = new List<int>();


        public static unsafe List<int> TesselateSimplePolyB(List<Vector3d> inputList)
        {
            List<int> results = new List<int>();

            Tessellator tess = new Tessellator();

            tess.Process(inputList, results);

            return results;
        }


        public static unsafe List<int> TesselateSimplePoly(List<Vector3d> inputList)
        {
            TriangleListOut.Clear();
            VertexList = inputList;

            //// fill the polygon
            //VertexList.Add(new Vector3d(0,0,0));
            //VertexList.Add(new Vector3d(20,0,0));
            //VertexList.Add(new Vector3d(0,20,0));
            //VertexList.Add(new Vector3d(20,20,0));


            IntPtr tess = gluNewTess();

            if (tess == null)
            {
                return null;
            }
            // register the callbacks

            BeginDataEvent = new TessCallback(BeginData);
            EdgeFlagEvent = new TessCallback(EdgeFlag);
            VertexEvent = new TessCallback(Vertex);
            EndDataEvent = new TessEndData(EndData);
            CombineEvent = new TessCombine(Combine);
            ErrorEvent = new TessCallback(GLU_ErrorEvent);

            gluTessCallback(tess, GLU_TESS_BEGIN_DATA, BeginDataEvent);
            gluTessCallback(tess, GLU_TESS_EDGE_FLAG_DATA, EdgeFlagEvent);
            gluTessCallback(tess, GLU_TESS_VERTEX_DATA, VertexEvent);
            gluTessCallback(tess, GLU_TESS_END_DATA, EndDataEvent);
            gluTessCallback(tess, GLU_TESS_COMBINE_DATA, CombineEvent);


            gluTessBeginPolygon(tess, IntPtr.Zero);

            gluTessBeginContour(tess);
            for (int i = 0; i < VertexList.Count; i++)
            {
                Vector3d v = VertexList[i];
                gluTessVertex(tess, new double[] { v.X, v.Y, v.Z }, i);
            }
            gluTessEndContour(tess);
            gluTessEndPolygon(tess);

            gluDeleteTess(tess);

            return TriangleListOut;
        }

        static void GLU_ErrorEvent(uint flag, int data)
        {
            
        }

        public static void BeginData(uint type, int data)
        {
        }

        public static void EdgeFlag(uint type, int data)
        {
        }
        public static void Vertex(uint index, int data)
        {
            TriangleListOut.Add((int)index);

        }

        public static void EndData(IntPtr data)
        {
        }

        public unsafe static void Combine(double* coords, uint[] vertexData, float[] weight, int* outData, int data)
        {
            outData[0] = VertexList.Count;
            VertexList.Add(new Vector3d(coords[0], coords[1], coords[2]));

        }

    }


    public class Tessellator
    {
        public static List<int> TesselateSimplePolyB(List<Vector3d> inputList)
        {
            List<int> results = new List<int>();

            Tessellator tess = new Tessellator();

            tess.Process(inputList, results);

            return results;
        }

        const float EPSILON = 0.0000000001f;

        double Area(List<Vector2d> poly)
        {

            int n = poly.Count;

            double A = 0.0f;

            for (int p = n - 1, q = 0; q < n; p = q++)
            {
                A += poly[p].X * poly[q].Y - poly[q].X * poly[p].Y;
            }
            return A * 0.5f;
        }
        private bool IsLeftOfHalfSpace(Vector3d pntA, Vector3d pntB, Vector3d pntTest)
        {
            pntA.Normalize();
            pntB.Normalize();
            Vector3d cross = Vector3d.Cross(pntA, pntB);

            double dot = Vector3d.Dot(cross, pntTest);

            return dot > 0;
        }

        private bool InsideTriangle(Vector3d pntA, Vector3d pntB, Vector3d pntC, Vector3d pntTest)
        {
            if (!IsLeftOfHalfSpace(pntA, pntB, pntTest))
            {
                return false;
            }
            if (!IsLeftOfHalfSpace(pntB, pntC, pntTest))
            {
                return false;
            }
            if (!IsLeftOfHalfSpace(pntC, pntA, pntTest))
            {
                return false;
            }

            return true;
        }


        /*
          InsideTriangle decides if a point P is Inside of the triangle
          defined by A, B, C.
        */
        //bool InsideTriangle(double Ax, double Ay, double Bx, double By, double Cx, double Cy, double Px, double Py)
        //{
        //    double ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        //    double cCROSSap, bCROSScp, aCROSSbp;

        //    ax = Cx - Bx; ay = Cy - By;
        //    bx = Ax - Cx; by = Ay - Cy;
        //    cx = Bx - Ax; cy = By - Ay;
        //    apx = Px - Ax; apy = Py - Ay;
        //    bpx = Px - Bx; bpy = Py - By;
        //    cpx = Px - Cx; cpy = Py - Cy;

        //    aCROSSbp = ax * bpy - ay * bpx;
        //    cCROSSap = cx * apy - cy * apx;
        //    bCROSScp = bx * cpy - by * cpx;

        //    return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
        //}

        bool Snip(List<Vector3d> poly, int u, int v, int w, int n, int[] V)
        {
            int p;
            //double Ax, Ay, Bx, By, Cx, Cy, Px, Py;

            //Ax = poly[V[u]].X;
            //Ay = poly[V[u]].Y;

            //Bx = poly[V[v]].X;
            //By = poly[V[v]].Y;

            //Cx = poly[V[w]].X;
            //Cy = poly[V[w]].Y;


            Vector3d a = poly[V[u]];
            Vector3d b = poly[V[v]];
            Vector3d c = poly[V[w]];
            Vector3d P;


            //if (EPSILON > (((Bx - Ax) * (Cy - Ay)) - ((By - Ay) * (Cx - Ax))))
            //{
            //    return false;
            //}

            Vector3d d = b-a;
            d.Normalize();
            Vector3d e = b-c;
            c.Normalize();

            Vector3d g= Vector3d.Cross(d,e);

            Vector3d bn = b;
            bn.Normalize();

            if (Vector3d.Dot(g, bn) > 0)
            {
                return false;
            }



            for (p = 0; p < n; p++)
            {
                if ((p == u) || (p == v) || (p == w))
                {
                    continue;
                }

                P = poly[V[p]];
                if (InsideTriangle(a,b,c,P))
                {
                    return false;
                }
            }

            return true;
        }

        bool Process(List<Vector3d> poly, List<Vector3d> result)
        {
            /* allocate and initialize list of Vertices in polygon */

            int n = poly.Count;
            if (n < 3)
            {
                return false;
            }

            int[] V = new int[n];

            /* we want a counter-clockwise polygon in V */

            //if (0.0f < Area(poly))
            //{
                for (int v = 0; v < n; v++)
                {
                    V[v] = v;
                }
            //}
            //else
            //{
            //    for (int v = 0; v < n; v++)
            //    {
            //        V[v] = (n - 1) - v;
            //    }
            //}
            int nv = n;

            /*  remove nv-2 Vertices, creating 1 triangle every time */
            int count = 2 * nv;   /* error detection */

            for (int m = 0, v = nv - 1; nv > 2; )
            {
                /* if we loop, it is probably a non-simple polygon */
                if (0 >= (count--))
                {
                    //** Triangulate: ERROR - probable bad polygon!
                    return false;
                }

                /* three consecutive vertices in current polygon, <u,v,w> */
                int u = v;
                if (nv <= u)
                {
                    u = 0;     /* previous */
                }

                v = u + 1;
                if (nv <= v)
                {
                    v = 0;     /* new v    */
                }

                int w = v + 1;
                if (nv <= w)
                {
                    w = 0;     /* next     */
                }

                if (Snip(poly, u, v, w, nv, V))
                {
                    int a, b, c, s, t;

                    /* true names of the vertices */
                    a = V[u];
                    b = V[v];
                    c = V[w];

                    /* output Triangle */
                    result.Add(poly[a]);
                    result.Add(poly[b]);
                    result.Add(poly[c]);

                    m++;

                    /* remove v from remaining polygon */
                    for (s = v, t = v + 1; t < nv; s++, t++)
                    {
                        V[s] = V[t];
                    }
                    nv--;

                    /* resest error detection counter */
                    count = 2 * nv;
                }
            }
            return true;
        }

        public bool Process(List<Vector3d> poly, List<int> result)
        {
            /* allocate and initialize list of Vertices in polygon */

            int n = poly.Count;
            if (n < 3)
            {
                return false;
            }

            int[] V = new int[n];

            /* we want a counter-clockwise polygon in V */

            //if (0.0f < Area(poly))
            //{
            for (int v = 0; v < n; v++)
            {
                V[v] = v;
            }
            //}
            //else
            //{
            //for (int v = 0; v < n; v++)
            //{
            //    V[v] = (n - 1) - v;
            //}
            //}
            int nv = n;

            /*  remove nv-2 Vertices, creating 1 triangle every time */
            int count = 2 * nv;   /* error detection */

            for (int m = 0, v = nv - 1; nv > 2; )
            {
                /* if we loop, it is probably a non-simple polygon */
                if (0 >= (count--))
                {
                    //** Triangulate: ERROR - probable bad polygon!
                    return false;
                }

                /* three consecutive vertices in current polygon, <u,v,w> */
                int u = v;
                if (nv <= u)
                {
                    u = 0;     /* previous */
                }

                v = u + 1;
                if (nv <= v)
                {
                    v = 0;     /* new v    */
                }

                int w = v + 1;
                if (nv <= w)
                {
                    w = 0;     /* next     */
                }

                if (Snip(poly, u, v, w, nv, V))
                {
                    int a, b, c, s, t;

                    /* true names of the vertices */
                    a = V[u];
                    b = V[v];
                    c = V[w];

                    /* output Triangle */
                    result.Add(a);
                    result.Add(b);
                    result.Add(c);

                    m++;

                    /* remove v from remaining polygon */
                    for (s = v, t = v + 1; t < nv; s++, t++)
                    {
                        V[s] = V[t];
                    }
                    nv--;

                    /* resest error detection counter */
                    count = 2 * nv;
                }
            }
            return true;
        }
    }
}