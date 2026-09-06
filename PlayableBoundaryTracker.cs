using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000275 RID: 629
public class PlayableBoundaryTracker : MonoBehaviour
{
	// Token: 0x170001AC RID: 428
	// (get) Token: 0x06001105 RID: 4357 RVA: 0x0005B67B File Offset: 0x0005987B
	// (set) Token: 0x06001106 RID: 4358 RVA: 0x0005B683 File Offset: 0x00059883
	public float signedDistanceToBoundary { get; private set; }

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x06001107 RID: 4359 RVA: 0x0005B68C File Offset: 0x0005988C
	// (set) Token: 0x06001108 RID: 4360 RVA: 0x0005B694 File Offset: 0x00059894
	public float prevSignedDistanceToBoundary { get; private set; }

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x06001109 RID: 4361 RVA: 0x0005B69D File Offset: 0x0005989D
	// (set) Token: 0x0600110A RID: 4362 RVA: 0x0005B6A5 File Offset: 0x000598A5
	public float timeSinceCrossingBorder { get; private set; }

	// Token: 0x0600110B RID: 4363 RVA: 0x0005B6AE File Offset: 0x000598AE
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsInsideZone()
	{
		return Mathf.Sign(this.signedDistanceToBoundary) < 0f;
	}

	// Token: 0x0600110C RID: 4364 RVA: 0x0005B6C4 File Offset: 0x000598C4
	public void UpdateSignedDistanceToBoundary(float newDistance, float elapsed)
	{
		this.prevSignedDistanceToBoundary = this.signedDistanceToBoundary;
		this.signedDistanceToBoundary = newDistance;
		if ((int)Mathf.Sign(this.prevSignedDistanceToBoundary) != (int)Mathf.Sign(this.signedDistanceToBoundary))
		{
			this.timeSinceCrossingBorder = 0f;
			return;
		}
		this.timeSinceCrossingBorder += elapsed;
	}

	// Token: 0x0600110D RID: 4365 RVA: 0x0005B718 File Offset: 0x00059918
	internal void ResetValues()
	{
		this.timeSinceCrossingBorder = 0f;
	}

	// Token: 0x04001449 RID: 5193
	public float radius = 1f;
}
