using System;
using GorillaNetworking;

// Token: 0x02000A49 RID: 2633
public class UnlockCompButton : GorillaPressableButton
{
	// Token: 0x06004398 RID: 17304 RVA: 0x00167DB3 File Offset: 0x00165FB3
	public override void Start()
	{
		this.initialized = false;
	}

	// Token: 0x06004399 RID: 17305 RVA: 0x00167DBC File Offset: 0x00165FBC
	public void Update()
	{
		if (this.testPress)
		{
			this.testPress = false;
			this.ButtonActivation();
		}
		if (!this.initialized && GorillaComputer.instance != null)
		{
			this.isOn = GorillaComputer.instance.allowedInCompetitive;
			this.UpdateColor();
			this.initialized = true;
		}
	}

	// Token: 0x0600439A RID: 17306 RVA: 0x00167E14 File Offset: 0x00166014
	public override void ButtonActivation()
	{
		if (!this.isOn)
		{
			base.ButtonActivation();
			GorillaComputer.instance.CompQueueUnlockButtonPress();
			this.isOn = true;
			this.UpdateColor();
		}
	}

	// Token: 0x04005594 RID: 21908
	public string gameMode;

	// Token: 0x04005595 RID: 21909
	private bool initialized;
}
