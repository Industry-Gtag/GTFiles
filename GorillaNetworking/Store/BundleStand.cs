using System;
using Cosmetics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GorillaNetworking.Store
{
	// Token: 0x0200113F RID: 4415
	public class BundleStand : MonoBehaviour, IBuildValidation
	{
		// Token: 0x17000A90 RID: 2704
		// (get) Token: 0x06006EC3 RID: 28355 RVA: 0x0023ADBB File Offset: 0x00238FBB
		public string playfabBundleID
		{
			get
			{
				return this._bundleDataReference.playfabBundleID;
			}
		}

		// Token: 0x06006EC4 RID: 28356 RVA: 0x0023ADC8 File Offset: 0x00238FC8
		bool IBuildValidation.BuildValidationCheck()
		{
			ICreatorCodeProvider creatorCodeProvider;
			if (this.creatorCodeProvider == null || !this.creatorCodeProvider.TryGetComponent<ICreatorCodeProvider>(out creatorCodeProvider))
			{
				Debug.LogError(base.name + " has no Creator Code Provider. This will break bundle purchasing.");
				return false;
			}
			return true;
		}

		// Token: 0x06006EC5 RID: 28357 RVA: 0x0023AE0C File Offset: 0x0023900C
		public void Awake()
		{
			this._bundlePurchaseButton.playfabID = this.playfabBundleID;
			if (this._bundleIcon != null && this._bundleDataReference != null && this._bundleDataReference.bundleImage != null)
			{
				this._bundleIcon.sprite = this._bundleDataReference.bundleImage;
			}
			this._bundlePurchaseButton.codeProvider = this.creatorCodeProvider.GetComponent<ICreatorCodeProvider>();
		}

		// Token: 0x06006EC6 RID: 28358 RVA: 0x0023AE85 File Offset: 0x00239085
		public void InitializeEventListeners()
		{
			this.AlreadyOwnEvent.AddListener(new UnityAction(this._bundlePurchaseButton.AlreadyOwn));
			this.ErrorHappenedEvent.AddListener(new UnityAction(this._bundlePurchaseButton.ErrorHappened));
		}

		// Token: 0x06006EC7 RID: 28359 RVA: 0x0023AEBF File Offset: 0x002390BF
		public void NotifyAlreadyOwn()
		{
			this.AlreadyOwnEvent.Invoke();
		}

		// Token: 0x06006EC8 RID: 28360 RVA: 0x0023AECC File Offset: 0x002390CC
		public void ErrorHappened()
		{
			this.ErrorHappenedEvent.Invoke();
		}

		// Token: 0x06006EC9 RID: 28361 RVA: 0x0023AED9 File Offset: 0x002390D9
		public void UpdatePurchaseButtonText(string purchaseText)
		{
			if (this._bundlePurchaseButton != null)
			{
				this._bundlePurchaseButton.UpdatePurchaseButtonText(purchaseText);
			}
		}

		// Token: 0x06006ECA RID: 28362 RVA: 0x0023AEF5 File Offset: 0x002390F5
		public void UpdateDescriptionText(string descriptionText)
		{
			if (this._bundleDescriptionText != null)
			{
				this._bundleDescriptionText.text = descriptionText;
			}
		}

		// Token: 0x04007EDA RID: 32474
		public BundlePurchaseButton _bundlePurchaseButton;

		// Token: 0x04007EDB RID: 32475
		[SerializeField]
		public StoreBundleData _bundleDataReference;

		// Token: 0x04007EDC RID: 32476
		[SerializeField]
		private GameObject creatorCodeProvider;

		// Token: 0x04007EDD RID: 32477
		public GameObject[] EditorOnlyObjects;

		// Token: 0x04007EDE RID: 32478
		public Text _bundleDescriptionText;

		// Token: 0x04007EDF RID: 32479
		public Image _bundleIcon;

		// Token: 0x04007EE0 RID: 32480
		public UnityEvent AlreadyOwnEvent;

		// Token: 0x04007EE1 RID: 32481
		public UnityEvent ErrorHappenedEvent;
	}
}
