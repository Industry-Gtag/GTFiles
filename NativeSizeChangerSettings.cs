using System;
using UnityEngine;

// Token: 0x0200042A RID: 1066
[Serializable]
public class NativeSizeChangerSettings
{
	// Token: 0x1700027C RID: 636
	// (get) Token: 0x06001956 RID: 6486 RVA: 0x0008E9A6 File Offset: 0x0008CBA6
	// (set) Token: 0x06001957 RID: 6487 RVA: 0x0008E9AE File Offset: 0x0008CBAE
	public Vector3 WorldPosition
	{
		get
		{
			return this.worldPosition;
		}
		set
		{
			this.worldPosition = value;
		}
	}

	// Token: 0x1700027D RID: 637
	// (get) Token: 0x06001958 RID: 6488 RVA: 0x0008E9B7 File Offset: 0x0008CBB7
	// (set) Token: 0x06001959 RID: 6489 RVA: 0x0008E9BF File Offset: 0x0008CBBF
	public float ActivationTime
	{
		get
		{
			return this.activationTime;
		}
		set
		{
			this.activationTime = value;
		}
	}

	// Token: 0x0400245F RID: 9311
	public const float MinAllowedSize = 0.1f;

	// Token: 0x04002460 RID: 9312
	public const float MaxAllowedSize = 10f;

	// Token: 0x04002461 RID: 9313
	private Vector3 worldPosition;

	// Token: 0x04002462 RID: 9314
	private float activationTime;

	// Token: 0x04002463 RID: 9315
	[Range(0.1f, 10f)]
	public float playerSizeScale = 1f;

	// Token: 0x04002464 RID: 9316
	public bool ExpireOnRoomJoin = true;

	// Token: 0x04002465 RID: 9317
	public bool ExpireInWater = true;

	// Token: 0x04002466 RID: 9318
	public float ExpireAfterSeconds;

	// Token: 0x04002467 RID: 9319
	public float ExpireOnDistance;
}
