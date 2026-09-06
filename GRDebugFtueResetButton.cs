using System;
using UnityEngine;

// Token: 0x0200076C RID: 1900
public class GRDebugFtueResetButton : GorillaPressableReleaseButton
{
	// Token: 0x0600301A RID: 12314 RVA: 0x001059AD File Offset: 0x00103BAD
	private void Awake()
	{
		if (!this.availableOnLive)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600301B RID: 12315 RVA: 0x001059C3 File Offset: 0x00103BC3
	public void OnPressedButton()
	{
		PlayerPrefs.SetString("spawnInWrongStump", "flagged");
		PlayerPrefs.Save();
	}

	// Token: 0x0600301C RID: 12316 RVA: 0x001059D9 File Offset: 0x00103BD9
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.isOn = true;
		this.UpdateColor();
	}

	// Token: 0x0600301D RID: 12317 RVA: 0x001059EE File Offset: 0x00103BEE
	public override void ButtonDeactivation()
	{
		base.ButtonDeactivation();
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x04003DB2 RID: 15794
	public bool availableOnLive;
}
