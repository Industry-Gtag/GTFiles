using System;
using GorillaExtensions;
using GorillaGameModes;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000A6D RID: 2669
public class CustomMapsAttackBehaviour : CustomMapsBehaviourBase
{
	// Token: 0x060044A3 RID: 17571 RVA: 0x0016F104 File Offset: 0x0016D304
	public CustomMapsAttackBehaviour(CustomMapsAIBehaviourController AIController, AIAgent agentSettings)
	{
		this.attackType = agentSettings.attackType;
		this.attackDist = agentSettings.attackDist;
		this.attackDistSq = this.attackDist * this.attackDist;
		this.stopMovingToAttack = agentSettings.stopMovingToAttack;
		this.useColliders = agentSettings.useColliders;
		this.damageDelayAfterPlayingAnimation = agentSettings.damageDelayAfterPlayingAnim;
		this.damageAmount = agentSettings.damageAmount;
		this.attackAnimName = agentSettings.attackAnimName;
		this.sightOffset = agentSettings.sightOffset;
		this.sightFOV = agentSettings.sightFOV;
		this.sightMinDot = Mathf.Cos(this.sightFOV / 2f * 0.017453292f);
		this.controller = AIController;
		this.animBlendTime = agentSettings.animBlendTime;
		this.turnSpeed = agentSettings.turnSpeed * 10f;
		this.timeBetweenAttacks = agentSettings.timeBetweenAttacks;
		this.controller.attributes.AddAttribute(GRAttributeType.PlayerDamage, this.damageAmount);
		this.state = CustomMapsAttackBehaviour.State.Idle;
	}

	// Token: 0x060044A4 RID: 17572 RVA: 0x0016F203 File Offset: 0x0016D403
	public override bool CanExecute()
	{
		return !this.controller.IsNull() && !this.controller.TargetPlayer.IsNull() && this.IsTargetInAttackRange(null) && this.IsTargetVisible();
	}

	// Token: 0x060044A5 RID: 17573 RVA: 0x0016F23C File Offset: 0x0016D43C
	private bool IsTargetVisible()
	{
		Vector3 vector = this.controller.transform.position + this.controller.transform.TransformVector(this.sightOffset);
		return this.controller.IsTargetVisible(vector, this.controller.TargetPlayer, this.attackDist);
	}

	// Token: 0x060044A6 RID: 17574 RVA: 0x0016F294 File Offset: 0x0016D494
	private bool IsTargetInAttackRange(GRPlayer target = null)
	{
		if (target.IsNull() && this.controller.TargetPlayer.IsNull())
		{
			return false;
		}
		if (target.IsNotNull())
		{
			Vector3 vector;
			return this.controller.IsTargetInRange(this.controller.transform.position, target, this.attackDistSq, out vector);
		}
		Vector3 vector2;
		return this.controller.IsTargetInRange(this.controller.transform.position, this.controller.TargetPlayer, this.attackDistSq, out vector2);
	}

	// Token: 0x060044A7 RID: 17575 RVA: 0x0016F318 File Offset: 0x0016D518
	public override bool CanContinueExecuting()
	{
		if (this.state != CustomMapsAttackBehaviour.State.Idle && this.controller.IsAnimationPlaying(this.attackAnimName))
		{
			return true;
		}
		if (this.controller.IsNull() || this.controller.TargetPlayer.IsNull())
		{
			return false;
		}
		if (!this.controller.IsTargetable(this.controller.TargetPlayer))
		{
			this.controller.ClearTarget();
			return false;
		}
		return this.CanExecute();
	}

	// Token: 0x060044A8 RID: 17576 RVA: 0x0016F38E File Offset: 0x0016D58E
	public override void Execute()
	{
		if (this.controller.IsNull())
		{
			return;
		}
		if (this.stopMovingToAttack)
		{
			this.controller.StopMoving();
		}
		this.FaceTarget();
		this.controller.agent.RequestBehaviorChange(2);
	}

	// Token: 0x060044A9 RID: 17577 RVA: 0x0016F3C8 File Offset: 0x0016D5C8
	public override void NetExecute()
	{
		if (this.controller.IsNull())
		{
			return;
		}
		if (this.state == CustomMapsAttackBehaviour.State.Attacking && !this.useColliders && this.startTime > this.lastAttackTime && Time.time > this.startTime + this.damageDelayAfterPlayingAnimation)
		{
			this.TriggerAttack(null);
		}
		if (this.controller.IsAnimationPlaying(this.attackAnimName))
		{
			return;
		}
		CustomMapsAttackBehaviour.State state = this.state;
		if (state != CustomMapsAttackBehaviour.State.Idle)
		{
			if (state != CustomMapsAttackBehaviour.State.Attacking)
			{
				return;
			}
			if (Time.time < this.startTime + this.timeBetweenAttacks)
			{
				this.state = CustomMapsAttackBehaviour.State.Idle;
				return;
			}
			this.startTime = Time.time;
			this.controller.PlayAnimation(this.attackAnimName, this.animBlendTime);
			return;
		}
		else
		{
			if (Time.time < this.startTime + this.timeBetweenAttacks)
			{
				return;
			}
			this.startTime = Time.time;
			this.state = CustomMapsAttackBehaviour.State.Attacking;
			this.controller.PlayAnimation(this.attackAnimName, this.animBlendTime);
			return;
		}
	}

	// Token: 0x060044AA RID: 17578 RVA: 0x0016F4BD File Offset: 0x0016D6BD
	public override void ResetBehavior()
	{
		this.state = CustomMapsAttackBehaviour.State.Idle;
	}

	// Token: 0x060044AB RID: 17579 RVA: 0x0016F4C8 File Offset: 0x0016D6C8
	private void FaceTarget()
	{
		if (this.controller.TargetPlayer.IsNull())
		{
			return;
		}
		GameAgent.UpdateFacingTarget(this.controller.transform, this.controller.agent.navAgent, this.controller.TargetPlayer.transform, this.turnSpeed);
	}

	// Token: 0x060044AC RID: 17580 RVA: 0x0016F520 File Offset: 0x0016D720
	public override void OnTriggerEnter(Collider otherCollider)
	{
		if (!this.useColliders)
		{
			return;
		}
		if (Time.time < this.lastAttackTime + this.timeBetweenAttacks || this.state != CustomMapsAttackBehaviour.State.Attacking)
		{
			return;
		}
		GRPlayer componentInParent = otherCollider.GetComponentInParent<GRPlayer>();
		if (componentInParent.IsNull())
		{
			return;
		}
		if (componentInParent.MyRig.IsNotNull() && !componentInParent.MyRig.isLocal)
		{
			return;
		}
		if (componentInParent.State == GRPlayer.GRPlayerState.Ghost)
		{
			return;
		}
		this.TriggerAttack(componentInParent);
	}

	// Token: 0x060044AD RID: 17581 RVA: 0x0016F590 File Offset: 0x0016D790
	private void TriggerAttack(GRPlayer targetPlayer = null)
	{
		this.lastAttackTime = Time.time;
		GRPlayer grplayer = ((targetPlayer != null) ? targetPlayer : (this.controller.entity.IsAuthority() ? this.controller.TargetPlayer : null));
		if (!this.controller.entity.IsAuthority() && grplayer == null)
		{
			Vector3 vector = this.controller.transform.position + this.controller.transform.TransformVector(this.sightOffset);
			grplayer = this.controller.FindBestTarget(vector, this.attackDist, this.attackDistSq, this.sightMinDot);
		}
		if (grplayer == null)
		{
			return;
		}
		if (!grplayer.MyRig.isLocal)
		{
			return;
		}
		if (this.controller.entity.IsAuthority() && !this.IsTargetInAttackRange(grplayer))
		{
			return;
		}
		switch (this.attackType)
		{
		case AttackType.Tag:
			if (GameMode.ActiveGameMode.GameType() != GameModeType.Custom)
			{
				GameMode.ReportHit();
				return;
			}
			CustomGameMode.TaggedByAI(this.controller.entity, grplayer.MyRig.OwningNetPlayer.ActorNumber);
			return;
		case AttackType.UseGT:
			CustomMapsGameManager.instance.OnPlayerHit(this.controller.entity.id, grplayer, this.controller.transform.position);
			return;
		case AttackType.UseLuau:
			CustomGameMode.OnPlayerHit(this.controller.entity, grplayer.MyRig.OwningNetPlayer.ActorNumber, this.damageAmount);
			return;
		default:
			return;
		}
	}

	// Token: 0x040056B0 RID: 22192
	private CustomMapsAIBehaviourController controller;

	// Token: 0x040056B1 RID: 22193
	private CustomMapsAttackBehaviour.State state;

	// Token: 0x040056B2 RID: 22194
	private AttackType attackType;

	// Token: 0x040056B3 RID: 22195
	private float attackDist;

	// Token: 0x040056B4 RID: 22196
	private float attackDistSq;

	// Token: 0x040056B5 RID: 22197
	private bool stopMovingToAttack;

	// Token: 0x040056B6 RID: 22198
	private bool useColliders;

	// Token: 0x040056B7 RID: 22199
	private float damageAmount;

	// Token: 0x040056B8 RID: 22200
	private Vector3 sightOffset;

	// Token: 0x040056B9 RID: 22201
	private float sightFOV;

	// Token: 0x040056BA RID: 22202
	private float sightMinDot;

	// Token: 0x040056BB RID: 22203
	private string attackAnimName;

	// Token: 0x040056BC RID: 22204
	private float timeBetweenAttacks;

	// Token: 0x040056BD RID: 22205
	private float damageDelayAfterPlayingAnimation;

	// Token: 0x040056BE RID: 22206
	private float animBlendTime;

	// Token: 0x040056BF RID: 22207
	private float startTime;

	// Token: 0x040056C0 RID: 22208
	private float turnSpeed;

	// Token: 0x040056C1 RID: 22209
	private float lastAttackTime;

	// Token: 0x02000A6E RID: 2670
	private enum State
	{
		// Token: 0x040056C3 RID: 22211
		Idle,
		// Token: 0x040056C4 RID: 22212
		Attacking
	}
}
