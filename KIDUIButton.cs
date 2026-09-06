using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

// Token: 0x02000BA7 RID: 2983
public class KIDUIButton : Button, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x1700073C RID: 1852
	// (get) Token: 0x06004B46 RID: 19270 RVA: 0x00088695 File Offset: 0x00086895
	private XRUIInputModule InputModule
	{
		get
		{
			return EventSystem.current.currentInputModule as XRUIInputModule;
		}
	}

	// Token: 0x06004B47 RID: 19271 RVA: 0x001921A6 File Offset: 0x001903A6
	protected override void OnEnable()
	{
		base.OnEnable();
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06004B48 RID: 19272 RVA: 0x001921D0 File Offset: 0x001903D0
	private void PostUpdate()
	{
		if (!KIDUIButton._canTrigger)
		{
			KIDUIButton._canTrigger = !ControllerBehaviour.Instance.TriggerDown;
		}
		if (!base.interactable || !this.inside || !KIDUIButton._canTrigger)
		{
			return;
		}
		if (ControllerBehaviour.Instance && ControllerBehaviour.Instance.TriggerDown && !KIDUIButton._triggeredThisFrame)
		{
			string text = string.Concat(new string[]
			{
				"[",
				base.transform.parent.parent.parent.name,
				".",
				base.transform.parent.parent.name,
				".",
				base.transform.parent.name,
				".",
				base.transform.name,
				"]"
			});
			Debug.Log(string.Concat(new string[]
			{
				"[KID::UIBUTTON::DEBUG] ",
				text,
				" - STEAM - OnClick is pressed. Time: [",
				Time.time.ToString(),
				"]"
			}), this);
			Button.ButtonClickedEvent onClick = base.onClick;
			if (onClick != null)
			{
				onClick.Invoke();
			}
			KIDUIButton._triggeredThisFrame = true;
			KIDUIButton._canTrigger = false;
		}
	}

	// Token: 0x06004B49 RID: 19273 RVA: 0x0019231C File Offset: 0x0019051C
	private void LateUpdate()
	{
		if (KIDUIButton._triggeredThisFrame)
		{
			string text = string.Concat(new string[]
			{
				"[",
				base.transform.parent.parent.parent.name,
				".",
				base.transform.parent.parent.name,
				".",
				base.transform.parent.name,
				".",
				base.transform.name,
				"]"
			});
			Debug.Log(string.Concat(new string[]
			{
				"[KID::UIBUTTON::DEBUG] ",
				text,
				" - STEAM - OnLateUpdate triggered and Triggered Frame Reset. Time: [",
				Time.time.ToString(),
				"]"
			}), this);
		}
		KIDUIButton._triggeredThisFrame = false;
	}

	// Token: 0x06004B4A RID: 19274 RVA: 0x00192401 File Offset: 0x00190601
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.inside = false;
	}

	// Token: 0x06004B4B RID: 19275 RVA: 0x00192411 File Offset: 0x00190611
	public void ResetButton()
	{
		this.inside = false;
		KIDUIButton._triggeredThisFrame = false;
	}

	// Token: 0x06004B4C RID: 19276 RVA: 0x00192420 File Offset: 0x00190620
	protected override void OnDisable()
	{
		this.FixStuckPressedState();
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x06004B4D RID: 19277 RVA: 0x0019244A File Offset: 0x0019064A
	private void FixStuckPressedState()
	{
		this.InstantClearState();
		this._buttonText.color = (base.interactable ? this._normalTextColor : this._disabledTextColor);
		this.inside = false;
		KIDUIButton._triggeredThisFrame = false;
	}

	// Token: 0x06004B4E RID: 19278 RVA: 0x00192480 File Offset: 0x00190680
	protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
	{
		base.DoStateTransition(state, instant);
		switch (state)
		{
		default:
			this._buttonText.color = this._normalTextColor;
			this.SetIcons(true, false);
			return;
		case Selectable.SelectionState.Highlighted:
			this._buttonText.color = this._highlightedTextColor;
			this.SetIcons(false, true);
			return;
		case Selectable.SelectionState.Pressed:
			this._buttonText.color = this._pressedTextColor;
			this.SetIcons(true, false);
			return;
		case Selectable.SelectionState.Selected:
			this._buttonText.color = this._selectedTextColor;
			this.SetIcons(true, false);
			return;
		case Selectable.SelectionState.Disabled:
			this._buttonText.color = this._disabledTextColor;
			this.SetIcons(true, false);
			return;
		}
	}

	// Token: 0x06004B4F RID: 19279 RVA: 0x00192530 File Offset: 0x00190730
	private void SetIcons(bool normalEnabled, bool highlightedEnabled)
	{
		if (this._normalIcon == null || this._highlightedIcon == null)
		{
			return;
		}
		GameObject normalIcon = this._normalIcon;
		if (normalIcon != null)
		{
			normalIcon.SetActive(normalEnabled);
		}
		GameObject highlightedIcon = this._highlightedIcon;
		if (highlightedIcon == null)
		{
			return;
		}
		highlightedIcon.SetActive(highlightedEnabled);
	}

	// Token: 0x06004B50 RID: 19280 RVA: 0x00192580 File Offset: 0x00190780
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.inside = true;
		if (!this.IsInteractable() || !this.IsActive())
		{
			return;
		}
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance != null)
		{
			instance.PlaySound(KIDAudioManager.KIDSoundType.Hover);
		}
		Debug.Log("[KID::UIBUTTON::KIDAudioManager] Hover played");
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
		if (!xrrayInteractor)
		{
			return;
		}
		xrrayInteractor.xrController.SendHapticImpulse(this._highlightedVibrationStrength, this._highlightedVibrationDuration);
	}

	// Token: 0x06004B51 RID: 19281 RVA: 0x00192600 File Offset: 0x00190800
	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		this.inside = false;
		if (!this.IsInteractable() || !this.IsActive())
		{
			return;
		}
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance != null)
		{
			instance.PlaySound(this.onClickSound);
		}
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
		if (!xrrayInteractor)
		{
			return;
		}
		xrrayInteractor.xrController.SendHapticImpulse(this._pressedVibrationStrength, this._pressedVibrationDuration);
	}

	// Token: 0x06004B52 RID: 19282 RVA: 0x0019267A File Offset: 0x0019087A
	public void SetText(string text)
	{
		this._buttonText.SetText(text);
	}

	// Token: 0x06004B53 RID: 19283 RVA: 0x00192688 File Offset: 0x00190888
	public void SetFont(TMP_FontAsset font)
	{
		this._buttonText.font = font;
	}

	// Token: 0x06004B54 RID: 19284 RVA: 0x00192696 File Offset: 0x00190896
	public string GetText()
	{
		return this._buttonText.text;
	}

	// Token: 0x06004B55 RID: 19285 RVA: 0x001926A3 File Offset: 0x001908A3
	public void SetBorderImage(Sprite newImg)
	{
		this._borderImage.sprite = newImg;
	}

	// Token: 0x04005E2A RID: 24106
	[SerializeField]
	private Image _borderImage;

	// Token: 0x04005E2B RID: 24107
	[SerializeField]
	private RectTransform _fillImageRef;

	// Token: 0x04005E2C RID: 24108
	[SerializeField]
	private TMP_Text _buttonText;

	// Token: 0x04005E2D RID: 24109
	[Header("Transition States")]
	[Header("Normal")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _normalBorderColor;

	// Token: 0x04005E2E RID: 24110
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _normalTextColor;

	// Token: 0x04005E2F RID: 24111
	[SerializeField]
	private float _normalBorderSize;

	// Token: 0x04005E30 RID: 24112
	[Header("Highlighted")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _highlightedBorderColor;

	// Token: 0x04005E31 RID: 24113
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _highlightedTextColor;

	// Token: 0x04005E32 RID: 24114
	[SerializeField]
	private float _highlightedBorderSize;

	// Token: 0x04005E33 RID: 24115
	[SerializeField]
	private float _highlightedVibrationStrength = 0.1f;

	// Token: 0x04005E34 RID: 24116
	[SerializeField]
	private float _highlightedVibrationDuration = 0.1f;

	// Token: 0x04005E35 RID: 24117
	[Header("Pressed")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _pressedBorderColor;

	// Token: 0x04005E36 RID: 24118
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _pressedTextColor;

	// Token: 0x04005E37 RID: 24119
	[SerializeField]
	private float _pressedBorderSize;

	// Token: 0x04005E38 RID: 24120
	[SerializeField]
	private float _pressedVibrationStrength = 0.5f;

	// Token: 0x04005E39 RID: 24121
	[SerializeField]
	private float _pressedVibrationDuration = 0.1f;

	// Token: 0x04005E3A RID: 24122
	[Header("Selected")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _selectedBorderColor;

	// Token: 0x04005E3B RID: 24123
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _selectedTextColor;

	// Token: 0x04005E3C RID: 24124
	[SerializeField]
	private float _selectedBorderSize;

	// Token: 0x04005E3D RID: 24125
	[Header("Disabled")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _disabledBorderColor;

	// Token: 0x04005E3E RID: 24126
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _disabledTextColor;

	// Token: 0x04005E3F RID: 24127
	[SerializeField]
	private float _disabledBorderSize;

	// Token: 0x04005E40 RID: 24128
	[Header("Audio")]
	[SerializeField]
	private KIDAudioManager.KIDSoundType onClickSound;

	// Token: 0x04005E41 RID: 24129
	[Header("Icon Swap Settings")]
	[SerializeField]
	private GameObject _normalIcon;

	// Token: 0x04005E42 RID: 24130
	[SerializeField]
	private GameObject _highlightedIcon;

	// Token: 0x04005E43 RID: 24131
	[Header("Steam Settings")]
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x04005E44 RID: 24132
	private bool inside;

	// Token: 0x04005E45 RID: 24133
	private static bool _triggeredThisFrame = false;

	// Token: 0x04005E46 RID: 24134
	private static bool _canTrigger = true;
}
