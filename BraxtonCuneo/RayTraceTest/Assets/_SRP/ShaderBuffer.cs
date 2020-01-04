using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System;
using UnityEngine;
using UnityEngine.Rendering;

public class ShaderBuffer : ScriptableObject
{
 
	protected virtual ComputeBufferType type { get; }
	protected int[] buffer = null;
	protected ComputeBuffer compute_buffer = null;
	protected bool inited = false;
	protected bool dumped = false;

	public ShaderBuffer(){
		
	}


	public void init(){
		//Debug.Log("INIT_ATTEMPT");
		if(compute_buffer == null){
			if(inited == true){
				//Debug.Log("RE-INITED");
			}
			compute_buffer = new ComputeBuffer(1,4,type);
		}
		if(buffer == null){
			if(dumped == true){
				return;
			}
			if(inited == true){
				//Debug.Log("RE-INITED");
			}
			buffer = new int[1];
		}
		inited = true;
	}

	protected void write(int dest_idx, List<int> src) {

		for (int i = 0; i < src.Count; i++) {
			buffer [dest_idx + i] = src [i];
		}

	}

	protected void write(int dest_idx, List<float> src) {

		for (int i = 0; i < src.Count; i++) {
			buffer [dest_idx + i] = BitConverter.ToInt32(BitConverter.GetBytes(src [i]),0);
		}

	}

	protected void write(int dest_idx, List<uint> src) {

		for (int i = 0; i < src.Count; i++) {
			buffer [dest_idx + i] = BitConverter.ToInt32(BitConverter.GetBytes(src [i]),0);
		}

	}

	protected void write_float(int dest_idx, float src) {

		buffer [dest_idx] = BitConverter.ToInt32(BitConverter.GetBytes(src),0);

	}

	protected void write(int dest_idx, Vector2 src) {

		for (int i = 0; i < 2; i++) {
			buffer [dest_idx + i] = BitConverter.ToInt32(BitConverter.GetBytes(src [i]),0);
		}

	}

	protected void write(int dest_idx, Vector3 src) {

		for (int i = 0; i < 3; i++) {
			buffer [dest_idx + i] = BitConverter.ToInt32(BitConverter.GetBytes(src [i]),0);
		}

	}

	protected void write(int dest_idx, Vector4 src) {

		for (int i = 0; i < 4; i++) {
			buffer [dest_idx + i] = BitConverter.ToInt32(BitConverter.GetBytes(src [i]),0);
			Debug.Log(src[i]);
		}

	}

	protected void write(int dest_idx, int src) {

		buffer [dest_idx] = BitConverter.ToInt32(BitConverter.GetBytes(src),0);

	}
		

	protected void write(int dest_idx, Vector2Int src) {

		for (int i = 0; i < 2; i++) {
			buffer [dest_idx + i] = BitConverter.ToInt32(BitConverter.GetBytes(src [i]),0);
		}

	}

	protected void write(int dest_idx, Vector3Int src) {

		for (int i = 0; i < 3; i++) {
			buffer [dest_idx + i] = BitConverter.ToInt32(BitConverter.GetBytes(src [i]),0);
		}

	}

	protected float read_float(int src_idx){
		
		return BitConverter.ToSingle(BitConverter.GetBytes(buffer [src_idx]),0);

	}

	protected Vector2 read_Vector2(int src_idx){

		return new Vector2(read_float(src_idx),read_float(src_idx+1));

	}

	protected Vector3 read_Vector3(int src_idx){

		return new Vector3(read_float(src_idx),read_float(src_idx+1),read_float(src_idx+2));

	}

	protected Vector4 read_Vector4(int src_idx){

		return new Vector4(read_float(src_idx),read_float(src_idx+1),read_float(src_idx+2),read_float(src_idx+3));

	}

	protected Vector2Int read_Vector2Int(int src_idx){

		return new Vector2Int(buffer[src_idx],buffer[src_idx+1]);

	}

	protected Vector3Int read_Vector3Int(int src_idx){

		return new Vector3Int(buffer[src_idx],buffer[src_idx+1],buffer[src_idx+2]);

	}




	/*
	protected void write(int dest_idx, Vector4Int src) {

		for (int i = 0; i < 4; i++) {
			buffer [dest_idx + i] = BitConverter.ToInt32(BitConverter.GetBytes(src [i]),0);
		}

	}
	*/


	public void resize(int new_size) {

		if( (compute_buffer != null) 				&&
			(buffer != null) 						&& 
			(compute_buffer.count == buffer.Length) && 
			(new_size == buffer.Length) 
		){
			//Debug.Log("ABORTING");
			return;
		}

		/*
		Debug.Log("RESIZING");
		Debug.Log((compute_buffer != null));
		Debug.Log((buffer != null));
		Debug.Log((compute_buffer.count == buffer.Length));
		Debug.Log((new_size == buffer.Length));
		*/


		init();


		int[]temp_buffer = new int[new_size];
		//List<int> temp_buffer = new List<int>(new_size);

		int lim;
		if(buffer != null){
			lim = Math.Min(buffer.Length, temp_buffer.Length);
		} else {
			lim = 0;
		}
		int i = 0;
		for (i = 0; i < lim; i++) {
			//temp_buffer.Add(buffer[i]);
			temp_buffer[i] = buffer[i];
		}
		for (; i < new_size; i++){
			//temp_buffer.Add(0);
			temp_buffer[i] = 0;
		}

		buffer = temp_buffer;
		compute_buffer = new ComputeBuffer (new_size,4,type);
		//Debug.Log(type);

	}


	virtual public void bind(CommandBuffer cb, ComputeShader shader, int kernel_index, int param_id){

		cb.SetComputeBufferParam(shader, kernel_index, param_id,	compute_buffer);
		//cb.SetComputeIntParams(shader,param_id, buffer);

	}

	virtual public void bind(CommandBuffer cb, ComputeShader shader, int kernel_index, string param_name){

		cb.SetComputeBufferParam(shader, kernel_index, param_name,	compute_buffer);
		//cb.SetComputeIntParams(shader,param_name, buffer);

	}

	virtual public void bind(ComputeShader shader, int kernel_index, int param_id){

		shader.SetBuffer(kernel_index, param_id,compute_buffer);
		//shader.SetInts(param_id,buffer);

	}

	virtual public void bind(ComputeShader shader, int kernel_index, string param_name){

		shader.SetBuffer(kernel_index, param_name,compute_buffer);
		//shader.SetInts(param_name,buffer);

	}


	virtual public void push(){
		//Debug.Log(type);
		//Debug.Log(compute_buffer.count);
		//Debug.Log(compute_buffer.stride);
		//Debug.Log(compute_buffer.IsValid());

		init();

		/*
		Debug.Log(inited);
		Debug.Log(compute_buffer);
		Debug.Log(buffer);
		Debug.Log(buffer.Length);
		*/


		compute_buffer.SetData (buffer,0,0,buffer.Length);
		//compute_buffer.GetData (buffer,0,0,buffer.Length);

		//Debug.Log("~~~TOP~~~");
		//foreach(var e in buffer) {
		//	Debug.Log(e);
		//}
		//Debug.Log("~~~BOT~~~");




	}

}
