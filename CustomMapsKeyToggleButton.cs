using System;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;

// Token: 0x02000AB8 RID: 2744
public class CustomMapsKeyToggleButton : CustomMapsKeyButton
{
	// Token: 0x0600464D RID: 17997 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void PressButtonColourUpdate()
	{
	}

	// Token: 0x0600464E RID: 17998 RVA: 0x0017AB2C File Offset: 0x00178D2C
	public void SetButtonStatus(bool newIsPressed)
	{
		if (this.isPressed == newIsPressed)
		{
			return;
		}
		this.isPressed = newIsPressed;
		this.propBlock.SetColor("_BaseColor", this.isPressed ? this.ButtonColorSettings.PressedColor : this.ButtonColorSettings.UnpressedColor);
		this.propBlock.SetColor("_Color", this.isPressed ? this.ButtonColorSettings.PressedColor : this.ButtonColorSettings.UnpressedColor);
		this.ButtonRenderer.SetPropertyBlock(this.propBlock);
	}

	// Token: 0x040058A4 RID: 22692
	private bool isPressed;
}
