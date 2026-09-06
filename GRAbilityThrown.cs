using System;
using UnityEngine;

// Token: 0x02000741 RID: 1857
[Serializable]
public class GRAbilityThrown : GRAbilityBase
{
	// Token: 0x06002F37 RID: 12087 RVA: 0x00101644 File Offset: 0x000FF844
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.idleAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002F38 RID: 12088 RVA: 0x00101669 File Offset: 0x000FF869
	protected override void OnStart()
	{
		this.agent.SetIsPathing(false, false);
		this.idleAbility.Start();
	}

	// Token: 0x06002F39 RID: 12089 RVA: 0x00101683 File Offset: 0x000FF883
	protected override void OnStop()
	{
		this.idleAbility.Stop();
		this.agent.SetIsPathing(true, false);
	}

	// Token: 0x06002F3A RID: 12090 RVA: 0x0010169D File Offset: 0x000FF89D
	public override bool IsDone()
	{
		return this.idleAbility.IsDone();
	}

	// Token: 0x06002F3B RID: 12091 RVA: 0x001016AA File Offset: 0x000FF8AA
	protected override void OnUpdateAuthority(float dt)
	{
		this.idleAbility.UpdateAuthority(dt);
	}

	// Token: 0x06002F3C RID: 12092 RVA: 0x001016B8 File Offset: 0x000FF8B8
	protected override void OnUpdateRemote(float dt)
	{
		this.idleAbility.UpdateRemote(dt);
	}

	// Token: 0x04003C5A RID: 15450
	public GRAbilityIdle idleAbility;
}
