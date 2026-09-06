using System;
using UnityEngine;

// Token: 0x020007E1 RID: 2017
public class GRReviveMeter : MonoBehaviourTick
{
	// Token: 0x06003381 RID: 13185 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void Awake()
	{
	}

	// Token: 0x06003382 RID: 13186 RVA: 0x00119B28 File Offset: 0x00117D28
	public override void Tick()
	{
		float num = 0f;
		if (this.reviveStation != null && VRRig.LocalRig.OwningNetPlayer != null && this.reviveStation.GetReviveCooldownSeconds() > 0.0)
		{
			num = (float)this.reviveStation.CalculateRemainingReviveCooldownSeconds(VRRig.LocalRig.OwningNetPlayer.ActorNumber) / (float)this.reviveStation.GetReviveCooldownSeconds();
		}
		num = Mathf.Clamp(num, 0f, 1f);
		num = 1f - num;
		this.meter.localScale = new Vector3(1f, num, 1f);
	}

	// Token: 0x040042D7 RID: 17111
	[SerializeField]
	private GRReviveStation reviveStation;

	// Token: 0x040042D8 RID: 17112
	[SerializeField]
	private Transform meter;
}
