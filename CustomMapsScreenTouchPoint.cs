using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaTag;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using UnityEngine;

// Token: 0x02000AC3 RID: 2755
public abstract class CustomMapsScreenTouchPoint : MonoBehaviour, IClickable
{
	// Token: 0x0600469F RID: 18079 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void Awake()
	{
	}

	// Token: 0x060046A0 RID: 18080 RVA: 0x0017D154 File Offset: 0x0017B354
	protected virtual void OnDisable()
	{
		if (this.colorUpdateCoroutine != null)
		{
			base.StopCoroutine(this.colorUpdateCoroutine);
		}
		if (this.buttonColorSettings != null)
		{
			this.touchPointRenderer.color = this.buttonColorSettings.UnpressedColor;
		}
	}

	// Token: 0x060046A1 RID: 18081 RVA: 0x0017D190 File Offset: 0x0017B390
	private void OnTriggerEnter(Collider collider)
	{
		GTDev.Log<string>(string.Format("trigger {0} pressTime={1} time={2}", base.gameObject.name, CustomMapsScreenTouchPoint.pressTime, Time.time), null);
		if (Time.time < CustomMapsScreenTouchPoint.pressTime + CustomMapsScreenTouchPoint.pressedTime)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
		{
			Vector3 vector = this.GetForwardDirection();
			if (Vector3.Dot((collider.transform.position - base.transform.position).normalized, vector) < 0f)
			{
				return;
			}
			GTDev.Log<string>(string.Format("trigger {0} collider {1} postion {2}", base.gameObject.name, collider.gameObject.name, collider.transform.position), null);
			GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
			CustomMapsScreenTouchPoint.pressTime = Time.time;
			this.OnButtonPressedEvent();
			this.PressButtonColourUpdate();
			if (this.screen != null)
			{
				this.screen.PressButton(this.keyBinding);
			}
			if (component != null)
			{
				GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			}
		}
	}

	// Token: 0x060046A2 RID: 18082 RVA: 0x0017D2CD File Offset: 0x0017B4CD
	public virtual void PressButtonColourUpdate()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.touchPointRenderer.color = this.buttonColorSettings.PressedColor;
		this.colorUpdateCoroutine = base.StartCoroutine(this.<PressButtonColourUpdate>g__ButtonColorUpdate_Local|12_0());
	}

	// Token: 0x060046A3 RID: 18083 RVA: 0x0017D308 File Offset: 0x0017B508
	private Vector3 GetForwardDirection()
	{
		switch (this.forwardDirection)
		{
		case CustomMapsScreenTouchPoint.TouchPointDirections.Forward:
			return base.transform.forward;
		case CustomMapsScreenTouchPoint.TouchPointDirections.Backward:
			return -base.transform.forward;
		case CustomMapsScreenTouchPoint.TouchPointDirections.Left:
			return -base.transform.right;
		case CustomMapsScreenTouchPoint.TouchPointDirections.Right:
			return base.transform.right;
		case CustomMapsScreenTouchPoint.TouchPointDirections.Up:
			return base.transform.up;
		case CustomMapsScreenTouchPoint.TouchPointDirections.Down:
			return -base.transform.up;
		default:
			return base.transform.forward;
		}
	}

	// Token: 0x060046A4 RID: 18084
	protected abstract void OnButtonPressedEvent();

	// Token: 0x060046A5 RID: 18085 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void Click(bool leftHand = false)
	{
	}

	// Token: 0x060046A8 RID: 18088 RVA: 0x0017D3AA File Offset: 0x0017B5AA
	[CompilerGenerated]
	private IEnumerator <PressButtonColourUpdate>g__ButtonColorUpdate_Local|12_0()
	{
		yield return new WaitForSeconds(CustomMapsScreenTouchPoint.pressedTime);
		if (CustomMapsScreenTouchPoint.pressTime != 0f && Time.time > CustomMapsScreenTouchPoint.pressedTime + CustomMapsScreenTouchPoint.pressTime)
		{
			this.touchPointRenderer.color = this.buttonColorSettings.UnpressedColor;
			CustomMapsScreenTouchPoint.pressTime = 0f;
		}
		yield break;
	}

	// Token: 0x04005929 RID: 22825
	[SerializeField]
	private CustomMapsTerminalScreen screen;

	// Token: 0x0400592A RID: 22826
	[SerializeField]
	private CustomMapKeyboardBinding keyBinding;

	// Token: 0x0400592B RID: 22827
	[SerializeField]
	private CustomMapsScreenTouchPoint.TouchPointDirections forwardDirection;

	// Token: 0x0400592C RID: 22828
	[SerializeField]
	protected SpriteRenderer touchPointRenderer;

	// Token: 0x0400592D RID: 22829
	[SerializeField]
	protected ButtonColorSettings buttonColorSettings;

	// Token: 0x0400592E RID: 22830
	private static float pressedTime = 0.25f;

	// Token: 0x0400592F RID: 22831
	protected static float pressTime;

	// Token: 0x04005930 RID: 22832
	private Coroutine colorUpdateCoroutine;

	// Token: 0x02000AC4 RID: 2756
	public enum TouchPointDirections
	{
		// Token: 0x04005932 RID: 22834
		Forward,
		// Token: 0x04005933 RID: 22835
		Backward,
		// Token: 0x04005934 RID: 22836
		Left,
		// Token: 0x04005935 RID: 22837
		Right,
		// Token: 0x04005936 RID: 22838
		Up,
		// Token: 0x04005937 RID: 22839
		Down
	}
}
