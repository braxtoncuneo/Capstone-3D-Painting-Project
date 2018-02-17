using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block: MonoBehaviour
{
    private GenCollider colGen;
    private Matrix4x4 Transform;
    private Mesh mesh;
    private MeshCollider col;
    private List<GameObject> children = new List<GameObject>();
    private GameObject child;
    public GameObject prefab;
    public float size;
    private int x = 0;

    void Start()
    {
        //size = 1f;
        //mesh = GetComponent<MeshFilter>().mesh;
        //col = GetComponent<MeshCollider>();
        //Transform = Matrix4x4.identity;
        //colGen = new GenCollider(size, Transform, mesh, col);
        spawnChild(size, Matrix4x4.identity);
    }

    void setVal(float s)
    {
        size *= 2f;
        mesh = GetComponent<MeshFilter>().mesh;
        col = GetComponent<MeshCollider>();
        colGen = new GenCollider(2 * s, Transform, mesh, col);
        Debug.Log("Test");
    }

    //Creates a new instance of block in the child array using the class constructor. 
    //Requires the child's new transformation matrix and base size
    public void spawnChild(float S, Matrix4x4 T)
    {
        child = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        Block reference = prefab.GetComponent<Block>();
        reference.setVal(S);
        children.Add(child);
    }

    //returns a Vector4 copy of a vector3 list of vertices with a 1 in the w column
    private List<Vector4> vec3To4(List<Vector3> pts)
    {
        List<Vector4> newVector = new List<Vector4>();
        
        foreach (Vector3 pt in pts)
        {
            newVector.Add(new Vector4(pt.x, pt.y, pt.z, 1));
        }

        return newVector;
    }

    //returns a Vector3 copy of a vector4 list deleting the w column
    private List<Vector3> vec4To3(List<Vector3> pts)
    {
        List<Vector3> newVector = new List<Vector3>();

        foreach (Vector4 pt in pts)
        {
            newVector.Add(new Vector3(pt.x, pt.y, pt.z));
        }

        return newVector;
    }

    private Vector3 matMultiply(Matrix4x4 X, Vector4 A)
    {
        Vector4 temp = new Vector4();
        for (int i = 0; i < 4; i++)
        {
            temp[i] = X[i, 0] * A.x + X[i,1] * A.y + X[i, 2] * A.z + X[i, 3] * A[3];
        }
        return new Vector3(temp.x, temp.y, temp.z);
    }

    private void transformBlock()
    {
        List<Vector4> temp = vec3To4(colGen.colVertices);
        List<Vector3> newBlock = new List<Vector3>();
        
        //apply the transformation across the points in the array
        foreach (Vector4 pt in temp)
        {
            newBlock.Add(matMultiply(Transform, pt)); 
        }
    }

    private Matrix4x4 shear(float shearA,float shearB, float shearC, float shearD, float shearE, float shearF)
    {
        return Matrix4x4.identity;
    }
}
