using System;
using UnityEngine;

// Token: 0x020005D9 RID: 1497
public class GorillaCameraSceneTrigger : MonoBehaviour
{
	// Token: 0x0600259C RID: 9628 RVA: 0x000C8508 File Offset: 0x000C6708
	public void ChangeScene(GorillaCameraTriggerIndex triggerLeft)
	{
		if (triggerLeft == this.currentSceneTrigger || this.currentSceneTrigger == null)
		{
			if (this.mostRecentSceneTrigger != this.currentSceneTrigger)
			{
				this.sceneCamera.SetSceneCamera(this.mostRecentSceneTrigger.sceneTriggerIndex);
				this.currentSceneTrigger = this.mostRecentSceneTrigger;
				return;
			}
			this.currentSceneTrigger = null;
		}
	}

	// Token: 0x0400310E RID: 12558
	public GorillaSceneCamera sceneCamera;

	// Token: 0x0400310F RID: 12559
	public GorillaCameraTriggerIndex currentSceneTrigger;

	// Token: 0x04003110 RID: 12560
	public GorillaCameraTriggerIndex mostRecentSceneTrigger;
}
