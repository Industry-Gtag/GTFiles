using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000525 RID: 1317
public class CosmeticAnchors : MonoBehaviour, ISpawnable
{
	// Token: 0x1700038F RID: 911
	// (get) Token: 0x06002100 RID: 8448 RVA: 0x000B0C4C File Offset: 0x000AEE4C
	// (set) Token: 0x06002101 RID: 8449 RVA: 0x000B0C54 File Offset: 0x000AEE54
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000390 RID: 912
	// (get) Token: 0x06002102 RID: 8450 RVA: 0x000B0C5D File Offset: 0x000AEE5D
	// (set) Token: 0x06002103 RID: 8451 RVA: 0x000B0C65 File Offset: 0x000AEE65
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06002104 RID: 8452 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnSpawn(VRRig rig)
	{
	}

	// Token: 0x06002105 RID: 8453 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06002106 RID: 8454 RVA: 0x000B0C70 File Offset: 0x000AEE70
	private void AssignAnchorToPath(ref GameObject anchorGObjRef, string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		Transform transform;
		if (!base.transform.TryFindByPath(path, out transform, false))
		{
			this.vrRig = base.GetComponentInParent<VRRig>(true);
			if (this.vrRig && this.vrRig.isOfflineVRRig)
			{
				Debug.LogError("CosmeticAnchors: Could not find path: \"" + path + "\".\nPath to this component: " + base.transform.GetPathQ(), this);
			}
			return;
		}
		anchorGObjRef = transform.gameObject;
	}

	// Token: 0x06002107 RID: 8455 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnEnable()
	{
	}

	// Token: 0x06002108 RID: 8456 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnDisable()
	{
	}

	// Token: 0x06002109 RID: 8457 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void TryUpdate()
	{
	}

	// Token: 0x0600210A RID: 8458 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void EnableAnchor(bool enable)
	{
	}

	// Token: 0x0600210B RID: 8459 RVA: 0x000B0CE8 File Offset: 0x000AEEE8
	private void SetHuntComputerAnchor(bool enable)
	{
		Transform huntComputer = this.anchorOverrides.HuntComputer;
		if (!GorillaTagger.Instance.offlineVRRig.huntComputer.activeSelf || !enable)
		{
			huntComputer.parent = this.anchorOverrides.HuntDefaultAnchor;
		}
		else
		{
			huntComputer.parent = this.huntComputerAnchor.transform;
		}
		huntComputer.transform.localPosition = Vector3.zero;
		huntComputer.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x0600210C RID: 8460 RVA: 0x000B0D60 File Offset: 0x000AEF60
	private void SetBuilderWatchAnchor(bool enable)
	{
		Transform builderWatch = this.anchorOverrides.BuilderWatch;
		if (!GorillaTagger.Instance.offlineVRRig.builderResizeWatch.activeSelf || !enable)
		{
			builderWatch.parent = this.anchorOverrides.BuilderWatchAnchor;
		}
		else
		{
			builderWatch.parent = this.builderWatchAnchor.transform;
		}
		builderWatch.transform.localPosition = Vector3.zero;
		builderWatch.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x0600210D RID: 8461 RVA: 0x000B0DD8 File Offset: 0x000AEFD8
	private void SetCustomAnchor(Transform target, bool enable, GameObject overrideAnchor, Transform defaultAnchor)
	{
		Transform transform = ((enable && overrideAnchor != null) ? overrideAnchor.transform : defaultAnchor);
		if (target != null && target.parent != transform)
		{
			target.parent = transform;
			target.transform.localPosition = Vector3.zero;
			target.transform.localRotation = Quaternion.identity;
			target.transform.localScale = Vector3.one;
		}
	}

	// Token: 0x0600210E RID: 8462 RVA: 0x000B0E4C File Offset: 0x000AF04C
	public Transform GetPositionAnchor(TransferrableObject.PositionState pos)
	{
		if (pos != TransferrableObject.PositionState.OnLeftArm)
		{
			if (pos != TransferrableObject.PositionState.OnRightArm)
			{
				if (pos != TransferrableObject.PositionState.OnChest)
				{
					return null;
				}
				if (!this.chestAnchor)
				{
					return null;
				}
				return this.chestAnchor.transform;
			}
			else
			{
				if (!this.rightArmAnchor)
				{
					return null;
				}
				return this.rightArmAnchor.transform;
			}
		}
		else
		{
			if (!this.leftArmAnchor)
			{
				return null;
			}
			return this.leftArmAnchor.transform;
		}
	}

	// Token: 0x0600210F RID: 8463 RVA: 0x000B0EBA File Offset: 0x000AF0BA
	public Transform GetNameAnchor()
	{
		if (!this.nameAnchor)
		{
			return null;
		}
		return this.nameAnchor.transform;
	}

	// Token: 0x06002110 RID: 8464 RVA: 0x000B0ED6 File Offset: 0x000AF0D6
	public bool AffectedByHunt()
	{
		return this.huntComputerAnchor != null;
	}

	// Token: 0x06002111 RID: 8465 RVA: 0x000B0EE4 File Offset: 0x000AF0E4
	public bool AffectedByBuilder()
	{
		return this.builderWatchAnchor != null;
	}

	// Token: 0x04002BB5 RID: 11189
	[SerializeField]
	private bool deprecatedWarning = true;

	// Token: 0x04002BB6 RID: 11190
	[SerializeField]
	protected GameObject nameAnchor;

	// Token: 0x04002BB7 RID: 11191
	[SerializeField]
	protected string nameAnchor_path;

	// Token: 0x04002BB8 RID: 11192
	[SerializeField]
	protected GameObject leftArmAnchor;

	// Token: 0x04002BB9 RID: 11193
	[SerializeField]
	protected string leftArmAnchor_path;

	// Token: 0x04002BBA RID: 11194
	[SerializeField]
	protected GameObject rightArmAnchor;

	// Token: 0x04002BBB RID: 11195
	[SerializeField]
	protected string rightArmAnchor_path;

	// Token: 0x04002BBC RID: 11196
	[SerializeField]
	protected GameObject chestAnchor;

	// Token: 0x04002BBD RID: 11197
	[SerializeField]
	protected string chestAnchor_path;

	// Token: 0x04002BBE RID: 11198
	[SerializeField]
	protected GameObject huntComputerAnchor;

	// Token: 0x04002BBF RID: 11199
	[SerializeField]
	protected string huntComputerAnchor_path;

	// Token: 0x04002BC0 RID: 11200
	[SerializeField]
	protected GameObject builderWatchAnchor;

	// Token: 0x04002BC1 RID: 11201
	[SerializeField]
	protected string builderWatchAnchor_path;

	// Token: 0x04002BC2 RID: 11202
	[SerializeField]
	protected GameObject friendshipBraceletLeftOverride;

	// Token: 0x04002BC3 RID: 11203
	[SerializeField]
	protected string friendshipBraceletLeftOverride_path;

	// Token: 0x04002BC4 RID: 11204
	[SerializeField]
	protected GameObject friendshipBraceletRightOverride;

	// Token: 0x04002BC5 RID: 11205
	[SerializeField]
	protected string friendshipBraceletRightOverride_path;

	// Token: 0x04002BC6 RID: 11206
	[SerializeField]
	protected GameObject badgeAnchor;

	// Token: 0x04002BC7 RID: 11207
	[SerializeField]
	protected string badgeAnchor_path;

	// Token: 0x04002BC8 RID: 11208
	[SerializeField]
	public CosmeticsController.CosmeticSlots slot;

	// Token: 0x04002BC9 RID: 11209
	private VRRig vrRig;

	// Token: 0x04002BCA RID: 11210
	private VRRigAnchorOverrides anchorOverrides;

	// Token: 0x04002BCB RID: 11211
	private bool anchorEnabled;

	// Token: 0x04002BCC RID: 11212
	private static GTLogErrorLimiter k_debugLogError_anchorOverridesNull = new GTLogErrorLimiter("The array `anchorOverrides` was null. Is the cosmetic getting initialized properly? ", 10, "\n- ");
}
