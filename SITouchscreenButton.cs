using System;
using GorillaTag.Audio;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000175 RID: 373
public class SITouchscreenButton : MonoBehaviour, IClickable
{
	// Token: 0x170000DA RID: 218
	// (get) Token: 0x060009D6 RID: 2518 RVA: 0x0003519C File Offset: 0x0003339C
	private bool IsReady
	{
		get
		{
			bool flag = Time.time - this._enableTime >= 0.2f;
			if (this._screenRegion)
			{
				flag = flag && !this._screenRegion.HasPressedButton;
			}
			return flag;
		}
	}

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x060009D7 RID: 2519 RVA: 0x000351E3 File Offset: 0x000333E3
	public bool IsToggledOn
	{
		get
		{
			return this._isToggledOn;
		}
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x000351EC File Offset: 0x000333EC
	private void Awake()
	{
		ITouchScreenStation componentInParent = base.GetComponentInParent<ITouchScreenStation>();
		if (componentInParent != null)
		{
			this._screenRegion = componentInParent.ScreenRegion;
		}
		if (this.buttonMode == SITouchscreenButton.ButtonMode.Toggle)
		{
			this._isToggledOn = this._startToggledOn;
		}
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x00035224 File Offset: 0x00033424
	private void OnEnable()
	{
		this._enableTime = Time.time;
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x00035234 File Offset: 0x00033434
	private void OnTriggerEnter(Collider other)
	{
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent)
		{
			this.PressButton();
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x00035280 File Offset: 0x00033480
	public void PressButton()
	{
		if (!this.IsReady || !this.isUsable)
		{
			return;
		}
		if (this._screenRegion)
		{
			this._screenRegion.RegisterButtonPress();
		}
		if (this.buttonMode == SITouchscreenButton.ButtonMode.Normal)
		{
			this.buttonPressed.Invoke(this.buttonType, this.data, NetworkSystem.Instance.LocalPlayer.ActorNumber);
		}
		else if (this.buttonMode == SITouchscreenButton.ButtonMode.Toggle)
		{
			bool flag = !this._isToggledOn;
			this.buttonToggled.Invoke(this.buttonType, this.data, NetworkSystem.Instance.LocalPlayer.ActorNumber, flag);
		}
		if (this._pressSound != null)
		{
			GTAudioOneShot.Play(this._pressSound, base.transform.position, this._pressSoundVolume, 1f);
		}
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x00035350 File Offset: 0x00033550
	public void SetToggleState(bool state, bool invokeEvent = false)
	{
		if (this.buttonMode != SITouchscreenButton.ButtonMode.Toggle)
		{
			return;
		}
		bool flag = this._isToggledOn != state;
		this._isToggledOn = state;
		if (invokeEvent && flag)
		{
			this.buttonToggled.Invoke(this.buttonType, this.data, NetworkSystem.Instance.LocalPlayer.ActorNumber, this._isToggledOn);
		}
	}

	// Token: 0x060009DD RID: 2525 RVA: 0x000353AC File Offset: 0x000335AC
	public void Click(bool leftHand = false)
	{
		this.PressButton();
	}

	// Token: 0x04000BFA RID: 3066
	public SITouchscreenButton.ButtonMode buttonMode;

	// Token: 0x04000BFB RID: 3067
	public SITouchscreenButton.SITouchscreenButtonType buttonType;

	// Token: 0x04000BFC RID: 3068
	public int data;

	// Token: 0x04000BFD RID: 3069
	[SerializeField]
	private AudioClip _pressSound;

	// Token: 0x04000BFE RID: 3070
	[SerializeField]
	private float _pressSoundVolume = 0.1f;

	// Token: 0x04000BFF RID: 3071
	[SerializeField]
	private bool _isToggledOn;

	// Token: 0x04000C00 RID: 3072
	[SerializeField]
	private bool _startToggledOn;

	// Token: 0x04000C01 RID: 3073
	public UnityEvent<SITouchscreenButton.SITouchscreenButtonType, int, int> buttonPressed;

	// Token: 0x04000C02 RID: 3074
	public UnityEvent<SITouchscreenButton.SITouchscreenButtonType, int, int, bool> buttonToggled;

	// Token: 0x04000C03 RID: 3075
	private SIScreenRegion _screenRegion;

	// Token: 0x04000C04 RID: 3076
	private const float DEBOUNCE_TIME = 0.2f;

	// Token: 0x04000C05 RID: 3077
	private float _enableTime;

	// Token: 0x04000C06 RID: 3078
	[NonSerialized]
	public bool isUsable = true;

	// Token: 0x02000176 RID: 374
	public enum ButtonMode
	{
		// Token: 0x04000C08 RID: 3080
		Normal,
		// Token: 0x04000C09 RID: 3081
		Toggle
	}

	// Token: 0x02000177 RID: 375
	public enum SITouchscreenButtonType
	{
		// Token: 0x04000C0B RID: 3083
		Back,
		// Token: 0x04000C0C RID: 3084
		Next,
		// Token: 0x04000C0D RID: 3085
		Exit,
		// Token: 0x04000C0E RID: 3086
		Help,
		// Token: 0x04000C0F RID: 3087
		Select,
		// Token: 0x04000C10 RID: 3088
		Dispense,
		// Token: 0x04000C11 RID: 3089
		Research,
		// Token: 0x04000C12 RID: 3090
		Collect,
		// Token: 0x04000C13 RID: 3091
		Debug,
		// Token: 0x04000C14 RID: 3092
		PageSelect,
		// Token: 0x04000C15 RID: 3093
		Purchase,
		// Token: 0x04000C16 RID: 3094
		Confirm,
		// Token: 0x04000C17 RID: 3095
		Cancel,
		// Token: 0x04000C18 RID: 3096
		OverrideFailure,
		// Token: 0x04000C19 RID: 3097
		None,
		// Token: 0x04000C1A RID: 3098
		Subscribe
	}
}
