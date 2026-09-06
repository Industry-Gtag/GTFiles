using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000A29 RID: 2601
public abstract class GorillaKeyButton<TBinding> : MonoBehaviour where TBinding : Enum
{
	// Token: 0x06004299 RID: 17049 RVA: 0x00162EEA File Offset: 0x001610EA
	private void Awake()
	{
		if (this.ButtonRenderer == null)
		{
			this.ButtonRenderer = base.GetComponent<Renderer>();
		}
		this.propBlock = new MaterialPropertyBlock();
		this.pressTime = 0f;
	}

	// Token: 0x0600429A RID: 17050 RVA: 0x00162F1C File Offset: 0x0016111C
	private void OnEnable()
	{
		for (int i = 0; i < this.linkedObjects.Length; i++)
		{
			if (this.linkedObjects[i].IsNotNull())
			{
				this.linkedObjects[i].SetActive(true);
			}
		}
		this.OnEnableEvents();
	}

	// Token: 0x0600429B RID: 17051 RVA: 0x00162F60 File Offset: 0x00161160
	private void OnDisable()
	{
		for (int i = 0; i < this.linkedObjects.Length; i++)
		{
			if (this.linkedObjects[i].IsNotNull())
			{
				this.linkedObjects[i].SetActive(false);
			}
		}
		this.OnDisableEvents();
	}

	// Token: 0x0600429C RID: 17052 RVA: 0x00162FA4 File Offset: 0x001611A4
	private void OnTriggerEnter(Collider collider)
	{
		GorillaTriggerColliderHandIndicator componentInParent = collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent)
		{
			this.PressButton(componentInParent.isLeftHand);
		}
	}

	// Token: 0x0600429D RID: 17053 RVA: 0x00162FCC File Offset: 0x001611CC
	private void PressButton(bool isLeftHand)
	{
		this.OnButtonPressedEvent();
		UnityEvent<TBinding> onKeyButtonPressed = this.OnKeyButtonPressed;
		if (onKeyButtonPressed != null)
		{
			onKeyButtonPressed.Invoke(this.Binding);
		}
		this.PressButtonColourUpdate();
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, isLeftHand, 0.1f);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { 66, isLeftHand, 0.1f });
		}
	}

	// Token: 0x0600429E RID: 17054 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnEnableEvents()
	{
	}

	// Token: 0x0600429F RID: 17055 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnDisableEvents()
	{
	}

	// Token: 0x060042A0 RID: 17056 RVA: 0x00163091 File Offset: 0x00161291
	public void Click(bool leftHand = false)
	{
		this.PressButton(leftHand);
	}

	// Token: 0x060042A1 RID: 17057 RVA: 0x0016309C File Offset: 0x0016129C
	public virtual void PressButtonColourUpdate()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.propBlock.SetColor(ShaderProps._BaseColor, this.ButtonColorSettings.PressedColor);
		this.propBlock.SetColor(ShaderProps._Color, this.ButtonColorSettings.PressedColor);
		this.ButtonRenderer.SetPropertyBlock(this.propBlock);
		this.pressTime = Time.time;
		base.StartCoroutine(this.<PressButtonColourUpdate>g__ButtonColorUpdate_Local|21_0());
	}

	// Token: 0x060042A2 RID: 17058
	protected abstract void OnButtonPressedEvent();

	// Token: 0x060042A4 RID: 17060 RVA: 0x00163134 File Offset: 0x00161334
	[CompilerGenerated]
	private IEnumerator <PressButtonColourUpdate>g__ButtonColorUpdate_Local|21_0()
	{
		yield return new WaitForSeconds(this.ButtonColorSettings.PressedTime);
		if (this.pressTime != 0f && Time.time > this.ButtonColorSettings.PressedTime + this.pressTime)
		{
			this.propBlock.SetColor(ShaderProps._BaseColor, this.ButtonColorSettings.UnpressedColor);
			this.propBlock.SetColor(ShaderProps._Color, this.ButtonColorSettings.UnpressedColor);
			this.ButtonRenderer.SetPropertyBlock(this.propBlock);
			this.pressTime = 0f;
		}
		yield break;
	}

	// Token: 0x0400545E RID: 21598
	public string characterString;

	// Token: 0x0400545F RID: 21599
	public TBinding Binding;

	// Token: 0x04005460 RID: 21600
	public bool functionKey;

	// Token: 0x04005461 RID: 21601
	public Renderer ButtonRenderer;

	// Token: 0x04005462 RID: 21602
	public ButtonColorSettings ButtonColorSettings;

	// Token: 0x04005463 RID: 21603
	[Tooltip("These GameObjects will be Activated/Deactivated when this button is Activated/Deactivated")]
	public GameObject[] linkedObjects;

	// Token: 0x04005464 RID: 21604
	[Tooltip("Intended for use with GorillaKeyWrapper")]
	public UnityEvent<TBinding> OnKeyButtonPressed = new UnityEvent<TBinding>();

	// Token: 0x04005465 RID: 21605
	public bool testClick;

	// Token: 0x04005466 RID: 21606
	public bool repeatTestClick;

	// Token: 0x04005467 RID: 21607
	public float repeatCooldown = 2f;

	// Token: 0x04005468 RID: 21608
	private float pressTime;

	// Token: 0x04005469 RID: 21609
	private float lastTestClick;

	// Token: 0x0400546A RID: 21610
	protected MaterialPropertyBlock propBlock;
}
