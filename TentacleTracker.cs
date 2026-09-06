using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;

// Token: 0x0200028D RID: 653
public class TentacleTracker : MonoBehaviour
{
	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x06001192 RID: 4498 RVA: 0x0005E78F File Offset: 0x0005C98F
	// (set) Token: 0x06001193 RID: 4499 RVA: 0x0005E797 File Offset: 0x0005C997
	public VRRig currentTargetRig { get; private set; }

	// Token: 0x06001194 RID: 4500 RVA: 0x0005E7A0 File Offset: 0x0005C9A0
	private void OnEnable()
	{
		this.tracking = true;
		this.testTriggersRemaining.Clear();
		this.testTriggersRemaining.AddRange(this.testTriggers);
		this.currentTargetRig = null;
		this.currentTargetIsLocal = false;
	}

	// Token: 0x06001195 RID: 4501 RVA: 0x0005E7D4 File Offset: 0x0005C9D4
	public void BeginGrab(VRRig targetRig, bool isLocalPlayer)
	{
		if (targetRig == null)
		{
			return;
		}
		base.gameObject.SetActive(true);
		this.tracking = true;
		this.currentTargetRig = targetRig;
		this.currentTargetIsLocal = isLocalPlayer;
		this.testTriggersRemaining.Clear();
		this.testTriggersRemaining.AddRange(this.testTriggers);
	}

	// Token: 0x06001196 RID: 4502 RVA: 0x0005E828 File Offset: 0x0005CA28
	public void Anim_OnReachEnded()
	{
		this.animator.SetTrigger(this.testTriggersRemaining[0]);
		this.testTriggersRemaining.RemoveAt(0);
		if (this.currentTargetIsLocal)
		{
			GTPlayer.Instance.BeginClimbing(this.climbable, EquipmentInteractor.instance.BodyClimber, null);
			EquipmentInteractor.instance.BodyClimber.SetCanRelease(false);
			this.tracking = false;
		}
	}

	// Token: 0x06001197 RID: 4503 RVA: 0x0005E898 File Offset: 0x0005CA98
	private void Update()
	{
		if (!this.tracking)
		{
			return;
		}
		Vector3 vector = ((this.currentTargetRig != null && !this.currentTargetIsLocal) ? this.currentTargetRig.head.rigTarget.position : GTPlayer.Instance.mainCamera.transform.position);
		Vector3 vector2 = vector - this.anchorPoint.position;
		float magnitude = vector2.magnitude;
		if (magnitude < 0.001f)
		{
			return;
		}
		Vector3 vector3 = vector2 / magnitude;
		Vector3 normalized = (this.playerRefPoint.localPosition - this.anchorRefPoint.localPosition).normalized;
		base.transform.rotation = Quaternion.LookRotation(vector3) * Quaternion.Inverse(Quaternion.LookRotation(normalized));
		base.transform.position += vector - this.playerRefPoint.position;
	}

	// Token: 0x06001198 RID: 4504 RVA: 0x0005E98C File Offset: 0x0005CB8C
	public void TestDrop()
	{
		if (this.currentTargetRig == null || this.currentTargetIsLocal)
		{
			EquipmentInteractor.instance.BodyClimber.SetCanRelease(true);
			GTPlayer.Instance.EndClimbing(EquipmentInteractor.instance.BodyClimber, false, false);
		}
		this.currentTargetRig = null;
		this.currentTargetIsLocal = false;
		this.tracking = true;
	}

	// Token: 0x06001199 RID: 4505 RVA: 0x00044B04 File Offset: 0x00042D04
	public void TestDisappear()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x040014F0 RID: 5360
	[SerializeField]
	private Transform anchorPoint;

	// Token: 0x040014F1 RID: 5361
	[SerializeField]
	private Transform anchorRefPoint;

	// Token: 0x040014F2 RID: 5362
	[SerializeField]
	private Transform playerRefPoint;

	// Token: 0x040014F3 RID: 5363
	[SerializeField]
	private Animator animator;

	// Token: 0x040014F4 RID: 5364
	[SerializeField]
	private GorillaClimbable climbable;

	// Token: 0x040014F5 RID: 5365
	[SerializeField]
	private string[] testTriggers;

	// Token: 0x040014F6 RID: 5366
	private List<string> testTriggersRemaining = new List<string>();

	// Token: 0x040014F7 RID: 5367
	private bool tracking = true;

	// Token: 0x040014F9 RID: 5369
	private bool currentTargetIsLocal;
}
