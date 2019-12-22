using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/traceSRP")]
public class TracePipelineAsset : RenderPipelineAsset {

	[SerializeField]
	TracePostProcessStack	defaultStack;


	protected override RenderPipeline CreatePipeline () {
		return new TracePipeline(defaultStack);
	}

}
	



