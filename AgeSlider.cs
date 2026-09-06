using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000B22 RID: 2850
public class AgeSlider : MonoBehaviour, IBuildValidation
{
	// Token: 0x170006F0 RID: 1776
	// (get) Token: 0x06004933 RID: 18739 RVA: 0x00187AA5 File Offset: 0x00185CA5
	// (set) Token: 0x06004934 RID: 18740 RVA: 0x00187AAD File Offset: 0x00185CAD
	public AgeSlider.SliderHeldEvent onHoldComplete
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

	// Token: 0x06004935 RID: 18741 RVA: 0x00187AB6 File Offset: 0x00185CB6
	private void OnEnable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06004936 RID: 18742 RVA: 0x00187ADA File Offset: 0x00185CDA
	private void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x06004937 RID: 18743 RVA: 0x00187B00 File Offset: 0x00185D00
	protected void Update()
	{
		if (!AgeSlider._ageGateActive)
		{
			return;
		}
		if (ControllerBehaviour.Instance.ButtonDown && this._confirmButton.activeInHierarchy)
		{
			this.progress += Time.deltaTime / this.holdTime;
			this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
			this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
			if (this.progress >= 1f)
			{
				this.m_OnHoldComplete.Invoke(this._currentAge);
				return;
			}
		}
		else
		{
			this.progress = 0f;
			this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
			this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
		}
	}

	// Token: 0x06004938 RID: 18744 RVA: 0x00187C0C File Offset: 0x00185E0C
	private void PostUpdate()
	{
		if (!AgeSlider._ageGateActive)
		{
			return;
		}
		if (ControllerBehaviour.Instance.IsLeftStick || ControllerBehaviour.Instance.IsUpStick)
		{
			this._currentAge = Mathf.Clamp(this._currentAge - 1, 0, this._maxAge);
			this._ageValueTxt.text = ((this._currentAge > 0) ? this._currentAge.ToString() : "?");
			this._confirmButton.SetActive(this._currentAge > 0);
		}
		if (ControllerBehaviour.Instance.IsRightStick || ControllerBehaviour.Instance.IsDownStick)
		{
			this._currentAge = Mathf.Clamp(this._currentAge + 1, 0, this._maxAge);
			this._ageValueTxt.text = ((this._currentAge > 0) ? this._currentAge.ToString() : "?");
			this._confirmButton.SetActive(this._currentAge > 0);
		}
	}

	// Token: 0x06004939 RID: 18745 RVA: 0x00187CF9 File Offset: 0x00185EF9
	public static void ToggleAgeGate(bool state)
	{
		AgeSlider._ageGateActive = state;
	}

	// Token: 0x0600493A RID: 18746 RVA: 0x00187D01 File Offset: 0x00185F01
	public bool BuildValidationCheck()
	{
		if (this._confirmButton == null)
		{
			Debug.LogError("[KID] Object [_confirmButton] is NULL. Must be assigned in editor");
			return false;
		}
		return true;
	}

	// Token: 0x04005B67 RID: 23399
	private const int MIN_AGE = 13;

	// Token: 0x04005B68 RID: 23400
	[SerializeField]
	private AgeSlider.SliderHeldEvent m_OnHoldComplete = new AgeSlider.SliderHeldEvent();

	// Token: 0x04005B69 RID: 23401
	[SerializeField]
	private int _maxAge = 99;

	// Token: 0x04005B6A RID: 23402
	[SerializeField]
	private TMP_Text _ageValueTxt;

	// Token: 0x04005B6B RID: 23403
	[SerializeField]
	private GameObject _confirmButton;

	// Token: 0x04005B6C RID: 23404
	[SerializeField]
	private float holdTime = 5f;

	// Token: 0x04005B6D RID: 23405
	[SerializeField]
	private LineRenderer progressBar;

	// Token: 0x04005B6E RID: 23406
	private int _currentAge;

	// Token: 0x04005B6F RID: 23407
	private static bool _ageGateActive;

	// Token: 0x04005B70 RID: 23408
	private float progress;

	// Token: 0x02000B23 RID: 2851
	[Serializable]
	public class SliderHeldEvent : UnityEvent<int>
	{
	}
}
