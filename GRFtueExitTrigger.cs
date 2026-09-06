using System;
using UnityEngine;

// Token: 0x020007B7 RID: 1975
public class GRFtueExitTrigger : GorillaTriggerBox
{
	// Token: 0x06003283 RID: 12931 RVA: 0x00114E01 File Offset: 0x00113001
	public override void OnBoxTriggered()
	{
		this.startTime = Time.time;
		this.ftueObject.InterruptWaitingTimer();
		this.ftueObject.playerLight.GetComponentInChildren<Light>().intensity = 0.25f;
	}

	// Token: 0x06003284 RID: 12932 RVA: 0x00114E33 File Offset: 0x00113033
	private void Update()
	{
		if (this.startTime > 0f && Time.time - this.startTime > this.delayTime)
		{
			this.ftueObject.ChangeState(GRFirstTimeUserExperience.TransitionState.Flicker);
			this.startTime = -1f;
		}
	}

	// Token: 0x0400417D RID: 16765
	public GRFirstTimeUserExperience ftueObject;

	// Token: 0x0400417E RID: 16766
	public float delayTime = 5f;

	// Token: 0x0400417F RID: 16767
	private float startTime = -1f;
}
