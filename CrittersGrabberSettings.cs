using System;
using UnityEngine;

// Token: 0x02000064 RID: 100
public class CrittersGrabberSettings : CrittersActorSettings
{
	// Token: 0x060001F0 RID: 496 RVA: 0x0000B77A File Offset: 0x0000997A
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersGrabber crittersGrabber = (CrittersGrabber)this.parentActor;
		crittersGrabber.grabPosition = this._grabPosition;
		crittersGrabber.grabDistance = this._grabDistance;
	}

	// Token: 0x04000235 RID: 565
	public Transform _grabPosition;

	// Token: 0x04000236 RID: 566
	public float _grabDistance;
}
