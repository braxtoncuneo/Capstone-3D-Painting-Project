using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBuffer : ShaderBuffer
{
	protected override ComputeBufferType type { get => ComputeBufferType.Structured; }


	public void init (int size){
		base.init ();
		Debug.Log("Data init");
		resize (1+size);
		write (0, buffer.Length-1);
	}


	public int this[int index] {
		get { return buffer [index + 1]; }
		set { buffer [index + 1] = value; }
	}

	/*
	public int this[int index] {
		get { return BitConverter.ToInt32(BitConverter.GetBytes(buffer [index + 1]),0); }
		set { buffer [index + 1] = BitConverter.ToSingle(BitConverter.GetBytes(value),0); }
	}
	*/

	public void set_all(int val){
		for(int i = 1; i < buffer.Length; i++){
			buffer[i] = val;
		}
	}

	public override void push () {

		write (0, buffer.Length-1);

		base.push ();

	}

	public void pull (){

		compute_buffer.GetData(buffer);

	}

	public void print_content (){

		string message = "";
		for(uint i = 0; i < 4400; i ++) {
			message += buffer[i].ToString() + " ";
		}
		Debug.Log(message);

	}


}
