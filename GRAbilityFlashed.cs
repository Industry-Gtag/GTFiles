using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000744 RID: 1860
[Serializable]
public class GRAbilityFlashed : GRAbilityBase
{
	// Token: 0x06002F58 RID: 12120 RVA: 0x00101FF0 File Offset: 0x001001F0
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002F59 RID: 12121 RVA: 0x00102001 File Offset: 0x00100201
	public void SetStunTime(float time)
	{
		this.stunTime = time;
	}

	// Token: 0x06002F5A RID: 12122 RVA: 0x0010200C File Offset: 0x0010020C
	protected override void OnStart()
	{
		if (this.flashAnimations.Count > 0)
		{
			this.flashAnimationIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.flashAnimations.Count, this.flashAnimationIndex);
			this.PlayAnim(this.flashAnimations[this.flashAnimationIndex].animName, 0.1f, this.flashAnimations[this.flashAnimationIndex].speed);
			this.behaviorEndTime = Time.timeAsDouble + (double)this.flashAnimations[this.flashAnimationIndex].duration + (double)this.stunTime;
		}
		else
		{
			this.PlayAnim("GREnemyFlashReaction01", 0.1f, 1f);
			this.behaviorEndTime = Time.timeAsDouble + 0.5 + (double)this.stunTime;
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
	}

	// Token: 0x06002F5B RID: 12123 RVA: 0x00100CBD File Offset: 0x000FEEBD
	protected override void OnStop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
	}

	// Token: 0x06002F5C RID: 12124 RVA: 0x001020F6 File Offset: 0x001002F6
	public override bool IsDone()
	{
		return Time.timeAsDouble >= this.behaviorEndTime;
	}

	// Token: 0x04003C71 RID: 15473
	public List<AnimationData> flashAnimations;

	// Token: 0x04003C72 RID: 15474
	private int flashAnimationIndex;

	// Token: 0x04003C73 RID: 15475
	private double behaviorEndTime;

	// Token: 0x04003C74 RID: 15476
	private float stunTime;
}
