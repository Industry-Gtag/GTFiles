using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000748 RID: 1864
[Serializable]
public class GRAbilityAttackSimple : GRAbilityBase
{
	// Token: 0x06002F68 RID: 12136 RVA: 0x001024A5 File Offset: 0x001006A5
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.EnableList(this.damageTrigger, false);
	}

	// Token: 0x06002F69 RID: 12137 RVA: 0x001024C4 File Offset: 0x001006C4
	protected override void OnStart()
	{
		if ((double)(this.tellDuration * this.timeMult) > 0.0)
		{
			this.PlayState(GRAbilityAttackSimple.State.Tell, this.tellAnimData, this.soundTell, false);
		}
		else
		{
			this.PlayState(GRAbilityAttackSimple.State.Attack, this.attackAnimData, this.soundAttack, true);
		}
		if (!this.allowMovement)
		{
			this.agent.SetIsPathing(false, true);
			this.agent.SetDisableNetworkSync(true);
		}
		this.events.Reset();
		this.events.OnAbilityStart(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
	}

	// Token: 0x06002F6A RID: 12138 RVA: 0x0010255C File Offset: 0x0010075C
	protected override void OnStop()
	{
		if (!this.allowMovement)
		{
			this.agent.SetIsPathing(true, true);
			this.agent.SetDisableNetworkSync(false);
		}
		this.EnableList(this.damageTrigger, false);
		this.events.OnAbilityStop(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
	}

	// Token: 0x06002F6B RID: 12139 RVA: 0x001025B4 File Offset: 0x001007B4
	private void PlayState(GRAbilityAttackSimple.State newState, AnimationData animData, AbilitySound sound, bool damageEnabled)
	{
		if (!string.IsNullOrEmpty(animData.animName))
		{
			this.PlayAnim(animData.animName, 0.1f, animData.speed);
			this.animNameString = animData.animName;
			this.timeMult = ((this.adjustByAnimationSpeed && !Mathf.Approximately(animData.speed, 0f)) ? (1f / animData.speed) : 1f);
		}
		sound.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		sound.Play(null);
		this.EnableList(this.damageTrigger, damageEnabled);
		this.state = newState;
	}

	// Token: 0x06002F6C RID: 12140 RVA: 0x00102647 File Offset: 0x00100847
	public override bool IsDone()
	{
		return this.state == GRAbilityAttackSimple.State.Done;
	}

	// Token: 0x06002F6D RID: 12141 RVA: 0x00102654 File Offset: 0x00100854
	protected override void OnUpdateShared(float dt)
	{
		float num = (float)(Time.timeAsDouble - this.startTime);
		switch (this.state)
		{
		case GRAbilityAttackSimple.State.Tell:
			if (num > this.tellDuration * this.timeMult)
			{
				this.PlayState(GRAbilityAttackSimple.State.Attack, this.attackAnimData, this.soundAttack, true);
			}
			break;
		case GRAbilityAttackSimple.State.Attack:
			if (num > (this.tellDuration + this.attackDuration) * this.timeMult)
			{
				this.PlayState(GRAbilityAttackSimple.State.FollowThrough, this.outroAnimData, this.soundOutro, false);
			}
			break;
		case GRAbilityAttackSimple.State.FollowThrough:
			if (num >= this.duration * this.timeMult)
			{
				this.state = GRAbilityAttackSimple.State.Done;
			}
			break;
		}
		this.events.TryPlay(num / this.timeMult, this.audioSource);
	}

	// Token: 0x06002F6E RID: 12142 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
	}

	// Token: 0x06002F6F RID: 12143 RVA: 0x0010270D File Offset: 0x0010090D
	public string GetAnimName()
	{
		return this.animNameString;
	}

	// Token: 0x06002F70 RID: 12144 RVA: 0x00102718 File Offset: 0x00100918
	public void EnableList(List<GameObject> objs, bool enable)
	{
		for (int i = 0; i < objs.Count; i++)
		{
			if (objs[i] != null)
			{
				objs[i].SetActive(enable);
			}
		}
	}

	// Token: 0x06002F71 RID: 12145 RVA: 0x00102752 File Offset: 0x00100952
	public override bool IsCoolDownOver()
	{
		return base.IsCoolDownOver(this.coolDown);
	}

	// Token: 0x06002F72 RID: 12146 RVA: 0x00102760 File Offset: 0x00100960
	public override float GetRange()
	{
		return this.range;
	}

	// Token: 0x04003C8A RID: 15498
	public float duration;

	// Token: 0x04003C8B RID: 15499
	public float tellDuration;

	// Token: 0x04003C8C RID: 15500
	public float attackDuration;

	// Token: 0x04003C8D RID: 15501
	public float coolDown;

	// Token: 0x04003C8E RID: 15502
	public float range;

	// Token: 0x04003C8F RID: 15503
	public bool allowMovement;

	// Token: 0x04003C90 RID: 15504
	public AnimationData tellAnimData;

	// Token: 0x04003C91 RID: 15505
	public AnimationData attackAnimData;

	// Token: 0x04003C92 RID: 15506
	public AnimationData outroAnimData;

	// Token: 0x04003C93 RID: 15507
	public AbilitySound soundTell;

	// Token: 0x04003C94 RID: 15508
	public AbilitySound soundAttack;

	// Token: 0x04003C95 RID: 15509
	public AbilitySound soundOutro;

	// Token: 0x04003C96 RID: 15510
	private float timeMult = 1f;

	// Token: 0x04003C97 RID: 15511
	private GRAbilityAttackSimple.State state;

	// Token: 0x04003C98 RID: 15512
	public float maxTurnSpeed;

	// Token: 0x04003C99 RID: 15513
	public List<GameObject> damageTrigger;

	// Token: 0x04003C9A RID: 15514
	private string animNameString;

	// Token: 0x04003C9B RID: 15515
	public GameAbilityEvents events;

	// Token: 0x04003C9C RID: 15516
	public bool adjustByAnimationSpeed;

	// Token: 0x02000749 RID: 1865
	private enum State
	{
		// Token: 0x04003C9E RID: 15518
		Tell,
		// Token: 0x04003C9F RID: 15519
		Attack,
		// Token: 0x04003CA0 RID: 15520
		FollowThrough,
		// Token: 0x04003CA1 RID: 15521
		Done
	}
}
