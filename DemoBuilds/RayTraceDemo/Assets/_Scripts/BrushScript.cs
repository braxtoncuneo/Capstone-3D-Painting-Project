using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BrushScript : MonoBehaviour {

    public BlockScript BlockPrefab;
    Matrix4x4 last;
    Vector4 lastColor;
    int gridWidth = 6; //$ The number of blocks to a side in the grid
    public ComputeShader BasicBrush;
    public List<BlockScript> blocks;
   // public Color brushColor;

	// Use this for initialization
	void Start () {
        last = transform.worldToLocalMatrix;
        BasicBrush.SetInt("texWidth",BlockScript.width);
        BlockScript b;
        for( int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridWidth; y++)
            {
                for(int z = 0; z < gridWidth; z++)
                {
                    b = Instantiate<BlockScript>(BlockPrefab);
                    b.transform.position = new Vector3(x, y, z);
                    blocks.Add(b);
                }
            }
        }
	}

    void setCenter(float x, float y, float z)
    {
        float[] center = new float[3];
        center[0] = x;
        center[1] = y;
        center[2] = z;
        transform.position.Set(x, y, z);
        BasicBrush.SetFloats("brushCenter", center);
    }

    void setStartColor(Vector4 color)
    {
        float[] col = new float[4];
        col[0] = color.x;
        col[1] = color.y;
        col[2] = color.z;
        col[3] = color.w;
        BasicBrush.SetFloats("startColor", col);
    }

    void setEndColor(Vector4 color)
    {
        float[] col = new float[4];
        col[0] = color.x;
        col[1] = color.y;
        col[2] = color.z;
        col[3] = color.w;
        BasicBrush.SetFloats("endColor", col);
    }

    // Update is called once per frame
    void Update ()
    {
        int kInd = BasicBrush.FindKernel("main");
        float[] offset = new float[3];

        setCenter(
            (gridWidth / 2.0f) + (float)(Math.Sin(Time.time * 2.0) * Math.Cos(Time.time * 3.5)) * gridWidth,
            (gridWidth / 2.0f) + (float)Math.Sin(Time.time * 0.23125) * gridWidth,
            (gridWidth / 2.0f) + (float)(Math.Cos(Time.time * 4.0) * Math.Sin(Time.time * 1.2)) * gridWidth
        );

        //$ New color components: RGBA
        Vector4 nowColor = new Vector4( (float)Math.Sin(Time.time * 2.3), 
                                        (float)Math.Sin(Time.time * 1.3), 
                                        (float)Math.Sin(Time.time * 1.7), 
                                        (float)Math.Sin(Time.time * 3.7) + 1.0f );
        
        setStartColor(lastColor);//$ The color at the beginning of the stroke
        setEndColor(nowColor);//$ The color at the end of the stroke
        lastColor = nowColor;//$ Store the new color for later


        float m = 1.0f;

        //$ Sets position and rotation of the brush
        transform.SetPositionAndRotation(
                new Vector3(
                    (gridWidth / 2.0f) + (float)(Math.Sin(Time.time * 2.0 * m) * Math.Cos(Time.time * 3.5 * m)) * (gridWidth / 2.0f),
                    (gridWidth / 2.0f) + (float)Math.Sin(Time.time * 0.23125 * m) * (gridWidth / 2.0f),
                    (gridWidth / 2.0f) + (float)(Math.Cos(Time.time * 4.0 * m) * Math.Sin(Time.time * 1.2 * m)) * (gridWidth / 2.0f)
                ), 
                new Quaternion()
        );

        Matrix4x4 current = transform.worldToLocalMatrix; //$ The starting inverse transform for the brush
        //Debug.Log(current);

        //$ Hand over the starting and ending transform of the brush
        BasicBrush.SetMatrix("start", last);
        BasicBrush.SetMatrix("end", current);
        last = current;

        //$ Iterates through the blocks and sets up each compute shader run
        foreach(BlockScript b in blocks)
        {
            offset[0] = b.transform.position.x;
            offset[1] = b.transform.position.y;
            offset[2] = b.transform.position.z;
            BasicBrush.SetFloats("baseOffset", offset);
            b.Brush(BasicBrush, kInd);
        }

    }
}
