using System;
using System.Collections;
using GorillaNetworking.Store;
using UnityEngine;

// Token: 0x0200057A RID: 1402
public class TryOnPurchaseButton : GorillaPressableButton
{
	// Token: 0x060023A4 RID: 9124 RVA: 0x000BFDB4 File Offset: 0x000BDFB4
	public void Update()
	{
		if (NetworkSystem.Instance != null && NetworkSystem.Instance.WrongVersion && !this.bError)
		{
			base.enabled = false;
			base.GetComponent<BoxCollider>().enabled = false;
			this.buttonRenderer.material = this.pressedMaterial;
			this.myText.text = "UNAVAILABLE";
		}
	}

	// Token: 0x060023A5 RID: 9125 RVA: 0x000BFE16 File Offset: 0x000BE016
	public override void ButtonActivation()
	{
		if (this.bError)
		{
			return;
		}
		base.ButtonActivation();
		BundleManager.instance.PressPurchaseTryOnBundleButton();
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x060023A6 RID: 9126 RVA: 0x000BFE40 File Offset: 0x000BE040
	public void AlreadyOwn()
	{
		if (this.bError)
		{
			return;
		}
		base.enabled = false;
		base.GetComponent<BoxCollider>().enabled = false;
		this.buttonRenderer.material = this.pressedMaterial;
		this.myText.text = this.AlreadyOwnText;
	}

	// Token: 0x060023A7 RID: 9127 RVA: 0x000BFE80 File Offset: 0x000BE080
	public void ResetButton()
	{
		if (this.bError)
		{
			return;
		}
		base.enabled = true;
		base.GetComponent<BoxCollider>().enabled = true;
		this.buttonRenderer.material = this.unpressedMaterial;
		this.SetOffText(true, false, false);
	}

	// Token: 0x060023A8 RID: 9128 RVA: 0x000BFEB8 File Offset: 0x000BE0B8
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}

	// Token: 0x060023A9 RID: 9129 RVA: 0x000BFEC7 File Offset: 0x000BE0C7
	public void ErrorHappened()
	{
		this.bError = true;
		this.myText.text = this.ErrorText;
		this.buttonRenderer.material = this.unpressedMaterial;
		base.enabled = false;
		this.isOn = false;
	}

	// Token: 0x04002EDD RID: 11997
	public bool bError;

	// Token: 0x04002EDE RID: 11998
	public string ErrorText = "ERROR COMPLETING PURCHASE! PLEASE RESTART THE GAME";

	// Token: 0x04002EDF RID: 11999
	public string AlreadyOwnText;
}
