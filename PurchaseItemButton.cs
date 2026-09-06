using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000565 RID: 1381
public class PurchaseItemButton : GorillaPressableButton
{
	// Token: 0x0600231D RID: 8989 RVA: 0x000BCBD7 File Offset: 0x000BADD7
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressPurchaseItemButton(this, isLeftHand);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x0600231E RID: 8990 RVA: 0x000BCBFA File Offset: 0x000BADFA
	private IEnumerator ButtonColorUpdate()
	{
		Debug.Log("did this happen?");
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}

	// Token: 0x04002E3D RID: 11837
	public string buttonSide;
}
