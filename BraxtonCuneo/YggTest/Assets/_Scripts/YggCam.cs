using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class YggCam : MonoBehaviour {

	public YggManager ygg;
	Matrix4x4 viewMatrix;
	uint pixWidth;
	uint pixHeight;
	uint depthLimit;
	List <uint> horizonMat;
	float alphaThresh;
	ComputeShader Tracer;
	RenderTexture outputImage;

	// Use this for initialization
	void Start () {
		Tracer = ygg.rayTracer;
		Rect r = Camera.current.pixelRect;
		pixWidth = unchecked((uint) r.height);
		pixWidth = unchecked((uint) r.width);
		viewMatrix = Matrix4x4.LookAt (new Vector3 (-2.5f, -2.5f, -2.5f), new Vector3 (0.0f, 0.0f, 0.0f), new Vector3 (0.0f, 1.0f, 0.0f));
		depthLimit = (uint) 5;
		alphaThresh = 0.5f;
		horizonMat = new List<uint>();
		horizonMat.Add (1);
		horizonMat.Add (1);
		horizonMat.Add (1);
		horizonMat.Add (1);
		outputImage = new RenderTexture (new RenderTextureDescriptor ((int)r.width,(int)r.height));
	}
	
	// Update is called once per frame
	
	void Update () {
		/*
		List<int> intHorizon = new List<int>();
		for(int i = 0; i< 4; i++){
			intHorizon.Add(unchecked((int)(horizonMat[i])));
		}
		Tracer.SetMatrix ("viewMatrix",viewMatrix);
		Tracer.SetInt ("pixWidth", unchecked((int)pixWidth));
		Tracer.SetInt ("pixHeight", unchecked((int)pixHeight));
		Tracer.SetInts ("horizonMat", intHorizon.ToArray());
		Tracer.SetFloat ("alphaThresh", alphaThresh);
		Tracer.SetTexture (0, "outputImage", outputImage);
		Tracer.Dispatch (0, ygg.theGroupSize, 1, 1);
		Graphics.DrawTexture (new Rect (0,0,pixWidth,pixHeight), outputImage);
		*/
	}

}
