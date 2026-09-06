using System;
using UnityEngine;

// Token: 0x0200058F RID: 1423
public class FirstPersonXRaySpecs : MonoBehaviour
{
	// Token: 0x06002411 RID: 9233 RVA: 0x000C220D File Offset: 0x000C040D
	private void OnEnable()
	{
		GorillaBodyRenderer.SetAllSkeletons(true);
	}

	// Token: 0x06002412 RID: 9234 RVA: 0x000C2215 File Offset: 0x000C0415
	private void OnDisable()
	{
		GorillaBodyRenderer.SetAllSkeletons(false);
	}
}
