using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200074D RID: 1869
[Serializable]
public class GRAbilityAttackLaser : GRAbilityBase
{
	// Token: 0x06002F84 RID: 12164 RVA: 0x00102E16 File Offset: 0x00101016
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002F85 RID: 12165 RVA: 0x00102E48 File Offset: 0x00101048
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
		this.state = GRAbilityAttackLaser.State.Tell;
	}

	// Token: 0x06002F86 RID: 12166 RVA: 0x00102F54 File Offset: 0x00101154
	protected override void OnStop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
		if (this.laserFx != null)
		{
			this.laserFx.DisableLazer();
		}
		if (this.tellLaserFx != null)
		{
			this.tellLaserFx.DisableLazer();
		}
	}

	// Token: 0x06002F87 RID: 12167 RVA: 0x00102FC6 File Offset: 0x001011C6
	public override bool IsDone()
	{
		return this.state == GRAbilityAttackLaser.State.Done;
	}

	// Token: 0x06002F88 RID: 12168 RVA: 0x00102FD4 File Offset: 0x001011D4
	protected override void OnUpdateShared(float dt)
	{
		float num = (float)(Time.timeAsDouble - this.startTime);
		switch (this.state)
		{
		case GRAbilityAttackLaser.State.Tell:
		{
			this.targetPos = this.root.position + this.root.transform.forward;
			if (this.target != null)
			{
				this.targetPos = this.target.position;
			}
			Vector3 position = this.head.position;
			Vector3 vector = this.targetPos - position;
			float num2 = vector.magnitude;
			if (num2 > 0f)
			{
				Vector3 vector2 = vector / num2;
				num2 = Mathf.Min(this.maxLaserRange, num2);
				this.targetPos = position + vector2 * num2;
			}
			if (!this.doNotFaceTarget)
			{
				GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
			}
			if (num > this.tellDuration)
			{
				this.state = GRAbilityAttackLaser.State.Attack;
				if (this.damageCollider != null && this.laserOrigins.Length != 0)
				{
					this.damageCollider.transform.position = (position + this.targetPos) / 2f;
					this.damageCollider.height = num2;
					this.damageCollider.direction = 2;
					if (num2 > 0f)
					{
						this.damageCollider.transform.rotation = Quaternion.LookRotation(vector / num2);
					}
				}
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(true);
				}
				if (this.tellLaserFx != null)
				{
					this.tellLaserFx.DisableLazer();
				}
				if (this.laserFx != null && this.target != null)
				{
					GamePlayer component = this.target.GetComponent<GamePlayer>();
					if (component != null && component.rig != null)
					{
						this.laserFx.EnableLazer(this.laserOrigins, this.targetPos);
					}
				}
				this.initialPos = this.root.position;
				this.initialVel = (this.targetPos - this.initialPos).normalized * this.attackMoveSpeed;
				return;
			}
			if (this.tellLaserFx != null)
			{
				this.tellLaserFx.EnableLazer(this.laserOrigins, this.targetPos);
				return;
			}
			break;
		}
		case GRAbilityAttackLaser.State.Attack:
		{
			float num3 = num - this.tellDuration;
			Vector3 vector3 = this.initialPos + this.initialVel * num3;
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(vector3, out navMeshHit, 0.5f, this.walkableArea))
			{
				vector3 = navMeshHit.position;
				if (NavMesh.Raycast(this.initialPos, vector3, out navMeshHit, this.walkableArea))
				{
					vector3 = navMeshHit.position;
				}
				this.root.position = vector3;
			}
			if (num > this.tellDuration + this.attackDuration)
			{
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(false);
				}
				if (this.laserFx != null)
				{
					this.laserFx.DisableLazer();
				}
				this.state = GRAbilityAttackLaser.State.FollowThrough;
				return;
			}
			break;
		}
		case GRAbilityAttackLaser.State.FollowThrough:
			if (num >= this.duration)
			{
				this.state = GRAbilityAttackLaser.State.Done;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002F89 RID: 12169 RVA: 0x00103320 File Offset: 0x00101520
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

	// Token: 0x06002F8A RID: 12170 RVA: 0x00103360 File Offset: 0x00101560
	public string GetAnimName()
	{
		return this.animNameString;
	}

	// Token: 0x06002F8B RID: 12171 RVA: 0x00103368 File Offset: 0x00101568
	public override bool IsCoolDownOver()
	{
		return base.IsCoolDownOver(this.coolDown);
	}

	// Token: 0x06002F8C RID: 12172 RVA: 0x00103376 File Offset: 0x00101576
	public override float GetRange()
	{
		return this.range;
	}

	// Token: 0x04003CC1 RID: 15553
	public float duration;

	// Token: 0x04003CC2 RID: 15554
	public float tellDuration;

	// Token: 0x04003CC3 RID: 15555
	public float attackDuration;

	// Token: 0x04003CC4 RID: 15556
	public float coolDown;

	// Token: 0x04003CC5 RID: 15557
	public float range;

	// Token: 0x04003CC6 RID: 15558
	public float attackMoveSpeed;

	// Token: 0x04003CC7 RID: 15559
	public bool doNotFaceTarget;

	// Token: 0x04003CC8 RID: 15560
	public List<AnimationData> animData;

	// Token: 0x04003CC9 RID: 15561
	public AbilitySound soundAttack;

	// Token: 0x04003CCA RID: 15562
	public float maxLaserRange;

	// Token: 0x04003CCB RID: 15563
	public Transform[] laserOrigins;

	// Token: 0x04003CCC RID: 15564
	public Monkeye_LazerFX tellLaserFx;

	// Token: 0x04003CCD RID: 15565
	public Monkeye_LazerFX laserFx;

	// Token: 0x04003CCE RID: 15566
	private GRAbilityAttackLaser.State state;

	// Token: 0x04003CCF RID: 15567
	public float maxTurnSpeed;

	// Token: 0x04003CD0 RID: 15568
	public GameObject damageTrigger;

	// Token: 0x04003CD1 RID: 15569
	public CapsuleCollider damageCollider;

	// Token: 0x04003CD2 RID: 15570
	private Transform target;

	// Token: 0x04003CD3 RID: 15571
	private string animNameString;

	// Token: 0x04003CD4 RID: 15572
	private int lastAnimIndex = -1;

	// Token: 0x04003CD5 RID: 15573
	public Vector3 targetPos;

	// Token: 0x04003CD6 RID: 15574
	public Vector3 initialPos;

	// Token: 0x04003CD7 RID: 15575
	public Vector3 initialVel;

	// Token: 0x0200074E RID: 1870
	private enum State
	{
		// Token: 0x04003CD9 RID: 15577
		Tell,
		// Token: 0x04003CDA RID: 15578
		Attack,
		// Token: 0x04003CDB RID: 15579
		FollowThrough,
		// Token: 0x04003CDC RID: 15580
		Done
	}
}
