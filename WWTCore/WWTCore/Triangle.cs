using System.Collections.Generic;

namespace TerraViewer
{
    public struct Triangle
    {
        // Vertex Indexies
        public int A;
        public int B;
        public int C;

        public Triangle(int a, int b, int c)
        {
            A = a;
            B = b;
            C = c;
        }



        public void SubDivide(List<Triangle> triList, List<PositionTexture> vertexList)
        {
            var a1 = Vector3d.Lerp(vertexList[B].Position, vertexList[C].Position, .5f);
            var b1 = Vector3d.Lerp(vertexList[C].Position, vertexList[A].Position, .5f);
            var c1 = Vector3d.Lerp(vertexList[A].Position, vertexList[B].Position, .5f);

            var a1uv = Vector2d.Lerp(new Vector2d(vertexList[B].Tu, vertexList[B].Tv), new Vector2d(vertexList[C].Tu, vertexList[C].Tv), .5f);
            var b1uv = Vector2d.Lerp(new Vector2d(vertexList[C].Tu, vertexList[C].Tv), new Vector2d(vertexList[A].Tu, vertexList[A].Tv), .5f);
            var c1uv = Vector2d.Lerp(new Vector2d(vertexList[A].Tu, vertexList[A].Tv), new Vector2d(vertexList[B].Tu, vertexList[B].Tv), .5f);

            a1.Normalize();
            b1.Normalize();
            c1.Normalize();

            var aIndex = vertexList.Count;
            var bIndex = vertexList.Count + 1;
            var cIndex = vertexList.Count + 2;

            vertexList.Add(new PositionTexture(a1, a1uv.X, a1uv.Y));
            vertexList.Add(new PositionTexture(b1, b1uv.X, b1uv.Y));
            vertexList.Add(new PositionTexture(c1, c1uv.X, c1uv.Y));

            triList.Add(new Triangle(A, cIndex, bIndex));
            triList.Add(new Triangle(B, aIndex, cIndex));
            triList.Add(new Triangle(C, bIndex, aIndex));
            triList.Add(new Triangle(aIndex, bIndex, cIndex));
        }
    }
}
