using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SplineMaker : MonoBehaviour
{
    private MeshFilter meshFilter;
    public float meshWidth;

    public List<SplineSegment> splineSegments = new List<SplineSegment>();

    // spline segment, while take 2 point
    [Serializable]
    public class SplineSegment
    {
        public SplinePoint splineStart;
        public SplinePoint splineEnd;
    }

    // spline point
    [Serializable]
    public class SplinePoint
    {
        public Vector3 point;
        public Vector3 handle;
    }

    // used to generate the mesh
    public class Square
    {
        private Vector3[] points;
        private Vector2[] uv;

        public void GeneratePoint(Vector3 startPos, Vector3 dir, float meshWidth)
        {
            Vector3 perpendicular = new Vector3(-dir.y, dir.x).normalized * meshWidth;
            points = new Vector3[4];
            points[0] = perpendicular + startPos;              // up left
            points[1] = dir + perpendicular + startPos;        // up right
            points[2] = -perpendicular + startPos;             // down left
            points[3] = dir - perpendicular + startPos;        // down right

            uv = new Vector2[4]
            {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1)
            };
        }

        public Vector3[] getPoints()
        {
            return (Vector3[])points.Clone();
        }
        public Vector2[] getUv()
        {
            return (Vector2[])uv.Clone();
        }
    }

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        GenerateMesh();
    }

    public void GenerateMesh()
    {
        List<Square> squares = new List<Square>();
        foreach (SplineSegment splineSegment in splineSegments)
        {
            for (int i = 0; i <= 100; i++)
            {
                Vector3 startPos = CubicLerp(splineSegment.splineStart.point, splineSegment.splineStart.handle, splineSegment.splineEnd.handle, splineSegment.splineEnd.point, ((float)i) / 100);
                Vector3 dir = CubicLerp(splineSegment.splineStart.point, splineSegment.splineStart.handle, splineSegment.splineEnd.handle, splineSegment.splineEnd.point, ((float)i + 1) / 100) - startPos;
                Square square = new Square();
                square.GeneratePoint(startPos, dir, meshWidth);
                squares.Add(square);
            }
        }
        meshFilter.mesh = CreateMeshFromSquare(squares);
    }

    public Mesh CreateMeshFromSquare(List<Square> squares)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[squares.Count * 4];
        int i = 0;
        foreach (Square square in squares)
        {
            foreach (Vector3 point in square.getPoints())
            {
                vertices[i] = point;
                i++;
            }
        }
        mesh.vertices = vertices;

        int[] tris = new int[squares.Count * 6];
        i = 0;
        int k = 0;
        foreach (Square square in squares)
        {
            tris[i] = 1 + k;
            tris[i+1] = 2 + k;
            tris[i+2] = 0 + k;

            tris[i+3] = 1 + k;
            tris[i+4] = 3 + k;
            tris[i+5] = 2 + k;
            k += 4;
            i += 6;
        }
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[squares.Count * 4];
        for (int j = 0; j < normals.Length; j++)
        {
            normals[j] = -Vector3.forward;
        }
        mesh.normals = normals;

        Vector2[] uv = new Vector2[squares.Count * 4];
        i = 0;
        foreach (Square square in squares)
        {
            foreach (Vector3 pointUv in square.getUv())
            {
                uv[i] = pointUv;
                i++;
            }
        }
        mesh.uv = uv;

        return mesh;
    }

    #region spline stuff
    private Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(ab, bc, t);
    }

    private Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 ab_bc = QuadraticLerp(a, b, c, t);
        Vector3 bc_cd = QuadraticLerp(b, c, d, t);

        return Vector3.Lerp(ab_bc, bc_cd, t);
    }
    #endregion
}
