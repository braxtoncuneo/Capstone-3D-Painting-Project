
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Block : MonoBehaviour
{

    static List<Block> blockRegistry = new List<Block>();//$

    public Material RayTracer;

    // The shader responsable for updating the SkipGrid
    public ComputeShader UpdateSkip;

    // The textures that house block data and metadata
    public RenderTexture ColorData;
    public RenderTexture SurfaceData;
    public RenderTexture SkipData;
    static ComputeBuffer SkipBuffer = null;

    // The brush that can write into the block
    public Brush theBrush;

    // The model that the contents of the block
    // will be rendered onto
    public Mesh Model;

    // The number of voxels to the side of a block
    public const int width = 128;
    const int layers = 1;
    const bool doesMipMapping = true;

    // The group dimensions used in compute shaders
    const int groupWidthX = 8;
    const int groupWidthY = 8;
    const int groupWidthZ = 8;

    // The normal width of voxels
    public const float blockWidth = 1f;
    // The kernel index of the skip grid updating shader
    int updInd;



    void Start()
    {
        
        ColorData = null;
        SurfaceData = null;
        SkipData = null;
        RayTracer = GetComponent<MeshRenderer>().material;
        Model = new Mesh();
        MeshFilter filter = GetComponent<MeshFilter>();

        new Mesh();
        List<Vector3> Pos = new List<Vector3>();
        List<Color> Col = new List<Color>();

        // Generates the vertices for rendering the contents of the block
        for (int i = 0; i < 8; i++)
        {
            Pos.Add((new Vector3(i % 2, (i % 4) / 2, i / 4)) * blockWidth);
            Col.Add(new Color(i % 2, (i % 4) / 2, i / 4, 1.0f));
        }

        // The indices for rendering the block vertices to triangles
        List<int> Ind = new List<int>
        {
            0,3,1,      0,2,3,
            4,5,7,      4,7,6,

            0,1,5,      0,5,4,
            2,7,3,      2,6,7,

            0,6,2,      0,4,6,
            1,3,7,      1,7,5
        };

        // Initializes model
        Model.SetVertices(Pos);
        Model.SetColors(Col);
        Model.SetIndices(Ind.ToArray(), MeshTopology.Triangles, 0);
        filter.mesh = Model;
        Model.RecalculateBounds();

        updInd = UpdateSkip.FindKernel("main");

        blockRegistry.Add(this);//$

        if(SkipBuffer == null)
        {
            SkipBuffer = new ComputeBuffer(width * width * width, sizeof(int));
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Keeps the raytracer updated with the current brush color and transform
        RayTracer.SetMatrix("brush_transform", theBrush.transform.worldToLocalMatrix);
        RayTracer.SetColor("brush_color", theBrush.lastColor);

    }

    // Performs all the initialization required for a standard
    // ColorGrid or SkipGrid
    RenderTexture makeDataGrid(int width, int height, int depth)
    {
        RenderTexture result = new RenderTexture(width, height, 0);
        result.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        result.width = width;
        result.height = height;
        result.volumeDepth = depth;
        result.enableRandomWrite = true;
        result.filterMode = FilterMode.Point;
        result.format = RenderTextureFormat.ARGBHalf;
        result.autoGenerateMips = false;
        result.useMipMap = false;
        result.Create();
        return result;
    }

    // Performs all the initialization required for a SkipGrid
    RenderTexture makeSkipGrid(int width, int height, int depth)
    {
        RenderTexture result = new RenderTexture(width, height, 0);
        result.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        result.width = width;
        result.height = height;
        result.volumeDepth = depth;
        result.enableRandomWrite = true;
        result.filterMode = FilterMode.Point;
        result.format = RenderTextureFormat.RInt;
        result.autoGenerateMips = false;
        result.useMipMap = false;
        result.Create();
        return result;
    }


    public void BrushOperation(ComputeShader brushShader, int kernelIndex)
    {
        // Initialize the 3D textures and raytracer if the block has
        // not yet been written to
        if (ColorData == null)
        {
            ColorData = makeDataGrid(width, width, width);
            RayTracer.SetTexture("ColorData", ColorData);

            SurfaceData = makeDataGrid(width, width, width);
            RayTracer.SetTexture("SurfaceData", SurfaceData);

            SkipData = makeSkipGrid(width, width, width);
            RayTracer.SetTexture("SkipData",SkipData);

            RayTracer.SetFloat("texWidth", width);
            RayTracer.SetFloat("blockWidth", blockWidth);
        }

        // Set all locally applicable variables for the brush compute shader
        brushShader.SetTexture(kernelIndex, "ColorData", ColorData);
        brushShader.SetTexture(kernelIndex, "SurfaceData", SurfaceData);
        brushShader.SetTexture(kernelIndex, "SkipData", SkipData);
        brushShader.SetFloat("blockWidth", blockWidth);
        brushShader.SetInt("texWidth", width);
        brushShader.Dispatch(kernelIndex, width / groupWidthX, width / groupWidthY, width / groupWidthZ);


        // Set all locally applicable variables for the SkipGrid updating compute shader
        UpdateSkip.SetTexture(updInd, "ColorData", ColorData);
        UpdateSkip.SetTexture(updInd, "SurfaceData", SurfaceData);
        UpdateSkip.SetTexture(updInd, "SkipData", SkipData);
        UpdateSkip.SetBuffer(updInd, "SkipBuffer", SkipBuffer);
        UpdateSkip.SetInt("texWidth", width);

        // Execute the SkipGrid updating compute shader for each level of the SkipGrid
        int lim = width >> 1;
        int s = 2;
        for (int i = 1; i < lim; i = i << 1)
        {
            UpdateSkip.SetInt("startLevel", i);
            UpdateSkip.SetInt("endLevel", i);
            int x = Math.Max(width / (groupWidthX * s), groupWidthX);
            int y = Math.Max(width / (groupWidthY * s), groupWidthY);
            int z = Math.Max(width / (groupWidthZ * s), groupWidthZ);
            UpdateSkip.Dispatch(kernelIndex,x,y,z);
            s = s << 1;
        }
        

    }

    // Scales the blocks given the position of the left and right
    // controllers in the current and previous frames
    public static void Scale(  Vector3 leftBefore, Vector3 rightBefore,
                        Vector3 leftAfter, Vector3 rightAfter)
    {
        
        Vector3 center = Vector3.Lerp(
                            Vector3.Lerp(leftBefore, rightBefore, 0.5f),
                            Vector3.Lerp(leftAfter,  rightAfter,  0.5f),
                            0.5f
                        );
        float scaleDelta = Vector3.Distance(leftAfter, rightAfter) / Vector3.Distance(leftBefore, rightBefore);
        Debug.Log(scaleDelta);

        // Performs scaling calculations and transform assignments for each block
        foreach(Block b in blockRegistry)
        {
            Vector3 newPos = b.transform.position + (b.transform.position - center) * ( scaleDelta - 1 );
            Vector3 newScale = Vector3.Scale(b.transform.localScale, new Vector3(scaleDelta,scaleDelta,scaleDelta));
            b.transform.position = newPos;
            b.transform.localScale = newScale;
            b.RayTracer.SetFloat("blockWidth", blockWidth*newScale.x);
        }

    }

}

