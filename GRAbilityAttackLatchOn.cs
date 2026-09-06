using System;
using UnityEngine;

// Token: 0x0200074A RID: 1866
[Serializable]
public class GRAbilityAttackLatchOn : GRAbilityBase
{
	// Token: 0x06002F74 RID: 12148 RVA: 0x0010277B File Offset: 0x0010097B
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002F75 RID: 12149 RVA: 0x001027B0 File Offset: 0x001009B0
	protected override void OnStart()
	{
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.agent.SetSpeed(this.tellMoveSpeed);
		this.startTime = Time.timeAsDouble;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002F76 RID: 12150 RVA: 0x0010280A File Offset: 0x00100A0A
	protected override void OnStop()
	{
		this.agent.transform.SetParent(null);
		this.agent.SetIsPathing(true, true);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002F77 RID: 12151 RVA: 0x00102844 File Offset: 0x00100A44
	public override bool IsDone()
	{
		return Time.timeAsDouble - this.startTime >= (double)this.duration;
	}

	// Token: 0x06002F78 RID: 12152 RVA: 0x0010285E File Offset: 0x00100A5E
	protected override void OnUpdateAuthority(float dt)
	{
		this.UpdateNavSpeed();
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
	}

	// Token: 0x06002F79 RID: 12153 RVA: 0x00102888 File Offset: 0x00100A88
	protected override void OnUpdateRemote(float dt)
	{
		this.UpdateNavSpeed();
	}

	// Token: 0x06002F7A RID: 12154 RVA: 0x00102890 File Offset: 0x00100A90
	private void UpdateNavSpeed()
	{
		if (Time.timeAsDouble - this.startTime > (double)this.tellDuration)
		{
			this.agent.SetSpeed(this.attackMoveSpeed);
			this.agent.SetVelocity(this.agent.navAgent.velocity.normalized * this.attackMoveSpeed);
			if (this.damageTrigger != null)
			{
				this.damageTrigger.SetActive(true);
			}
		}
	}

	// Token: 0x06002F7B RID: 12155 RVA: 0x0010290C File Offset: 0x00100B0C
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.target = null;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				this.target = grplayer.transform;
				this.agent.transform.SetParent(grplayer.attachEnemy);
				this.agent.transform.localPosition = Vector3.zero;
				this.agent.transform.localRotation = Quaternion.identity;
				this.agent.SetIsPathing(false, true);
			}
		}
	}

	// Token: 0x04003CA2 RID: 15522
	public float duration;

	// Token: 0x04003CA3 RID: 15523
	public float attackMoveSpeed;

	// Token: 0x04003CA4 RID: 15524
	public float tellDuration;

	// Token: 0x04003CA5 RID: 15525
	public float tellMoveSpeed;

	// Token: 0x04003CA6 RID: 15526
	public string animName;

	// Token: 0x04003CA7 RID: 15527
	public float animSpeed;

	// Token: 0x04003CA8 RID: 15528
	public float maxTurnSpeed;

	// Token: 0x04003CA9 RID: 15529
	public Transform target;

	// Token: 0x04003CAA RID: 15530
	public GameObject damageTrigger;
}
