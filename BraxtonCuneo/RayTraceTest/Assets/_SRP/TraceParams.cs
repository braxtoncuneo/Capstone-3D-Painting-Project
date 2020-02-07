using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceParams :  ShaderBuffer
{

	protected override ComputeBufferType type { get => ComputeBufferType.Default; }


	public void init() {
		base.init ();
		resize ( 16 );
	}

	// 2 @ 0
	public Vector2Int	screen_dims{
		get {	return read_Vector2Int(0);	}
		set {	write(0,value);	}
	}

	// 4 @ 4
	public Vector4	camera_dims{
		get {	return read_Vector4(4);	}
		set {	write(4,value);	}
	}

	// 3 @ 8
	public Vector3	camera_pos{
		get {	return read_Vector3(8);	}
		set {	write(8,value);	}
	}

	// 3 @ 12
	public Vector3	camera_dir{
		get {	return read_Vector3(12);	}
		set {	write(12,value);	}
	}

	// 1 @ 15
	public int	step_count{
		get {	return buffer[15];	}
		set {	write(15,value);	}
	}

	public void push () {

		base.push();

		/*
		int[] SD = {screen_dims.x,screen_dims.y};
		shader.SetInts("trace_screen_dims",SD);
		shader.SetVector("trace_camera_dims",camera_dims);
		float[] CP = { camera_pos.x, camera_pos.y, camera_pos.z };
		shader.SetFloats("trace_camera_pos",CP);
		float[] CD = { camera_dir.x, camera_dir.y,camera_dir.z };
		shader.SetFloats("trace_camera_dir",CD);
		shader.SetInt("trace_step_count",step_count);
		*/

	}


}

