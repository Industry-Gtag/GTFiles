using System;
using System.Collections;
using GorillaNetworking.Store;
using UnityEngine;

// Token: 0x02000563 RID: 1379
public class PurchaseCurrencyButton : GorillaPressableButton
{
	// Token: 0x06002314 RID: 8980 RVA: 0x000BCB02 File Offset: 0x000BAD02
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		ATM_Manager.instance.PressCurrencyPurchaseButton(base.GetComponentInParent<ATM_UI>(), this.purchaseCurrencySize);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06002315 RID: 8981 RVA: 0x000BCB2F File Offset: 0x000BAD2F
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.sharedMaterial = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.sharedMaterial = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x04002E38 RID: 11832
	public string purchaseCurrencySize;

	// Token: 0x04002E39 RID: 11833
	public float buttonFadeTime = 0.25f;
}
