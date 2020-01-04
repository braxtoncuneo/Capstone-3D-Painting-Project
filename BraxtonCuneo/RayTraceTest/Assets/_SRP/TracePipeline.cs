using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;


public class TracePipeline : RenderPipeline {

	struct rt_bundle{
		public RenderTexture color;
		public RenderTexture old_depth;
		public RenderTexture new_depth;
	}


	//CullResults cull;


	Material nullMaterial;

	CommandBuffer camera_buffer = new CommandBuffer{ name = "render_camera" };
	CommandBuffer post_proc_buffer = new CommandBuffer{ name = "post_proc" };


	TracePostProcessStack defaultStack;

	Dictionary<int, rt_bundle> rt_registry = new Dictionary<int, rt_bundle>();


	protected override void Render (
		ScriptableRenderContext renderContext, Camera[] cameras
	) {

		defaultStack.Draw(post_proc_buffer);
		renderContext.ExecuteCommandBuffer (post_proc_buffer);
		post_proc_buffer.Clear ();

		//base.Render (renderContext, cameras);
		foreach (var camera in cameras) {
			Render (renderContext, camera);
		}
	}


	void Render (ScriptableRenderContext context, Camera camera) {


		context.SetupCameraProperties(camera);


		/*
		if (defaultStack) {
			camera_buffer.GetTemporaryRT(
				cameraColorTextureId, camera.pixelWidth, camera.pixelHeight, 0,
				FilterMode.Point, RenderTextureFormat.ARGB32
			);
			camera_buffer.GetTemporaryRT(
				cameraDepthTextureId, camera.pixelWidth, camera.pixelHeight, 24,
				FilterMode.Point, RenderTextureFormat.Depth
			);
			camera_buffer.SetRenderTarget(
				cameraColorTextureId,
				RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
				cameraDepthTextureId,
				RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
			);
		}
		*/



		rt_bundle bundle;

		//if (defaultStack) {

			if (! rt_registry.ContainsKey (camera.GetInstanceID())) {
				rt_bundle new_bundle;
				new_bundle.color = new RenderTexture (camera.pixelWidth, camera.pixelHeight, 0, RenderTextureFormat.ARGB32);
				new_bundle.color.enableRandomWrite = true;
				//new_bundle.color.filterMode = FilterMode.Point;
				//new_bundle.color.format = RenderTextureFormat.ARGB32;
				new_bundle.color.autoGenerateMips = false;
				new_bundle.color.useMipMap = false;
				new_bundle.color.Create();

				new_bundle.old_depth = new RenderTexture (camera.pixelWidth, camera.pixelHeight, 16, RenderTextureFormat.Depth);
				new_bundle.old_depth.enableRandomWrite = false;
				//new_bundle.depth.filterMode = FilterMode.Point;
				//new_bundle.depth.format = RenderTextureFormat.Depth;
				new_bundle.old_depth.autoGenerateMips = false;
				new_bundle.old_depth.useMipMap = false;
				new_bundle.old_depth.Create();

				new_bundle.new_depth = new RenderTexture (camera.pixelWidth, camera.pixelHeight, 0, RenderTextureFormat.RFloat);
				new_bundle.new_depth.enableRandomWrite = true;
				//new_bundle.depth.filterMode = FilterMode.Point;
				//new_bundle.depth.format = RenderTextureFormat.Depth;
				new_bundle.new_depth.autoGenerateMips = false;
				new_bundle.new_depth.useMipMap = false;
				new_bundle.new_depth.Create();

				rt_registry [camera.GetInstanceID()] = new_bundle;
			}


			bundle = rt_registry[camera.GetInstanceID()];


			camera_buffer.SetRenderTarget(
				bundle.color,
				RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
				bundle.old_depth,
				RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store
			);
		//}

		ScriptableCullingParameters cullingParameters;

		if (!camera.TryGetCullingParameters(out cullingParameters)) {
			return;
		}


		#if UNITY_EDITOR
		if (camera.cameraType == CameraType.SceneView) {
		ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
		}
		#endif

		CullingResults cull = context.Cull(ref cullingParameters);


		//buffer.BeginSample ("render_camera");

		CameraClearFlags clearFlags = camera.clearFlags;
		camera_buffer.ClearRenderTarget(
			(clearFlags & CameraClearFlags.Depth) != 0,
			(clearFlags & CameraClearFlags.Color) != 0,
			camera.backgroundColor
		);


		context.ExecuteCommandBuffer(camera_buffer);
		camera_buffer.Clear();


		context.DrawSkybox(camera);

		var sortSettings = new SortingSettings (camera);
		var colSettings = new DrawingSettings(new ShaderTagId("Yo"),sortSettings);
		var fwdSettings = new DrawingSettings(new ShaderTagId("ForwardBase"),sortSettings);
		var addSettings = new DrawingSettings(new ShaderTagId("ForwardAdd"),sortSettings);
		var allSettings = new DrawingSettings(new ShaderTagId("Always"),sortSettings);
		var filterSettings = new FilteringSettings (RenderQueueRange.opaque);



		context.DrawRenderers(
			cull, ref colSettings, ref filterSettings
		);


		context.DrawRenderers(
			cull, ref fwdSettings, ref filterSettings
		);


		context.DrawRenderers(
			cull, ref addSettings, ref filterSettings
		);
		/*

		context.DrawRenderers(
			cull, ref allSettings, ref filterSettings
		);
		//*/
//*

//*/

		//DrawDefaultPipeline(context, camera); 
/*
		filterSettings.renderQueueRange = RenderQueueRange.transparent;
		context.DrawRenderers(
			cull, ref drawSettings, ref filterSettings
		);
//*/


		//if (defaultStack) {
		defaultStack.Render (post_proc_buffer, camera, bundle.color, bundle.old_depth, bundle.new_depth);
			context.ExecuteCommandBuffer (post_proc_buffer);
			post_proc_buffer.Clear ();
			//camera_buffer.ReleaseTemporaryRT(cameraColorTextureId);
			//camera_buffer.ReleaseTemporaryRT(cameraDepthTextureId);
		//}

		//context.ExecuteCommandBuffer(buffer);
		//buffer.Clear();
		//buffer.EndSample ("render_camera");
		context.Submit();
	}

	public TracePipeline (
		TracePostProcessStack defaultStack
	)
	{
		this.defaultStack = defaultStack;
	}


}
