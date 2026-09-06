using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000A28 RID: 2600
public class GorillaHeldItemPressableButton : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x14000082 RID: 130
	// (add) Token: 0x0600428E RID: 17038 RVA: 0x00162A9C File Offset: 0x00160C9C
	// (remove) Token: 0x0600428F RID: 17039 RVA: 0x00162AD4 File Offset: 0x00160CD4
	public event Action<GorillaHeldItemPressableButton, TransferrableObject, bool> onPressed;

	// Token: 0x14000083 RID: 131
	// (add) Token: 0x06004290 RID: 17040 RVA: 0x00162B0C File Offset: 0x00160D0C
	// (remove) Token: 0x06004291 RID: 17041 RVA: 0x00162B44 File Offset: 0x00160D44
	public event Action<GorillaHeldItemPressableButton, TransferrableObject, bool> onReleased;

	// Token: 0x06004292 RID: 17042 RVA: 0x00162B7C File Offset: 0x00160D7C
	private void Start()
	{
		if (this.acceptAnyHoldableThatMatchesType)
		{
			this.acceptedTypes = new List<Type>();
			foreach (TransferrableObject transferrableObject in this.acceptedHoldables)
			{
				this.acceptedTypes.Add(transferrableObject.GetType());
			}
		}
	}

	// Token: 0x06004293 RID: 17043 RVA: 0x00162BEC File Offset: 0x00160DEC
	protected void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.delayBetweenSuccessfulPresses >= Time.time)
		{
			return;
		}
		TransferrableObject transferrableObject = collider.GetComponentInParent<TransferrableObject>();
		if (transferrableObject == null)
		{
			transferrableObject = collider.transform.parent.GetComponentInParent<TransferrableObject>();
		}
		if (transferrableObject == null || !transferrableObject.InHand())
		{
			return;
		}
		if (this.acceptAnyHoldableThatMatchesType)
		{
			if (!this.acceptedTypes.Contains(transferrableObject.GetType()))
			{
				return;
			}
		}
		else if (!this.acceptedHoldables.Contains(transferrableObject))
		{
			return;
		}
		this.touchTime = Time.time;
		switch (this.mode)
		{
		case HeldItemButtonMode.OneShot:
		{
			UnityEvent<TransferrableObject> unityEvent = this.onPressButton;
			if (unityEvent != null)
			{
				unityEvent.Invoke(transferrableObject);
			}
			Action<GorillaHeldItemPressableButton, TransferrableObject, bool> action = this.onPressed;
			if (action != null)
			{
				action(this, transferrableObject, transferrableObject.InLeftHand());
			}
			this.ButtonActivation(transferrableObject);
			this.ButtonActivationWithHand(transferrableObject, transferrableObject.InLeftHand());
			break;
		}
		case HeldItemButtonMode.ResetAfterDelay:
		{
			this.isOn = true;
			UnityEvent<TransferrableObject> unityEvent2 = this.onPressButton;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(transferrableObject);
			}
			Action<GorillaHeldItemPressableButton, TransferrableObject, bool> action2 = this.onPressed;
			if (action2 != null)
			{
				action2(this, transferrableObject, transferrableObject.InLeftHand());
			}
			this.ButtonActivation(transferrableObject);
			this.ButtonActivationWithHand(transferrableObject, transferrableObject.InLeftHand());
			GTDelayedExec.Add(this, this.delayBetweenSuccessfulPresses, 0);
			break;
		}
		case HeldItemButtonMode.Toggle:
			this.isOn = !this.isOn;
			if (this.isOn)
			{
				UnityEvent<TransferrableObject> unityEvent3 = this.onPressButton;
				if (unityEvent3 != null)
				{
					unityEvent3.Invoke(transferrableObject);
				}
				Action<GorillaHeldItemPressableButton, TransferrableObject, bool> action3 = this.onPressed;
				if (action3 != null)
				{
					action3(this, transferrableObject, transferrableObject.InLeftHand());
				}
				this.ButtonActivation(transferrableObject);
				this.ButtonActivationWithHand(transferrableObject, transferrableObject.InLeftHand());
			}
			else
			{
				UnityEvent<TransferrableObject> unityEvent4 = this.onReleaseButton;
				if (unityEvent4 != null)
				{
					unityEvent4.Invoke(transferrableObject);
				}
				Action<GorillaHeldItemPressableButton, TransferrableObject, bool> action4 = this.onReleased;
				if (action4 != null)
				{
					action4(this, transferrableObject, transferrableObject.InLeftHand());
				}
			}
			break;
		}
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, transferrableObject.InLeftHand(), 0.05f);
		GorillaTagger.Instance.StartVibration(transferrableObject.InLeftHand(), GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
			{
				67,
				transferrableObject.InLeftHand(),
				0.05f
			});
		}
		switch (this.consumeItem)
		{
		case HeldItemButtonConsumeMode.None:
			break;
		case HeldItemButtonConsumeMode.Destroy:
			transferrableObject.OnMyCreatorLeft();
			return;
		case HeldItemButtonConsumeMode.Disable:
			transferrableObject.gameObject.SetActive(false);
			break;
		default:
			return;
		}
	}

	// Token: 0x06004294 RID: 17044 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void ButtonActivation(TransferrableObject holdable)
	{
	}

	// Token: 0x06004295 RID: 17045 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void ButtonActivationWithHand(TransferrableObject holdable, bool isLeftHand)
	{
	}

	// Token: 0x06004296 RID: 17046 RVA: 0x00162E92 File Offset: 0x00161092
	public virtual void ResetState()
	{
		this.isOn = false;
		UnityEvent<TransferrableObject> unityEvent = this.onReleaseButton;
		if (unityEvent != null)
		{
			unityEvent.Invoke(null);
		}
		Action<GorillaHeldItemPressableButton, TransferrableObject, bool> action = this.onReleased;
		if (action == null)
		{
			return;
		}
		action(this, null, false);
	}

	// Token: 0x06004297 RID: 17047 RVA: 0x00162EC0 File Offset: 0x001610C0
	public void OnDelayedAction(int contextIndex)
	{
		this.ResetState();
	}

	// Token: 0x04005451 RID: 21585
	public int pressButtonSoundIndex = 67;

	// Token: 0x04005452 RID: 21586
	public bool isOn;

	// Token: 0x04005453 RID: 21587
	public float delayBetweenSuccessfulPresses = 0.25f;

	// Token: 0x04005454 RID: 21588
	private float touchTime;

	// Token: 0x04005455 RID: 21589
	public HeldItemButtonMode mode;

	// Token: 0x04005456 RID: 21590
	public List<TransferrableObject> acceptedHoldables;

	// Token: 0x04005457 RID: 21591
	private List<Type> acceptedTypes;

	// Token: 0x04005458 RID: 21592
	public bool acceptAnyHoldableThatMatchesType = true;

	// Token: 0x04005459 RID: 21593
	public HeldItemButtonConsumeMode consumeItem;

	// Token: 0x0400545A RID: 21594
	[Space]
	public UnityEvent<TransferrableObject> onPressButton;

	// Token: 0x0400545C RID: 21596
	[Space]
	public UnityEvent<TransferrableObject> onReleaseButton;
}
