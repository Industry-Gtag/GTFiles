using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200085D RID: 2141
internal struct OnHandTapFX : IFXEffectContext<HandEffectContext>
{
	// Token: 0x170004EC RID: 1260
	// (get) Token: 0x06003750 RID: 14160 RVA: 0x0012FD38 File Offset: 0x0012DF38
	public HandEffectContext effectContext
	{
		get
		{
			HandEffectContext handEffect = this.rig.GetHandEffect(this.isLeftHand, this.stiltID);
			this.rig.SetHandEffectData(handEffect, this.surfaceIndex, this.isDownTap, this.isLeftHand, this.stiltID, this.volume, this.speed, this.tapDir);
			return handEffect;
		}
	}

	// Token: 0x170004ED RID: 1261
	// (get) Token: 0x06003751 RID: 14161 RVA: 0x0012FD94 File Offset: 0x0012DF94
	public FXSystemSettings settings
	{
		get
		{
			return this.rig.fxSettings;
		}
	}

	// Token: 0x040047A5 RID: 18341
	public VRRig rig;

	// Token: 0x040047A6 RID: 18342
	public Vector3 tapDir;

	// Token: 0x040047A7 RID: 18343
	public bool isDownTap;

	// Token: 0x040047A8 RID: 18344
	public bool isLeftHand;

	// Token: 0x040047A9 RID: 18345
	public StiltID stiltID;

	// Token: 0x040047AA RID: 18346
	public int surfaceIndex;

	// Token: 0x040047AB RID: 18347
	public float volume;

	// Token: 0x040047AC RID: 18348
	public float speed;
}
