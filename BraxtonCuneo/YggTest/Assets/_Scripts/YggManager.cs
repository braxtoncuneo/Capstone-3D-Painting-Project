
#define DBUG


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
    public ComputeShader atomicSanityCheck;
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
		return (((int)(dataSat * totalGPURAM * 1024 * 1024) / (threadCount * 3 * 16))) * (threadCount * 3 * 16);
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
        dataSat = 0.1f;
		threadCount = 4096;
		groupSize = 64;
		extraneousNodes = 0;
		demandedNodes = 0;
		dataBufferUsage = 0;
		blockNumber = 1;
		exchangeSize = threadCount;
        Debug.Log("System has " + totalGPURAM + " MB of Video Memory");
        Debug.Log("Databuffer size is " + dataBufferSize());
        Debug.Log("Nodes per heap list is "+ ( dataBufferSize() / ( threadCount * 16 * 3 ) ) );
        dataBuffer = new ComputeBuffer (dataBufferSize (), sizeof(uint));
		heapBuffer = new ComputeBuffer (heapBufferSize (), sizeof(uint));
		taskBuffer = new ComputeBuffer (taskBufferSize (), sizeof(uint));
		blockBuffer = new ComputeBuffer (blockNumber, sizeof(block));
		exchangeBuffer = new ComputeBuffer (exchangeSize, sizeof(uint));
         
		init ();
        #if DBUG
        doAtomicSanity (); 
        #endif
    }

	void Update(){
        //Debug.Log("Frame");
    }
		

	void loadCommon(ComputeShader prog)
    {
        int k = prog.FindKernel("main");
        prog.SetBuffer (k, "DataBuffer", dataBuffer);
		prog.SetBuffer (k, "HeapBuffer", heapBuffer);
		prog.SetBuffer (k, "TaskBuffer", taskBuffer);
		prog.SetBuffer (k, "BlockBuffer", blockBuffer);
		prog.SetBuffer (k, "ExchangeBuffer", exchangeBuffer);
	}

	void loadCommon(ComputeShader prog, ComputeBuffer exg)
    {
        int k = prog.FindKernel("main");
        prog.SetBuffer (k, "DataBuffer", dataBuffer);
		prog.SetBuffer (k, "HeapBuffer", heapBuffer);
		prog.SetBuffer (k, "TaskBuffer", taskBuffer);
		prog.SetBuffer (k, "BlockBuffer", blockBuffer);
		prog.SetBuffer (k, "ExchangeBuffer", exg);
	}

    void run(ComputeShader prog)
    {
        int k = prog.FindKernel("main");
        prog.Dispatch(k, threadCount/groupSize, 1, 1);
    }


    bool checkInitData()
    {
        bool result = true;
        uint[] state = new uint[dataBufferSize()];
        dataBuffer.GetData(state);
        int lim = dataBufferSize() / 16;
        int nodesPer = (dataBufferSize() / (threadCount * 16 * 3));
        for (int i = 0; i < lim; i++)
        {
            if (i % nodesPer == (nodesPer - 1))
            {
                if (state[i * 16] != 0x7FFFFFFF)
                {
                    Debug.Log(i + " - D -> " + state[i*16]);
                    result = false;
                }

            }
            else
            {
                if (state[i * 16] != (i + 1))
                {
                    Debug.Log(i + " - D -> " + state[i*16]);
                    result = false;
                }
            }
            if (state[i * 16 + 1] != (i / (nodesPer*3)))
            {
                Debug.Log(i + " - D ID -> " + state[i * 16 + 1] + "," + (i/(nodesPer*3)));
            }
        }
        return result;
    }

    bool checkInitHeap()
    {
        bool result = true;
        uint[] state = new uint[heapBufferSize()];
        heapBuffer.GetData(state);
        int lim = (threadCount * 3);
        int nodesPer = (dataBufferSize() / (threadCount * 16 * 3));
        for (int i = 0; i < lim; i++)
        {
            if ( ( state[i * 2] != i*nodesPer ) || ( state[i * 2 + 1] != nodesPer) )
            {
                Debug.Log(i + " - H -> " + state[i * 2] + " , " + state[i * 2 + 1]);
                result = false;
            }
        }
        return result;
    }

    bool checkInitTask()
    {
        bool result = true;
        uint[] state = new uint[taskBufferSize()];
        taskBuffer.GetData(state);
        int lim = (threadCount * 3);
        int nodesPer = (dataBufferSize() / (threadCount * 16 * 3));
        for (int i = 0; i < lim; i++)
        {
            if ((state[i * 2] != 0x7FFFFFFF) || (state[i * 2 + 1] != 0))
            {
                Debug.Log(i + " - T -> " + state[i * 2] + " , " + state[i * 2 + 1]);
                result = false;
            }
        }
        return result;
    }


    bool checkDiagnostic(ComputeBuffer DiagExg)
    {
        bool result = true;
        int lim = threadCount*3;
        uint[] state = new uint[lim];
        uint[] check = new uint[lim];
        for (int i = 0; i < lim; i++)
        {
            check[i] = 0;
        }
        DiagExg.GetData(state);
        for ( int i = 0; i < lim; i++)
        { 
            //Debug.Log("Heap Pos: " + state[i]+ " at " + i/3);
            if (state[i] < lim)
            {
                check[state[i]]++;
                if (check[state[i]] > 1)
                {
                    Debug.Log("Duplicate heap position " + state[i] + " at " + i/3);
                }
            }
            else
            {
                Debug.Log("Out of Bounds " + state[i] + " at " + i);
            }
            /*
            if (state[i] != 0x80000000)
            {
                Debug.Log(state[i].ToString("X"));
                result = false;
            }
            */
        }
        return result;
    }

    bool checkAtomicSanity(ComputeBuffer AtomSanExg)
    {
        bool result = true;
        int lim = threadCount + 1;
        uint[] state = new uint[lim];
        uint[] check = new uint[lim];
        for (int i = 0; i < lim; i++)
        {
            check[i] = 0;
        }
        AtomSanExg.GetData(state);
        for (int i = 0; i < lim; i++)
        {
            //Debug.Log("Atom Thread Pos: " + state[i]+ " at " + i);
            if (state[i] < lim)
            {
                check[state[i]]++;
                if (check[state[i]] > 1)
                {
                    Debug.Log("Duplicate atomic value for " + state[i] + " at " + i);
                }
            }
            else
            {
                Debug.Log("Out of Bounds " + state[i] + " at " + i);
            }
            /*
            if (state[i] != 0x80000000)
            {
                Debug.Log(state[i].ToString("X"));
                result = false;
            }
            */
        }
        for(int i = 0; i < lim; i++)
        {
            if (check[i] > 1)
            {
                Debug.Log("Duplicate atomic value for " + i);
            }
            if (check[i] == 0)
            {
                Debug.Log("Missing atomic value for " + i);
            }
            
        }
        return result;
    }

    void init(){
		loadCommon (initializer);
        initializer.SetInt("_dataBufferSize", dataBufferSize());
        float start = Time.realtimeSinceStartup;
        run(initializer);
        float end = Time.realtimeSinceStartup;
        Debug.Log("Initialization took " + (end - start) + " seconds");
        #if DBUG
        if (checkInitData() && checkInitHeap() && checkInitTask())
        {
            Debug.Log("Initialization checks passed");
        }
        else
        {
            Debug.Log("Initialization checks failed");
        }
        #endif
    }

	void diagnose(){
		ComputeBuffer DiagExg = new ComputeBuffer (threadCount*3, sizeof(uint));
		loadCommon (diagnostic,DiagExg);
        diagnostic.SetInt("_loopNo",0x00000800);
        float start = Time.realtimeSinceStartup;
        run(diagnostic);
        checkDiagnostic(DiagExg);
        float end = Time.realtimeSinceStartup;
        Debug.Log("Diagnostics took " + (end - start) + " seconds");
        DiagExg.Dispose();
	}

    void doAtomicSanity()
    {
        ComputeBuffer AtomSan = new ComputeBuffer(threadCount+1, sizeof(uint));
        loadCommon(atomicSanityCheck, AtomSan);
        float start = Time.realtimeSinceStartup;
        run(atomicSanityCheck);
        checkAtomicSanity(AtomSan);
        float end = Time.realtimeSinceStartup;
        Debug.Log("AtomicSanity took " + (end - start) + " seconds");
        AtomSan.Dispose();
    }

}

