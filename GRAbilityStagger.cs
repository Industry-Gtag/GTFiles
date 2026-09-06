using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200073D RID: 1853
[Serializable]
public class GRAbilityStagger : GRAbilityBase
{
	// Token: 0x06002F10 RID: 12048 RVA: 0x00100B34 File Offset: 0x000FED34
	public void SetStunTime(float time)
	{
		this.stunTime = time;
	}

	// Token: 0x06002F11 RID: 12049 RVA: 0x00100B40 File Offset: 0x000FED40
	public void SetStaggerVelocity(Vector3 vel)
	{
		float magnitude = vel.magnitude;
		if (magnitude > 0f)
		{
			Vector3 vector = vel / magnitude;
			vector.y = 0f;
			vel = vector * magnitude;
		}
		this.staggerMovement.InitFromVelocityAndDuration(vel, this.duration);
	}

	// Token: 0x06002F12 RID: 12050 RVA: 0x00100B8C File Offset: 0x000FED8C
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.staggerMovement.Setup(root);
		this.staggerMovement.interpolationType = GRAbilityInterpolatedMovement.InterpType.EaseOut;
	}

	// Token: 0x06002F13 RID: 12051 RVA: 0x00100BB8 File Offset: 0x000FEDB8
	protected override void OnStart()
	{
		if (this.animData.Count > 0)
		{
			this.lastAnimIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.animData.Count, this.lastAnimIndex);
			this.duration = this.animData[this.lastAnimIndex].duration + this.stunTime;
			this.PlayAnim(this.animData[this.lastAnimIndex].animName, 0.1f, this.animData[this.lastAnimIndex].speed);
			this.animNameString = this.animData[this.lastAnimIndex].animName;
		}
		else
		{
			this.duration = 0.5f + this.stunTime;
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		this.staggerMovement.InitFromVelocityAndDuration(this.staggerMovement.velocity, this.duration);
		this.staggerMovement.Start();
	}

	// Token: 0x06002F14 RID: 12052 RVA: 0x00100CBD File Offset: 0x000FEEBD
	protected override void OnStop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
	}

	// Token: 0x06002F15 RID: 12053 RVA: 0x00100CD8 File Offset: 0x000FEED8
	public override bool IsDone()
	{
		return this.staggerMovement.IsDone();
	}

	// Token: 0x06002F16 RID: 12054 RVA: 0x00100CE5 File Offset: 0x000FEEE5
	protected override void OnUpdateShared(float dt)
	{
		this.staggerMovement.Update(dt);
	}

	// Token: 0x06002F17 RID: 12055 RVA: 0x00100CF3 File Offset: 0x000FEEF3
	public string GetAnimName()
	{
		return this.animNameString;
	}

	// Token: 0x04003C3A RID: 15418
	private float duration;

	// Token: 0x04003C3B RID: 15419
	public List<AnimationData> animData;

	// Token: 0x04003C3C RID: 15420
	private int lastAnimIndex = -1;

	// Token: 0x04003C3D RID: 15421
	private string animNameString;

	// Token: 0x04003C3E RID: 15422
	public GRAbilityInterpolatedMovement staggerMovement;

	// Token: 0x04003C3F RID: 15423
	private float stunTime;
}
