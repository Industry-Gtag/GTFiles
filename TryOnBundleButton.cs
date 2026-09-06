using System;
using GorillaNetworking.Store;

// Token: 0x02000577 RID: 1399
public class TryOnBundleButton : GorillaPressableButton
{
	// Token: 0x06002383 RID: 9091 RVA: 0x000BF148 File Offset: 0x000BD348
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		BundleManager.instance.PressTryOnBundleButton(this, isLeftHand);
	}

	// Token: 0x06002384 RID: 9092 RVA: 0x000BF160 File Offset: 0x000BD360
	public override void UpdateColor()
	{
		if (this.playfabBundleID == "NULL")
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = "";
			}
			return;
		}
		base.UpdateColor();
	}

	// Token: 0x04002EC0 RID: 11968
	public int buttonIndex;

	// Token: 0x04002EC1 RID: 11969
	public string playfabBundleID = "NULL";
}
