using System;
using UnityEngine;

// Token: 0x0200049B RID: 1179
public class PlayerPrefFlagButton : GorillaPressableButton
{
	// Token: 0x06001C9C RID: 7324 RVA: 0x0009B102 File Offset: 0x00099302
	protected override void OnEnable()
	{
		base.OnEnable();
		this.isOn = PlayerPrefFlags.Check(this.flag);
		this.UpdateColor();
	}

	// Token: 0x06001C9D RID: 7325 RVA: 0x0009B124 File Offset: 0x00099324
	public override void ButtonActivation()
	{
		PlayerPrefFlagButton.ButtonMode buttonMode = this.mode;
		if (buttonMode == PlayerPrefFlagButton.ButtonMode.SET_VALUE)
		{
			PlayerPrefFlags.Set(this.flag, this.value);
			this.isOn = this.value;
			this.UpdateColor();
			return;
		}
		if (buttonMode != PlayerPrefFlagButton.ButtonMode.TOGGLE)
		{
			return;
		}
		this.isOn = PlayerPrefFlags.Flip(this.flag);
		this.UpdateColor();
	}

	// Token: 0x040026B6 RID: 9910
	[SerializeField]
	private PlayerPrefFlags.Flag flag;

	// Token: 0x040026B7 RID: 9911
	[SerializeField]
	private PlayerPrefFlagButton.ButtonMode mode;

	// Token: 0x040026B8 RID: 9912
	[SerializeField]
	private bool value;

	// Token: 0x0200049C RID: 1180
	private enum ButtonMode
	{
		// Token: 0x040026BA RID: 9914
		SET_VALUE,
		// Token: 0x040026BB RID: 9915
		TOGGLE
	}
}
