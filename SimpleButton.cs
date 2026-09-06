using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000D17 RID: 3351
public class SimpleButton : MonoBehaviour, IClickable
{
	// Token: 0x06005320 RID: 21280 RVA: 0x001B6D48 File Offset: 0x001B4F48
	protected void OnTriggerEnter(Collider collider)
	{
		if (this.activator != null || Time.time - this.pressTime < this.coolDown)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator componentInParent = collider.gameObject.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (!componentInParent)
		{
			return;
		}
		this.activator = collider.gameObject;
		this.pressTime = Time.time;
		if (this.audioCLipIndex > 0)
		{
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.audioCLipIndex, componentInParent.isLeftHand, 0.05f);
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
		this.DoPress(componentInParent.isLeftHand);
	}

	// Token: 0x06005321 RID: 21281 RVA: 0x001B6E03 File Offset: 0x001B5003
	protected void OnTriggerExit(Collider collider)
	{
		if (this.activator == collider.gameObject)
		{
			this.activator = null;
			this.pressTime = Time.time;
			UnityEvent release = this.Release;
			if (release == null)
			{
				return;
			}
			release.Invoke();
		}
	}

	// Token: 0x06005322 RID: 21282 RVA: 0x001B6E3A File Offset: 0x001B503A
	private void DoPress(bool isLeft)
	{
		UnityEvent press = this.Press;
		if (press != null)
		{
			press.Invoke();
		}
		this.handlePress(isLeft);
	}

	// Token: 0x06005323 RID: 21283 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void handlePress(bool isLeft)
	{
	}

	// Token: 0x06005324 RID: 21284 RVA: 0x001B6E54 File Offset: 0x001B5054
	public void Click(bool leftHand = false)
	{
		this.DoPress(false);
	}

	// Token: 0x040064C0 RID: 25792
	protected GameObject activator;

	// Token: 0x040064C1 RID: 25793
	private float pressTime;

	// Token: 0x040064C2 RID: 25794
	[SerializeField]
	private float coolDown = 0.1f;

	// Token: 0x040064C3 RID: 25795
	[SerializeField]
	private int audioCLipIndex = 67;

	// Token: 0x040064C4 RID: 25796
	[SerializeField]
	private UnityEvent Press;

	// Token: 0x040064C5 RID: 25797
	[SerializeField]
	private UnityEvent Release;
}
