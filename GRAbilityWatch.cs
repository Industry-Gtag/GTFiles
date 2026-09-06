using System;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x02000739 RID: 1849
[Serializable]
public class GRAbilityWatch : GRAbilityBase
{
	// Token: 0x06002EF4 RID: 12020 RVA: 0x001005C8 File Offset: 0x000FE7C8
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
	}

	// Token: 0x06002EF5 RID: 12021 RVA: 0x001005E0 File Offset: 0x000FE7E0
	protected override void OnStart()
	{
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.endTime = -1.0;
		if (this.duration > 0f)
		{
			this.endTime = Time.timeAsDouble + (double)this.duration;
		}
		this.agent.SetStopped(true);
	}

	// Token: 0x06002EF6 RID: 12022 RVA: 0x0010063F File Offset: 0x000FE83F
	protected override void OnStop()
	{
		this.agent.SetStopped(false);
	}

	// Token: 0x06002EF7 RID: 12023 RVA: 0x0010064D File Offset: 0x000FE84D
	public override bool IsDone()
	{
		return this.endTime > 0.0 && Time.timeAsDouble >= this.endTime;
	}

	// Token: 0x06002EF8 RID: 12024 RVA: 0x00100672 File Offset: 0x000FE872
	protected override void OnUpdateShared(float dt)
	{
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
	}

	// Token: 0x06002EF9 RID: 12025 RVA: 0x00100698 File Offset: 0x000FE898
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.target = null;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				this.target = grplayer.transform;
			}
		}
	}

	// Token: 0x04003C20 RID: 15392
	public float duration;

	// Token: 0x04003C21 RID: 15393
	public string animName;

	// Token: 0x04003C22 RID: 15394
	public float animSpeed;

	// Token: 0x04003C23 RID: 15395
	public float maxTurnSpeed;

	// Token: 0x04003C24 RID: 15396
	private Transform target;

	// Token: 0x04003C25 RID: 15397
	[ReadOnly]
	public double endTime;
}
