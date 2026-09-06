using System;
using GorillaExtensions;
using TMPro;
using UnityEngine;

// Token: 0x02000AC2 RID: 2754
public class CustomMapsScreenButton : CustomMapsScreenTouchPoint
{
	// Token: 0x06004699 RID: 18073 RVA: 0x0017D0BE File Offset: 0x0017B2BE
	protected override void OnDisable()
	{
		base.OnDisable();
		if (this.isToggle)
		{
			this.SetButtonActive(this.isActive);
			return;
		}
		this.isActive = false;
	}

	// Token: 0x0600469A RID: 18074 RVA: 0x0017D0E2 File Offset: 0x0017B2E2
	public void SetButtonText(string text)
	{
		if (this.bttnText.IsNull())
		{
			return;
		}
		this.bttnText.text = text;
	}

	// Token: 0x0600469B RID: 18075 RVA: 0x0017D0FE File Offset: 0x0017B2FE
	public void SetButtonActive(bool active)
	{
		this.isActive = active;
		this.touchPointRenderer.color = (this.isActive ? this.buttonColorSettings.PressedColor : this.buttonColorSettings.UnpressedColor);
	}

	// Token: 0x0600469C RID: 18076 RVA: 0x0017D132 File Offset: 0x0017B332
	public override void PressButtonColourUpdate()
	{
		if (!this.isToggle)
		{
			base.PressButtonColourUpdate();
			return;
		}
	}

	// Token: 0x0600469D RID: 18077 RVA: 0x0017D143 File Offset: 0x0017B343
	protected override void OnButtonPressedEvent()
	{
		this.isActive = !this.isActive;
	}

	// Token: 0x04005926 RID: 22822
	[SerializeField]
	private TMP_Text bttnText;

	// Token: 0x04005927 RID: 22823
	[SerializeField]
	private bool isToggle;

	// Token: 0x04005928 RID: 22824
	private bool isActive;
}
