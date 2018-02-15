
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;


unsafe struct block{
	uint parent;
	uint referenceCount;
	uint type;
	uint root;
	fixed float transform [16];
};


public class YggManager : MonoBehaviour 
{


	public ComputeBuffer dataBuffer;
	public ComputeBuffer heapBuffer;
	public ComputeBuffer taskBuffer;
	public ComputeBuffer blockBuffer;
	public ComputeBuffer exchangeBuffer;

	public ComputeShader initializer;
	public ComputeShader diagnostic;
	public ComputeShader rayTracer;
	public ComputeShader treeManager;
	public ComputeShader memoryManager;
	public List<ComputeShader> tools;

	int totalGPURAM;
	float dataSat;
	int groupSize;
	public int theGroupSize { get{ return groupSize; } }
	int threadCount;
	int dataBufferUsage;
	int extraneousNodes;
	int demandedNodes;
	int blockNumber;
	int exchangeSize;


	int dataBufferSize(){
		return (((int)(dataSat * totalGPURAM * 1024 * 1024) / (threadCount * 3))) * (threadCount * 3);
	}
	int heapBufferSize(){
		return (threadCount * 2 * 3) + (threadCount * 2);
	}
	int taskBufferSize(){
		return (threadCount * 2 * 3) + (threadCount * 2) + 1;
	}



	unsafe void Start(){
		if (!SystemInfo.supportsComputeShaders){
			Debug.LogError("This system cannot run compute shaders. This is bad. Go fix that.");
		}
		totalGPURAM = SystemInfo.graphicsMemorySize;
		Debug.Log("System has "+totalGPURAM+" MB of Video Memory");
		dataSat = 0.0078125f;
		threadCount = 1024;
		groupSize = 64;
		extraneousNodes = 0;
		demandedNodes = 0;
		dataBufferUsage = 0;
		blockNumber = 1;
		exchangeSize = threadCount;
		dataBuffer = new ComputeBuffer (dataBufferSize (), sizeof(uint));
		heapBuffer = new ComputeBuffer (heapBufferSize (), sizeof(uint));
		taskBuffer = new ComputeBuffer (taskBufferSize (), sizeof(uint));
		blockBuffer = new ComputeBuffer (blockNumber, sizeof(block));
		exchangeBuffer = new ComputeBuffer (exchangeSize, sizeof(uint));

		init ();
		diagnose ();

	}

	void Update(){
        Debug.LogError("Frame");
    }
		

	void loadCommon(ComputeShader prog){
		prog.SetBuffer (0, "dataBuffer", dataBuffer);
		prog.SetBuffer (0, "heapBuffer", heapBuffer);
		prog.SetBuffer (0, "taskBuffer", taskBuffer);
		prog.SetBuffer (0, "blockBuffer", blockBuffer);
		prog.SetBuffer (0, "exchangeBuffer", exchangeBuffer);
	}

	void loadCommon(ComputeShader prog, ComputeBuffer exg){
		prog.SetBuffer (0, "dataBuffer", dataBuffer);
		prog.SetBuffer (0, "heapBuffer", heapBuffer);
		prog.SetBuffer (0, "taskBuffer", taskBuffer);
		prog.SetBuffer (0, "blockBuffer", blockBuffer);
		prog.SetBuffer (0, "exchangeBuffer", exg);
	}

	void init(){
		loadCommon (initializer);
		initializer.Dispatch (0, threadCount, 1, 1);
	}

	void diagnose(){
		ComputeBuffer diagExg = new ComputeBuffer (threadCount, sizeof(uint));
		loadCommon (diagnostic,diagExg);
		diagnostic.Dispatch (0, threadCount, 1, 1);

		uint[] result = new uint[threadCount];
		diagExg.GetData(result);
		for(int i = 0; i < threadCount; i++)
		{
			if(result[i] != 0)
			{
				Debug.LogError("Thread had status "+result);
			}
		}
	}

}
