using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000378 RID: 888
[DefaultExecutionOrder(-2147483648)]
public class GTFirstPersonCamera : MonoBehaviour
{
	// Token: 0x1700022C RID: 556
	// (get) Token: 0x060015CD RID: 5581 RVA: 0x000735B8 File Offset: 0x000717B8
	// (set) Token: 0x060015CE RID: 5582 RVA: 0x000735BF File Offset: 0x000717BF
	public static Camera camera { get; private set; }

	// Token: 0x060015CF RID: 5583 RVA: 0x000735C7 File Offset: 0x000717C7
	public void Awake()
	{
		GTFirstPersonCamera.camera = base.GetComponent<Camera>();
		if (GTFirstPersonCamera.camera == null)
		{
			Debug.LogError("[GTFirstPersonCamera]  ERROR!!!  Could not find Camera on same GameObject!");
			return;
		}
		RenderPipelineManager.beginCameraRendering += this._OnPreRender;
	}

	// Token: 0x060015D0 RID: 5584 RVA: 0x000735FD File Offset: 0x000717FD
	private void _OnPreRender(ScriptableRenderContext context, Camera cam)
	{
		if (cam == GTFirstPersonCamera.camera)
		{
			Action onPreRenderEvent = GTFirstPersonCamera.OnPreRenderEvent;
			if (onPreRenderEvent == null)
			{
				return;
			}
			onPreRenderEvent();
		}
	}

	// Token: 0x04001A9A RID: 6810
	private const string preLog = "[GTFirstPersonCamera]  ";

	// Token: 0x04001A9B RID: 6811
	private const string preErr = "[GTFirstPersonCamera]  ERROR!!!  ";

	// Token: 0x04001A9D RID: 6813
	public static Action OnPreRenderEvent;
}
