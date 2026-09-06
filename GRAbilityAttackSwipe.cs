using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000746 RID: 1862
[Serializable]
public class GRAbilityAttackSwipe : GRAbilityBase
{
	// Token: 0x06002F5F RID: 12127 RVA: 0x00102108 File Offset: 0x00100308
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002F60 RID: 12128 RVA: 0x0010213C File Offset: 0x0010033C
	protected override void OnStart()
	{
		if (this.animData.Count > 0)
		{
			this.lastAnimIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.animData.Count, this.lastAnimIndex);
			this.duration = this.animData[this.lastAnimIndex].duration;
			this.PlayAnim(this.animData[this.lastAnimIndex].animName, 0.1f, this.animData[this.lastAnimIndex].speed);
			this.animNameString = this.animData[this.lastAnimIndex].animName;
		}
		else
		{
			this.duration = 0.5f;
		}
		this.soundAttack.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		this.soundAttack.Play(null);
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
		this.state = GRAbilityAttackSwipe.State.Tell;
	}

	// Token: 0x06002F61 RID: 12129 RVA: 0x00102245 File Offset: 0x00100445
	protected override void OnStop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002F62 RID: 12130 RVA: 0x0010227A File Offset: 0x0010047A
	public override bool IsDone()
	{
		return this.state == GRAbilityAttackSwipe.State.Done;
	}

	// Token: 0x06002F63 RID: 12131 RVA: 0x00102288 File Offset: 0x00100488
	protected override void OnUpdateShared(float dt)
	{
		float num = (float)(Time.timeAsDouble - this.startTime);
		switch (this.state)
		{
		case GRAbilityAttackSwipe.State.Tell:
			this.targetPos = this.root.position + this.root.transform.forward;
			if (this.target != null)
			{
				this.targetPos = this.target.position;
			}
			GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
			if (num > this.tellDuration)
			{
				this.state = GRAbilityAttackSwipe.State.Attack;
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(true);
				}
				this.initialPos = this.root.position;
				this.initialVel = (this.targetPos - this.initialPos).normalized * this.attackMoveSpeed;
				return;
			}
			break;
		case GRAbilityAttackSwipe.State.Attack:
		{
			float num2 = num - this.tellDuration;
			Vector3 vector = this.initialPos + this.initialVel * num2;
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(vector, out navMeshHit, 0.5f, this.walkableArea))
			{
				vector = navMeshHit.position;
				if (NavMesh.Raycast(this.initialPos, vector, out navMeshHit, this.walkableArea))
				{
					vector = navMeshHit.position;
				}
				this.root.position = vector;
			}
			if (num > this.tellDuration + this.attackDuration)
			{
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(false);
				}
				this.state = GRAbilityAttackSwipe.State.FollowThrough;
				return;
			}
			break;
		}
		case GRAbilityAttackSwipe.State.FollowThrough:
			if (num >= this.duration)
			{
				this.state = GRAbilityAttackSwipe.State.Done;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002F64 RID: 12132 RVA: 0x00102440 File Offset: 0x00100640
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

	// Token: 0x06002F65 RID: 12133 RVA: 0x00102480 File Offset: 0x00100680
	public string GetAnimName()
	{
		return this.animNameString;
	}

	// Token: 0x06002F66 RID: 12134 RVA: 0x00102488 File Offset: 0x00100688
	public override bool IsCoolDownOver()
	{
		return base.IsCoolDownOver(this.coolDown);
	}

	// Token: 0x04003C75 RID: 15477
	public float duration;

	// Token: 0x04003C76 RID: 15478
	public float tellDuration;

	// Token: 0x04003C77 RID: 15479
	public float attackDuration;

	// Token: 0x04003C78 RID: 15480
	public float coolDown;

	// Token: 0x04003C79 RID: 15481
	public float attackMoveSpeed;

	// Token: 0x04003C7A RID: 15482
	public List<AnimationData> animData;

	// Token: 0x04003C7B RID: 15483
	public AbilitySound soundAttack;

	// Token: 0x04003C7C RID: 15484
	private GRAbilityAttackSwipe.State state;

	// Token: 0x04003C7D RID: 15485
	public float maxTurnSpeed;

	// Token: 0x04003C7E RID: 15486
	public GameObject damageTrigger;

	// Token: 0x04003C7F RID: 15487
	private Transform target;

	// Token: 0x04003C80 RID: 15488
	private string animNameString;

	// Token: 0x04003C81 RID: 15489
	private int lastAnimIndex = -1;

	// Token: 0x04003C82 RID: 15490
	public Vector3 targetPos;

	// Token: 0x04003C83 RID: 15491
	public Vector3 initialPos;

	// Token: 0x04003C84 RID: 15492
	public Vector3 initialVel;

	// Token: 0x02000747 RID: 1863
	private enum State
	{
		// Token: 0x04003C86 RID: 15494
		Tell,
		// Token: 0x04003C87 RID: 15495
		Attack,
		// Token: 0x04003C88 RID: 15496
		FollowThrough,
		// Token: 0x04003C89 RID: 15497
		Done
	}
}
