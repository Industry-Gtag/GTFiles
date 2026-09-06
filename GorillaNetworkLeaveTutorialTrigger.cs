using System;

// Token: 0x02000CCD RID: 3277
public class GorillaNetworkLeaveTutorialTrigger : GorillaTriggerBox
{
	// Token: 0x0600513B RID: 20795 RVA: 0x001AED78 File Offset: 0x001ACF78
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		NetworkSystem.Instance.SetMyTutorialComplete();
	}
}
