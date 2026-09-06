using System;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x02000735 RID: 1845
[Serializable]
public class GRAbilityIdle : GRAbilityBase
{
	// Token: 0x06002EDA RID: 11994 RVA: 0x000FFEDD File Offset: 0x000FE0DD
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.animLoops = 0;
		this.cachedDuration = this.duration;
		this.cachedAnimSpeed = this.animSpeed;
	}

	// Token: 0x06002EDB RID: 11995 RVA: 0x000FFF10 File Offset: 0x000FE110
	protected override void OnStart()
	{
		this.agent.SetStopped(true);
		this.PlayAnim(this.animName, 0.3f, this.animSpeed);
		this.animLoops = 0;
		this.events.Reset();
		this.events.OnAbilityStart(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
	}

	// Token: 0x06002EDC RID: 11996 RVA: 0x000FFF6E File Offset: 0x000FE16E
	protected override void OnStop()
	{
		this.events.OnAbilityStop(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
		this.agent.SetStopped(false);
	}

	// Token: 0x06002EDD RID: 11997 RVA: 0x000FFF98 File Offset: 0x000FE198
	protected override void OnUpdateShared(float dt)
	{
		float num = (float)(Time.timeAsDouble - this.startTime);
		if (this.anim != null && this.anim[this.animName] != null)
		{
			if ((int)this.anim[this.animName].normalizedTime > this.animLoops)
			{
				this.events.Reset();
				this.animLoops = (int)this.anim[this.animName].normalizedTime;
			}
			num = this.anim[this.animName].time - this.anim[this.animName].length * (float)this.animLoops;
		}
		this.events.TryPlay(num, this.audioSource);
	}

	// Token: 0x06002EDE RID: 11998 RVA: 0x0010006C File Offset: 0x000FE26C
	public override bool IsDone()
	{
		return (double)this.duration > 0.0 && Time.timeAsDouble >= this.startTime + (double)this.duration;
	}

	// Token: 0x06002EDF RID: 11999 RVA: 0x0010009A File Offset: 0x000FE29A
	public override bool IsCoolDownOver()
	{
		return base.IsCoolDownOver(this.coolDown);
	}

	// Token: 0x06002EE0 RID: 12000 RVA: 0x001000A8 File Offset: 0x000FE2A8
	public override float GetRange()
	{
		return this.range;
	}

	// Token: 0x06002EE1 RID: 12001 RVA: 0x001000B0 File Offset: 0x000FE2B0
	public void SpeedUp(float mult)
	{
		this.duration = this.cachedDuration / mult;
		this.animSpeed = this.cachedAnimSpeed * mult;
	}

	// Token: 0x04003C01 RID: 15361
	public float duration;

	// Token: 0x04003C02 RID: 15362
	public string animName;

	// Token: 0x04003C03 RID: 15363
	public float animSpeed;

	// Token: 0x04003C04 RID: 15364
	public float coolDown;

	// Token: 0x04003C05 RID: 15365
	public float range;

	// Token: 0x04003C06 RID: 15366
	private float cachedDuration;

	// Token: 0x04003C07 RID: 15367
	private float cachedAnimSpeed;

	// Token: 0x04003C08 RID: 15368
	public GameAbilityEvents events;

	// Token: 0x04003C09 RID: 15369
	[ReadOnly]
	public int animLoops;
}
