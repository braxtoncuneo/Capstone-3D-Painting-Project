
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;


struct block{
	uint parent;
	uint referenceCount;
	uint type;
	uint root;
	Matrix4x4 transformation;
};


public class YggManager : MonoBehaviour 
{

	public int totalGPURAM;
	public int groupSize;
	public float startingDataSize = 0.60f;

	public ComputeBuffer dataBuffer;
	public ComputeBuffer heapBuffer;
	public ComputeBuffer blockBuffer;
	public ComputeBuffer exchangeBuffer;

	public ComputeShader initializer;
	public ComputeShader rayTracer;
	public ComputeShader treeManager;
	public ComputeShader memoryManager;
	public List<ComputeShader> tools;

	int dataBufferUsage;
	int extraneousNodes;
	int demandedNodes;



	void Start(){
		if (!SystemInfo.supportsComputeShaders){
			Debug.LogError("This system cannot run compute shaders. This is bad. Go fix that.");
		}
		totalGPURAM = SystemInfo.graphicsMemorySize;
		Debug.Log("System has "+totalGPURAM+" MB of Video Memory");
		groupSize = 64;
		extraneousNodes = 0;
		demandedNodes = 0;
		dataBufferUsage = 0;
	}

	void Update(){

	}


}
