using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000BAE RID: 2990
public class KIDUIToggle : Slider
{
	// Token: 0x17000740 RID: 1856
	// (get) Token: 0x06004B88 RID: 19336 RVA: 0x00193191 File Offset: 0x00191391
	// (set) Token: 0x06004B89 RID: 19337 RVA: 0x00193199 File Offset: 0x00191399
	public bool CurrentValue { get; private set; }

	// Token: 0x17000741 RID: 1857
	// (get) Token: 0x06004B8A RID: 19338 RVA: 0x001931A2 File Offset: 0x001913A2
	public bool IsOn
	{
		get
		{
			return this.CurrentValue;
		}
	}

	// Token: 0x06004B8B RID: 19339 RVA: 0x001931AA File Offset: 0x001913AA
	protected override void Awake()
	{
		base.Awake();
		this.SetupToggleComponent();
	}

	// Token: 0x06004B8C RID: 19340 RVA: 0x001931B8 File Offset: 0x001913B8
	protected override void Start()
	{
		base.Start();
		base.interactable = false;
	}

	// Token: 0x06004B8D RID: 19341 RVA: 0x001931C7 File Offset: 0x001913C7
	protected override void OnEnable()
	{
		base.OnEnable();
		base.interactable = false;
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06004B8E RID: 19342 RVA: 0x001931F8 File Offset: 0x001913F8
	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		this.Toggle();
	}

	// Token: 0x06004B8F RID: 19343 RVA: 0x00193207 File Offset: 0x00191407
	public override void OnPointerEnter(PointerEventData pointerEventData)
	{
		this.SetHighlighted();
		this.inside = true;
	}

	// Token: 0x06004B90 RID: 19344 RVA: 0x00193216 File Offset: 0x00191416
	public override void OnPointerExit(PointerEventData pointerEventData)
	{
		this.SetNormal();
		this.inside = false;
	}

	// Token: 0x06004B91 RID: 19345 RVA: 0x00193228 File Offset: 0x00191428
	protected virtual void SetupToggleComponent()
	{
		this.SetupSliderComponent();
		base.handleRect.anchorMin = new Vector2(0f, 0.5f);
		base.handleRect.anchorMax = new Vector3(0f, 0.5f);
		base.handleRect.pivot = new Vector2(0f, 0.5f);
		base.handleRect.sizeDelta = new Vector2(base.handleRect.sizeDelta.x, base.handleRect.sizeDelta.x);
	}

	// Token: 0x06004B92 RID: 19346 RVA: 0x001932C0 File Offset: 0x001914C0
	protected virtual void SetupSliderComponent()
	{
		base.interactable = false;
		base.colors.disabledColor = Color.white;
		this.SetColors();
		base.transition = Selectable.Transition.None;
	}

	// Token: 0x06004B93 RID: 19347 RVA: 0x001932F4 File Offset: 0x001914F4
	public void RegisterOnChangeEvent(Action onChange)
	{
		this._onToggleChanged.AddListener(delegate
		{
			Action onChange2 = onChange;
			if (onChange2 == null)
			{
				return;
			}
			onChange2();
		});
	}

	// Token: 0x06004B94 RID: 19348 RVA: 0x00193328 File Offset: 0x00191528
	public void UnregisterOnChangeEvent(Action onChange)
	{
		this._onToggleChanged.RemoveListener(delegate
		{
			Action onChange2 = onChange;
			if (onChange2 == null)
			{
				return;
			}
			onChange2();
		});
	}

	// Token: 0x06004B95 RID: 19349 RVA: 0x0019335C File Offset: 0x0019155C
	public void RegisterToggleOnEvent(Action onToggle)
	{
		this._onToggleOn.AddListener(delegate
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2();
		});
	}

	// Token: 0x06004B96 RID: 19350 RVA: 0x00193390 File Offset: 0x00191590
	public void UnregisterToggleOnEvent(Action onToggle)
	{
		this._onToggleOn.RemoveListener(delegate
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2();
		});
	}

	// Token: 0x06004B97 RID: 19351 RVA: 0x001933C4 File Offset: 0x001915C4
	public void RegisterToggleOffEvent(Action onToggle)
	{
		this._onToggleOff.AddListener(delegate
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2();
		});
	}

	// Token: 0x06004B98 RID: 19352 RVA: 0x001933F8 File Offset: 0x001915F8
	public void UnregisterToggleOffEvent(Action onToggle)
	{
		this._onToggleOff.RemoveListener(delegate
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2();
		});
	}

	// Token: 0x06004B99 RID: 19353 RVA: 0x00193429 File Offset: 0x00191629
	private void SetColors()
	{
		base.colors = this._fillColors;
	}

	// Token: 0x06004B9A RID: 19354 RVA: 0x00193437 File Offset: 0x00191637
	private void Toggle()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetStateAndStartAnimation(!this.CurrentValue, false);
	}

	// Token: 0x06004B9B RID: 19355 RVA: 0x00193452 File Offset: 0x00191652
	public void SetValue(bool newValue)
	{
		if (newValue == this.CurrentValue)
		{
			return;
		}
		this.SetStateAndStartAnimation(newValue, false);
	}

	// Token: 0x06004B9C RID: 19356 RVA: 0x00193468 File Offset: 0x00191668
	private void SetStateAndStartAnimation(bool state, bool skipAnim = false)
	{
		if (this.CurrentValue == state)
		{
			Debug.Log("IS SAME STATE, WILL NOT CHANGE");
			return;
		}
		this.CurrentValue = state;
		UnityEvent onToggleChanged = this._onToggleChanged;
		if (onToggleChanged != null)
		{
			onToggleChanged.Invoke();
		}
		if (this.CurrentValue)
		{
			UnityEvent onToggleOn = this._onToggleOn;
			if (onToggleOn != null)
			{
				onToggleOn.Invoke();
			}
			KIDAudioManager.Instance.PlaySound(KIDAudioManager.KIDSoundType.Success);
		}
		else
		{
			UnityEvent onToggleOff = this._onToggleOff;
			if (onToggleOff != null)
			{
				onToggleOff.Invoke();
			}
			KIDAudioManager.Instance.PlaySound(KIDAudioManager.KIDSoundType.TurnOffPermission);
		}
		if (this._animationCoroutine != null)
		{
			base.StopCoroutine(this._animationCoroutine);
		}
		this._handleUnlockIcon.gameObject.SetActive(this.CurrentValue);
		this._handleLockIcon.gameObject.SetActive(!this.CurrentValue);
		if (this._animationDuration == 0f || skipAnim)
		{
			Debug.Log("[KID::UI::SetStateAndStartAnimation] Skipping animation. Setting value to " + (this.CurrentValue ? "1f" : "0f"));
			this.value = (this.CurrentValue ? 1f : 0f);
			return;
		}
		this._animationCoroutine = base.StartCoroutine(this.AnimateSlider());
	}

	// Token: 0x06004B9D RID: 19357 RVA: 0x00193587 File Offset: 0x00191787
	private IEnumerator AnimateSlider()
	{
		Debug.Log(string.Format("[KID::UI::TOGGLE] Toggle: [{0}] is {1}", base.name, this.CurrentValue));
		float startValue = (this.CurrentValue ? 0f : 1f);
		float endValue = (this.CurrentValue ? 1f : 0f);
		Debug.Log(string.Format("[KID::UI::TOGGLE] Toggle: [{0}] Start: {1}, End: {2}, Value: {3}", new object[] { base.name, startValue, endValue, this.value }));
		float time = 0f;
		while (time < this._animationDuration)
		{
			time += Time.deltaTime;
			float num = this._toggleEase.Evaluate(time / this._animationDuration);
			this.value = Mathf.Lerp(startValue, endValue, num);
			yield return null;
		}
		this.value = endValue;
		yield break;
	}

	// Token: 0x06004B9E RID: 19358 RVA: 0x00193598 File Offset: 0x00191798
	private void PostUpdate()
	{
		if (!this.inside)
		{
			return;
		}
		if (ControllerBehaviour.Instance)
		{
			if (ControllerBehaviour.Instance.TriggerDown && KIDUIToggle._canTrigger)
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
				this.Toggle();
				KIDUIToggle._triggeredThisFrame = true;
				KIDUIToggle._canTrigger = false;
				return;
			}
			if (!ControllerBehaviour.Instance.TriggerDown)
			{
				KIDUIToggle._canTrigger = true;
			}
		}
	}

	// Token: 0x06004B9F RID: 19359 RVA: 0x001936C4 File Offset: 0x001918C4
	private void LateUpdate()
	{
		if (KIDUIToggle._triggeredThisFrame)
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
		KIDUIToggle._triggeredThisFrame = false;
	}

	// Token: 0x06004BA0 RID: 19360 RVA: 0x001937A9 File Offset: 0x001919A9
	protected new void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
		this.inside = false;
	}

	// Token: 0x06004BA1 RID: 19361 RVA: 0x001937D4 File Offset: 0x001919D4
	private void SetDisabled(bool isLockedButEnabled)
	{
		this.SetSwitchColors(this._borderColors.disabledColor, this._handleColors.disabledColor, this._fillColors.disabledColor);
		this.SetBorderSize(this._disabledBorderSize);
		this.SetBackgroundActive(false);
	}

	// Token: 0x06004BA2 RID: 19362 RVA: 0x00193810 File Offset: 0x00191A10
	private void SetNormal()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.normalColor, this._handleColors.normalColor, this._fillColors.normalColor);
		this.SetBorderSize(this._normalBorderSize);
		this.SetBackgroundActive(false);
	}

	// Token: 0x06004BA3 RID: 19363 RVA: 0x00193860 File Offset: 0x00191A60
	private void SetSelected()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.selectedColor, this._handleColors.selectedColor, this._fillColors.selectedColor);
		this.SetBorderSize(this._selectedBorderSize);
		this.SetBackgroundActive(true);
	}

	// Token: 0x06004BA4 RID: 19364 RVA: 0x001938B0 File Offset: 0x00191AB0
	private void SetHighlighted()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.highlightedColor, this._handleColors.highlightedColor, this._fillColors.highlightedColor);
		this.SetBorderSize(this._highlightedBorderSize);
		this.SetBackgroundActive(true);
	}

	// Token: 0x06004BA5 RID: 19365 RVA: 0x00193900 File Offset: 0x00191B00
	private void SetPressed()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.pressedColor, this._handleColors.pressedColor, this._fillColors.pressedColor);
		this.SetBorderSize(this._pressedBorderSize);
		this.SetBackgroundActive(true);
	}

	// Token: 0x06004BA6 RID: 19366 RVA: 0x00193950 File Offset: 0x00191B50
	private void SetSwitchColors(Color borderColor, Color handleColor, Color fillColor)
	{
		this._borderImg.color = borderColor;
		this._handleImg.color = handleColor;
	}

	// Token: 0x06004BA7 RID: 19367 RVA: 0x0019396A File Offset: 0x00191B6A
	private void SetBorderSize(float borderScale)
	{
		this._borderImgRef.offsetMin = new Vector2(-borderScale, -borderScale * this._borderHeightRatio);
		this._borderImgRef.offsetMax = new Vector2(borderScale, borderScale * this._borderHeightRatio);
	}

	// Token: 0x06004BA8 RID: 19368 RVA: 0x001939A0 File Offset: 0x00191BA0
	private void SetBackgroundActive(bool isActive)
	{
		this._fillImg.gameObject.SetActive(isActive);
		this._fillInactiveImg.gameObject.SetActive(!isActive);
		this.SetBackgroundLocksActive(isActive);
	}

	// Token: 0x06004BA9 RID: 19369 RVA: 0x001939D0 File Offset: 0x00191BD0
	private void SetBackgroundLocksActive(bool isActive)
	{
		Color color = (isActive ? this._lockActiveColor : this._lockInactiveColor);
		this._lockIcon.color = color;
		this._unlockIcon.color = color;
	}

	// Token: 0x04005E66 RID: 24166
	[Header("Toggle Setup")]
	[SerializeField]
	[Range(0f, 1f)]
	private float _initValue;

	// Token: 0x04005E67 RID: 24167
	[SerializeField]
	private Image _borderImg;

	// Token: 0x04005E68 RID: 24168
	[SerializeField]
	private float _borderHeightRatio = 2f;

	// Token: 0x04005E69 RID: 24169
	[SerializeField]
	private Image _fillImg;

	// Token: 0x04005E6A RID: 24170
	[SerializeField]
	private Image _fillInactiveImg;

	// Token: 0x04005E6B RID: 24171
	[SerializeField]
	private Image _handleImg;

	// Token: 0x04005E6C RID: 24172
	[SerializeField]
	private Image _lockIcon;

	// Token: 0x04005E6D RID: 24173
	[SerializeField]
	private Image _unlockIcon;

	// Token: 0x04005E6E RID: 24174
	[SerializeField]
	private Image _handleLockIcon;

	// Token: 0x04005E6F RID: 24175
	[SerializeField]
	private Image _handleUnlockIcon;

	// Token: 0x04005E70 RID: 24176
	[SerializeField]
	private Color _lockActiveColor;

	// Token: 0x04005E71 RID: 24177
	[SerializeField]
	private Color _lockInactiveColor;

	// Token: 0x04005E72 RID: 24178
	[SerializeField]
	private RectTransform _borderImgRef;

	// Token: 0x04005E73 RID: 24179
	[Header("Steam Settings")]
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x04005E74 RID: 24180
	[Header("Animation")]
	[SerializeField]
	private float _animationDuration = 0.15f;

	// Token: 0x04005E75 RID: 24181
	[SerializeField]
	private AnimationCurve _toggleEase = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04005E76 RID: 24182
	[Header("Fill Colors")]
	[SerializeField]
	private ColorBlock _fillColors;

	// Token: 0x04005E77 RID: 24183
	[Header("Border Colors")]
	[SerializeField]
	private ColorBlock _borderColors;

	// Token: 0x04005E78 RID: 24184
	[Header("Borders")]
	[SerializeField]
	private float _normalBorderSize = 1f;

	// Token: 0x04005E79 RID: 24185
	[SerializeField]
	private float _disabledBorderSize = 1f;

	// Token: 0x04005E7A RID: 24186
	[SerializeField]
	private float _highlightedBorderSize = 1f;

	// Token: 0x04005E7B RID: 24187
	[SerializeField]
	private float _pressedBorderSize = 1f;

	// Token: 0x04005E7C RID: 24188
	[SerializeField]
	private float _selectedBorderSize = 1f;

	// Token: 0x04005E7D RID: 24189
	[Header("Handle Colors")]
	[SerializeField]
	private ColorBlock _handleColors;

	// Token: 0x04005E7E RID: 24190
	[Header("Events")]
	[SerializeField]
	private UnityEvent _onToggleOn;

	// Token: 0x04005E7F RID: 24191
	[SerializeField]
	private UnityEvent _onToggleOff;

	// Token: 0x04005E80 RID: 24192
	[SerializeField]
	private UnityEvent _onToggleChanged;

	// Token: 0x04005E81 RID: 24193
	private bool _previousValue;

	// Token: 0x04005E82 RID: 24194
	private bool _isDisabled;

	// Token: 0x04005E83 RID: 24195
	private Coroutine _animationCoroutine;

	// Token: 0x04005E85 RID: 24197
	private bool inside;

	// Token: 0x04005E86 RID: 24198
	private static bool _triggeredThisFrame = false;

	// Token: 0x04005E87 RID: 24199
	private static bool _canTrigger = true;
}
