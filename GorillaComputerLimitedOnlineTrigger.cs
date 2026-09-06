using System;
using GorillaNetworking;

// Token: 0x02000A20 RID: 2592
public class GorillaComputerLimitedOnlineTrigger : GorillaTriggerBox
{
	// Token: 0x06004274 RID: 17012 RVA: 0x0016276B File Offset: 0x0016096B
	public override void OnBoxTriggered()
	{
		GorillaComputer.instance.SetLimitOnlineScreens(true);
	}

	// Token: 0x06004275 RID: 17013 RVA: 0x0016277A File Offset: 0x0016097A
	public override void OnBoxExited()
	{
		GorillaComputer.instance.SetLimitOnlineScreens(false);
	}
}
