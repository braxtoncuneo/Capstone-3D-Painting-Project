using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskBuffer : ShaderBuffer
{

	uint EMPTY_PTR                = 0xC0FFEE42;

	protected override ComputeBufferType type { get => ComputeBufferType.Default; }




	public int height 			{get => buffer[0];}
	public int width 			{get => buffer[1];}
	public int ready_active 	{get => buffer[2];}

	public int this[int index] {
		get { return buffer [index + 3]; }
		set { buffer [index + 3] = value; }
	}

	public void init(int height_inp, int width_inp) {
		base.init ();
		resize ( 3 + (height_inp * width_inp) );
		write (0, height_inp );
		write (1, width_inp  );
		write (2, unchecked ((int) 0x80000000)  );

		int len = height_inp * width_inp;
		for(int idx = 0; idx < len; idx++){
			this[idx] = unchecked ( (int) EMPTY_PTR );
		}

	}

	public void prime(){
		write (2,  unchecked ((int) 0x80000000)  );

		int len = height * width;
		for(int idx = 0; idx < len; idx++){
			this[idx] = unchecked ( (int) EMPTY_PTR );
		}
		base.push ();
	}



	public override void push () {

		base.push ();

	}

}