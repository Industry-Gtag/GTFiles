using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000A2C RID: 2604
public class GorillaPhysicalButton : MonoBehaviour
{
	// Token: 0x14000084 RID: 132
	// (add) Token: 0x060042AC RID: 17068 RVA: 0x00163228 File Offset: 0x00161428
	// (remove) Token: 0x060042AD RID: 17069 RVA: 0x00163260 File Offset: 0x00161460
	public event Action<GorillaPhysicalButton, bool> onPressedOn;

	// Token: 0x14000085 RID: 133
	// (add) Token: 0x060042AE RID: 17070 RVA: 0x00163298 File Offset: 0x00161498
	// (remove) Token: 0x060042AF RID: 17071 RVA: 0x001632D0 File Offset: 0x001614D0
	public event Action<GorillaPhysicalButton, bool> onToggledOff;

	// Token: 0x060042B0 RID: 17072 RVA: 0x00163308 File Offset: 0x00161508
	public virtual void Start()
	{
		if (this.moveableChildren != null)
		{
			this.moveableChildrenStartPositions = new List<Vector3>(this.moveableChildren.Count);
			for (int i = 0; i < this.moveableChildren.Count; i++)
			{
				this.moveableChildrenStartPositions.Add(this.moveableChildren[i].position);
			}
		}
		this.startButtonPosition = base.transform.position;
		base.enabled = true;
	}

	// Token: 0x060042B1 RID: 17073 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnEnable()
	{
	}

	// Token: 0x060042B2 RID: 17074 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnDisable()
	{
	}

	// Token: 0x060042B3 RID: 17075 RVA: 0x00163380 File Offset: 0x00161580
	private float GetSurfaceDistanceFromKeyToCollider(Collider collider)
	{
		if (collider == null)
		{
			return 1f;
		}
		SphereCollider sphereCollider = collider as SphereCollider;
		float num = (sphereCollider ? sphereCollider.radius : 0f);
		float num2 = base.transform.localScale.z * 0.5f;
		if (Vector3.Distance(collider.transform.position, base.transform.position) > (base.transform.localScale.magnitude * 0.5f + num) * 1.5f)
		{
			return 1f;
		}
		return Vector3.Dot(base.transform.position - collider.transform.position, -base.transform.forward) - num - num2;
	}

	// Token: 0x060042B4 RID: 17076 RVA: 0x00163448 File Offset: 0x00161648
	protected void OnTriggerEnter(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		if (other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		this.recentFingerCollider = other;
		if (this.buttonTestCoroutine == null)
		{
			this.buttonTestCoroutine = base.StartCoroutine(this.ButtonUpdate());
		}
	}

	// Token: 0x060042B5 RID: 17077 RVA: 0x00163483 File Offset: 0x00161683
	protected IEnumerator ButtonUpdate()
	{
		for (;;)
		{
			this.UpdateButtonFromCollider();
			if (!base.enabled || this.recentFingerCollider == null)
			{
				break;
			}
			yield return null;
		}
		this.buttonTestCoroutine = null;
		yield break;
	}

	// Token: 0x060042B6 RID: 17078 RVA: 0x00163494 File Offset: 0x00161694
	protected void UpdateButtonFromCollider()
	{
		if (this.recentFingerCollider != null)
		{
			float surfaceDistanceFromKeyToCollider = this.GetSurfaceDistanceFromKeyToCollider(this.recentFingerCollider);
			this.currentButtonDepthFromPressing -= surfaceDistanceFromKeyToCollider;
			this.currentButtonDepthFromPressing = Mathf.Clamp(this.currentButtonDepthFromPressing, 0f, this.buttonPushDepth);
		}
		else
		{
			this.currentButtonDepthFromPressing = 0f;
		}
		if (this.currentButtonDepthFromPressing == 0f)
		{
			if (!this.canToggleOn && !this.canToggleOff)
			{
				this.isOn = false;
			}
			this.recentFingerCollider = null;
			this.waitingForReleaseAfterStateChange = false;
		}
		this.TestForButtonStateChange();
		this.UpdateButtonVisuals();
	}

	// Token: 0x060042B7 RID: 17079 RVA: 0x00163534 File Offset: 0x00161734
	protected void TestForButtonStateChange()
	{
		if (this.waitingForReleaseAfterStateChange)
		{
			return;
		}
		if (this.currentButtonDepthFromPressing > this.buttonDepthForTrigger && !this.isOn && this.recentFingerCollider != null)
		{
			this.isOn = true;
			this.waitingForReleaseAfterStateChange = true;
			GorillaTriggerColliderHandIndicator component = this.recentFingerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (component == null)
			{
				return;
			}
			UnityEvent unityEvent = this.onPressButtonOn;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			Action<GorillaPhysicalButton, bool> action = this.onPressedOn;
			if (action != null)
			{
				action(this, component.isLeftHand);
			}
			this.ButtonPressedOn();
			this.ButtonPressedOnWithHand(component.isLeftHand);
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, component.isLeftHand, 0.05f);
			GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { 67, component.isLeftHand, 0.05f });
				return;
			}
		}
		else if (this.currentButtonDepthFromPressing > this.buttonDepthForTrigger && this.canToggleOff && this.isOn && this.recentFingerCollider != null)
		{
			this.isOn = false;
			this.waitingForReleaseAfterStateChange = true;
			GorillaTriggerColliderHandIndicator component2 = this.recentFingerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (component2 == null)
			{
				return;
			}
			UnityEvent unityEvent2 = this.onPressButtonToggleOff;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke();
			}
			Action<GorillaPhysicalButton, bool> action2 = this.onToggledOff;
			if (action2 != null)
			{
				action2(this, component2.isLeftHand);
			}
			this.ButtonToggledOff();
			this.ButtonToggledOffWithHand(component2.isLeftHand);
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, component2.isLeftHand, 0.05f);
			GorillaTagger.Instance.StartVibration(component2.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { 67, component2.isLeftHand, 0.05f });
			}
		}
	}

	// Token: 0x060042B8 RID: 17080 RVA: 0x001637C4 File Offset: 0x001619C4
	protected void UpdateButtonVisuals()
	{
		float num = this.currentButtonDepthFromPressing;
		if ((this.canToggleOff || this.canToggleOn) && this.isOn)
		{
			num = Mathf.Max(this.buttonDepthForTrigger, num);
		}
		base.transform.position = this.startButtonPosition - base.transform.forward * num;
		if (this.moveableChildren != null)
		{
			for (int i = 0; i < this.moveableChildren.Count; i++)
			{
				this.moveableChildren[i].position = this.moveableChildrenStartPositions[i] - base.transform.forward * num;
			}
		}
		this.UpdateColorWithState(this.isOn);
	}

	// Token: 0x060042B9 RID: 17081 RVA: 0x00163884 File Offset: 0x00161A84
	protected void UpdateColorWithState(bool state)
	{
		if (state)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			if ((!string.IsNullOrEmpty(this.onText) || !string.IsNullOrEmpty(this.offText)) && this.textField != null)
			{
				this.textField.text = this.onText;
				return;
			}
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if ((!string.IsNullOrEmpty(this.offText) || !string.IsNullOrEmpty(this.onText)) && this.textField != null)
			{
				this.textField.text = this.offText;
			}
		}
	}

	// Token: 0x060042BA RID: 17082 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void ButtonPressedOn()
	{
	}

	// Token: 0x060042BB RID: 17083 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void ButtonPressedOnWithHand(bool isLeftHand)
	{
	}

	// Token: 0x060042BC RID: 17084 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void ButtonToggledOff()
	{
	}

	// Token: 0x060042BD RID: 17085 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void ButtonToggledOffWithHand(bool isLeftHand)
	{
	}

	// Token: 0x060042BE RID: 17086 RVA: 0x00163929 File Offset: 0x00161B29
	public virtual void ResetState()
	{
		this.isOn = false;
		this.currentButtonDepthFromPressing = 0f;
		this.waitingForReleaseAfterStateChange = false;
		this.UpdateButtonVisuals();
	}

	// Token: 0x060042BF RID: 17087 RVA: 0x0016394A File Offset: 0x00161B4A
	public void SetText(string newText)
	{
		if (this.textField != null)
		{
			this.textField.text = this.offText;
		}
	}

	// Token: 0x060042C0 RID: 17088 RVA: 0x0016396C File Offset: 0x00161B6C
	public virtual void SetButtonState(bool setToOn)
	{
		if (this.canToggleOn || this.canToggleOff)
		{
			if (this.isOn != setToOn)
			{
				this.isOn = setToOn;
				if (this.isOn)
				{
					UnityEvent unityEvent = this.onPressButtonOn;
					if (unityEvent != null)
					{
						unityEvent.Invoke();
					}
					this.ButtonPressedOn();
				}
				else
				{
					UnityEvent unityEvent2 = this.onPressButtonToggleOff;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke();
					}
					this.ButtonToggledOff();
				}
			}
			this.UpdateButtonVisuals();
		}
	}

	// Token: 0x0400546E RID: 21614
	public Material pressedMaterial;

	// Token: 0x0400546F RID: 21615
	public Material unpressedMaterial;

	// Token: 0x04005470 RID: 21616
	public MeshRenderer buttonRenderer;

	// Token: 0x04005471 RID: 21617
	public int pressButtonSoundIndex = 67;

	// Token: 0x04005472 RID: 21618
	[SerializeField]
	public bool canToggleOn;

	// Token: 0x04005473 RID: 21619
	public bool canToggleOff;

	// Token: 0x04005474 RID: 21620
	private bool waitingForReleaseAfterStateChange;

	// Token: 0x04005475 RID: 21621
	public bool isOn;

	// Token: 0x04005476 RID: 21622
	public bool testPress;

	// Token: 0x04005477 RID: 21623
	public bool testHandLeft;

	// Token: 0x04005478 RID: 21624
	[SerializeField]
	protected float buttonPushDepth = 0.0125f;

	// Token: 0x04005479 RID: 21625
	[SerializeField]
	protected float buttonDepthForTrigger = 0.01f;

	// Token: 0x0400547A RID: 21626
	[SerializeField]
	public List<Transform> moveableChildren;

	// Token: 0x0400547B RID: 21627
	[NonSerialized]
	public List<Vector3> moveableChildrenStartPositions;

	// Token: 0x0400547C RID: 21628
	private Vector3 startButtonPosition;

	// Token: 0x0400547D RID: 21629
	[TextArea]
	public string offText = "OFF";

	// Token: 0x0400547E RID: 21630
	[TextArea]
	public string onText = "ON";

	// Token: 0x0400547F RID: 21631
	[SerializeField]
	public TMP_Text textField;

	// Token: 0x04005480 RID: 21632
	[Space]
	public UnityEvent onPressButtonOn;

	// Token: 0x04005481 RID: 21633
	public UnityEvent onPressButtonToggleOff;

	// Token: 0x04005484 RID: 21636
	private Collider recentFingerCollider;

	// Token: 0x04005485 RID: 21637
	protected float currentButtonDepthFromPressing;

	// Token: 0x04005486 RID: 21638
	private Coroutine buttonTestCoroutine;
}
