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
    public class Line
    {
        private Vector3[] points;
        private Vector2[] uv;

        public void GeneratePoint(Vector3 startPos, Vector3 dir, float meshWidth)
        {
            Vector3 perpendicular = new Vector3(-dir.y, dir.x).normalized * meshWidth;
            points = new Vector3[2];
            points[0] = perpendicular + startPos;              // up left
            points[1] = -perpendicular + startPos;             // down left
        }

        public Vector3[] getPoints()
        {
            return (Vector3[])points.Clone();
        }
    }

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        GenerateMesh();
    }

    public void GenerateMesh()
    {
        List<Line> lines = new List<Line>();
        foreach (SplineSegment splineSegment in splineSegments)
        {
            for (int i = 0; i <= 100; i++)
            {
                Vector3 startPos = CubicLerp(splineSegment.splineStart.point, splineSegment.splineStart.handle, splineSegment.splineEnd.handle, splineSegment.splineEnd.point, ((float)i) / 100);
                Vector3 dir = Vector3.zero;
                if(i == 100)
                    dir = startPos - CubicLerp(splineSegment.splineStart.point, splineSegment.splineStart.handle, splineSegment.splineEnd.handle, splineSegment.splineEnd.point, ((float)i - 1) / 100);
                else
                    dir = CubicLerp(splineSegment.splineStart.point, splineSegment.splineStart.handle, splineSegment.splineEnd.handle, splineSegment.splineEnd.point, ((float)i + 1) / 100) - startPos;
                Line line = new Line();
                line.GeneratePoint(startPos, dir, meshWidth);
                lines.Add(line);
            }
        }
        meshFilter.mesh = CreateMeshFromLine(lines);
    }

    public Mesh CreateMeshFromLine(List<Line> lines)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[lines.Count * 2];
        int i = 0;
        foreach (Line square in lines)
        {
            foreach (Vector3 point in square.getPoints())
            {
                vertices[i] = point;
                i++;
            }
        }
        mesh.vertices = vertices;

        int[] tris = new int[(lines.Count - 1) * 6];
        i = 0;
        int k = 0;
        for (int j = 0; j < lines.Count - 1; j++)
        {
            tris[i] = 0 + k;
            tris[i+1] = 2 + k;
            tris[i+2] = 1 + k;

            tris[i+3] = 2 + k;
            tris[i+4] = 3 + k;
            tris[i+5] = 1 + k;
            k += 2;
            i += 6;
        }
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[lines.Count * 2];
        for (int j = 0; j < normals.Length; j++)
        {
            normals[j] = -Vector3.forward;
        }
        mesh.normals = normals;

        Vector2[] uv = new Vector2[lines.Count * 2];
        i = 0;
        bool changeUvPos = false;
        foreach (Line square in lines)
        {
            if(changeUvPos)
            {
                uv[i] = new Vector2(1, 1);
                uv[1 + 1] = new Vector2(1, 0);
                changeUvPos = false;
            }
            else
            {
                uv[i] = new Vector2(0,1);
                uv[1 + 1] = new Vector2(0, 0);
                changeUvPos = true;
            }
            i+=2;
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
