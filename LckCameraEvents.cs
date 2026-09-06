using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

// Token: 0x020003F5 RID: 1013
public class LckCameraEvents : MonoBehaviour
{
	// Token: 0x06001818 RID: 6168 RVA: 0x00089DE1 File Offset: 0x00087FE1
	private void OnEnable()
	{
		RenderPipelineManager.beginCameraRendering += this.RenderPipelineManagerOnbeginCameraRendering;
		RenderPipelineManager.endCameraRendering += this.RenderPipelineManagerOnendCameraRendering;
	}

	// Token: 0x06001819 RID: 6169 RVA: 0x00089E05 File Offset: 0x00088005
	private void OnDisable()
	{
		RenderPipelineManager.beginCameraRendering -= this.RenderPipelineManagerOnbeginCameraRendering;
		RenderPipelineManager.endCameraRendering -= this.RenderPipelineManagerOnendCameraRendering;
	}

	// Token: 0x0600181A RID: 6170 RVA: 0x00089E29 File Offset: 0x00088029
	private void RenderPipelineManagerOnbeginCameraRendering(ScriptableRenderContext scriptableRenderContext, Camera camera)
	{
		if (this._camera != camera)
		{
			return;
		}
		UnityEvent unityEvent = this.onPreRender;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x0600181B RID: 6171 RVA: 0x00089E4A File Offset: 0x0008804A
	private void RenderPipelineManagerOnendCameraRendering(ScriptableRenderContext scriptableRenderContext, Camera camera)
	{
		if (this._camera != camera)
		{
			return;
		}
		UnityEvent unityEvent = this.onPostRender;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04002351 RID: 9041
	[SerializeField]
	private Camera _camera;

	// Token: 0x04002352 RID: 9042
	public UnityEvent onPreRender;

	// Token: 0x04002353 RID: 9043
	public UnityEvent onPostRender;
}
