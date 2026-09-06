using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000900 RID: 2304
public class HeldButton : MonoBehaviour
{
	// Token: 0x06003C67 RID: 15463 RVA: 0x00149BDC File Offset: 0x00147DDC
	private void OnTriggerEnter(Collider other)
	{
		if (!base.enabled)
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
		if (!this.pendingPress || other != this.pendingPressCollider)
		{
			UnityEvent unityEvent = this.onStartPressingButton;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.touchTime = Time.time;
			this.pendingPressCollider = other;
			this.pressingHand = componentInParent;
			this.pendingPress = true;
			this.SetOn(true);
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x06003C68 RID: 15464 RVA: 0x00149C9C File Offset: 0x00147E9C
	private void LateUpdate()
	{
		if (!this.pendingPress)
		{
			return;
		}
		if (this.touchTime < this.releaseTime && this.releaseTime + this.debounceTime < Time.time)
		{
			UnityEvent unityEvent = this.onStopPressingButton;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.pendingPress = false;
			this.pendingPressCollider = null;
			this.pressingHand = null;
			this.SetOn(false);
			return;
		}
		if (this.touchTime + this.pressDuration < Time.time)
		{
			this.onPressButton.Invoke();
			if (this.pressingHand != null)
			{
				GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, this.pressingHand.isLeftHand, 0.1f);
				GorillaTagger.Instance.StartVibration(this.pressingHand.isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			UnityEvent unityEvent2 = this.onStopPressingButton;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke();
			}
			this.pendingPress = false;
			this.pendingPressCollider = null;
			this.pressingHand = null;
			this.releaseTime = Time.time;
			this.SetOn(false);
			return;
		}
		if (this.touchTime > this.releaseTime && this.pressingHand != null)
		{
			GorillaTagger.Instance.StartVibration(this.pressingHand.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 4f, Time.fixedDeltaTime);
		}
	}

	// Token: 0x06003C69 RID: 15465 RVA: 0x00149DFB File Offset: 0x00147FFB
	private void OnTriggerExit(Collider other)
	{
		if (this.pendingPress && this.pendingPressCollider == other)
		{
			this.releaseTime = Time.time;
			UnityEvent unityEvent = this.onStopPressingButton;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x06003C6A RID: 15466 RVA: 0x00149E30 File Offset: 0x00148030
	public void SetOn(bool inOn)
	{
		if (inOn == this.isOn)
		{
			return;
		}
		this.isOn = inOn;
		if (this.isOn)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.onText;
				return;
			}
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.offText;
			}
		}
	}

	// Token: 0x04004D17 RID: 19735
	public Material pressedMaterial;

	// Token: 0x04004D18 RID: 19736
	public Material unpressedMaterial;

	// Token: 0x04004D19 RID: 19737
	public MeshRenderer buttonRenderer;

	// Token: 0x04004D1A RID: 19738
	private bool isOn;

	// Token: 0x04004D1B RID: 19739
	public float debounceTime = 0.25f;

	// Token: 0x04004D1C RID: 19740
	public bool leftHandPressable;

	// Token: 0x04004D1D RID: 19741
	public bool rightHandPressable = true;

	// Token: 0x04004D1E RID: 19742
	public float pressDuration = 0.5f;

	// Token: 0x04004D1F RID: 19743
	public UnityEvent onStartPressingButton;

	// Token: 0x04004D20 RID: 19744
	public UnityEvent onStopPressingButton;

	// Token: 0x04004D21 RID: 19745
	public UnityEvent onPressButton;

	// Token: 0x04004D22 RID: 19746
	[TextArea]
	public string offText;

	// Token: 0x04004D23 RID: 19747
	[TextArea]
	public string onText;

	// Token: 0x04004D24 RID: 19748
	public Text myText;

	// Token: 0x04004D25 RID: 19749
	private float touchTime;

	// Token: 0x04004D26 RID: 19750
	private float releaseTime;

	// Token: 0x04004D27 RID: 19751
	private bool pendingPress;

	// Token: 0x04004D28 RID: 19752
	private Collider pendingPressCollider;

	// Token: 0x04004D29 RID: 19753
	private GorillaTriggerColliderHandIndicator pressingHand;
}
