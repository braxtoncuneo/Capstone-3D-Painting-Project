

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Block : MonoBehaviour
{

    public Material RayTracer;
    public RenderTexture ColorData;
    public RenderTexture SurfaceData;


    public Mesh Model;
    public const int width = 64;
    const int layers = 1;
    const bool doesMipMapping = true;
    const int groupWidthX = 8;
    const int groupWidthY = 8;
    const int groupWidthZ = 8;
    public const float blockWidth = 3f;


    // Use this for initialization
    void Start()
    {
        ColorData = null;
        SurfaceData = null;
        RayTracer = GetComponent<MeshRenderer>().material;
        Model = new Mesh();
        MeshFilter filter = GetComponent<MeshFilter>();

        new Mesh();
        List<Vector3> Pos = new List<Vector3>();
        List<Color> Col = new List<Color>();

        for (int i = 0; i < 8; i++)
        {
            Pos.Add((new Vector3(i % 2, (i % 4) / 2, i / 4))*blockWidth);
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
        result.format = RenderTextureFormat.ARGBFloat;
        result.autoGenerateMips = false;
        result.useMipMap = false;
        result.Create();
        return result;
    }

    RenderTexture makeSeekGrid(int width, int height, int depth)
    {
        RenderTexture result = new RenderTexture(width, height, 0);
        result.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        result.width = width;
        result.height = height;
        result.volumeDepth = depth;
        result.enableRandomWrite = true;
        result.filterMode = FilterMode.Point;
        result.format = RenderTextureFormat.R8;
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

            RayTracer.SetFloat("texWidth", width);
            RayTracer.SetFloat("blockWidth", blockWidth);
        }
        brushShader.SetTexture(kernelIndex, "ColorData", ColorData);
        brushShader.SetTexture(kernelIndex, "SurfaceData", SurfaceData);
        brushShader.SetFloat("blockWidth", blockWidth);
        brushShader.Dispatch(kernelIndex, width / groupWidthX, width / groupWidthY, width / groupWidthZ);
    }
    


}

