

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Block : MonoBehaviour
{

    public Material RayTracer;
    public RenderTexture VoxelData;


    public Mesh Model;
    public const int width = 64;
    const int layers = 1;
    const bool doesMipMapping = true;
    const int groupWidth = 4;


    // Use this for initialization
    void Start()
    {
        VoxelData = null;
        RayTracer = GetComponent<MeshRenderer>().material;
        Model = new Mesh();
        MeshFilter filter = GetComponent<MeshFilter>();

        new Mesh();
        List<Vector3> Pos = new List<Vector3>();
        List<Color> Col = new List<Color>();

        for (int i = 0; i < 8; i++)
        {
            Pos.Add(new Vector3(i % 2, (i % 4) / 2, i / 4));
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

    }

    // Update is called once per frame
    void Update()
    {



    }


    public void BrushOperation(ComputeShader brushShader, int kernelIndex)
    {
        if (VoxelData == null)
        {
            VoxelData = new RenderTexture(width, width, 0);
            VoxelData.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
            VoxelData.width = width * layers;
            VoxelData.width = width;
            VoxelData.volumeDepth = width;
            VoxelData.enableRandomWrite = true;
            VoxelData.filterMode = FilterMode.Point;
            VoxelData.format = RenderTextureFormat.ARGBFloat;
            VoxelData.Create();
            RayTracer.SetTexture("_Data", VoxelData);
            RayTracer.SetFloat("texWidth", width);
            Debug.Log("Made RenderTex");
        }
        else
        {
            Debug.Log("Not missing RenderTex?");
        }
        brushShader.SetTexture(0, "_Data", VoxelData);
        brushShader.Dispatch(kernelIndex, width / groupWidth, width / groupWidth, width / groupWidth);
    }
    


}

