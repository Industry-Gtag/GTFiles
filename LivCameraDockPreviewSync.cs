using System;
using Docking;
using UnityEngine;

// Token: 0x02000420 RID: 1056
[ExecuteAlways]
public class LivCameraDockPreviewSync : MonoBehaviour
{
	// Token: 0x0400243D RID: 9277
	private LivCameraDock dock;

	// Token: 0x0400243E RID: 9278
	private Camera parentCamera;

	// Token: 0x0400243F RID: 9279
	private float _lastCameraFOV = -1f;

	// Token: 0x04002440 RID: 9280
	private float _lastDockFOV = -1f;
}
