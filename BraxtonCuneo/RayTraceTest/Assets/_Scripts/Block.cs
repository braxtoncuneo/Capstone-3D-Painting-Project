
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Block : MonoBehaviour
{

    static List<Block> blockRegistry = new List<Block>();//$

    public Material RayTracer;
    public ComputeShader UpdateSkip;
    public RenderTexture ColorData;
    public RenderTexture SurfaceData;
    public RenderTexture SkipData;


    public Mesh Model;
    public const int width = 32;
    const int layers = 1;
    const bool doesMipMapping = true;
    const int groupWidthX = 8;
    const int groupWidthY = 8;
    const int groupWidthZ = 8;
    public const float blockWidth = 1f;
    int updInd;


    // Use this for initialization
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

        for (int i = 0; i < 8; i++)
        {
            Pos.Add((new Vector3(i % 2, (i % 4) / 2, i / 4)) * blockWidth);
            Col.Add(new Color(i % 2, (i % 4) / 2, i / 4, 1.0f));
        }

        List<int> Ind = new List<int>
        {
            0,3,1,      0,2,3,
            4,5,7,      4,7,6,

            0,1,5,      0,5,4,
            2,7,3,      2,6,7,

            0,6,2,      0,4,6,
            1,3,7,      1,7,5
        };


        Model.SetVertices(Pos);
        Model.SetColors(Col);
        Model.SetIndices(Ind.ToArray(), MeshTopology.Triangles, 0);
        filter.mesh = Model;
        Model.RecalculateBounds();

        updInd = UpdateSkip.FindKernel("main");

        blockRegistry.Add(this);//$

    }

    // Update is called once per frame
    void Update()
    {



    }

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
        brushShader.SetTexture(kernelIndex, "ColorData", ColorData);
        brushShader.SetTexture(kernelIndex, "SurfaceData", SurfaceData);
        brushShader.SetTexture(kernelIndex, "SkipData", SkipData);
        brushShader.SetFloat("blockWidth", blockWidth);
        brushShader.SetInt("texWidth", width);
        brushShader.Dispatch(kernelIndex, width / groupWidthX, width / groupWidthY, width / groupWidthZ);

        ComputeBuffer SkipBuffer = new ComputeBuffer(width * width * width, sizeof(int));

        UpdateSkip.SetTexture(updInd, "ColorData", ColorData);
        UpdateSkip.SetTexture(updInd, "SurfaceData", SurfaceData);
        UpdateSkip.SetTexture(updInd, "SkipData", SkipData);
        UpdateSkip.SetBuffer(updInd, "SkipBuffer", SkipBuffer);
        UpdateSkip.SetInt("texWidth", width);

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

        SkipBuffer.Release();

    }

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

