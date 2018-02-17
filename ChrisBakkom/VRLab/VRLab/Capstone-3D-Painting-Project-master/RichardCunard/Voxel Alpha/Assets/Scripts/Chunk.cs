using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private List<Vector3> tempVerts = new List<Vector3>();
    private List<int> tempTri = new List<int>();
    private List<Vector2> tempUV = new List<Vector2>();

    private float textureOffset = 0.25f;
    private Vector2 blue = new Vector2(2.0f, 0.0f);
    private Vector2 green = new Vector2(0.0f, 2.0f);
    private Vector2 red = new Vector2(0.0f, 0.0f);
    private Vector2 yellow = new Vector2(2.0f, 2.0f);

    private Mesh mesh;
    private World world;
    private int polyCount;

    public int chunkSize = 100;
    public GameObject worldReference;

    public int chunkX;
    public int chunkY;
    public int chunkZ;


    public bool update;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        world = worldReference.GetComponent("World") as World;
        GenerateMesh();
    }

    void LateUpdate()
    {
        if (update)
        {
            GenerateMesh();
            update = false;
        }
    }

    void CubeTop(int x, int y, int z, int block)
    {
        tempVerts.Add(new Vector3(x, y, z + 1));
        tempVerts.Add(new Vector3(x + 1, y, z + 1));
        tempVerts.Add(new Vector3(x + 1, y, z));
        tempVerts.Add(new Vector3(x, y, z));

        Vector2 color = new Vector2(0, 0);

        if (Block(x, y, z) == 1)
        {
            color = blue;
        }
        else if (Block(x, y, z) == 2)
        {
            color = green;
        }
        else if (Block(x, y, z) == 3)
        {
            color = red;
        }
        else if (Block(x, y, z) == 4)
        {
            color = yellow;
        }

        Cube(color);
    }

    void CubeFront(int x, int y, int z, int block)
    {
        tempVerts.Add(new Vector3(x + 1, y - 1, z + 1));
        tempVerts.Add(new Vector3(x + 1, y, z + 1));
        tempVerts.Add(new Vector3(x, y, z + 1));
        tempVerts.Add(new Vector3(x, y - 1, z + 1));

        Vector2 color = new Vector2(0, 0);

        if (Block(x, y, z) == 1)
        {
            color = blue;
        }
        else if (Block(x, y, z) == 2)
        {
            color = green;
        }
        else if (Block(x, y, z) == 3)
        {
            color = red;
        }
        else if (Block(x, y, z) == 4)
        {
            color = yellow;
        }

        Cube(color);

    }

    void CubeRight(int x, int y, int z, int block)
    {
        tempVerts.Add(new Vector3(x + 1, y - 1, z));
        tempVerts.Add(new Vector3(x + 1, y, z));
        tempVerts.Add(new Vector3(x + 1, y, z + 1));
        tempVerts.Add(new Vector3(x + 1, y - 1, z + 1));

        Vector2 color = new Vector2(0, 0);

        if (Block(x, y, z) == 1)
        {
            color = blue;
        }
        else if (Block(x, y, z) == 2)
        {
            color = green;
        }
        else if (Block(x, y, z) == 3)
        {
            color = red;
        }
        else if (Block(x, y, z) == 4)
        {
            color = yellow;
        }

        Cube(color);

    }

    void CubeBack(int x, int y, int z, int block)
    {
        tempVerts.Add(new Vector3(x, y - 1, z));
        tempVerts.Add(new Vector3(x, y, z));
        tempVerts.Add(new Vector3(x + 1, y, z));
        tempVerts.Add(new Vector3(x + 1, y - 1, z));

        Vector2 color = new Vector2(0, 0);

        if (Block(x, y, z) == 1)
        {
            color = blue;
        }
        else if (Block(x, y, z) == 2)
        {
            color = green;
        }
        else if (Block(x, y, z) == 3)
        {
            color = red;
        }
        else if (Block(x, y, z) == 4)
        {
            color = yellow;
        }

        Cube(color);

    }

    void CubeLeft(int x, int y, int z, int block)
    {
        tempVerts.Add(new Vector3(x, y - 1, z + 1));
        tempVerts.Add(new Vector3(x, y, z + 1));
        tempVerts.Add(new Vector3(x, y, z));
        tempVerts.Add(new Vector3(x, y - 1, z));

        Vector2 color = new Vector2(0, 0);

        if (Block(x, y, z) == 1)
        {
            color = blue;
        }
        else if (Block(x, y, z) == 2)
        {
            color = green;
        }
        else if (Block(x, y, z) == 3)
        {
            color = red;
        }
        else if (Block(x, y, z) == 4)
        {
            color = yellow;
        }

        Cube(color);

    }

    void CubeBot(int x, int y, int z, int block)
    {
        tempVerts.Add(new Vector3(x, y - 1, z));
        tempVerts.Add(new Vector3(x + 1, y - 1, z));
        tempVerts.Add(new Vector3(x + 1, y - 1, z + 1));
        tempVerts.Add(new Vector3(x, y - 1, z + 1));

        Vector2 color = new Vector2(0, 0);

        if (Block(x, y, z) == 1)
        {
            color = blue;
        }
        else if (Block(x, y, z) == 2)
        {
            color = green;
        }
        else if (Block(x, y, z) == 2)
        {
            color = red;
        }
        else if (Block(x, y, z) == 2)
        {
            color = yellow;
        }

        Cube(color);

    }

    void Cube(Vector2 color)
    {
        tempTri.Add(polyCount * 4); 
        tempTri.Add(polyCount * 4 + 1); 
        tempTri.Add(polyCount * 4 + 2); 
        tempTri.Add(polyCount * 4); 
        tempTri.Add(polyCount * 4 + 2); 
        tempTri.Add(polyCount * 4 + 3); 

        tempUV.Add(new Vector2(textureOffset * color.x + textureOffset, textureOffset * color.y));
        tempUV.Add(new Vector2(textureOffset * color.x + textureOffset, textureOffset * color.y + textureOffset));
        tempUV.Add(new Vector2(textureOffset * color.x, textureOffset * color.y + textureOffset));
        tempUV.Add(new Vector2(textureOffset * color.x, textureOffset * color.y));

        polyCount++;
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = tempVerts.ToArray();
        mesh.uv = tempUV.ToArray();
        mesh.triangles = tempTri.ToArray();
        mesh.RecalculateNormals();

        tempVerts.Clear();
        tempUV.Clear();
        tempTri.Clear();

        polyCount = 0; //Fixed: Added this thanks to a bug pointed out by ratnushock!
    }

    //Renders any voxel side that is visible
    public void GenerateMesh()
    {

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    //This code will run for every block in the chunk

                    if (Block(x, y, z) != 0)
                    {
                        //If the block is solid

                        if (Block(x, y + 1, z) == 0)
                        {
                            //Block above is air
                            CubeTop(x, y, z, 0);
                        }

                        if (Block(x, y - 1, z) == 0)
                        {
                            //Block below is air
                            CubeBot(x, y, z, Block(x, y, z));

                        }

                        if (Block(x + 1, y, z) == 0)
                        {
                            //Block east is air
                            CubeRight(x, y, z, Block(x, y, z));

                        }

                        if (Block(x - 1, y, z) == 0)
                        {
                            //Block west is air
                            CubeLeft(x, y, z, Block(x, y, z));

                        }

                        if (Block(x, y, z + 1) == 0)
                        {
                            //Block north is air
                            CubeFront(x, y, z, Block(x, y, z));

                        }

                        if (Block(x, y, z - 1) == 0)
                        {
                            //Block south is air
                            CubeBack(x, y, z, Block(x, y, z));

                        }

                    }

                }
            }
        }

        UpdateMesh();
    }

    int Block(int x, int y, int z)
    {
        return world.Block(x + chunkX, y + chunkY, z + chunkZ);
    }
}
