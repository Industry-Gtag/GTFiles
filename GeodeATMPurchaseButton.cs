using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000574 RID: 1396
public class GeodeATMPurchaseButton : GorillaPressableButton
{
	// Token: 0x06002378 RID: 9080 RVA: 0x000BF076 File Offset: 0x000BD276
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06002379 RID: 9081 RVA: 0x000BF08B File Offset: 0x000BD28B
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.sharedMaterial = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.sharedMaterial = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x04002EBB RID: 11963
	public float buttonFadeTime = 0.25f;
}
