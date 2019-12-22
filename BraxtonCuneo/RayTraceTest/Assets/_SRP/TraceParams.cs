using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceParams :  ScriptableObject
{

	public Vector2Int	screen_dims;	//2 @ 0
	public Vector4		camera_dims;	//4 @ 4
	public Vector3		camera_pos;		//3 @ 8
	public Vector3		camera_dir;		//3 @ 12
	public int 			step_count;		//1 @ 15

	public void push (ComputeShader shader) {

		int[] SD = {screen_dims.x,screen_dims.y};
		shader.SetInts("trace_screen_dims",SD);
		shader.SetVector("trace_camera_dims",camera_dims);
		float[] CP = { camera_pos.x, camera_pos.y, camera_pos.z };
		shader.SetFloats("trace_camera_pos",CP);
		float[] CD = { camera_dir.x, camera_dir.y,camera_dir.z };
		shader.SetFloats("trace_camera_dir",CD);
		shader.SetInt("trace_step_count",step_count);

	}


}

