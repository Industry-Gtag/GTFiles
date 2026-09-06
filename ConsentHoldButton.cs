using System;
using UnityEngine;

// Token: 0x02000C15 RID: 3093
public class ConsentHoldButton : MonoBehaviour
{
	// Token: 0x14000088 RID: 136
	// (add) Token: 0x06004D65 RID: 19813 RVA: 0x0019C8A0 File Offset: 0x0019AAA0
	// (remove) Token: 0x06004D66 RID: 19814 RVA: 0x0019C8D8 File Offset: 0x0019AAD8
	public event Action HoldComplete;

	// Token: 0x06004D67 RID: 19815 RVA: 0x0019C910 File Offset: 0x0019AB10
	public void ResetHold()
	{
		this.pressing = false;
		this.elapsed = 0f;
		this.pressingCollider = null;
		if (this.fill != null)
		{
			this.fill.localScale = new Vector3(0f, 1f, 1f);
		}
	}

	// Token: 0x06004D68 RID: 19816 RVA: 0x0019C963 File Offset: 0x0019AB63
	private void OnDisable()
	{
		this.ResetHold();
	}

	// Token: 0x06004D69 RID: 19817 RVA: 0x0019C96C File Offset: 0x0019AB6C
	private void OnTriggerEnter(Collider other)
	{
		if (this.pressing || Time.time < this.cooldownUntil)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent == null)
		{
			return;
		}
		if ((componentInParent.isLeftHand && !this.leftHandPressable) || (!componentInParent.isLeftHand && !this.rightHandPressable))
		{
			return;
		}
		this.pressing = true;
		this.elapsed = 0f;
		this.pressingCollider = other;
		this.pressingHandIsLeft = componentInParent.isLeftHand;
		if (GorillaTagger.Instance != null)
		{
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength * this.feedback.pressHapticScale, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x06004D6A RID: 19818 RVA: 0x0019CA23 File Offset: 0x0019AC23
	private void OnTriggerExit(Collider other)
	{
		if (this.pressing && other == this.pressingCollider)
		{
			this.ResetHold();
		}
	}

	// Token: 0x06004D6B RID: 19819 RVA: 0x0019CA44 File Offset: 0x0019AC44
	private void Update()
	{
		if (!this.pressing)
		{
			return;
		}
		if (this.pressingCollider == null || !this.pressingCollider.gameObject.activeInHierarchy)
		{
			this.ResetHold();
			return;
		}
		this.elapsed += Time.deltaTime;
		float num = ((this.holdDurationSeconds > 0f) ? Mathf.Clamp01(this.elapsed / this.holdDurationSeconds) : 1f);
		if (this.fill != null)
		{
			this.fill.localScale = new Vector3(num, 1f, 1f);
		}
		if (GorillaTagger.Instance != null)
		{
			GorillaTagger.Instance.StartVibration(this.pressingHandIsLeft, GorillaTagger.Instance.tapHapticStrength * this.feedback.holdPulseHapticScale, Time.fixedDeltaTime);
		}
		if (num >= 1f)
		{
			bool flag = this.pressingHandIsLeft;
			this.ResetHold();
			this.cooldownUntil = Time.time + this.feedback.retriggerCooldownSeconds;
			if (GorillaTagger.Instance != null)
			{
				GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.feedback.completeSoundIndex, flag, this.feedback.completeSoundVolume);
				GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength * this.feedback.completeHapticScale, GorillaTagger.Instance.tapHapticDuration);
			}
			Action holdComplete = this.HoldComplete;
			if (holdComplete == null)
			{
				return;
			}
			holdComplete();
		}
	}

	// Token: 0x040060BD RID: 24765
	[Tooltip("Seconds the button must be held before it triggers.")]
	[SerializeField]
	private float holdDurationSeconds = 1.5f;

	// Token: 0x040060BE RID: 24766
	[SerializeField]
	private bool leftHandPressable;

	// Token: 0x040060BF RID: 24767
	[SerializeField]
	private bool rightHandPressable = true;

	// Token: 0x040060C0 RID: 24768
	[Tooltip("Stretched child image scaled on X from 0 to 1 while held (pivot must be on the left edge).")]
	[SerializeField]
	private RectTransform fill;

	// Token: 0x040060C1 RID: 24769
	[SerializeField]
	private ConsentHoldButton.Feedback feedback = ConsentHoldButton.Feedback.Default;

	// Token: 0x040060C3 RID: 24771
	private bool pressing;

	// Token: 0x040060C4 RID: 24772
	private float elapsed;

	// Token: 0x040060C5 RID: 24773
	private Collider pressingCollider;

	// Token: 0x040060C6 RID: 24774
	private bool pressingHandIsLeft;

	// Token: 0x040060C7 RID: 24775
	private float cooldownUntil;

	// Token: 0x02000C16 RID: 3094
	[Serializable]
	public struct Feedback
	{
		// Token: 0x1700076B RID: 1899
		// (get) Token: 0x06004D6D RID: 19821 RVA: 0x0019CBE0 File Offset: 0x0019ADE0
		public static ConsentHoldButton.Feedback Default
		{
			get
			{
				return new ConsentHoldButton.Feedback
				{
					completeSoundIndex = 67,
					completeSoundVolume = 0.1f,
					pressHapticScale = 0.5f,
					holdPulseHapticScale = 0.25f,
					completeHapticScale = 1f,
					retriggerCooldownSeconds = 0.25f
				};
			}
		}

		// Token: 0x040060C8 RID: 24776
		[Tooltip("Hand-tap sound index passed to VRRig.PlayHandTapLocal when the hold completes.")]
		public int completeSoundIndex;

		// Token: 0x040060C9 RID: 24777
		public float completeSoundVolume;

		// Token: 0x040060CA RID: 24778
		[Tooltip("Haptic strength while touching the button, as a multiplier on GorillaTagger.tapHapticStrength.")]
		public float pressHapticScale;

		// Token: 0x040060CB RID: 24779
		[Tooltip("Haptic strength of the per-frame pulse while holding, as a multiplier on GorillaTagger.tapHapticStrength.")]
		public float holdPulseHapticScale;

		// Token: 0x040060CC RID: 24780
		[Tooltip("Haptic strength when the hold completes, as a multiplier on GorillaTagger.tapHapticStrength.")]
		public float completeHapticScale;

		// Token: 0x040060CD RID: 24781
		[Tooltip("Seconds after a completed hold before the button can be pressed again.")]
		public float retriggerCooldownSeconds;
	}
}
