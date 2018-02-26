using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class World : MonoBehaviour
{
    public int[,,] vState;
    public int worldX = 16;
    public int worldY = 16;
    public int worldZ = 16;
    public GameObject chunk;
    public Chunk[,,] chunks;
    public int chunkSize = 16;
    public float scale;

    void Start ()
    {
        //initialize a byte array using the predefined world size
        vState = new int[worldX, worldY, worldZ];

        //fill world with blocks
        for (int x = 0; x < worldX; x++)
        {
            //Generate a 'starting canvas' for testing purposes
            for (int y = 0; y < worldY; y++)
            {
                for (int z = 0; z < worldZ; z++)
                {
                    if (0 == y)
                        vState[x, y, z] = 1;
                    else
                        vState[x, y, z] = 0;
                }
            }
        }

        chunks = new Chunk[Mathf.FloorToInt(worldX / chunkSize),
        Mathf.FloorToInt(worldY / chunkSize),
        Mathf.FloorToInt(worldZ / chunkSize)];
    }

    public bool checkCoords(int x, int y, int z)
    {
        if (0 == y)
            return true;
        if (1 == y)
        {
            if (x != 0 && z != 0)
                return true;
        }
        return false;
    }
	
	void Update ()
    {
		
	}

    public int Block(int x, int y, int z)
    {

        if (x >= worldX || x < 0 || y >= worldY || y < 0 || z >= worldZ || z < 0)
        {
            return (byte)1;
        }

        return vState[x, y, z];
    }

    public void GenColumn(int x, int z)
    {
        for (int y = 0; y < chunks.GetLength(1); y++)
        {
            //instantiates chunks as game objects for easy spawning and despawning
            GameObject newChunk = Instantiate(chunk, new Vector3(scale * x * chunkSize - 0.5f * scale, scale * y * chunkSize + 0.5f * scale, scale * z * chunkSize - 0.5f * scale), new Quaternion(0, 0, 0, 0)) as GameObject; 
            chunks[x, y, z] = newChunk.GetComponent("Chunk") as Chunk;
            chunks[x, y, z].worldReference = gameObject;
            chunks[x, y, z].chunkSize = chunkSize;
            chunks[x, y, z].chunkX = x * chunkSize;
            chunks[x, y, z].chunkY = y * chunkSize;
            chunks[x, y, z].chunkZ = z * chunkSize;
        }
    }

    public void UnloadColumn(int x, int z)
    {
        for (int y = 0; y < chunks.GetLength(1); y++)
        {
            //Object.Destroy(chunks[x, y, z].gameObject);
        }
    }
}
