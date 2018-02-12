using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootBlock : MonoBehaviour {

	uint parentSVO;
	Transform tform;
	Mesh mesh;

	Mesh getBoxMesh()
	{
		int ptsPerCube = 26;
		int edgeOffset = 8;
		int faceOffset = 20;
		Mesh result = new Mesh();
		result.vertices = new Vector3[ptsPerCube];
		result.uv = new Vector2[ptsPerCube];
		uint pos = 0;

		// Make the corner vertices
		for(uint i = 0; i < 8; i++)
		{
			result.vertices[pos].x = ( (i & 1) != 0 ) ? 1.0f : 0.0f;
			result.vertices[pos].y = ( (i & 2) != 0 ) ? 1.0f : 0.0f;
			result.vertices[pos].z = ( (i & 4) != 0 ) ? 1.0f : 0.0f;
			pos++;
		}
		// Make the edge vertices 
		for(int i = 0; i < 3; i++)
		{
			uint axisBit = 1U << i;
			uint lowBit = ((axisBit & 1U) == 0U) ? 1U : 2U;
			uint highBit = ((axisBit & 4U) == 0U) ? 4U : 2U;
			for (uint j = 0; j < 4; j++)
			{
				uint first = (((j & 1U) != 0U) ? lowBit : 0U) & (((j & 2U) != 0U) ? highBit : 0U);
				uint second = first & axisBit;
				result.vertices[pos] = Vector3.Lerp(result.vertices[first], result.vertices[second], 0.5f);
				pos++;
			}
		}
		// Make the face vertices
		for (int i = 0; i < 3; i++)
		{
			uint axisBit = 1U << i;
			uint lowBit = ((axisBit & 1U) == 0U) ? 1U : 2U;
			uint highBit = ((axisBit & 4U) == 0U) ? 4U : 2U;
			for (uint j = 0; j < 2; j++)
			{
				uint first = (((j & 1U) != 0U) ? axisBit : 0U);
				uint second = (((j & 1U) != 0U) ? axisBit : 0U) & lowBit & highBit;
				result.vertices[pos] = Vector3.Lerp(result.vertices[first], result.vertices[second], 0.5f);
				pos++;
			}
		}

		// Make corner triangles
		for (int i = 0; i < 8; i++)
		{

		}
		
		return result;
	}


	// Use this for initialization
	void Start () {
		tform = GetComponent<Transform>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
