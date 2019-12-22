using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WGCountBuffer : ScriptableObject
{

	public Vector3Int	work_group_count;	//3 @ 0

	public void push (ComputeShader shader) {

		int[] WGC = {work_group_count.x, work_group_count.y, work_group_count.z};
		shader.SetInts("SPIRV_Cross_NumWorkgroups_1_count",WGC);

	}

}

