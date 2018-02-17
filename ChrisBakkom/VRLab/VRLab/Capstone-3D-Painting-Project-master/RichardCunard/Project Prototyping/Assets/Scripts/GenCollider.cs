using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenCollider
{
    public List<Vector3> newVertices = new List<Vector3>();
    public List<int> newTriangles = new List<int>();
    public List<Vector2> newUV = new List<Vector2>();
    public List<Vector3> colVertices = new List<Vector3>();
    public List<int> colTriangles = new List<int>();

    private Mesh mesh;
    private int squareCount;
    private int colCount;
    private MeshCollider col;

    public GenCollider(float size, Matrix4x4 transform, Mesh m, MeshCollider c)
    {
        mesh = m;
        col = c;
        mesh.Clear();
        BuildCollider(size, transform);
    }

    void ColliderTriangles()
    {
        colTriangles.Add(colCount * 4);
        colTriangles.Add((colCount * 4) + 1);
        colTriangles.Add((colCount * 4) + 3);
        colTriangles.Add((colCount * 4) + 1);
        colTriangles.Add((colCount * 4) + 2);
        colTriangles.Add((colCount * 4) + 3);
    }

    //Generates a collider for a block given its size and transformation matrix
    public void BuildCollider(float size, Matrix4x4 transform)
    {
        //begins by manually writing a non-transformed cube out of triangles
        //Top
        colVertices.Add(new Vector3(size / 2f, -size / 2f, -size / 2f));
        colVertices.Add(new Vector3(-size / 2f, -size / 2f, -size / 2f));
        colVertices.Add(new Vector3(-size / 2f, -size / 2f, size / 2f));
        colVertices.Add(new Vector3(size / 2f, -size / 2f, size / 2f));

        ColliderTriangles();

        colCount++;

        //bot
        colVertices.Add(new Vector3(-size / 2f, size / 2f, size / 2f));
        colVertices.Add(new Vector3(size / 2f, size / 2f, size / 2f));
        colVertices.Add(new Vector3(size / 2f, size / 2f, -size / 2f));
        colVertices.Add(new Vector3(-size / 2f, size / 2f, -size / 2f));

        ColliderTriangles();
        colCount++;

        //front
        colVertices.Add(new Vector3(-size / 2f, -size / 2f, -size / 2f));
        colVertices.Add(new Vector3(-size / 2f, size / 2f, -size / 2f));
        colVertices.Add(new Vector3(size / 2f, size / 2f, -size / 2f));
        colVertices.Add(new Vector3(size / 2f, -size / 2f, -size / 2f));

        ColliderTriangles();

        colCount++;

        //back
        colVertices.Add(new Vector3(-size / 2f, -size / 2f, size / 2f));
        colVertices.Add(new Vector3(-size / 2f, size / 2f, size / 2f));
        colVertices.Add(new Vector3(size / 2f, size / 2f, size / 2f));
        colVertices.Add(new Vector3(size / 2f, -size / 2f, size / 2f));

        ColliderTriangles();

        colCount++;

        //left
        colVertices.Add(new Vector3(-size / 2f, size / 2f, -size / 2f));
        colVertices.Add(new Vector3(-size / 2f, -size / 2f, -size / 2f));
        colVertices.Add(new Vector3(-size / 2f, -size / 2f, size / 2f));
        colVertices.Add(new Vector3(-size / 2f, size / 2f, size / 2f));

        ColliderTriangles();

        colCount++;

        //right
        colVertices.Add(new Vector3(size / 2f, -size / 2f, size / 2f));
        colVertices.Add(new Vector3(size / 2f, size / 2f, size / 2f));
        colVertices.Add(new Vector3(size / 2f, size / 2f, -size / 2f));
        colVertices.Add(new Vector3(size / 2f, -size / 2f, -size / 2f));



        ColliderTriangles();

        colCount++;

        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.RecalculateNormals();

        squareCount = 0;
        newVertices.Clear();
        newTriangles.Clear();
        newUV.Clear();

        Mesh newMesh = new Mesh();
        newMesh.vertices = colVertices.ToArray();
        newMesh.triangles = colTriangles.ToArray();
        col.sharedMesh = newMesh;

        colVertices.Clear();
        colTriangles.Clear();
        colCount = 0;
    }
}
