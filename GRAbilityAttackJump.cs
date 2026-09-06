using System;
using UnityEngine;

// Token: 0x0200074B RID: 1867
[Serializable]
public class GRAbilityAttackJump : GRAbilityBase
{
	// Token: 0x06002F7D RID: 12157 RVA: 0x00102999 File Offset: 0x00100B99
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002F7E RID: 12158 RVA: 0x001029CC File Offset: 0x00100BCC
	protected override void OnStart()
	{
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.startTime = Time.timeAsDouble;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		this.state = GRAbilityAttackJump.State.Tell;
	}

	// Token: 0x06002F7F RID: 12159 RVA: 0x00102A35 File Offset: 0x00100C35
	protected override void OnStop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002F80 RID: 12160 RVA: 0x00102A6A File Offset: 0x00100C6A
	public override bool IsDone()
	{
		return Time.timeAsDouble - this.startTime >= (double)this.duration;
	}

	// Token: 0x06002F81 RID: 12161 RVA: 0x00102A84 File Offset: 0x00100C84
	protected override void OnUpdateShared(float dt)
	{
		double num = (double)((float)Time.timeAsDouble) - this.startTime;
		switch (this.state)
		{
		case GRAbilityAttackJump.State.Tell:
			if (num > (double)this.jumpTime)
			{
				this.targetPos = this.agent.transform.position + this.agent.transform.forward * 0.5f;
				if (this.target != null)
				{
					Vector3 vector = this.target.transform.position - this.agent.transform.position;
					this.targetPos = this.agent.transform.position + vector * this.jumpLengthScale;
					this.targetPos.y = this.target.transform.position.y;
				}
				float num2 = this.attackLandTime - this.jumpTime;
				num2 = Mathf.Max(0.1f, num2);
				this.initialPos = this.agent.transform.position;
				Vector3 vector2 = this.targetPos - this.initialPos;
				float y = vector2.y;
				vector2.y = 0f;
				float num3 = num2;
				float num4 = 0f;
				if (num3 > 0f)
				{
					Vector3 gravity = Physics.gravity;
					num4 = (y - 0.5f * gravity.y * num3 * num3) / num3;
				}
				this.initialVel = vector2 / num2;
				this.initialVel.y = num4;
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(true);
				}
				this.PlayAnim(this.jumpAnimName, 0.1f, this.animSpeed);
				this.jumpSound.Play(null);
				this.state = GRAbilityAttackJump.State.Jump;
			}
			break;
		case GRAbilityAttackJump.State.Jump:
		{
			float num5 = (float)(num - (double)this.jumpTime);
			Vector3 vector3 = this.initialPos + this.initialVel * num5 + 0.5f * Physics.gravity * num5 * num5;
			this.root.position = vector3;
			if (num > (double)this.attackLandTime)
			{
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(false);
				}
				if (this.doReturnPhase)
				{
					float num6 = this.attackReturnTime - this.attackLandTime;
					num6 = Mathf.Max(0.1f, num6);
					Vector3 vector4 = this.initialPos;
					this.initialPos = this.agent.transform.position;
					this.initialVel = (vector4 - this.initialPos) / num6;
					this.state = GRAbilityAttackJump.State.Return;
				}
				else
				{
					this.state = GRAbilityAttackJump.State.Done;
				}
			}
			break;
		}
		case GRAbilityAttackJump.State.Return:
		{
			float num7 = (float)(num - (double)this.attackLandTime);
			Vector3 vector5 = this.initialPos + this.initialVel * num7;
			this.root.position = vector5;
			if (num > (double)this.attackReturnTime)
			{
				this.state = GRAbilityAttackJump.State.Done;
			}
			break;
		}
		}
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
	}

	// Token: 0x06002F82 RID: 12162 RVA: 0x00102DBC File Offset: 0x00100FBC
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

	// Token: 0x04003CAB RID: 15531
	public float duration;

	// Token: 0x04003CAC RID: 15532
	public float jumpTime;

	// Token: 0x04003CAD RID: 15533
	public float attackLandTime;

	// Token: 0x04003CAE RID: 15534
	public float attackReturnTime;

	// Token: 0x04003CAF RID: 15535
	public bool doReturnPhase = true;

	// Token: 0x04003CB0 RID: 15536
	public float jumpLengthScale = 1f;

	// Token: 0x04003CB1 RID: 15537
	public string animName;

	// Token: 0x04003CB2 RID: 15538
	public float animSpeed;

	// Token: 0x04003CB3 RID: 15539
	public float maxTurnSpeed;

	// Token: 0x04003CB4 RID: 15540
	public string jumpAnimName;

	// Token: 0x04003CB5 RID: 15541
	public AbilitySound jumpSound;

	// Token: 0x04003CB6 RID: 15542
	public GameObject damageTrigger;

	// Token: 0x04003CB7 RID: 15543
	private Transform target;

	// Token: 0x04003CB8 RID: 15544
	private GRAbilityAttackJump.State state;

	// Token: 0x04003CB9 RID: 15545
	public Vector3 targetPos;

	// Token: 0x04003CBA RID: 15546
	public Vector3 initialPos;

	// Token: 0x04003CBB RID: 15547
	public Vector3 initialVel;

	// Token: 0x0200074C RID: 1868
	private enum State
	{
		// Token: 0x04003CBD RID: 15549
		Tell,
		// Token: 0x04003CBE RID: 15550
		Jump,
		// Token: 0x04003CBF RID: 15551
		Return,
		// Token: 0x04003CC0 RID: 15552
		Done
	}
}
