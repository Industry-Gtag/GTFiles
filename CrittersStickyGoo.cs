using System;
using UnityEngine;

// Token: 0x0200007A RID: 122
public class CrittersStickyGoo : CrittersActor
{
	// Token: 0x060002F5 RID: 757 RVA: 0x00011A9F File Offset: 0x0000FC9F
	public override void Initialize()
	{
		base.Initialize();
		this.readyToDisable = false;
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x00011AB0 File Offset: 0x0000FCB0
	public bool CanAffect(Vector3 position)
	{
		return (base.transform.position - position).magnitude < this.range;
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x00011ADE File Offset: 0x0000FCDE
	public void EffectApplied(CrittersPawn critter)
	{
		if (this.destroyOnApply)
		{
			this.readyToDisable = true;
		}
		CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StickyTriggered, this.actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x00011B20 File Offset: 0x0000FD20
	public override bool ProcessLocal()
	{
		bool flag = base.ProcessLocal();
		if (this.readyToDisable)
		{
			base.gameObject.SetActive(false);
			return true;
		}
		return flag;
	}

	// Token: 0x04000360 RID: 864
	[Header("Sticky Goo")]
	public float range = 1f;

	// Token: 0x04000361 RID: 865
	public float slowModifier = 0.3f;

	// Token: 0x04000362 RID: 866
	public float slowDuration = 3f;

	// Token: 0x04000363 RID: 867
	public bool destroyOnApply = true;

	// Token: 0x04000364 RID: 868
	private bool readyToDisable;
}
