using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000B24 RID: 2852
public class AgeSliderWithProgressBar : MonoBehaviourTick
{
	// Token: 0x170006F1 RID: 1777
	// (get) Token: 0x0600493D RID: 18749 RVA: 0x00187D4C File Offset: 0x00185F4C
	// (set) Token: 0x0600493E RID: 18750 RVA: 0x00187D54 File Offset: 0x00185F54
	public AgeSliderWithProgressBar.SliderHeldEvent onHoldComplete
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

	// Token: 0x170006F2 RID: 1778
	// (get) Token: 0x0600493F RID: 18751 RVA: 0x00187D5D File Offset: 0x00185F5D
	public bool AdjustAge
	{
		get
		{
			return this._adjustAge;
		}
	}

	// Token: 0x170006F3 RID: 1779
	// (get) Token: 0x06004940 RID: 18752 RVA: 0x00187D65 File Offset: 0x00185F65
	// (set) Token: 0x06004941 RID: 18753 RVA: 0x00187D6D File Offset: 0x00185F6D
	public bool ControllerActive
	{
		get
		{
			return this.controllerActive;
		}
		set
		{
			if (value)
			{
				ControllerBehaviour.Instance.OnAction += this.PostUpdate;
			}
			else
			{
				ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
			}
			this.controllerActive = value;
		}
	}

	// Token: 0x170006F4 RID: 1780
	// (get) Token: 0x06004942 RID: 18754 RVA: 0x00187DA7 File Offset: 0x00185FA7
	// (set) Token: 0x06004943 RID: 18755 RVA: 0x00187DAF File Offset: 0x00185FAF
	public string LockMessage
	{
		get
		{
			return this._lockMessage;
		}
		set
		{
			this._lockMessage = value;
		}
	}

	// Token: 0x170006F5 RID: 1781
	// (get) Token: 0x06004944 RID: 18756 RVA: 0x00187DB8 File Offset: 0x00185FB8
	public int CurrentAge
	{
		get
		{
			return this._currentAge;
		}
	}

	// Token: 0x06004945 RID: 18757 RVA: 0x00187DC0 File Offset: 0x00185FC0
	private void Awake()
	{
		if (this._messageText)
		{
			this._originalText = this._messageText.text;
		}
	}

	// Token: 0x06004946 RID: 18758 RVA: 0x00187DE0 File Offset: 0x00185FE0
	public void SetOriginalText(string text)
	{
		this._originalText = text;
	}

	// Token: 0x06004947 RID: 18759 RVA: 0x00187DEC File Offset: 0x00185FEC
	private new void OnEnable()
	{
		base.OnEnable();
		if (this._progressBarContainer != null && this.progressBarFill != null)
		{
			this.progressBarFill.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		}
		if (this._ageValueTxt)
		{
			this._ageValueTxt.text = ((this._currentAge > 0) ? this._currentAge.ToString() : "?");
		}
	}

	// Token: 0x06004948 RID: 18760 RVA: 0x00187E74 File Offset: 0x00186074
	public override void Tick()
	{
		if (!this._progressBarContainer)
		{
			return;
		}
		if (!this.ControllerActive)
		{
			return;
		}
		if (!this._lockMessage.IsNullOrEmpty())
		{
			this.progress = 0f;
			if (this._messageText)
			{
				this._messageText.text = this.LockMessage;
			}
		}
		else
		{
			if (this._messageText)
			{
				this._messageText.text = this._originalText;
			}
			if ((double)this.progress == 1.0)
			{
				this.m_OnHoldComplete.Invoke(this._currentAge);
				this.progress = 0f;
			}
			if (ControllerBehaviour.Instance.ButtonDown && this._progressBarContainer != null && (this._currentAge > 0 || !this.AdjustAge))
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progress = Mathf.Clamp01(this.progress);
			}
			else
			{
				this.progress = 0f;
			}
		}
		if (this._progressBarContainer != null)
		{
			this.progressBarFill.rectTransform.localScale = new Vector3(this.progress, 1f, 1f);
		}
	}

	// Token: 0x06004949 RID: 18761 RVA: 0x00187FB8 File Offset: 0x001861B8
	private void PostUpdate()
	{
		if (this.ControllerActive && this._ageValueTxt && this._ageSlidable && !this._incrementButtonsLockingSlider)
		{
			if (ControllerBehaviour.Instance.IsLeftStick)
			{
				this._currentAge = Mathf.Clamp(this._currentAge - 1, 0, this._maxAge);
				if (this._currentAge > 0 && this._currentAge < this._maxAge)
				{
					HandRayController.Instance.PulseActiveHandray(this._stickVibrationStrength, this._stickVibrationDuration);
				}
			}
			if (ControllerBehaviour.Instance.IsRightStick)
			{
				this._currentAge = Mathf.Clamp(this._currentAge + 1, 0, this._maxAge);
				if (this._currentAge > 0 && this._currentAge < this._maxAge)
				{
					HandRayController.Instance.PulseActiveHandray(this._stickVibrationStrength, this._stickVibrationDuration);
				}
			}
		}
		if (this._ageValueTxt)
		{
			this._ageValueTxt.text = this.GetAgeString();
			if (this._progressBarContainer != null)
			{
				this._progressBarContainer.SetActive(this._currentAge > 0);
			}
		}
	}

	// Token: 0x0600494A RID: 18762 RVA: 0x001880DC File Offset: 0x001862DC
	public void EnableEditing()
	{
		this._ageSlidable = true;
	}

	// Token: 0x0600494B RID: 18763 RVA: 0x001880E5 File Offset: 0x001862E5
	public void DisableEditing()
	{
		this._ageSlidable = false;
	}

	// Token: 0x0600494C RID: 18764 RVA: 0x001880F0 File Offset: 0x001862F0
	public string GetAgeString()
	{
		if (this._confirmButton)
		{
			this._confirmButton.interactable = true;
		}
		if (this._currentAge == 0)
		{
			if (this._confirmButton)
			{
				this._confirmButton.interactable = false;
			}
			return "?";
		}
		if (this._currentAge == this._maxAge)
		{
			return this._maxAge.ToString() + "+";
		}
		return this._currentAge.ToString();
	}

	// Token: 0x0600494D RID: 18765 RVA: 0x0018816C File Offset: 0x0018636C
	public void ForceAddAge(int number)
	{
		this._incrementButtonsLockingSlider = true;
		this._currentAge = Math.Min(this._currentAge + number, this._maxAge);
	}

	// Token: 0x0600494E RID: 18766 RVA: 0x0018818E File Offset: 0x0018638E
	public void ForceSubtractAge(int number)
	{
		this._incrementButtonsLockingSlider = true;
		this._currentAge = Math.Max(this._currentAge - number, 1);
	}

	// Token: 0x04005B71 RID: 23409
	private const int MIN_AGE = 13;

	// Token: 0x04005B72 RID: 23410
	[SerializeField]
	private AgeSliderWithProgressBar.SliderHeldEvent m_OnHoldComplete = new AgeSliderWithProgressBar.SliderHeldEvent();

	// Token: 0x04005B73 RID: 23411
	[SerializeField]
	private bool _adjustAge;

	// Token: 0x04005B74 RID: 23412
	[SerializeField]
	private int _maxAge = 25;

	// Token: 0x04005B75 RID: 23413
	[SerializeField]
	private TMP_Text _ageValueTxt;

	// Token: 0x04005B76 RID: 23414
	[Tooltip("Optional game object that should hold the Progress Bar Fill. Disables Hold functionality if null.")]
	[SerializeField]
	private GameObject _progressBarContainer;

	// Token: 0x04005B77 RID: 23415
	[SerializeField]
	private float holdTime = 2.5f;

	// Token: 0x04005B78 RID: 23416
	[SerializeField]
	private Image progressBarFill;

	// Token: 0x04005B79 RID: 23417
	[SerializeField]
	private TMP_Text _messageText;

	// Token: 0x04005B7A RID: 23418
	[SerializeField]
	private float _stickVibrationStrength = 0.1f;

	// Token: 0x04005B7B RID: 23419
	[SerializeField]
	private float _stickVibrationDuration = 0.05f;

	// Token: 0x04005B7C RID: 23420
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x04005B7D RID: 23421
	private bool _ageSlidable = true;

	// Token: 0x04005B7E RID: 23422
	private bool _incrementButtonsLockingSlider;

	// Token: 0x04005B7F RID: 23423
	private bool controllerActive;

	// Token: 0x04005B80 RID: 23424
	[SerializeField]
	private string _lockMessage;

	// Token: 0x04005B81 RID: 23425
	private string _originalText;

	// Token: 0x04005B82 RID: 23426
	private int _currentAge;

	// Token: 0x04005B83 RID: 23427
	private float progress;

	// Token: 0x02000B25 RID: 2853
	[Serializable]
	public class SliderHeldEvent : UnityEvent<int>
	{
	}
}
