using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Brush : MonoBehaviour
{
    // Stores the ending state of the previous stroke
    Matrix4x4 lastTransform;
    public Color lastColor;

    // Stores shaders for use in brush strokes
    public ComputeShader CurrentBrush;
    public ComputeShader WipeBrush;
    public ComputeShader BasicBrush;

    // Stores the blocks affected by this brush
    public List<Block> blocks;
    // public Color brushColor;

    // Use this for initialization
    void Start()
    {
        // Initializes the default brush
        lastTransform = transform.worldToLocalMatrix;
        BasicBrush.SetInt("texWidth", Block.width);
        setVector4(BasicBrush,"color0",new Vector4(0, 0, 0, 0));
        setVector4(BasicBrush, "color1",new Vector4(0, 0, 0, 0));
        setVector4(BasicBrush, "sliders", new Vector4(0, 0, 0, 0));
        transform.SetPositionAndRotation(new Vector3(0, 0, 0), new Quaternion());
        transform.localScale = new Vector3(0, 0, 0);
        Wipe(new Vector4(0,0,0,0),new Vector4(0,0,0,0));
    }

    // Used for conversion when assigning shader variables
    float[] arrFromVec4(Vector4 value)
    {
        float[] result = new float[4];
        result[0] = value.x;
        result[1] = value.y;
        result[2] = value.z;
        result[3] = value.w;
        return result;
    }

    void setVector4(ComputeShader brush, string name, Vector4 value)
    {
        float[] val = arrFromVec4(value);
        brush.SetFloats(name, val);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Records current position of brush and primes for strokes
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

    // Performs brush stroke given current and previous state of brush
    public void Stroke(Vector4 color0, Vector4 color1, Vector4 sliders)
    {
        if(CurrentBrush != null)
        {
            PerformBrush(CurrentBrush, color0, color1, sliders, lastTransform, transform.worldToLocalMatrix);
        }
        lastTransform = transform.worldToLocalMatrix;
    }

    // Ceases to perform brush strokes until the next "Down"
    public void Up()
    {
        CurrentBrush = null;
    }


    // Assigns all necessary variables to perform a brush stroke and then does so
    public void PerformBrush(  ComputeShader brush, Vector4 color0, Vector4 color1, Vector4 sliders,
                        Matrix4x4 startTransform, Matrix4x4 endTransform)
    {
        lastColor = new Color(color1.x, color1.y, color1.z, color1.w);
        int kInd = BasicBrush.FindKernel("main");
        float[] offset = new float[3];

        setVector4(brush,"color0", color0);
        setVector4(brush, "color1", color1);
        setVector4(brush, "sliders", sliders);

        brush.SetMatrix("start", startTransform);
        brush.SetMatrix("end", endTransform);

        // Sets up and executes the stroke shader across all blocks under the
        // control of the brush
        foreach (Block b in blocks)
        {
            offset[0] = b.transform.position.x;
            offset[1] = b.transform.position.y;
            offset[2] = b.transform.position.z;
            BasicBrush.SetFloats("baseOffset", offset);
            b.BrushOperation(BasicBrush, kInd);
        }
    }

    // Wipes all blocks clean
    public void Wipe( Vector4 color0, Vector4 color1 )
    {
        PerformBrush(WipeBrush, color0, color1, new Vector4(0, 0, 0), new Matrix4x4(), new Matrix4x4());        
    }


}
