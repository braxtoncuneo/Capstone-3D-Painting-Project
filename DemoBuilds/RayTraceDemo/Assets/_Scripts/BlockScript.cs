#define READWRITE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BlockScript : MonoBehaviour
{

    public Material RayTracer;

#if READWRITE
    public RenderTexture VoxelData;
#else
    public Texture3D VoxelData;
#endif

    public Mesh Model;
    public const int width = 32;//$ The number of voxels to a side in a block
    const int layers = 1;//$ Do not mess with right now
    const bool doesMipMapping = true;
    const int groupWidth = 4;//$ The side length in the compute groups being used. Set this too high, and Unity will irrevocably freeze.


    // Use this for initialization
    public void Start()
    {
        VoxelData = null;
        RayTracer = GetComponent<MeshRenderer>().material;
        Model = new Mesh();
        MeshFilter filter = GetComponent<MeshFilter>();
        Debug.Log(Model.vertexCount);

        //$ Make the block mesh
        new Mesh();
        List<Vector3> Pos = new List<Vector3>();
        List<Color> Col = new List<Color>();

        //$ Colors correspond to positions. The colors are what are used to determine position on the cube surface
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

#if READWRITE
#else
        MakeSphereTex();
#endif

    }

    // Update is called once per frame
    void Update()
    {



    }

#if READWRITE
#else
    void MakeSphereTex()
    {

        VoxelData = new Texture3D(width * layers, width, width, TextureFormat.RGBAFloat, doesMipMapping);

        List<Color> vox = new List<Color>();
        float negHalfWid = 0.0f - width / 2.0f;
        Vector3 coord = new Vector3(0, 0, 0);
        Vector3 offset = new Vector3(negHalfWid, negHalfWid, negHalfWid);
        for (int z = 0; z < width; z++)
        {
            coord.z = z;
            for (int y = 0; y < width; y++)
            {
                coord.y = y;
                for (int x = 0; x < width; x++)
                {
                    coord.x = x;
                    if (((coord + offset) / width).magnitude < 0.25)
                    {
                        vox.Add(new Color(1.0f, 0.0f, 0.0f, 10000.0f));
                    }
                    else
                    {
                        vox.Add(new Color(0.0f, 0.0f, 1.0f, 0.08f));
                    }
                }
            }
        }
        List<Color> dummy = new List<Color>(VoxelData.GetPixels());
        Debug.Log("Voxel Count: " + vox.Count);
        Debug.Log("TexLocation : " + RayTracer.HasProperty("_Data"));
        VoxelData.SetPixels(vox.ToArray());
        VoxelData.Apply();
        VoxelData.filterMode = FilterMode.Point;
        RayTracer.SetTexture("_Data", VoxelData);

    }
#endif

#if READWRITE
    public void Brush(ComputeShader brushShader, int kernelIndex)
    {
        if(VoxelData == null)
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
            RayTracer.SetFloat("texWidth",width);
        }
        brushShader.SetTexture(0, "_Data", VoxelData);
        brushShader.Dispatch(kernelIndex, width / groupWidth, width / groupWidth, width / groupWidth);
    }

#else

    public void Brush(ComputeShader brushShader, int kernelIndex)
    {

    }

#endif

}
