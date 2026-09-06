using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200055B RID: 1371
public class VRRigAnchorOverrides : MonoBehaviour
{
	// Token: 0x170003AC RID: 940
	// (get) Token: 0x060022E4 RID: 8932 RVA: 0x000BB521 File Offset: 0x000B9721
	// (set) Token: 0x060022E5 RID: 8933 RVA: 0x000BB52C File Offset: 0x000B972C
	[DebugOption]
	public Transform CurrentBadgeTransform
	{
		get
		{
			return this.currentBadgeTransform;
		}
		set
		{
			if (value != this.currentBadgeTransform)
			{
				this.ResetBadge();
				this.currentBadgeTransform = value;
				this.badgeDefaultRot = this.currentBadgeTransform.localRotation;
				this.badgeDefaultPos = this.currentBadgeTransform.localPosition;
				this.UpdateBadge();
			}
		}
	}

	// Token: 0x170003AD RID: 941
	// (get) Token: 0x060022E6 RID: 8934 RVA: 0x000BB57C File Offset: 0x000B977C
	public Transform HuntDefaultAnchor
	{
		get
		{
			return this.huntComputerDefaultAnchor;
		}
	}

	// Token: 0x170003AE RID: 942
	// (get) Token: 0x060022E7 RID: 8935 RVA: 0x000BB584 File Offset: 0x000B9784
	public Transform HuntComputer
	{
		get
		{
			return this.huntComputer;
		}
	}

	// Token: 0x170003AF RID: 943
	// (get) Token: 0x060022E8 RID: 8936 RVA: 0x000BB58C File Offset: 0x000B978C
	public Transform BuilderWatchAnchor
	{
		get
		{
			return this.builderResizeButtonDefaultAnchor;
		}
	}

	// Token: 0x170003B0 RID: 944
	// (get) Token: 0x060022E9 RID: 8937 RVA: 0x000BB594 File Offset: 0x000B9794
	public Transform BuilderWatch
	{
		get
		{
			return this.builderResizeButton;
		}
	}

	// Token: 0x060022EA RID: 8938 RVA: 0x000BB59C File Offset: 0x000B979C
	private void Awake()
	{
		for (int i = 0; i < 8; i++)
		{
			this.overrideAnchors[i] = null;
		}
		int num = this.MapPositionToIndex(TransferrableObject.PositionState.OnChest);
		this.overrideAnchors[num] = this.chestDefaultTransform;
		this.chestDefaultLocalPos = this.chestDefaultTransform.localPosition;
		this.huntDefaultTransform = this.huntComputer;
		this.builderResizeButtonDefaultTransform = this.builderResizeButton;
		this.activeAntiClippingOffsets = default(CosmeticAnchorAntiIntersectOffsets);
	}

	// Token: 0x060022EB RID: 8939 RVA: 0x000BB60A File Offset: 0x000B980A
	public void EnableChestBodyTracking(bool enabled)
	{
		this.chestDefaultTransform.localPosition = ((enabled && this.chestBodyTrackingOffset != null) ? this.chestBodyTrackingOffset.localPosition : this.chestDefaultLocalPos);
	}

	// Token: 0x060022EC RID: 8940 RVA: 0x000BB63C File Offset: 0x000B983C
	private void OnEnable()
	{
		if (this.nameDefaultAnchor && this.nameDefaultAnchor.parent)
		{
			this.nameTransform.parent = this.nameDefaultAnchor.parent;
		}
		else
		{
			Debug.LogError("VRRigAnchorOverrides: could not set parent `nameTransform` because `nameDefaultAnchor` or its parent was null!" + base.transform.GetPathQ(), this);
		}
		this.huntComputer = this.huntDefaultTransform;
		if (this.huntComputerDefaultAnchor && this.huntComputerDefaultAnchor.parent)
		{
			this.huntComputer.parent = this.huntComputerDefaultAnchor.parent;
		}
		else
		{
			Debug.LogError("VRRigAnchorOverrides: could not set parent `huntComputer` because `huntComputerDefaultAnchor` or its parent was null!" + base.transform.GetPathQ(), this);
		}
		this.builderResizeButton = this.builderResizeButtonDefaultTransform;
		if (this.builderResizeButtonDefaultAnchor && this.builderResizeButtonDefaultAnchor.parent)
		{
			this.builderResizeButton.parent = this.builderResizeButtonDefaultAnchor.parent;
			return;
		}
		Debug.LogError("VRRigAnchorOverrides: could not set parent `builderResizeButton` because `builderResizeButtonDefaultAnchor` or its parent was null! Path: " + base.transform.GetPathQ(), this);
	}

	// Token: 0x060022ED RID: 8941 RVA: 0x000BB758 File Offset: 0x000B9958
	private int MapPositionToIndex(TransferrableObject.PositionState pos)
	{
		int num = (int)pos;
		int num2 = 0;
		while ((num >>= 1) != 0)
		{
			num2++;
		}
		return num2;
	}

	// Token: 0x060022EE RID: 8942 RVA: 0x000BB778 File Offset: 0x000B9978
	public void ApplyAntiClippingOffsets(TransferrableObject.PositionState pos, XformOffset offset, bool enable, Transform defaultAnchor)
	{
		int num = this.MapPositionToIndex(pos);
		if (pos != TransferrableObject.PositionState.OnLeftArm)
		{
			if (pos != TransferrableObject.PositionState.OnRightArm)
			{
				if (pos != TransferrableObject.PositionState.OnChest)
				{
					GTDev.LogWarning<string>(string.Format("Anti Clipping offset for position {0} is not implemented", pos), null);
					return;
				}
				this.activeAntiClippingOffsets.chest.enabled = enable;
				this.activeAntiClippingOffsets.chest.offset = (enable ? offset : XformOffset.Identity);
			}
			else
			{
				this.activeAntiClippingOffsets.rightArm.enabled = enable;
				this.activeAntiClippingOffsets.rightArm.offset = (enable ? offset : XformOffset.Identity);
			}
		}
		else
		{
			this.activeAntiClippingOffsets.leftArm.enabled = enable;
			this.activeAntiClippingOffsets.leftArm.offset = (enable ? offset : XformOffset.Identity);
		}
		if (enable && (this.overrideAnchors[num] == null || (pos == TransferrableObject.PositionState.OnChest && this.overrideAnchors[num] == this.chestDefaultTransform)))
		{
			if (this.clippingOffsetTransforms[num] == null)
			{
				GameObject gameObject = new GameObject("Anti Clipping Offset");
				gameObject.transform.SetParent(defaultAnchor);
				this.clippingOffsetTransforms[num] = gameObject.transform;
			}
			Transform transform = this.clippingOffsetTransforms[num];
			transform.SetParent(defaultAnchor);
			transform.localPosition = offset.pos;
			transform.localRotation = offset.rot;
			transform.localScale = Vector3.one;
			this.OverrideAnchor(pos, transform);
			return;
		}
		if (!enable && this.overrideAnchors[num] == this.clippingOffsetTransforms[num])
		{
			if (pos == TransferrableObject.PositionState.OnChest)
			{
				this.OverrideAnchor(pos, this.chestDefaultTransform);
				return;
			}
			this.OverrideAnchor(pos, null);
		}
	}

	// Token: 0x060022EF RID: 8943 RVA: 0x000BB91C File Offset: 0x000B9B1C
	public void OverrideAnchor(TransferrableObject.PositionState pos, Transform anchor)
	{
		int num = this.MapPositionToIndex(pos);
		if (this.overrideAnchors[num] == this.chestDefaultTransform)
		{
			foreach (object obj in this.overrideAnchors[num])
			{
				Transform transform = (Transform)obj;
				if (!transform.name.Equals("DropZoneChest") && transform != anchor)
				{
					transform.parent = null;
				}
			}
			this.overrideAnchors[num] = anchor;
			return;
		}
		if (this.overrideAnchors[num])
		{
			foreach (object obj2 in this.overrideAnchors[num])
			{
				Transform transform2 = (Transform)obj2;
				if (transform2 != anchor)
				{
					transform2.parent = null;
				}
			}
		}
		this.overrideAnchors[num] = anchor;
	}

	// Token: 0x060022F0 RID: 8944 RVA: 0x000BBA28 File Offset: 0x000B9C28
	public Transform AnchorOverride(TransferrableObject.PositionState pos, Transform fallback)
	{
		int num = this.MapPositionToIndex(pos);
		Transform transform = this.overrideAnchors[num];
		if (transform != null)
		{
			return transform;
		}
		return fallback;
	}

	// Token: 0x060022F1 RID: 8945 RVA: 0x000BBA4C File Offset: 0x000B9C4C
	public void UpdateHuntWatchOffset(XformOffset offset, bool enable)
	{
		this.activeAntiClippingOffsets.huntComputer.enabled = enable;
		this.activeAntiClippingOffsets.huntComputer.offset = (enable ? offset : XformOffset.Identity);
		this.huntComputer.parent = this.HuntDefaultAnchor;
		this.huntComputer.localPosition = this.activeAntiClippingOffsets.huntComputer.offset.pos;
		this.huntComputer.localRotation = this.activeAntiClippingOffsets.huntComputer.offset.rot;
	}

	// Token: 0x060022F2 RID: 8946 RVA: 0x000BBAD8 File Offset: 0x000B9CD8
	public void UpdateBuilderWatchOffset(XformOffset offset, bool enable)
	{
		this.activeAntiClippingOffsets.builderWatch.enabled = enable;
		this.activeAntiClippingOffsets.builderWatch.offset = (enable ? offset : XformOffset.Identity);
		this.BuilderWatch.parent = this.BuilderWatchAnchor;
		this.BuilderWatch.localPosition = this.activeAntiClippingOffsets.builderWatch.offset.pos;
		this.BuilderWatch.localRotation = this.activeAntiClippingOffsets.builderWatch.offset.rot;
	}

	// Token: 0x060022F3 RID: 8947 RVA: 0x000BBB64 File Offset: 0x000B9D64
	public void UpdateFriendshipBraceletOffset(XformOffset offset, bool left, bool enable)
	{
		if (left)
		{
			this.activeAntiClippingOffsets.friendshipBraceletLeft.enabled = enable;
			this.activeAntiClippingOffsets.friendshipBraceletLeft.offset = (enable ? offset : XformOffset.Identity);
			this.friendshipBraceletLeftAnchor.parent = this.friendshipBraceletLeftDefaultAnchor;
			this.friendshipBraceletLeftAnchor.localPosition = this.activeAntiClippingOffsets.friendshipBraceletLeft.offset.pos;
			this.friendshipBraceletLeftAnchor.localRotation = this.activeAntiClippingOffsets.friendshipBraceletLeft.offset.rot;
			this.friendshipBraceletLeftAnchor.localScale = this.activeAntiClippingOffsets.friendshipBraceletLeft.offset.scale;
			return;
		}
		this.activeAntiClippingOffsets.friendshipBraceletRight.enabled = enable;
		this.activeAntiClippingOffsets.friendshipBraceletRight.offset = (enable ? offset : XformOffset.Identity);
		this.friendshipBraceletRightAnchor.parent = this.friendshipBraceletRightDefaultAnchor;
		this.friendshipBraceletRightAnchor.localPosition = this.activeAntiClippingOffsets.friendshipBraceletRight.offset.pos;
		this.friendshipBraceletRightAnchor.localRotation = this.activeAntiClippingOffsets.friendshipBraceletRight.offset.rot;
		this.friendshipBraceletRightAnchor.localScale = this.activeAntiClippingOffsets.friendshipBraceletRight.offset.scale;
	}

	// Token: 0x060022F4 RID: 8948 RVA: 0x000BBCB4 File Offset: 0x000B9EB4
	public void UpdateNameTagOffset(XformOffset offset, bool enable, CosmeticsController.CosmeticSlots slot)
	{
		switch (slot)
		{
		case CosmeticsController.CosmeticSlots.Hat:
			this.nameOffsets[5].enabled = enable;
			this.nameOffsets[5].offset = offset;
			break;
		case CosmeticsController.CosmeticSlots.Badge:
			this.nameOffsets[6].enabled = enable;
			this.nameOffsets[6].offset = offset;
			break;
		case CosmeticsController.CosmeticSlots.Face:
			this.nameOffsets[4].enabled = enable;
			this.nameOffsets[4].offset = offset;
			break;
		default:
			switch (slot)
			{
			case CosmeticsController.CosmeticSlots.Fur:
				this.nameOffsets[1].enabled = enable;
				this.nameOffsets[1].offset = offset;
				break;
			case CosmeticsController.CosmeticSlots.Shirt:
				this.nameOffsets[0].enabled = enable;
				this.nameOffsets[0].offset = offset;
				break;
			case CosmeticsController.CosmeticSlots.Pants:
				this.nameOffsets[2].enabled = enable;
				this.nameOffsets[2].offset = offset;
				break;
			case CosmeticsController.CosmeticSlots.Back:
				this.nameOffsets[3].enabled = enable;
				this.nameOffsets[3].offset = offset;
				break;
			}
			break;
		}
		this.UpdateName();
	}

	// Token: 0x060022F5 RID: 8949 RVA: 0x000BBE08 File Offset: 0x000BA008
	[Obsolete("Use UpdateNameOffset", true)]
	public void UpdateNameAnchor(GameObject nameAnchor, CosmeticsController.CosmeticSlots slot)
	{
		if (slot != CosmeticsController.CosmeticSlots.Badge)
		{
			if (slot != CosmeticsController.CosmeticSlots.Face)
			{
				switch (slot)
				{
				case CosmeticsController.CosmeticSlots.Fur:
					this.nameAnchors[1] = nameAnchor;
					break;
				case CosmeticsController.CosmeticSlots.Shirt:
					this.nameAnchors[0] = nameAnchor;
					break;
				case CosmeticsController.CosmeticSlots.Pants:
					this.nameAnchors[2] = nameAnchor;
					break;
				case CosmeticsController.CosmeticSlots.Back:
					this.nameAnchors[3] = nameAnchor;
					break;
				}
			}
			else
			{
				this.nameAnchors[4] = nameAnchor;
			}
		}
		else
		{
			this.nameAnchors[5] = nameAnchor;
		}
		this.UpdateName();
	}

	// Token: 0x060022F6 RID: 8950 RVA: 0x000BBE80 File Offset: 0x000BA080
	private static bool TryGetLargestOffset(CosmeticAnchorAntiClipEntry[] entries, out XformOffset best)
	{
		best = XformOffset.Identity;
		float num = -1f;
		for (int i = 0; i < entries.Length; i++)
		{
			if (entries[i].enabled)
			{
				float sqrMagnitude = entries[i].offset.pos.sqrMagnitude;
				if (sqrMagnitude > num)
				{
					num = sqrMagnitude;
					best = entries[i].offset;
				}
			}
		}
		return num >= 0f;
	}

	// Token: 0x060022F7 RID: 8951 RVA: 0x000BBEF4 File Offset: 0x000BA0F4
	private void UpdateName()
	{
		XformOffset xformOffset;
		if (VRRigAnchorOverrides.TryGetLargestOffset(this.nameOffsets, out xformOffset))
		{
			this.nameTransform.parent = this.nameDefaultAnchor;
			this.nameTransform.localRotation = xformOffset.rot;
			this.nameTransform.localPosition = xformOffset.pos;
			return;
		}
		if (this.nameDefaultAnchor)
		{
			this.nameTransform.parent = this.nameDefaultAnchor;
			this.nameTransform.localRotation = Quaternion.identity;
			this.nameTransform.localPosition = Vector3.zero;
			return;
		}
		Debug.LogError("VRRigAnchorOverrides: could not set parent for `nameTransform` because `nameDefaultAnchor` or its parent was null! Path: " + base.transform.GetPathQ(), this);
	}

	// Token: 0x060022F8 RID: 8952 RVA: 0x000BBFA0 File Offset: 0x000BA1A0
	public void UpdateBadgeOffset(XformOffset offset, bool enable, CosmeticsController.CosmeticSlots slot)
	{
		if (slot != CosmeticsController.CosmeticSlots.Hat)
		{
			if (slot != CosmeticsController.CosmeticSlots.Face)
			{
				switch (slot)
				{
				case CosmeticsController.CosmeticSlots.Fur:
					this.badgeOffsets[1].enabled = enable;
					this.badgeOffsets[1].offset = offset;
					break;
				case CosmeticsController.CosmeticSlots.Shirt:
					this.badgeOffsets[0].enabled = enable;
					this.badgeOffsets[0].offset = offset;
					break;
				case CosmeticsController.CosmeticSlots.Pants:
					this.badgeOffsets[2].enabled = enable;
					this.badgeOffsets[2].offset = offset;
					break;
				case CosmeticsController.CosmeticSlots.Back:
					this.badgeOffsets[3].enabled = enable;
					this.badgeOffsets[3].offset = offset;
					break;
				}
			}
			else
			{
				this.badgeOffsets[4].enabled = enable;
				this.badgeOffsets[4].offset = offset;
			}
		}
		else
		{
			this.badgeOffsets[5].enabled = enable;
			this.badgeOffsets[5].offset = offset;
		}
		this.UpdateBadge();
	}

	// Token: 0x060022F9 RID: 8953 RVA: 0x000BC0C8 File Offset: 0x000BA2C8
	[Obsolete("Use UpdateBadgeOffset", true)]
	public void UpdateBadgeAnchor(GameObject badgeAnchor, CosmeticsController.CosmeticSlots slot)
	{
		switch (slot)
		{
		case CosmeticsController.CosmeticSlots.Fur:
			this.badgeAnchors[1] = badgeAnchor;
			break;
		case CosmeticsController.CosmeticSlots.Shirt:
			this.badgeAnchors[0] = badgeAnchor;
			break;
		case CosmeticsController.CosmeticSlots.Pants:
			this.badgeAnchors[2] = badgeAnchor;
			break;
		case CosmeticsController.CosmeticSlots.Back:
			this.badgeAnchors[3] = badgeAnchor;
			break;
		}
		this.UpdateBadge();
	}

	// Token: 0x060022FA RID: 8954 RVA: 0x000BC120 File Offset: 0x000BA320
	private void UpdateBadge()
	{
		if (!this.currentBadgeTransform)
		{
			return;
		}
		XformOffset xformOffset;
		if (VRRigAnchorOverrides.TryGetLargestOffset(this.badgeOffsets, out xformOffset))
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(this.badgeDefaultPos, this.badgeDefaultRot, this.currentBadgeTransform.localScale);
			Matrix4x4 matrix4x2 = Matrix4x4.TRS(xformOffset.pos, xformOffset.rot, Vector3.one) * matrix4x;
			this.currentBadgeTransform.localRotation = matrix4x2.rotation;
			this.currentBadgeTransform.localPosition = matrix4x2.Position();
			return;
		}
		foreach (GameObject gameObject in this.badgeAnchors)
		{
			if (gameObject)
			{
				this.currentBadgeTransform.localRotation = gameObject.transform.localRotation;
				this.currentBadgeTransform.localPosition = gameObject.transform.localPosition;
				return;
			}
		}
		this.ResetBadge();
	}

	// Token: 0x060022FB RID: 8955 RVA: 0x000BC206 File Offset: 0x000BA406
	private void ResetBadge()
	{
		if (!this.currentBadgeTransform)
		{
			return;
		}
		this.currentBadgeTransform.localRotation = this.badgeDefaultRot;
		this.currentBadgeTransform.localPosition = this.badgeDefaultPos;
	}

	// Token: 0x060022FC RID: 8956 RVA: 0x000BC238 File Offset: 0x000BA438
	private void OnDestroy()
	{
		for (int i = 0; i < this.clippingOffsetTransforms.Length; i++)
		{
			if (this.clippingOffsetTransforms[i] != null)
			{
				foreach (object obj in this.clippingOffsetTransforms[i])
				{
					((Transform)obj).parent = null;
				}
				Object.Destroy(this.clippingOffsetTransforms[i].gameObject);
			}
		}
	}

	// Token: 0x04002DFB RID: 11771
	[SerializeField]
	public Transform nameDefaultAnchor;

	// Token: 0x04002DFC RID: 11772
	[SerializeField]
	public Transform nameTransform;

	// Token: 0x04002DFD RID: 11773
	[SerializeField]
	public Transform chestDefaultTransform;

	// Token: 0x04002DFE RID: 11774
	[SerializeField]
	private Transform chestBodyTrackingOffset;

	// Token: 0x04002DFF RID: 11775
	private Vector3 chestDefaultLocalPos;

	// Token: 0x04002E00 RID: 11776
	[SerializeField]
	public Transform huntComputer;

	// Token: 0x04002E01 RID: 11777
	[SerializeField]
	public Transform huntComputerDefaultAnchor;

	// Token: 0x04002E02 RID: 11778
	public Transform huntDefaultTransform;

	// Token: 0x04002E03 RID: 11779
	[SerializeField]
	protected Transform builderResizeButton;

	// Token: 0x04002E04 RID: 11780
	[SerializeField]
	protected Transform builderResizeButtonDefaultAnchor;

	// Token: 0x04002E05 RID: 11781
	private Transform builderResizeButtonDefaultTransform;

	// Token: 0x04002E06 RID: 11782
	private readonly Transform[] overrideAnchors = new Transform[8];

	// Token: 0x04002E07 RID: 11783
	private CosmeticAnchorAntiIntersectOffsets activeAntiClippingOffsets;

	// Token: 0x04002E08 RID: 11784
	private Transform[] clippingOffsetTransforms = new Transform[8];

	// Token: 0x04002E09 RID: 11785
	private GameObject nameLastObjectToAttach;

	// Token: 0x04002E0A RID: 11786
	private Transform currentBadgeTransform;

	// Token: 0x04002E0B RID: 11787
	private Vector3 badgeDefaultPos;

	// Token: 0x04002E0C RID: 11788
	private Quaternion badgeDefaultRot;

	// Token: 0x04002E0D RID: 11789
	private GameObject[] badgeAnchors = new GameObject[4];

	// Token: 0x04002E0E RID: 11790
	private GameObject[] nameAnchors = new GameObject[6];

	// Token: 0x04002E0F RID: 11791
	private CosmeticAnchorAntiClipEntry[] badgeOffsets = new CosmeticAnchorAntiClipEntry[6];

	// Token: 0x04002E10 RID: 11792
	private CosmeticAnchorAntiClipEntry[] nameOffsets = new CosmeticAnchorAntiClipEntry[7];

	// Token: 0x04002E11 RID: 11793
	[SerializeField]
	public Transform friendshipBraceletLeftDefaultAnchor;

	// Token: 0x04002E12 RID: 11794
	public Transform friendshipBraceletLeftAnchor;

	// Token: 0x04002E13 RID: 11795
	[SerializeField]
	public Transform friendshipBraceletRightDefaultAnchor;

	// Token: 0x04002E14 RID: 11796
	public Transform friendshipBraceletRightAnchor;
}
