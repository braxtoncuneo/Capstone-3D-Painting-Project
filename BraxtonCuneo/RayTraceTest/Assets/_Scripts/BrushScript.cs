using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BrushScript : MonoBehaviour {

    Transform tform;
    public ComputeShader BasicBrush;
    public List<BlockScript> blocks;
   // public Color brushColor;

	// Use this for initialization
	void Start () {
        tform = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        int kInd = BasicBrush.FindKernel("main");
        float[] cent = new float[3];
        //float[] col = new float[4];
        float[] offset = new float[3];
        cent[0] = (float)Math.Sin(Time.time);
        cent[1] = 0;
        cent[2] = (float)Math.Cos(Time.time);
        //Debug.Log(cent[0] + "," + cent[1] + "," + cent[2]);
        tform.position.Set(cent[0], cent[1], cent[2]);
        BasicBrush.SetFloats("brushCenter", cent);

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
