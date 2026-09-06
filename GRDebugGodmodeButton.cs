using System;

// Token: 0x0200076D RID: 1901
public class GRDebugGodmodeButton : GorillaPressableReleaseButton
{
	// Token: 0x0600301F RID: 12319 RVA: 0x00044B04 File Offset: 0x00042D04
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06003020 RID: 12320 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPressedButton()
	{
	}

	// Token: 0x06003021 RID: 12321 RVA: 0x00105A0B File Offset: 0x00103C0B
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.UpdateColor();
	}

	// Token: 0x06003022 RID: 12322 RVA: 0x00105A19 File Offset: 0x00103C19
	public override void ButtonDeactivation()
	{
		base.ButtonDeactivation();
		this.UpdateColor();
	}
}
