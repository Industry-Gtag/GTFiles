using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTagScripts;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.UI;

// Token: 0x02000A33 RID: 2611
public class GorillaPressableButton : MonoBehaviour, IClickable
{
	// Token: 0x14000086 RID: 134
	// (add) Token: 0x060042F5 RID: 17141 RVA: 0x00164C64 File Offset: 0x00162E64
	// (remove) Token: 0x060042F6 RID: 17142 RVA: 0x00164C9C File Offset: 0x00162E9C
	public event Action<GorillaPressableButton, bool> onPressed;

	// Token: 0x060042F7 RID: 17143 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void Start()
	{
	}

	// Token: 0x060042F8 RID: 17144 RVA: 0x00164CD4 File Offset: 0x00162ED4
	protected virtual void OnEnable()
	{
		if (this.isSubscriberOnlyButton)
		{
			SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnLocalSubscriptionData, new Action(this.CheckSubscription));
			this.CheckSubscription();
		}
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.RefreshText));
		this.RefreshText();
	}

	// Token: 0x060042F9 RID: 17145 RVA: 0x00164D28 File Offset: 0x00162F28
	public void SetIsSubscriberButton(bool newIsSubscriberToggle)
	{
		if (this.isSubscriberOnlyButton == newIsSubscriberToggle)
		{
			return;
		}
		this.isSubscriberOnlyButton = newIsSubscriberToggle;
		if (this.isSubscriberOnlyButton)
		{
			SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnLocalSubscriptionData, new Action(this.CheckSubscription));
			this.CheckSubscription();
			return;
		}
		SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnLocalSubscriptionData, new Action(this.CheckSubscription));
	}

	// Token: 0x060042FA RID: 17146 RVA: 0x00164D95 File Offset: 0x00162F95
	protected virtual void OnDisable()
	{
		if (this.isSubscriberOnlyButton)
		{
			SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnLocalSubscriptionData, new Action(this.CheckSubscription));
		}
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.RefreshText));
	}

	// Token: 0x060042FB RID: 17147 RVA: 0x00164DD4 File Offset: 0x00162FD4
	private void CheckSubscription()
	{
		bool flag = SubscriptionManager.IsLocalSubscribed();
		if (!this._subscriptionChecked || flag != this._localPlayerSubscribed)
		{
			this.UpdateSubscriptionState(flag);
		}
	}

	// Token: 0x060042FC RID: 17148 RVA: 0x00164DFF File Offset: 0x00162FFF
	private void UpdateSubscriptionState(bool subscribed)
	{
		this._localPlayerSubscribed = subscribed;
		this.UpdateColor();
		this._subscriptionChecked = true;
	}

	// Token: 0x060042FD RID: 17149 RVA: 0x00164E18 File Offset: 0x00163018
	protected virtual void RefreshText()
	{
		if (this._offLocalizedText == null || this._offLocalizedText.IsEmpty || this._onLocalizedText == null || this._onLocalizedText.IsEmpty)
		{
			return;
		}
		if (!this._useOnOffText)
		{
			return;
		}
		string text;
		if (!this.isOn)
		{
			text = this.offText;
			text = this._offLocalizedText.GetLocalizedString();
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_PRESSABLE_BUTTON] Null or empty string returned for OFF localized text", this);
				text = this.offText;
			}
		}
		else
		{
			text = this.onText;
			text = this._onLocalizedText.GetLocalizedString();
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_PRESSABLE_BUTTON] Null or empty string returned for ON localized text", this);
				text = this.onText;
			}
		}
		if (this._myTxtSet || this.myText.IsNotNull())
		{
			this.myText.text = text;
		}
		if (this._myTmpTxtSet || this.myTmpText.IsNotNull())
		{
			this.myTmpText.text = text;
		}
		if (this._myTmpTxt2Set || this.myTmpText2.IsNotNull())
		{
			this.myTmpText2.text = text;
		}
	}

	// Token: 0x060042FE RID: 17150 RVA: 0x00164F28 File Offset: 0x00163128
	protected virtual void SetOffText(bool setMyText, bool setMyTmpText = false, bool setMyTmpText2 = false)
	{
		if (!this._useOnOffText)
		{
			return;
		}
		string localizedString = this.offText;
		if (this._offLocalizedText != null && !this._offLocalizedText.IsEmpty)
		{
			localizedString = this._offLocalizedText.GetLocalizedString();
			if (string.IsNullOrEmpty(localizedString))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_PRESSABLE_BUTTON] Null or empty string returned for OFF localized text", this);
				localizedString = this.offText;
			}
		}
		this._myTxtSet = setMyText;
		this._myTmpTxtSet = setMyTmpText;
		this._myTmpTxt2Set = setMyTmpText2;
		if (setMyText)
		{
			this.myText.text = localizedString;
		}
		if (setMyTmpText)
		{
			this.myTmpText.text = localizedString;
		}
		if (setMyTmpText2)
		{
			this.myTmpText2.text = localizedString;
		}
	}

	// Token: 0x060042FF RID: 17151 RVA: 0x00164FC4 File Offset: 0x001631C4
	protected virtual void SetOnText(bool setMyText, bool setMyTmpText = false, bool setMyTmpText2 = false)
	{
		if (!this._useOnOffText)
		{
			return;
		}
		string localizedString = this.onText;
		if (this._onLocalizedText != null && !this._onLocalizedText.IsEmpty)
		{
			localizedString = this._onLocalizedText.GetLocalizedString();
			if (string.IsNullOrEmpty(localizedString))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_PRESSABLE_BUTTON] Null or empty string returned for ON localized text", this);
				localizedString = this.onText;
			}
		}
		this._myTxtSet = setMyText;
		this._myTmpTxtSet = setMyTmpText;
		this._myTmpTxt2Set = setMyTmpText2;
		if (setMyText)
		{
			this.myText.text = localizedString;
		}
		if (setMyTmpText)
		{
			this.myTmpText.text = localizedString;
		}
		if (setMyTmpText2)
		{
			this.myTmpText2.text = localizedString;
		}
	}

	// Token: 0x06004300 RID: 17152 RVA: 0x00165060 File Offset: 0x00163260
	protected void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator component = collider.gameObject.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (!component)
		{
			return;
		}
		this.PressButton(component.isLeftHand);
	}

	// Token: 0x06004301 RID: 17153 RVA: 0x001650AC File Offset: 0x001632AC
	private void PressButton(bool isLeftHand)
	{
		if (this.isSubscriberOnlyButton && !this._localPlayerSubscribed && (!this.allowNonSubscriberBypassCheck || !this.AllowNonSubscribedPress()))
		{
			return;
		}
		if (this.isOwnerOnlyButton && !this.IsOwnedByLocalPlayer())
		{
			return;
		}
		this.touchTime = Time.time;
		UnityEvent unityEvent = this.onPressButton;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		Action<GorillaPressableButton, bool> action = this.onPressed;
		if (action != null)
		{
			action(this, isLeftHand);
		}
		this.ButtonActivation();
		this.ButtonActivationWithHand(isLeftHand);
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, isLeftHand, 0.05f);
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { 67, isLeftHand, 0.05f });
		}
	}

	// Token: 0x06004302 RID: 17154 RVA: 0x00002076 File Offset: 0x00000276
	protected virtual bool AllowNonSubscribedPress()
	{
		return false;
	}

	// Token: 0x06004303 RID: 17155 RVA: 0x001651C0 File Offset: 0x001633C0
	public void Click(bool leftHand = false)
	{
		this.PressButton(leftHand);
	}

	// Token: 0x06004304 RID: 17156 RVA: 0x001651CC File Offset: 0x001633CC
	private bool IsOwnedByLocalPlayer()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		if (componentInParent != null)
		{
			return componentInParent.isLocal;
		}
		CosmeticCollectionDisplay componentInParent2 = base.GetComponentInParent<CosmeticCollectionDisplay>(true);
		return !(componentInParent2 != null) || componentInParent2.IsLocal;
	}

	// Token: 0x06004305 RID: 17157 RVA: 0x0016520A File Offset: 0x0016340A
	public virtual void UpdateColor()
	{
		this.UpdateColorWithState(this.isOn);
	}

	// Token: 0x06004306 RID: 17158 RVA: 0x00165218 File Offset: 0x00163418
	protected void UpdateColorWithState(bool state)
	{
		if (this.isSubscriberOnlyButton && !this._localPlayerSubscribed)
		{
			this.SetUnsubscribedMaterial();
			this.SetOffText(this.myText.IsNotNull(), this.myTmpText.IsNotNull(), this.myTmpText2.IsNotNull());
			return;
		}
		if (state)
		{
			this.SetPressedMaterial();
			this.SetOnText(this.myText.IsNotNull(), this.myTmpText.IsNotNull(), this.myTmpText2.IsNotNull());
			return;
		}
		this.SetUnpressedMaterial();
		this.SetOffText(this.myText.IsNotNull(), this.myTmpText.IsNotNull(), this.myTmpText2.IsNotNull());
	}

	// Token: 0x06004307 RID: 17159 RVA: 0x001652C1 File Offset: 0x001634C1
	public void SetRendererMaterial(Material mat)
	{
		if (this.buttonRenderer)
		{
			this.buttonRenderer.material = mat;
		}
	}

	// Token: 0x06004308 RID: 17160 RVA: 0x001652DC File Offset: 0x001634DC
	public void SetPressedMaterial()
	{
		this.SetRendererMaterial(this.pressedMaterial);
	}

	// Token: 0x06004309 RID: 17161 RVA: 0x001652EA File Offset: 0x001634EA
	public void SetUnpressedMaterial()
	{
		this.SetRendererMaterial(this.unpressedMaterial);
	}

	// Token: 0x0600430A RID: 17162 RVA: 0x001652F8 File Offset: 0x001634F8
	public void SetUnsubscribedMaterial()
	{
		this.SetRendererMaterial(this.nonSubscriberMaterial ? this.nonSubscriberMaterial : this.unpressedMaterial);
	}

	// Token: 0x0600430B RID: 17163 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void ButtonActivation()
	{
	}

	// Token: 0x0600430C RID: 17164 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void ButtonActivationWithHand(bool isLeftHand)
	{
	}

	// Token: 0x0600430D RID: 17165 RVA: 0x0016531B File Offset: 0x0016351B
	public virtual void ResetState()
	{
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x0600430E RID: 17166 RVA: 0x0016532C File Offset: 0x0016352C
	public void SetText(string newText)
	{
		if (this.myTmpText != null)
		{
			this.myTmpText.text = newText;
		}
		if (this.myTmpText2 != null)
		{
			this.myTmpText2.text = newText;
		}
		if (this.myText != null)
		{
			this.myText.text = newText;
		}
	}

	// Token: 0x040054DC RID: 21724
	public Material pressedMaterial;

	// Token: 0x040054DD RID: 21725
	public Material unpressedMaterial;

	// Token: 0x040054DE RID: 21726
	public MeshRenderer buttonRenderer;

	// Token: 0x040054DF RID: 21727
	public int pressButtonSoundIndex = 67;

	// Token: 0x040054E0 RID: 21728
	public bool isOn;

	// Token: 0x040054E1 RID: 21729
	public float debounceTime = 0.25f;

	// Token: 0x040054E2 RID: 21730
	public float touchTime;

	// Token: 0x040054E3 RID: 21731
	public bool testPress;

	// Token: 0x040054E4 RID: 21732
	public bool testHandLeft;

	// Token: 0x040054E5 RID: 21733
	[SerializeField]
	private bool _useOnOffText = true;

	// Token: 0x040054E6 RID: 21734
	[TextArea]
	public string offText;

	// Token: 0x040054E7 RID: 21735
	[SerializeField]
	private LocalizedString _offLocalizedText;

	// Token: 0x040054E8 RID: 21736
	[TextArea]
	public string onText;

	// Token: 0x040054E9 RID: 21737
	[SerializeField]
	private LocalizedString _onLocalizedText;

	// Token: 0x040054EA RID: 21738
	[SerializeField]
	[Tooltip("Use this one when you can. Don't use MyText if you can help it!")]
	public TMP_Text myTmpText;

	// Token: 0x040054EB RID: 21739
	[SerializeField]
	[Tooltip("Use this one when you can. Don't use MyText if you can help it!")]
	public TMP_Text myTmpText2;

	// Token: 0x040054EC RID: 21740
	public Text myText;

	// Token: 0x040054ED RID: 21741
	public bool isSubscriberOnlyButton;

	// Token: 0x040054EE RID: 21742
	public Material nonSubscriberMaterial;

	// Token: 0x040054EF RID: 21743
	public bool allowNonSubscriberBypassCheck;

	// Token: 0x040054F0 RID: 21744
	protected bool _localPlayerSubscribed;

	// Token: 0x040054F1 RID: 21745
	private bool _subscriptionChecked;

	// Token: 0x040054F2 RID: 21746
	[Tooltip("For buttons on cosmetics: when true, only the player wearing this cosmetic can press the button.Leave off for world/UI buttons.")]
	public bool isOwnerOnlyButton;

	// Token: 0x040054F3 RID: 21747
	[Space]
	public UnityEvent onPressButton;

	// Token: 0x040054F4 RID: 21748
	protected bool _myTxtSet;

	// Token: 0x040054F5 RID: 21749
	protected bool _myTmpTxtSet;

	// Token: 0x040054F6 RID: 21750
	protected bool _myTmpTxt2Set;
}
