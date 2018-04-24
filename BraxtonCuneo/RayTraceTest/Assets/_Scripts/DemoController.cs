using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DemoController : MonoBehaviour {

    public Block blockPrefab;
    public Brush controlledBrush;
    Vector4 lastColor;
    bool done;

    // Use this for initialization
    void Start () {
        lastColor = new Vector4(0, 0, 0, 0);
        makeGrid(2);
        controlledBrush.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        controlledBrush.Down(null);
        done = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (Time.time < 15)
        {
            float m = 1.0f;

            Vector4 nowColor = new Vector4((float)Math.Sin(Time.time * 2.3),
                                                (float)Math.Sin(Time.time * 1.3),
                                                (float)Math.Sin(Time.time * 1.7),
                                                1.0f);

            controlledBrush.transform.SetPositionAndRotation(

                    new Vector3(
                        1 + (float)(Math.Sin(Time.time * 2.0 * m) * Math.Cos(Time.time * 3.5 * m)),
                        1 + (float)Math.Sin(Time.time * 0.23125 * m),
                        1 + (float)(Math.Cos(Time.time * 4.0 * m) * Math.Sin(Time.time * 1.2 * m))
                    ),
                    //new Quaternion()
                    
                    new Quaternion(((float)Math.Cos(Time.time * 0.93 * m)) * 10.0f,
                                    ((float)Math.Cos(Time.time * 1.11 * m)) * 10.0f,
                                    ((float)Math.Cos(Time.time * 1.45 * m)) * 10.0f,
                                    ((float)Math.Cos(Time.time * 1.76 * m)) * 10.0f)


            );
            controlledBrush.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            
            controlledBrush.transform.localScale = 
                    new Vector3(    ((float)Math.Cos(Time.time * 0.73 * m) + 1.0f) * 0.1f,
                                    ((float)Math.Cos(Time.time * 1.42 * m) + 1.0f) * 0.1f,
                                    ((float)Math.Cos(Time.time * 1.05 * m) + 1.0f) * 0.1f
                    );
            controlledBrush.Stroke(lastColor, nowColor, new Vector4(0, 0, 0, 0));
            lastColor = nowColor;
        }
        else if(!done)
        {
            controlledBrush.Up();
            done = true;
        }
    }


    void makeGrid(int gWidth)
    {
        Block b;
        float w = Block.blockWidth;
        for(int z = 0; z < gWidth; z++)
        {
            for(int y = 0; y < gWidth; y++)
            {
                for(int x = 0; x < gWidth; x++)
                {
                    b = Instantiate<Block>(blockPrefab);
                    b.transform.position = new Vector3(x*w,y*w,z*w);
                    controlledBrush.blocks.Add(b);
                }
            }
        }
    }


}
