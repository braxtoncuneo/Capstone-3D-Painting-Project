using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public static class Yggdrasil
{

	static int totalGPURAM;
	const float startingDataSize = 0.60f;

	static ComputeBuffer dataBuffer;
	static ComputeBuffer heapBuffer;
	static ComputeBuffer headBuffer;
	static ComputeBuffer exchangeBuffer;

	static ComputeShader treeManager;
	static ComputeShader memoryManager;
	static List<ComputeShader> tools;

	static int dataBufferUsage;
	static int extraneousNodes;
	static int demandedNodes;



	static Yggdrasil(){

		if (!SystemInfo.supportsComputeShaders)
		{
			Debug.LogError("This system cannot run compute shaders. This is bad. Go fix that.");
		}

		totalGPURAM = SystemInfo.graphicsMemorySize;
		Debug.Log("System has "+totalGPURAM+" MB of Video Memory");
		extraneousNodes = 0;
		demandedNodes = 0;
		dataBufferUsage = 0;

	}


}
