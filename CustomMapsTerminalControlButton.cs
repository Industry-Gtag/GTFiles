using System;
using TMPro;
using UnityEngine;

// Token: 0x02000A53 RID: 2643
public class CustomMapsTerminalControlButton : CustomMapsScreenTouchPoint
{
	// Token: 0x17000666 RID: 1638
	// (get) Token: 0x060043D2 RID: 17362 RVA: 0x00168FA7 File Offset: 0x001671A7
	// (set) Token: 0x060043D3 RID: 17363 RVA: 0x00168FAF File Offset: 0x001671AF
	public bool IsLocked
	{
		get
		{
			return this.isLocked;
		}
		set
		{
			this.isLocked = value;
		}
	}

	// Token: 0x060043D4 RID: 17364 RVA: 0x00168FB8 File Offset: 0x001671B8
	protected override void OnButtonPressedEvent()
	{
		GTDev.Log<string>("terminal control pressed", null);
		if (this.mapsTerminal == null)
		{
			return;
		}
		this.mapsTerminal.HandleTerminalControlButtonPressed();
	}

	// Token: 0x060043D5 RID: 17365 RVA: 0x00168FDF File Offset: 0x001671DF
	public void LockTerminalControl()
	{
		if (this.IsLocked)
		{
			return;
		}
		this.IsLocked = true;
		this.PressButtonColourUpdate();
	}

	// Token: 0x060043D6 RID: 17366 RVA: 0x00168FF7 File Offset: 0x001671F7
	public void UnlockTerminalControl()
	{
		if (!this.IsLocked)
		{
			return;
		}
		this.IsLocked = false;
		this.PressButtonColourUpdate();
	}

	// Token: 0x060043D7 RID: 17367 RVA: 0x00169010 File Offset: 0x00167210
	public override void PressButtonColourUpdate()
	{
		this.bttnText.fontSize = (this.isLocked ? this.lockedFontSize : this.unlockedFontSize);
		this.bttnText.text = (this.isLocked ? this.lockedText : this.unlockedText);
		this.bttnText.color = (this.isLocked ? this.lockedTextColor : this.unlockedTextColor);
		this.touchPointRenderer.color = (this.isLocked ? this.buttonColorSettings.PressedColor : this.buttonColorSettings.UnpressedColor);
	}

	// Token: 0x040055C8 RID: 21960
	[SerializeField]
	private TMP_Text bttnText;

	// Token: 0x040055C9 RID: 21961
	[SerializeField]
	private string unlockedText = "TERMINAL AVAILABLE";

	// Token: 0x040055CA RID: 21962
	[SerializeField]
	private string lockedText = "TERMINAL UNAVAILABLE";

	// Token: 0x040055CB RID: 21963
	[SerializeField]
	private float unlockedFontSize = 30f;

	// Token: 0x040055CC RID: 21964
	[SerializeField]
	private float lockedFontSize = 30f;

	// Token: 0x040055CD RID: 21965
	[SerializeField]
	private Color unlockedTextColor = Color.black;

	// Token: 0x040055CE RID: 21966
	[SerializeField]
	private Color lockedTextColor = Color.white;

	// Token: 0x040055CF RID: 21967
	private bool isLocked;

	// Token: 0x040055D0 RID: 21968
	[SerializeField]
	private CustomMapsTerminal mapsTerminal;
}
