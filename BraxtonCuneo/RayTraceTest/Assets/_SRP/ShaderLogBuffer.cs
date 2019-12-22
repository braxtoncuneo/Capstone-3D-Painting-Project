using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderLogBuffer : ShaderBuffer
{
	protected override ComputeBufferType type { get => ComputeBufferType.Default; }


	public void init(int size) {
		base.init ();
		resize (2+size);
		write (0, buffer.Length-1);
		write (1, 0);
	}

	public int cursor_pos {get => buffer[1];}

	public int this[int index] {
		get { return buffer [index + 2]; }
		set { buffer [index + 2] = value; }
	}

	/*
	public int cursor_pos {get => BitConverter.ToInt32(BitConverter.GetBytes(buffer [1]),0);}

	public int this[int index] {
		get { return BitConverter.ToInt32(BitConverter.GetBytes(buffer [index + 2]),0); }
		set { buffer [index + 2] = BitConverter.ToSingle(BitConverter.GetBytes(value),0); }
	}
	*/


	public override void push () {

		write (0, buffer.Length-2);

		base.push ();

	}

}

