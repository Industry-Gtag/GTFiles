using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000BAA RID: 2986
public class KIDUIHoldableButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	// Token: 0x1700073E RID: 1854
	// (get) Token: 0x06004B72 RID: 19314 RVA: 0x00192C6A File Offset: 0x00190E6A
	// (set) Token: 0x06004B73 RID: 19315 RVA: 0x00192C72 File Offset: 0x00190E72
	public KIDUIHoldableButton.ButtonHoldCompleteEvent onHoldComplete
	{
		get
		{
			return this.m_OnHoldComplete;
		}
		set
		{
			this.m_OnHoldComplete = value;
		}
	}

	// Token: 0x1700073F RID: 1855
	// (get) Token: 0x06004B74 RID: 19316 RVA: 0x00192C7B File Offset: 0x00190E7B
	public float HoldPercentage
	{
		get
		{
			return this._elapsedTime / this._holdDuration;
		}
	}

	// Token: 0x06004B75 RID: 19317 RVA: 0x00192C8C File Offset: 0x00190E8C
	private void OnEnable()
	{
		this._holdProgressFill.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06004B76 RID: 19318 RVA: 0x00192CDF File Offset: 0x00190EDF
	private void Update()
	{
		this.ManageButtonInteraction(false);
	}

	// Token: 0x06004B77 RID: 19319 RVA: 0x00192CE8 File Offset: 0x00190EE8
	public void OnPointerDown(PointerEventData eventData)
	{
		this._isHoldingMouse = true;
		this.ToggleHoldingButton(true);
	}

	// Token: 0x06004B78 RID: 19320 RVA: 0x00192CF8 File Offset: 0x00190EF8
	public void OnPointerUp(PointerEventData eventData)
	{
		this._isHoldingMouse = false;
		this.ManageButtonInteraction(true);
		this.ToggleHoldingButton(false);
	}

	// Token: 0x06004B79 RID: 19321 RVA: 0x00192D10 File Offset: 0x00190F10
	private void ToggleHoldingButton(bool isPointerDown)
	{
		this._isHoldingButton = isPointerDown && this._button.interactable;
		this._holdProgressFill.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		if (isPointerDown)
		{
			this._elapsedTime = 0f;
			KIDUIHoldableButton.ButtonHoldStartEvent onHoldStart = this.m_OnHoldStart;
			if (onHoldStart != null)
			{
				onHoldStart.Invoke();
			}
			KIDAudioManager.Instance.StartButtonHeldSound();
			return;
		}
		KIDUIHoldableButton.ButtonHoldReleaseEvent onHoldRelease = this.m_OnHoldRelease;
		if (onHoldRelease != null)
		{
			onHoldRelease.Invoke();
		}
		KIDAudioManager.Instance.StopButtonHeldSound();
	}

	// Token: 0x06004B7A RID: 19322 RVA: 0x00192DA0 File Offset: 0x00190FA0
	private void ManageButtonInteraction(bool isPointerUp = false)
	{
		if (!this._isHoldingButton)
		{
			return;
		}
		if (isPointerUp)
		{
			return;
		}
		if (this._holdDuration <= 0f)
		{
			this.HoldComplete();
			return;
		}
		this._elapsedTime += Time.deltaTime;
		bool flag = this._elapsedTime > this._holdDuration;
		float num = this._elapsedTime / this._holdDuration;
		this._holdProgressFill.rectTransform.localScale = new Vector3(num, 1f, 1f);
		HandRayController.Instance.PulseActiveHandray(num, 0.1f);
		if (flag)
		{
			this.HoldComplete();
		}
	}

	// Token: 0x06004B7B RID: 19323 RVA: 0x00192E34 File Offset: 0x00191034
	private void HoldComplete()
	{
		this.ToggleHoldingButton(false);
		KIDUIHoldableButton.ButtonHoldCompleteEvent onHoldComplete = this.m_OnHoldComplete;
		if (onHoldComplete != null)
		{
			onHoldComplete.Invoke();
		}
		Debug.Log("[HOLD_BUTTON " + base.name + " ]: Hold Complete");
		this.ResetButton();
	}

	// Token: 0x06004B7C RID: 19324 RVA: 0x00192E6E File Offset: 0x0019106E
	private void ResetButton()
	{
		this._elapsedTime = 0f;
		this.inside = false;
		KIDUIHoldableButton._triggeredThisFrame = false;
		this._button.ResetButton();
	}

	// Token: 0x06004B7D RID: 19325 RVA: 0x00192E93 File Offset: 0x00191093
	protected void Awake()
	{
		if (this._button != null)
		{
			return;
		}
		this._button = base.GetComponentInChildren<KIDUIButton>();
		if (this._button == null)
		{
			Debug.LogError("[KID::UI_BUTTON] Could not find [KIDUIButton] in children, trying to create a new one.");
			return;
		}
	}

	// Token: 0x06004B7E RID: 19326 RVA: 0x00192ECC File Offset: 0x001910CC
	private void PostUpdate()
	{
		if (!KIDUIHoldableButton._canTrigger)
		{
			KIDUIHoldableButton._canTrigger = !ControllerBehaviour.Instance.TriggerDown;
		}
		if (!this._button.interactable || !KIDUIHoldableButton._canTrigger)
		{
			return;
		}
		if (ControllerBehaviour.Instance)
		{
			if (ControllerBehaviour.Instance.TriggerDown && this.inside)
			{
				if (!this._isHoldingButton)
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
					this.ToggleHoldingButton(true);
					KIDUIHoldableButton._triggeredThisFrame = true;
					KIDUIHoldableButton._canTrigger = false;
					return;
				}
			}
			else if (this._isHoldingButton && !this._isHoldingMouse)
			{
				this.ToggleHoldingButton(false);
			}
		}
	}

	// Token: 0x06004B7F RID: 19327 RVA: 0x00193030 File Offset: 0x00191230
	private void LateUpdate()
	{
		if (KIDUIHoldableButton._triggeredThisFrame)
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
		KIDUIHoldableButton._triggeredThisFrame = false;
	}

	// Token: 0x06004B80 RID: 19328 RVA: 0x00193115 File Offset: 0x00191315
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.inside = true;
	}

	// Token: 0x06004B81 RID: 19329 RVA: 0x0019311E File Offset: 0x0019131E
	public void OnPointerExit(PointerEventData eventData)
	{
		this.inside = false;
	}

	// Token: 0x06004B82 RID: 19330 RVA: 0x00193127 File Offset: 0x00191327
	protected void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
		this.inside = false;
	}

	// Token: 0x04005E59 RID: 24153
	public KIDUIButton _button;

	// Token: 0x04005E5A RID: 24154
	[SerializeField]
	private float _holdDuration;

	// Token: 0x04005E5B RID: 24155
	[SerializeField]
	private Image _holdProgressFill;

	// Token: 0x04005E5C RID: 24156
	[Header("Steam Settings")]
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x04005E5D RID: 24157
	[SerializeField]
	private KIDUIHoldableButton.ButtonHoldCompleteEvent m_OnHoldComplete = new KIDUIHoldableButton.ButtonHoldCompleteEvent();

	// Token: 0x04005E5E RID: 24158
	[SerializeField]
	private KIDUIHoldableButton.ButtonHoldStartEvent m_OnHoldStart = new KIDUIHoldableButton.ButtonHoldStartEvent();

	// Token: 0x04005E5F RID: 24159
	[SerializeField]
	private KIDUIHoldableButton.ButtonHoldReleaseEvent m_OnHoldRelease = new KIDUIHoldableButton.ButtonHoldReleaseEvent();

	// Token: 0x04005E60 RID: 24160
	private bool _isHoldingButton;

	// Token: 0x04005E61 RID: 24161
	private float _elapsedTime;

	// Token: 0x04005E62 RID: 24162
	private bool inside;

	// Token: 0x04005E63 RID: 24163
	private bool _isHoldingMouse;

	// Token: 0x04005E64 RID: 24164
	private static bool _triggeredThisFrame = false;

	// Token: 0x04005E65 RID: 24165
	private static bool _canTrigger = true;

	// Token: 0x02000BAB RID: 2987
	[Serializable]
	public class ButtonHoldCompleteEvent : UnityEvent
	{
	}

	// Token: 0x02000BAC RID: 2988
	[Serializable]
	public class ButtonHoldStartEvent : UnityEvent
	{
	}

	// Token: 0x02000BAD RID: 2989
	[Serializable]
	public class ButtonHoldReleaseEvent : UnityEvent
	{
	}
}
