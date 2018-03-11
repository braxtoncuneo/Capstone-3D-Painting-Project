using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BrushScript : MonoBehaviour {

    Matrix4x4 last;
    Vector4 lastColor;
    public ComputeShader BasicBrush;
    public List<BlockScript> blocks;
   // public Color brushColor;

	// Use this for initialization
	void Start () {
        last = transform.worldToLocalMatrix;
        BasicBrush.SetInt("texWidth",BlockScript.width);
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
            1 + (float)(Math.Sin(Time.time * 2.0) * Math.Cos(Time.time * 3.5)),
            1 + (float)Math.Sin(Time.time * 0.23125),
            1 + (float)(Math.Cos(Time.time * 4.0) * Math.Sin(Time.time * 1.2))
        );

        Vector4 nowColor = new Vector4((float)Math.Sin(Time.time * 2.3), (float)Math.Sin(Time.time * 1.3), (float)Math.Sin(Time.time * 1.7), 128.0f);
        
        setStartColor(lastColor);
        setEndColor(nowColor);
        lastColor = nowColor;


        float m = 1.0f;

        transform.SetPositionAndRotation(
                new Vector3(
                    1 + (float)(Math.Sin(Time.time * 2.0 * m) * Math.Cos(Time.time * 3.5 * m)),
                    1 + (float)Math.Sin(Time.time * 0.23125 * m),
                    1 + (float)(Math.Cos(Time.time * 4.0 * m) * Math.Sin(Time.time * 1.2 * m))
                ), 
                new Quaternion()
        );

        Matrix4x4 current = transform.worldToLocalMatrix;
        Debug.Log(current);
        BasicBrush.SetMatrix("start", last);
        BasicBrush.SetMatrix("end", current);
        last = current;

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
