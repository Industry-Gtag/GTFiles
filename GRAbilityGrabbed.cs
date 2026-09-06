using System;
using UnityEngine;

// Token: 0x02000740 RID: 1856
[Serializable]
public class GRAbilityGrabbed : GRAbilityBase
{
	// Token: 0x06002F30 RID: 12080 RVA: 0x001015C2 File Offset: 0x000FF7C2
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.idleAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002F31 RID: 12081 RVA: 0x001015E7 File Offset: 0x000FF7E7
	protected override void OnStart()
	{
		this.agent.SetIsPathing(false, true);
		this.idleAbility.Start();
	}

	// Token: 0x06002F32 RID: 12082 RVA: 0x00101601 File Offset: 0x000FF801
	protected override void OnStop()
	{
		this.idleAbility.Stop();
		this.agent.SetIsPathing(true, true);
	}

	// Token: 0x06002F33 RID: 12083 RVA: 0x0010161B File Offset: 0x000FF81B
	public override bool IsDone()
	{
		return this.idleAbility.IsDone();
	}

	// Token: 0x06002F34 RID: 12084 RVA: 0x00101628 File Offset: 0x000FF828
	protected override void OnUpdateAuthority(float dt)
	{
		this.idleAbility.UpdateAuthority(dt);
	}

	// Token: 0x06002F35 RID: 12085 RVA: 0x00101636 File Offset: 0x000FF836
	protected override void OnUpdateRemote(float dt)
	{
		this.idleAbility.UpdateRemote(dt);
	}

	// Token: 0x04003C59 RID: 15449
	public GRAbilityIdle idleAbility;
}
