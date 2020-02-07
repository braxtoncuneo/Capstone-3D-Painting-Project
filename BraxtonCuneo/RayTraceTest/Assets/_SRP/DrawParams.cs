using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawParams :  ShaderBuffer
{

	protected override ComputeBufferType type { get => ComputeBufferType.Default; }


	public void init() {
		base.init ();
		resize ( 8 );
		Debug.Log("INIT DRAW");

	}

	public int block_index {
		get {	return buffer[0];	}
		set {	buffer[0] = value;	}
	}

	public Vector3 brush_center {
		get {
			return read_Vector3(4);
		}
		set {
			write(4,value);
		}
	}

	public void push (ComputeShader shader) {

		base.push ();


		/*
		shader.SetInt("draw_block_index",block_index);
		float[] BC = { brush_center.x, brush_center.y, brush_center.z };
		shader.SetFloats("draw_brush_center",BC);
		*/


	}


}

