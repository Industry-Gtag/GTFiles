using System;
using UnityEngine;

// Token: 0x020000C1 RID: 193
public class CosmeticCritterCatcherButterflyNet : CosmeticCritterCatcher
{
	// Token: 0x060004BF RID: 1215 RVA: 0x0001A994 File Offset: 0x00018B94
	public override CosmeticCritterAction GetLocalCatchAction(CosmeticCritter critter)
	{
		if (!(critter is CosmeticCritterButterfly) || (critter.transform.position - this.velocityEstimator.transform.position).sqrMagnitude > this.maxCatchRadius * this.maxCatchRadius || this.velocityEstimator.linearVelocity.sqrMagnitude < this.minCatchSpeed * this.minCatchSpeed)
		{
			return CosmeticCritterAction.None;
		}
		return CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn;
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0001AA08 File Offset: 0x00018C08
	public override bool ValidateRemoteCatchAction(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		return base.ValidateRemoteCatchAction(critter, catchAction, serverTime) && critter is CosmeticCritterButterfly && (critter.transform.position - this.velocityEstimator.transform.position).sqrMagnitude <= this.maxCatchRadius * this.maxCatchRadius + 1f && this.velocityEstimator.linearVelocity.sqrMagnitude >= this.minCatchSpeed * this.minCatchSpeed - 1f && catchAction == (CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn);
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x0001AA93 File Offset: 0x00018C93
	public override void OnCatch(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		this.caughtButterflyParticleSystem.Emit((critter as CosmeticCritterButterfly).GetEmitParams, 1);
		this.catchFX.Play();
		this.catchSFX.Play();
	}

	// Token: 0x0400053C RID: 1340
	[Tooltip("Use this for calculating the catch position and velocity.")]
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x0400053D RID: 1341
	[Tooltip("Catch the Butterfly if it is within this radius.")]
	[SerializeField]
	private float maxCatchRadius;

	// Token: 0x0400053E RID: 1342
	[Tooltip("Only catch the Butterfly if the net is moving faster than this speed.")]
	[SerializeField]
	private float minCatchSpeed;

	// Token: 0x0400053F RID: 1343
	[Tooltip("Spawn a particle inside the net representing the caught Butterfly.")]
	[SerializeField]
	private ParticleSystem caughtButterflyParticleSystem;

	// Token: 0x04000540 RID: 1344
	[Tooltip("Play this particle effect when catching a Butterfly.")]
	[SerializeField]
	private ParticleSystem catchFX;

	// Token: 0x04000541 RID: 1345
	[Tooltip("Play this sound when catching a Butterfly.")]
	[SerializeField]
	private AudioSource catchSFX;
}
