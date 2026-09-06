using System;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class SIResourceMonkeIdol : SIResource
{
	// Token: 0x0600094C RID: 2380 RVA: 0x0003250A File Offset: 0x0003070A
	protected override void OnEnable()
	{
		base.OnEnable();
		this.depositEnabledParticle.SetActive(SIPlayer.LocalPlayer.CanLimitedResourceBeDeposited(this.limitedDepositType));
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x0003252D File Offset: 0x0003072D
	public override void HandleDepositAuth(SIPlayer depositingPlayer)
	{
		SIPlayer.LocalPlayer.TriggerIdolDepositedCelebration(base.transform.position);
	}

	// Token: 0x04000B65 RID: 2917
	[SerializeField]
	private GameObject depositEnabledParticle;
}
