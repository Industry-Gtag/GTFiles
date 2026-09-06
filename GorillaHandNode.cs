using System;
using UnityEngine;

// Token: 0x02000878 RID: 2168
public class GorillaHandNode : MonoBehaviour
{
	// Token: 0x17000504 RID: 1284
	// (get) Token: 0x0600387A RID: 14458 RVA: 0x00133F84 File Offset: 0x00132184
	public bool isGripping
	{
		get
		{
			return this.PollGrip();
		}
	}

	// Token: 0x17000505 RID: 1285
	// (get) Token: 0x0600387B RID: 14459 RVA: 0x00133F8C File Offset: 0x0013218C
	public bool isLeftHand
	{
		get
		{
			return this._isLeftHand;
		}
	}

	// Token: 0x17000506 RID: 1286
	// (get) Token: 0x0600387C RID: 14460 RVA: 0x00133F94 File Offset: 0x00132194
	public bool isRightHand
	{
		get
		{
			return this._isRightHand;
		}
	}

	// Token: 0x0600387D RID: 14461 RVA: 0x00133F9C File Offset: 0x0013219C
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x0600387E RID: 14462 RVA: 0x00133FA4 File Offset: 0x001321A4
	private bool PollGrip()
	{
		if (this.rig == null)
		{
			return false;
		}
		bool flag = this.PollThumb() >= 0.25f;
		bool flag2 = this.PollIndex() >= 0.25f;
		bool flag3 = this.PollMiddle() >= 0.25f;
		return flag && flag2 && flag3;
	}

	// Token: 0x0600387F RID: 14463 RVA: 0x00133FF8 File Offset: 0x001321F8
	private void Setup()
	{
		if (this.rig == null)
		{
			this.rig = base.GetComponentInParent<VRRig>();
		}
		if (this.rigidbody == null)
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
		}
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		if (this.rig)
		{
			this.vrIndex = (this._isLeftHand ? this.rig.leftIndex : this.rig.rightIndex);
			this.vrThumb = (this._isLeftHand ? this.rig.leftThumb : this.rig.rightThumb);
			this.vrMiddle = (this._isLeftHand ? this.rig.leftMiddle : this.rig.rightMiddle);
		}
		this._isLeftHand = base.name.Contains("left", StringComparison.OrdinalIgnoreCase);
		this._isRightHand = base.name.Contains("right", StringComparison.OrdinalIgnoreCase);
		int num = 0;
		num |= 1024;
		num |= 2097152;
		num |= 16777216;
		base.gameObject.SetTag(this._isLeftHand ? UnityTag.GorillaHandLeft : UnityTag.GorillaHandRight);
		base.gameObject.SetLayer(UnityLayer.GorillaHand);
		this.rigidbody.includeLayers = num;
		this.rigidbody.excludeLayers = ~num;
		this.rigidbody.isKinematic = true;
		this.rigidbody.useGravity = false;
		this.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		this.collider.isTrigger = true;
		this.collider.includeLayers = num;
		this.collider.excludeLayers = ~num;
	}

	// Token: 0x06003880 RID: 14464 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnTriggerStay(Collider other)
	{
	}

	// Token: 0x06003881 RID: 14465 RVA: 0x001341B7 File Offset: 0x001323B7
	private float PollIndex()
	{
		return Mathf.Clamp01(this.vrIndex.calcT / 0.88f);
	}

	// Token: 0x06003882 RID: 14466 RVA: 0x001341CF File Offset: 0x001323CF
	private float PollMiddle()
	{
		return this.vrIndex.calcT;
	}

	// Token: 0x06003883 RID: 14467 RVA: 0x001341CF File Offset: 0x001323CF
	private float PollThumb()
	{
		return this.vrIndex.calcT;
	}

	// Token: 0x0400487D RID: 18557
	public VRRig rig;

	// Token: 0x0400487E RID: 18558
	public Collider collider;

	// Token: 0x0400487F RID: 18559
	public Rigidbody rigidbody;

	// Token: 0x04004880 RID: 18560
	[Space]
	[NonSerialized]
	public VRMapIndex vrIndex;

	// Token: 0x04004881 RID: 18561
	[NonSerialized]
	public VRMapThumb vrThumb;

	// Token: 0x04004882 RID: 18562
	[NonSerialized]
	public VRMapMiddle vrMiddle;

	// Token: 0x04004883 RID: 18563
	[Space]
	public GorillaHandSocket attachedToSocket;

	// Token: 0x04004884 RID: 18564
	[Space]
	[SerializeField]
	private bool _isLeftHand;

	// Token: 0x04004885 RID: 18565
	[SerializeField]
	private bool _isRightHand;

	// Token: 0x04004886 RID: 18566
	public bool ignoreSockets;
}
