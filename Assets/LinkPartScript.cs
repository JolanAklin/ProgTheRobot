using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkPartScript : MonoBehaviour
{
    public Transform a;
    public Transform b;
    public Transform c;
    public Transform d;
    public Transform abcd;
    private float interpolateAmount;

    public MeshFilter meshFilter;
    public float meshWidth;

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
        List<Square> squares = new List<Square>();
        for (int i = 0; i <= 100; i++)
        {
            Vector3 startPos = CubicLerp1(a.transform.position, b.transform.position, c.transform.position, d.transform.position, ((float)i) / 100);
            Vector3 dir = CubicLerp1(a.transform.position, b.transform.position, c.transform.position, d.transform.position, ((float)i + 1) / 100) - startPos;
            Square square = new Square();
            square.GeneratePoint(startPos, dir, meshWidth);
            squares.Add(square);
        }
        meshFilter.mesh = CreateMeshFromSquare(squares);
    }

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i <= 100; i++)
    //    {
    //        Gizmos.DrawLine(CubicLerp1(a.transform.position, b.transform.position, c.transform.position, d.transform.position, ((float)i) / 100), CubicLerp1(a.transform.position, b.transform.position, c.transform.position, d.transform.position, ((float)i + 1) / 100));
    //    }

    //    // get all the point needed to create a mesh
    //    Vector3 test = CubicLerp1(a.transform.position, b.transform.position, c.transform.position, d.transform.position, 0.2f) - CubicLerp1(a.transform.position, b.transform.position, c.transform.position, d.transform.position, 0.1f);
    //    Square square = new Square();
    //    square.GeneratePoint(CubicLerp1(a.transform.position, b.transform.position, c.transform.position, d.transform.position, 0.1f), test, meshWidth);
    //    Gizmos.color = Color.red;
    //    foreach (Vector3 point in square.getPoints())
    //    {
    //        Gizmos.DrawSphere(point, 0.02f);
    //    }
    //    //Gizmos.color = Color.red;
    //    //Vector3 testPerpendicular = new Vector3(-test.y, test.x).normalized * meshWidth;
    //    //Vector3 up = test + testPerpendicular;
    //    //Vector3 down = test - testPerpendicular;
    //    //Gizmos.DrawLine(Vector3.zero, test);
    //    //Gizmos.DrawLine(Vector3.zero, testPerpendicular);
    //    //Gizmos.DrawLine(testPerpendicular, up);
    //    //Gizmos.DrawLine(-testPerpendicular, down);
    //}

    //private void Update()
    //{
    //    interpolateAmount = (interpolateAmount + Time.deltaTime) % 1f;

    //    abcd.position = CubicLerp(a.transform.position, b.transform.position, c.transform.position, d.transform.position, interpolateAmount);
    //}

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
        //{
        //        // lower left triangle
        //        0, 2, 1,
        //        // upper right triangle
        //        2, 3, 1
        //};
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

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
                new Vector3(0, 0, 0),
                new Vector3(meshWidth, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(meshWidth, 1, 0)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
                // lower left triangle
                0, 2, 1,
                // upper right triangle
                2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
        };
        mesh.uv = uv;

        return mesh;
    }

    #region spline stuff
    private Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(ab, bc, interpolateAmount);
    }

    private Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 ab_bc = QuadraticLerp(a, b, c, t);
        Vector3 bc_cd = QuadraticLerp(b, c, d, t);

        return Vector3.Lerp(ab_bc, bc_cd, interpolateAmount);
    }

    private Vector3 QuadraticLerp1(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(ab, bc, t);
    }

    private Vector3 CubicLerp1(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 ab_bc = QuadraticLerp1(a, b, c, t);
        Vector3 bc_cd = QuadraticLerp1(b, c, d, t);

        return Vector3.Lerp(ab_bc, bc_cd, t);
    }
    #endregion
}
