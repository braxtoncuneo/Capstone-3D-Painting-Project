using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System;



[CreateAssetMenu(menuName = "Rendering/TracePostProcessStack")]
public class TracePostProcessStack : ScriptableObject {

	static int cameraColorId 		= Shader.PropertyToID("color_texture");
	static int oldDepthId 			= Shader.PropertyToID("old_depth");
	static int traceParamId 		= Shader.PropertyToID("trace_params");
	static int dataBuffId	 		= Shader.PropertyToID("data");
	static int memManId		 		= Shader.PropertyToID("mem_man");
	static int debugLogId	 		= Shader.PropertyToID("debug_log");
	static int wgCountId	 		= Shader.PropertyToID("SPIRV_Cross_NumWorkgroups");
	static int newDepthId	 		= Shader.PropertyToID("new_depth");
	static int screenDimsId 		= Shader.PropertyToID("screen_dims");
	static int mainTexId 			= Shader.PropertyToID("_MainTex");
	static int depthTexId 			= Shader.PropertyToID("_DepthTex");



	static Mesh fullScreenTriangle 	= null;
	static Material material;


	[SerializeField]
	ComputeShader tracer;

	[SerializeField]
	Shader blitter;


	static Dictionary<int, TraceParams> trace_param_registry = null;
	static DataBuffer 		data_buffer = null;
	static DataBuffer 		mem_man_buffer = null;
	static ShaderLogBuffer  log_buffer = null;

	int trace_kernel_index = -1;
	WGCountBuffer	wg_count_buffer = null;



	static void InitializeStatic () {
		if (fullScreenTriangle) {
			return;
		}

		if (trace_param_registry == null) {
			trace_param_registry = new Dictionary<int, TraceParams>();
		}

		fullScreenTriangle = new Mesh {
			name = "My Post-Processing Stack Full-Screen Triangle",
			vertices = new Vector3[] {
				new Vector3(-1f, -1f, 0f),
				new Vector3(-1f,  3f, 0f),
				new Vector3( 3f, -1f, 0f)
			},
			triangles = new int[] { 0, 1, 2 },
		};
			
		fullScreenTriangle.UploadMeshData (true);

		material =
			new Material(Shader.Find("_Shaders/BlitShader")) {
			name = "blit_material",
			hideFlags = HideFlags.HideAndDontSave
		};


	}
		

	void InitializeNonStatic(Camera camera){
		//trace_kernel_index = tracer.FindKernel("main");

		if(trace_kernel_index == -1){
			trace_kernel_index = tracer.FindKernel("main");
		}

		if(wg_count_buffer == null){
			wg_count_buffer = ScriptableObject.CreateInstance<WGCountBuffer>();
			wg_count_buffer.work_group_count = new Vector3Int (64,1,1);
			wg_count_buffer	.push(tracer);
		}

		if (!trace_param_registry.ContainsKey (camera.GetInstanceID ())) {
			TraceParams tp_new = ScriptableObject.CreateInstance<TraceParams>();
			tp_new.screen_dims	= new Vector2Int (camera.pixelWidth,camera.pixelHeight);
			tp_new.camera_dims	= new Vector4 (camera.aspect,1,camera.nearClipPlane,camera.farClipPlane);
			tp_new.camera_pos 	= camera.transform.position;
			tp_new.camera_dir	= camera.transform.forward;
			tp_new.step_count	= 32;
			trace_param_registry [camera.GetInstanceID ()] = tp_new;
		}



		//Debug.Log("HMMM");
		//Debug.Log(data_buffer);
		if(data_buffer == null){
			Debug.Log("YUP");
			data_buffer		= ScriptableObject.CreateInstance<DataBuffer>();
			mem_man_buffer	= ScriptableObject.CreateInstance<DataBuffer>();
			log_buffer 		= ScriptableObject.CreateInstance<ShaderLogBuffer>();


			data_buffer.init(1048576);
			mem_man_buffer.init(1048576);
			log_buffer.init(1048576);

			data_buffer.resize(1048576);
			mem_man_buffer.resize(1048576);
			log_buffer.resize(1048576);

			data_buffer.set_all(0);
			mem_man_buffer.set_all(0x08102040);



			data_buffer		.push();
			mem_man_buffer	.push();
			log_buffer		.push();

		}



		//data_buffer.resize(1048576);
		//mem_man_buffer.resize(1048576);
		//log_buffer.resize(1048576);
		mem_man_buffer.set_all(0x08102040);

		data_buffer		.bind(tracer, trace_kernel_index, dataBuffId);
		mem_man_buffer	.bind(tracer, trace_kernel_index, "mem_man");
		log_buffer		.bind(tracer, trace_kernel_index, debugLogId);



		TraceParams tp = trace_param_registry [camera.GetInstanceID ()];
		tp.screen_dims	= new Vector2Int (camera.pixelWidth,camera.pixelHeight);
		tp.camera_dims	= new Vector4 (camera.aspect,1,camera.nearClipPlane,camera.farClipPlane);
		tp.camera_pos 	= camera.transform.position;
		tp.camera_dir	= camera.transform.forward;
		tp.step_count	= 32;
		tp.push(tracer);


		wg_count_buffer.work_group_count.x = 64;
		wg_count_buffer.work_group_count.y = 1;
		wg_count_buffer.work_group_count.z = 1;
		wg_count_buffer	.push(tracer);




	}


	public void Render (CommandBuffer cb, Camera camera, RenderTexture color_rt, RenderTexture old_depth_rt, RenderTexture new_depth_rt) {


		InitializeStatic();


		cb.SetGlobalTexture(cameraColorId,  color_rt);
		cb.SetGlobalTexture(newDepthId, new_depth_rt);


		//cb.SetComputeTextureParam	(tracer, trace_kernel_index, cameraColorId,	color_rt);
		//cb.SetComputeTextureParam	(tracer, trace_kernel_index, oldDepthId, 		old_depth_rt);
		//cb.SetComputeIntParams 		(tracer, screenDimsId, 		screen_dims);
			
		cb.SetComputeTextureParam	(tracer, trace_kernel_index, cameraColorId,		color_rt);
		cb.SetComputeTextureParam	(tracer, trace_kernel_index, oldDepthId, 		old_depth_rt);


		//Debug.Log(trace_kernel_index);


		InitializeNonStatic(camera);
		cb.DispatchCompute (tracer, trace_kernel_index, 64,1,1);//camera.pixelWidth / 8, camera.pixelHeight / 8, 1);

		GraphicsFence f = cb.CreateAsyncGraphicsFence ();

		cb.WaitOnAsyncGraphicsFence (f);

		//mem_man_buffer.pull();
		//mem_man_buffer.print_content();


		cb.SetRenderTarget(
			BuiltinRenderTextureType.CameraTarget,
			RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
		);
		cb.DrawMesh(fullScreenTriangle, Matrix4x4.identity, material);


		try {
			


		} catch (Exception ex) {

			Debug.Log(ex);

		}

		try{

		/*
		
		//cb.SetComputeTextureParam (tracer, trace_kernel_index, newDepthId,		new_depth_rt);

		cb.SetRenderTarget(
			BuiltinRenderTextureType.CameraTarget,
			RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
		);
		cb.DrawMesh(fullScreenTriangle, Matrix4x4.identity, material);



		//cb.Blit(color_rt, BuiltinRenderTextureType.CameraTarget);
		//cb.Blit(depth_rt, BuiltinRenderTextureType.Depth);

		*/

		}
		catch {

		}

	}
}


