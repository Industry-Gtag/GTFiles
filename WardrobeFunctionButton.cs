using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000588 RID: 1416
public class WardrobeFunctionButton : GorillaPressableButton
{
	// Token: 0x060023F9 RID: 9209 RVA: 0x000C1F3E File Offset: 0x000C013E
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressWardrobeFunctionButton(this.function);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x060023FA RID: 9210 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void UpdateColor()
	{
	}

	// Token: 0x060023FB RID: 9211 RVA: 0x000C1F65 File Offset: 0x000C0165
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x04002F41 RID: 12097
	public string function;

	// Token: 0x04002F42 RID: 12098
	public float buttonFadeTime = 0.25f;
}
