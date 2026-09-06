using System;
using UnityEngine;

// Token: 0x02000752 RID: 1874
[Serializable]
public class GRAbilityAttackSimpleWander : GRAbilityBase
{
	// Token: 0x06002F9E RID: 12190 RVA: 0x0010383B File Offset: 0x00101A3B
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.wander.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.attack.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002F9F RID: 12191 RVA: 0x00103874 File Offset: 0x00101A74
	protected override void OnStart()
	{
		this.wander.Start();
		this.attack.Start();
	}

	// Token: 0x06002FA0 RID: 12192 RVA: 0x0010388C File Offset: 0x00101A8C
	protected override void OnStop()
	{
		this.wander.Stop();
		this.attack.Stop();
	}

	// Token: 0x06002FA1 RID: 12193 RVA: 0x001038A4 File Offset: 0x00101AA4
	protected override void OnThink(float dt)
	{
		this.wander.Think(dt);
		this.attack.Think(dt);
	}

	// Token: 0x06002FA2 RID: 12194 RVA: 0x001038BE File Offset: 0x00101ABE
	protected override void OnUpdateAuthority(float dt)
	{
		this.wander.UpdateAuthority(dt);
		this.attack.UpdateAuthority(dt);
	}

	// Token: 0x06002FA3 RID: 12195 RVA: 0x001038D8 File Offset: 0x00101AD8
	protected override void OnUpdateRemote(float dt)
	{
		this.wander.UpdateRemote(dt);
		this.attack.UpdateRemote(dt);
	}

	// Token: 0x06002FA4 RID: 12196 RVA: 0x001038F2 File Offset: 0x00101AF2
	public override bool IsDone()
	{
		return this.attack.IsDone();
	}

	// Token: 0x06002FA5 RID: 12197 RVA: 0x001038FF File Offset: 0x00101AFF
	public override bool IsCoolDownOver()
	{
		return this.attack.IsCoolDownOver();
	}

	// Token: 0x06002FA6 RID: 12198 RVA: 0x0010390C File Offset: 0x00101B0C
	public override float GetRange()
	{
		return this.attack.GetRange();
	}

	// Token: 0x04003CF7 RID: 15607
	public GRAbilityWander wander;

	// Token: 0x04003CF8 RID: 15608
	public GRAbilityAttackSimple attack;
}
