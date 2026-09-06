using System;
using System.Collections;
using Cosmetics;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x0200113D RID: 4413
	public class BundlePurchaseButton : GorillaPressableButton, IGorillaSliceableSimple
	{
		// Token: 0x06006EB2 RID: 28338 RVA: 0x00019260 File Offset: 0x00017460
		public new void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06006EB3 RID: 28339 RVA: 0x00019269 File Offset: 0x00017469
		public new void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06006EB4 RID: 28340 RVA: 0x0023AB00 File Offset: 0x00238D00
		public void SliceUpdate()
		{
			if (NetworkSystem.Instance != null && NetworkSystem.Instance.WrongVersion && !this.bError)
			{
				base.enabled = false;
				base.GetComponent<BoxCollider>().enabled = false;
				this.buttonRenderer.material = this.pressedMaterial;
				this.myText.text = this.UnavailableText;
			}
		}

		// Token: 0x06006EB5 RID: 28341 RVA: 0x0023AB63 File Offset: 0x00238D63
		public override void ButtonActivation()
		{
			if (this.bError)
			{
				return;
			}
			base.ButtonActivation();
			BundleManager.instance.BundlePurchaseButtonPressed(this.playfabID, this.codeProvider);
			base.StartCoroutine(this.ButtonColorUpdate());
		}

		// Token: 0x06006EB6 RID: 28342 RVA: 0x0023AB9C File Offset: 0x00238D9C
		public void AlreadyOwn()
		{
			if (this.bError)
			{
				return;
			}
			base.enabled = false;
			base.GetComponent<BoxCollider>().enabled = false;
			this.buttonRenderer.material = this.pressedMaterial;
			this.onText = this.AlreadyOwnText;
			this.myText.text = this.AlreadyOwnText;
			this.isOn = true;
		}

		// Token: 0x06006EB7 RID: 28343 RVA: 0x0023ABFA File Offset: 0x00238DFA
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
			this.isOn = false;
		}

		// Token: 0x06006EB8 RID: 28344 RVA: 0x0023AC39 File Offset: 0x00238E39
		private IEnumerator ButtonColorUpdate()
		{
			this.buttonRenderer.material = this.pressedMaterial;
			yield return new WaitForSeconds(this.debounceTime);
			this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
			yield break;
		}

		// Token: 0x06006EB9 RID: 28345 RVA: 0x0023AC48 File Offset: 0x00238E48
		public void ErrorHappened()
		{
			this.bError = true;
			this.myText.text = this.ErrorText;
			this.buttonRenderer.material = this.unpressedMaterial;
			base.enabled = false;
			this.offText = this.ErrorText;
			this.onText = this.ErrorText;
			this.isOn = false;
		}

		// Token: 0x06006EBA RID: 28346 RVA: 0x0023ACA4 File Offset: 0x00238EA4
		public void InitializeData()
		{
			if (this.bError)
			{
				return;
			}
			this.SetOffText(true, false, false);
			this.buttonRenderer.material = this.unpressedMaterial;
			base.enabled = true;
			this.isOn = false;
		}

		// Token: 0x06006EBB RID: 28347 RVA: 0x0023ACD7 File Offset: 0x00238ED7
		public void UpdatePurchaseButtonText(string purchaseText)
		{
			if (!this.bError)
			{
				this.offText = purchaseText;
				this.UpdateColor();
			}
		}

		// Token: 0x04007ECE RID: 32462
		private const string MONKE_BLOCKS_BUNDLE_ALREADY_OWN_KEY = "MONKE_BLOCKS_BUNDLE_ALREADY_OWN";

		// Token: 0x04007ECF RID: 32463
		private const string MONKE_BLOCKS_BUNDLE_UNAVAILABLE_KEY = "MONKE_BLOCKS_BUNDLE_UNAVAILABLE";

		// Token: 0x04007ED0 RID: 32464
		private const string MONKE_BLOCKS_BUNDLE_ERROR_KEY = "MONKE_BLOCKS_BUNDLE_ERROR";

		// Token: 0x04007ED1 RID: 32465
		public bool bError;

		// Token: 0x04007ED2 RID: 32466
		public string ErrorText = "ERROR COMPLETING PURCHASE! PLEASE RESTART THE GAME";

		// Token: 0x04007ED3 RID: 32467
		public string AlreadyOwnText = "YOU OWN THE BUNDLE ALREADY! THANK YOU!";

		// Token: 0x04007ED4 RID: 32468
		public string UnavailableText = "UNAVAILABLE";

		// Token: 0x04007ED5 RID: 32469
		public string playfabID = "";

		// Token: 0x04007ED6 RID: 32470
		public ICreatorCodeProvider codeProvider;
	}
}
