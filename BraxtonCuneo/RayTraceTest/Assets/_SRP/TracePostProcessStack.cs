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
	static int traceParamId 		= Shader.PropertyToID("trace");
	static int drawParamId 			= Shader.PropertyToID("draw");
	static int dataBuffId	 		= Shader.PropertyToID("data");
	static int geomBuffId	 		= Shader.PropertyToID("geom");
	static int taskBuffId	 		= Shader.PropertyToID("task");
	static int memManId		 		= Shader.PropertyToID("mem_man");
	static int noUnrollId		 	= Shader.PropertyToID("no_unroll");
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
	ComputeShader drawer;

	[SerializeField]
	Shader blitter;


	static Dictionary<int, TraceParams> trace_param_registry = null;
	static DrawParams		draw_params = null;
	static DataBuffer 		data_buffer = null;
	static TaskBuffer 		task_buffer = null;
	static DataBuffer 		geom_buffer = null;
	static DataBuffer 		mem_man_buffer = null;
	static ShaderLogBuffer  log_buffer = null;
	static ComputeBuffer	no_unroll_buffer = null;

	int trace_kernel_index = -1;
	int draw_kernel_index = -1;

	int frame = 0;

	WGCountBuffer	wg_count_buffer = null;



	static void InitializeStatic () {


		//Debug.Log("HMMM");
		//Debug.Log(data_buffer);
		if(data_buffer == null){
			//Debug.Log("YUP");
			data_buffer		= ScriptableObject.CreateInstance<DataBuffer>();
			geom_buffer		= ScriptableObject.CreateInstance<DataBuffer>();
			task_buffer		= ScriptableObject.CreateInstance<TaskBuffer>();
			mem_man_buffer	= ScriptableObject.CreateInstance<DataBuffer>();
			log_buffer 		= ScriptableObject.CreateInstance<ShaderLogBuffer>();

			no_unroll_buffer = new ComputeBuffer(1,4,ComputeBufferType.Default);
			int[] nu_buff = {0};
			no_unroll_buffer.SetData (nu_buff,0,0,1);


			data_buffer.init(1048576);
			geom_buffer.init(32);
			task_buffer.init(16,16);
			mem_man_buffer.init(1048576);
			log_buffer.init(1048576);

			/*
			data_buffer.resize(1048576);
			mem_man_buffer.resize(1048576);
			log_buffer.resize(1048576);
			*/

			data_buffer.set_all(0);
			geom_buffer.set_all(unchecked( (int) 0xDEADBEEF ));
			geom_buffer[1] = 0;
			mem_man_buffer.set_all(0x08102040);



			data_buffer		.push();
			geom_buffer		.push();
			task_buffer		.push();
			mem_man_buffer	.push();
			log_buffer		.push();

		}

		if (trace_param_registry == null) {
			trace_param_registry = new Dictionary<int, TraceParams>();
		}

		if (fullScreenTriangle) {
			return;
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



	void InitializeDraw(){

		if(draw_kernel_index == -1){
			draw_kernel_index = drawer.FindKernel("main");
		}

		if(wg_count_buffer == null){
			wg_count_buffer = ScriptableObject.CreateInstance<WGCountBuffer>();
			wg_count_buffer.work_group_count = new Vector3Int (64,1,1);
			wg_count_buffer	.push(drawer);
		}

		if( draw_params == null){
			draw_params = ScriptableObject.CreateInstance<DrawParams>();
			draw_params.init();
		}


		//data_buffer.resize(1048576);
		//mem_man_buffer.resize(1048576);
		//log_buffer.resize(1048576);
		//mem_man_buffer.set_all(0x08102040);

		data_buffer		.bind(drawer, draw_kernel_index, dataBuffId);
		geom_buffer		.bind(drawer, draw_kernel_index, geomBuffId);
		task_buffer		.bind(drawer, draw_kernel_index, taskBuffId);
		mem_man_buffer	.bind(drawer, draw_kernel_index, "mem_man");
		log_buffer		.bind(drawer, draw_kernel_index, debugLogId);
		draw_params		.bind(drawer, draw_kernel_index, drawParamId);

		drawer.SetBuffer(draw_kernel_index, noUnrollId, no_unroll_buffer);


		wg_count_buffer.work_group_count.x = 64;
		wg_count_buffer.work_group_count.y = 1;
		wg_count_buffer.work_group_count.z = 1;
		wg_count_buffer	.push(drawer);



	}


	void InitializeTrace(Camera camera){
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
			tp_new.init();
			tp_new.screen_dims	= new Vector2Int (camera.pixelWidth,camera.pixelHeight);
			tp_new.camera_dims	= new Vector4 (camera.aspect,1,camera.nearClipPlane,camera.farClipPlane);
			tp_new.camera_pos 	= camera.transform.position;
			tp_new.camera_dir	= camera.transform.forward;
			tp_new.step_count	= 32;
			trace_param_registry [camera.GetInstanceID ()] = tp_new;
		}

		//data_buffer.resize(1048576);
		//mem_man_buffer.resize(1048576);
		//log_buffer.resize(1048576);
		//mem_man_buffer.set_all(0x08102040);

		data_buffer		.bind(tracer, trace_kernel_index, dataBuffId);
		geom_buffer		.bind(tracer, trace_kernel_index, geomBuffId);
		mem_man_buffer	.bind(tracer, trace_kernel_index, "mem_man");
		log_buffer		.bind(tracer, trace_kernel_index, debugLogId);

		tracer.SetBuffer(trace_kernel_index, noUnrollId, no_unroll_buffer);



		TraceParams tp = trace_param_registry [camera.GetInstanceID ()];
		tp.screen_dims	= new Vector2Int (camera.pixelWidth,camera.pixelHeight);
		tp.camera_dims	= new Vector4 (camera.aspect,1,camera.nearClipPlane,camera.farClipPlane);
		tp.camera_pos 	= camera.transform.position;
		tp.camera_dir	= camera.transform.forward;
		tp.step_count	= 32;

		tp.bind(tracer, trace_kernel_index, traceParamId);
		tp.push();


		wg_count_buffer.work_group_count.x = 64;
		wg_count_buffer.work_group_count.y = 1;
		wg_count_buffer.work_group_count.z = 1;
		wg_count_buffer	.push(tracer);


	}


	public void Draw (CommandBuffer cb) {

		//*

		InitializeStatic();
		InitializeDraw();

		draw_params.block_index = 0;
		draw_params.brush_center = new Vector3(0f,0f,0f);

		task_buffer.prime();


		if( (frame % 100) == 0){
			geom_buffer[1] = 0;
			geom_buffer.push();
		}

		draw_params.push(drawer);





		cb.DispatchCompute (drawer, draw_kernel_index, 64,1,1);//camera.pixelWidth / 8, camera.pixelHeight / 8, 1);

		GraphicsFence f = cb.CreateAsyncGraphicsFence ();

		cb.WaitOnAsyncGraphicsFence (f);
		//*/


		frame++;

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


		InitializeTrace(camera);
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


