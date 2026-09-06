using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x02000516 RID: 1302
public class FriendshipCharm : HoldableObject
{
	// Token: 0x06002081 RID: 8321 RVA: 0x000AEB04 File Offset: 0x000ACD04
	private void Awake()
	{
		this.parent = base.transform.parent;
	}

	// Token: 0x06002082 RID: 8322 RVA: 0x000AEB18 File Offset: 0x000ACD18
	private void LateUpdate()
	{
		if (!this.isBroken && (this.lineStart.transform.position - this.lineEnd.transform.position).IsLongerThan(this.breakBraceletLength * GTPlayer.Instance.scale))
		{
			this.DestroyBracelet();
		}
	}

	// Token: 0x06002083 RID: 8323 RVA: 0x000AEB70 File Offset: 0x000ACD70
	public void OnEnable()
	{
		this.interactionPoint.enabled = true;
		this.meshRenderer.enabled = true;
		this.isBroken = false;
		this.UpdatePosition();
	}

	// Token: 0x06002084 RID: 8324 RVA: 0x000AEB97 File Offset: 0x000ACD97
	private void DestroyBracelet()
	{
		this.interactionPoint.enabled = false;
		this.isBroken = true;
		Debug.Log("LeaveGroup: bracelet destroyed");
		FriendshipGroupDetection.Instance.LeaveParty();
	}

	// Token: 0x06002085 RID: 8325 RVA: 0x000AEBC0 File Offset: 0x000ACDC0
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
		GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 2f);
		base.transform.SetParent(flag ? this.leftHandHoldAnchor : this.rightHandHoldAnchor);
		base.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06002086 RID: 8326 RVA: 0x000AEC48 File Offset: 0x000ACE48
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(null, flag);
		this.UpdatePosition();
		return base.OnRelease(zoneReleased, releasingHand);
	}

	// Token: 0x06002087 RID: 8327 RVA: 0x000AEC84 File Offset: 0x000ACE84
	private void UpdatePosition()
	{
		base.transform.SetParent(this.parent);
		base.transform.localPosition = this.releasePosition.localPosition;
		base.transform.localRotation = this.releasePosition.localRotation;
	}

	// Token: 0x06002088 RID: 8328 RVA: 0x000AECC4 File Offset: 0x000ACEC4
	private void OnCollisionEnter(Collision other)
	{
		if (!this.isBroken)
		{
			return;
		}
		if (this.breakItemLayerMask != (this.breakItemLayerMask | (1 << other.gameObject.layer)))
		{
			return;
		}
		this.meshRenderer.enabled = false;
		this.UpdatePosition();
	}

	// Token: 0x06002089 RID: 8329 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x0600208A RID: 8330 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void DropItemCleanup()
	{
	}

	// Token: 0x04002B55 RID: 11093
	[SerializeField]
	private InteractionPoint interactionPoint;

	// Token: 0x04002B56 RID: 11094
	[SerializeField]
	private Transform rightHandHoldAnchor;

	// Token: 0x04002B57 RID: 11095
	[SerializeField]
	private Transform leftHandHoldAnchor;

	// Token: 0x04002B58 RID: 11096
	[SerializeField]
	private MeshRenderer meshRenderer;

	// Token: 0x04002B59 RID: 11097
	[SerializeField]
	private Transform lineStart;

	// Token: 0x04002B5A RID: 11098
	[SerializeField]
	private Transform lineEnd;

	// Token: 0x04002B5B RID: 11099
	[SerializeField]
	private Transform releasePosition;

	// Token: 0x04002B5C RID: 11100
	[SerializeField]
	private float breakBraceletLength;

	// Token: 0x04002B5D RID: 11101
	[SerializeField]
	private LayerMask breakItemLayerMask;

	// Token: 0x04002B5E RID: 11102
	private Transform parent;

	// Token: 0x04002B5F RID: 11103
	private bool isBroken;
}
