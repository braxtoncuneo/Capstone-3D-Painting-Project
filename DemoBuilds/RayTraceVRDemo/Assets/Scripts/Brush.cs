﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Brush : MonoBehaviour
{

    Matrix4x4 lastTransform;
    public ComputeShader CurrentBrush;
    public ComputeShader WipeBrush;

    public ComputeShader BasicBrush;
    public List<Block> blocks;
    // public Color brushColor;

    // Use this for initialization
    void Start()
    {
        lastTransform = transform.worldToLocalMatrix;
        BasicBrush.SetInt("texWidth", Block.width);
        setVector4(BasicBrush,"color0",new Vector4(0, 0, 0, 0));
        setVector4(BasicBrush, "color1",new Vector4(0, 0, 0, 0));
        setVector4(BasicBrush, "sliders", new Vector4(0, 0, 0, 0));
        transform.SetPositionAndRotation(new Vector3(0, 0, 0), new Quaternion());
        Wipe(new Vector4(0,0,0,0),new Vector4(0,0,0,0));
    }


    void setVector4(ComputeShader brush, string name, Vector4 value)
    {
        float[] val = new float[4];
        val[0] = value.x;
        val[1] = value.y;
        val[2] = value.z;
        val[3] = value.w;
        brush.SetFloats(name, val);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Down(ComputeShader brush)
    {
        if(brush != null)
        {
            CurrentBrush = brush;
        }
        else
        {
            CurrentBrush = BasicBrush;
        }
        lastTransform = transform.worldToLocalMatrix;
    }

    public void Stroke(Vector4 color0, Vector4 color1, Vector4 sliders)
    {
        if(CurrentBrush != null)
        {
            PerformBrush(CurrentBrush, color0, color1, sliders, lastTransform, transform.worldToLocalMatrix);
        }
        lastTransform = transform.worldToLocalMatrix;
    }

    public void Up()
    {
        CurrentBrush = null;
    }



    public void PerformBrush(  ComputeShader brush, Vector4 color0, Vector4 color1, Vector4 sliders,
                        Matrix4x4 startTransform, Matrix4x4 endTransform)
    {
        int kInd = BasicBrush.FindKernel("main");
        float[] offset = new float[3];

        setVector4(brush,"color0", color0);
        setVector4(brush, "color1", color1);
        setVector4(brush, "sliders", sliders);

        brush.SetMatrix("start", startTransform);
        brush.SetMatrix("end", endTransform);

        foreach (Block b in blocks)
        {
            offset[0] = b.transform.position.x;
            offset[1] = b.transform.position.y;
            offset[2] = b.transform.position.z;
            BasicBrush.SetFloats("baseOffset", offset);
            b.BrushOperation(BasicBrush, kInd);
        }
    }


    public void Wipe( Vector4 color0, Vector4 color1 )
    {
        PerformBrush(WipeBrush, color0, color1, new Vector4(0, 0, 0), new Matrix4x4(), new Matrix4x4());        
    }


}