using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeState : MonoBehaviour
{
    World world;
    public Controller controller;
    private int w = 1;
    
    //load in world
    void Start ()
    {
        world = gameObject.GetComponent("World") as World;
    }
	
    //check for user input
	void Update ()
    {
        //if input is received, check controller position and change voxel state at the controller coords
        if (Input.GetButton("Blue"))
        {
            changeState(controller.x, controller.y + 1, controller.z, 1, w);
        }

        else if (Input.GetButton("Green"))
        {
            changeState(controller.x, controller.y + 1, controller.z, 2, w);
        }

        else if (Input.GetButton("Red"))
        {
            changeState(controller.x, controller.y + 1, controller.z, 3, w);
        }

        else if (Input.GetButton("Yellow"))
        {
            changeState(controller.x, controller.y + 1, controller.z, 4, w);
        }

        else if (Input.GetButton("Destroy"))
        {
            changeState(controller.x, controller.y + 1, controller.z, 0, w);
        }

        else if (Input.GetButtonDown("WeightUp"))
        {
            w++;
            Debug.Log("WeightUp");
        }

        else if (Input.GetButtonDown("WeightDown"))
        {
            if (w != 1)
                w--;
        }

        //reload current chunks
        LoadChunks(GameObject.FindGameObjectWithTag("Player").transform.position, 200, 300);
    }

    public void changeState(int x, int y, int z, int block, int weight)
    {
        //adds the specified block at these coordinates

        print("Adding: " + x + ", " + y + ", " + z);
        for (int i = 0; i < weight; i ++)
        {
            for (int j = 0; j < weight; j++)
            {
                for (int k = 0; k < weight; k++)
                {
                    world.vState[x + i - w / 2, y + j - w / 2, z + k - w / 2] = block;
                    UpdateChunk(x, y, z);
                }
            }
        }
    }

    //updates chunks after modifying a voxel
    public void UpdateChunk(int x, int y, int z)
    {
        //Updates the chunk containing this block

        int updateX = Mathf.FloorToInt(x / world.chunkSize);
        int updateY = Mathf.FloorToInt(y / world.chunkSize);
        int updateZ = Mathf.FloorToInt(z / world.chunkSize);

        print("Updating: " + updateX + ", " + updateY + ", " + updateZ);

        world.chunks[updateX, updateY, updateZ].update = true;

        //check each loaded chunk to see if an update needs to be called
        if (x - (world.chunkSize * updateX) == 0 && updateX != 0)
        {
            world.chunks[updateX - 1, updateY, updateZ].update = true;
        }

        if (x - (world.chunkSize * updateX) == 15 && updateX != world.chunks.GetLength(0) - 1)
        {
            world.chunks[updateX + 1, updateY, updateZ].update = true;
        }

        if (y - (world.chunkSize * updateY) == 0 && updateY != 0)
        {
            world.chunks[updateX, updateY - 1, updateZ].update = true;
        }

        if (y - (world.chunkSize * updateY) == 15 && updateY != world.chunks.GetLength(1) - 1)
        {
            world.chunks[updateX, updateY + 1, updateZ].update = true;
        }

        if (z - (world.chunkSize * updateZ) == 0 && updateZ != 0)
        {
            world.chunks[updateX, updateY, updateZ - 1].update = true;
        }

        if (z - (world.chunkSize * updateZ) == 15 && updateZ != world.chunks.GetLength(2) - 1)
        {
            world.chunks[updateX, updateY, updateZ + 1].update = true;
        }
    }

    //loads chunks that are near to the user and unloads those that have moved away
    public void LoadChunks(Vector3 playerPos, float loadDist, float unloadDist)
    {
        for (int x = 0; x < world.chunks.GetLength(0); x++)
        {
            for (int z = 0; z < world.chunks.GetLength(2); z++)
            {
                float dist = Vector2.Distance(new Vector2(x * world.chunkSize, z * world.chunkSize), new Vector2(playerPos.x, playerPos.z));

                if (dist < loadDist)
                {
                    if (world.chunks[x, 0, z] == null)
                    {
                        world.GenColumn(x, z);
                    }
                }
                else if (dist > unloadDist)
                {
                    if (world.chunks[x, 0, z] != null)
                    {
                        world.UnloadColumn(x, z);
                    }
                }
            }
        }

    }
}
