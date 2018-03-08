using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour {

    public static Material RayTracer;
    public Texture3D VoxelData;
    public Mesh Model;
    const int width = 1024;
    const int layers = 2;
    const bool doesMipMapping = false;
    const int groupWidth = 16;


    // Use this for initialization
    void Start () {
        VoxelData = null;
        Model = new Mesh();
        MeshFilter filter = GetComponent<MeshFilter>();
        Debug.Log(Model.vertexCount);
         
        new Mesh();
        List<Vector3> Pos = new List<Vector3>();
        List<Color> Col = new List<Color>();

        for(int i = 0; i < 8; i++)
        {
            Pos.Add(new Vector3(i % 2, (i % 4) / 2, i / 4));
            Col.Add(new Color(i % 2, (i % 4) / 2, i / 4,1.0f));
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
	void Update () {
		


	}

    void Brush(ComputeShader brushShader, int kernelIndex)
    {
        if(VoxelData == null)
        {
            VoxelData = new Texture3D(width * 2, width, width, TextureFormat.RGBA32, doesMipMapping);
        }
        brushShader.SetTexture(0, "VoxelData", VoxelData);
        brushShader.Dispatch(kernelIndex, width / groupWidth, width / groupWidth, width / groupWidth);
    }


}
