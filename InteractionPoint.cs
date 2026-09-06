using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020004B3 RID: 1203
public class InteractionPoint : MonoBehaviour, ISpawnable, IBuildValidation
{
	// Token: 0x1700031F RID: 799
	// (get) Token: 0x06001D5A RID: 7514 RVA: 0x0009EFA7 File Offset: 0x0009D1A7
	// (set) Token: 0x06001D5B RID: 7515 RVA: 0x0009EFAF File Offset: 0x0009D1AF
	public bool ignoreLeftHand { get; private set; }

	// Token: 0x17000320 RID: 800
	// (get) Token: 0x06001D5C RID: 7516 RVA: 0x0009EFB8 File Offset: 0x0009D1B8
	// (set) Token: 0x06001D5D RID: 7517 RVA: 0x0009EFC0 File Offset: 0x0009D1C0
	public bool ignoreRightHand { get; private set; }

	// Token: 0x17000321 RID: 801
	// (get) Token: 0x06001D5E RID: 7518 RVA: 0x0009EFC9 File Offset: 0x0009D1C9
	public IHoldableObject Holdable
	{
		get
		{
			return this.parentHoldable;
		}
	}

	// Token: 0x17000322 RID: 802
	// (get) Token: 0x06001D5F RID: 7519 RVA: 0x0009EFD1 File Offset: 0x0009D1D1
	// (set) Token: 0x06001D60 RID: 7520 RVA: 0x0009EFD9 File Offset: 0x0009D1D9
	public bool IsSpawned { get; set; }

	// Token: 0x17000323 RID: 803
	// (get) Token: 0x06001D61 RID: 7521 RVA: 0x0009EFE2 File Offset: 0x0009D1E2
	// (set) Token: 0x06001D62 RID: 7522 RVA: 0x0009EFEA File Offset: 0x0009D1EA
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06001D63 RID: 7523 RVA: 0x0009EFF4 File Offset: 0x0009D1F4
	public void OnSpawn(VRRig rig)
	{
		if (!this.IsSpawned)
		{
			this.IsSpawned = true;
		}
		this.interactor = EquipmentInteractor.instance;
		this.myCollider = base.GetComponent<Collider>();
		if (this.parentHoldableObject != null)
		{
			this.parentHoldable = this.parentHoldableObject.GetComponent<IHoldableObject>();
		}
		else
		{
			this.parentHoldable = base.GetComponentInParent<IHoldableObject>(true);
			if (this.parentHoldable != null)
			{
				this.parentHoldableObject = this.parentHoldable.gameObject;
			}
		}
		if (this.parentHoldable == null)
		{
			if (this.parentHoldableObject == null)
			{
				Debug.LogError("InteractionPoint: Disabling because expected field `parentHoldableObject` is null. Path=" + base.transform.GetPathQ());
				base.enabled = false;
				return;
			}
			Debug.LogError("InteractionPoint: Disabling because `parentHoldableObject` does not have a IHoldableObject component. Path=" + base.transform.GetPathQ());
		}
		TransferrableObject transferrableObject = this.parentHoldable as TransferrableObject;
		this.forLocalPlayer = transferrableObject == null || transferrableObject.IsLocalObject() || transferrableObject.isSceneObject || transferrableObject.canDrop;
	}

	// Token: 0x06001D64 RID: 7524 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001D65 RID: 7525 RVA: 0x0009F0F6 File Offset: 0x0009D2F6
	private void Awake()
	{
		if (this.isNonSpawnedObject)
		{
			this.OnSpawn(null);
		}
	}

	// Token: 0x06001D66 RID: 7526 RVA: 0x0009F107 File Offset: 0x0009D307
	private void OnEnable()
	{
		this.wasInLeft = false;
		this.wasInRight = false;
		if (!this.IsSpawned && this.parentHoldableObject != null)
		{
			this.OnSpawn(null);
		}
	}

	// Token: 0x06001D67 RID: 7527 RVA: 0x0009F134 File Offset: 0x0009D334
	public void OnDisable()
	{
		if (!this.forLocalPlayer || this.interactor == null)
		{
			return;
		}
		this.interactor.InteractionPointDisabled(this);
	}

	// Token: 0x06001D68 RID: 7528 RVA: 0x0009F15C File Offset: 0x0009D35C
	protected void LateUpdate()
	{
		if (!this.IsSpawned)
		{
			return;
		}
		if (!this.forLocalPlayer)
		{
			base.enabled = false;
			if (this.myCollider.IsNotNull())
			{
				this.myCollider.enabled = false;
			}
			return;
		}
		if (this.interactor == null)
		{
			this.interactor = EquipmentInteractor.instance;
			return;
		}
		if (this.interactionRadius > 0f || this.myCollider != null)
		{
			if (!this.ignoreLeftHand && this.OverlapCheck(this.interactor.leftHand.transform.position) != this.wasInLeft)
			{
				if (!this.wasInLeft && !this.interactor.overlapInteractionPointsLeft.Contains(this))
				{
					this.interactor.overlapInteractionPointsLeft.Add(this);
					this.wasInLeft = true;
				}
				else if (this.wasInLeft && this.interactor.overlapInteractionPointsLeft.Contains(this))
				{
					this.interactor.overlapInteractionPointsLeft.Remove(this);
					this.wasInLeft = false;
				}
			}
			if (!this.ignoreRightHand && this.OverlapCheck(this.interactor.rightHand.transform.position) != this.wasInRight)
			{
				if (!this.wasInRight && !this.interactor.overlapInteractionPointsRight.Contains(this))
				{
					this.interactor.overlapInteractionPointsRight.Add(this);
					this.wasInRight = true;
					return;
				}
				if (this.wasInRight && this.interactor.overlapInteractionPointsRight.Contains(this))
				{
					this.interactor.overlapInteractionPointsRight.Remove(this);
					this.wasInRight = false;
				}
			}
		}
	}

	// Token: 0x06001D69 RID: 7529 RVA: 0x0009F304 File Offset: 0x0009D504
	public bool OverlapCheck(Vector3 point)
	{
		if (this.interactionRadius > 0f)
		{
			return (base.transform.position - point).IsShorterThan(this.interactionRadius * base.transform.lossyScale);
		}
		return this.myCollider != null && this.myCollider.bounds.Contains(point);
	}

	// Token: 0x06001D6A RID: 7530 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool BuildValidationCheck()
	{
		return true;
	}

	// Token: 0x0400279E RID: 10142
	[SerializeField]
	[FormerlySerializedAs("parentTransferrableObject")]
	public GameObject parentHoldableObject;

	// Token: 0x0400279F RID: 10143
	private IHoldableObject parentHoldable;

	// Token: 0x040027A2 RID: 10146
	[SerializeField]
	private bool isNonSpawnedObject;

	// Token: 0x040027A3 RID: 10147
	[SerializeField]
	private float interactionRadius;

	// Token: 0x040027A4 RID: 10148
	public Collider myCollider;

	// Token: 0x040027A5 RID: 10149
	public EquipmentInteractor interactor;

	// Token: 0x040027A6 RID: 10150
	public bool wasInLeft;

	// Token: 0x040027A7 RID: 10151
	public bool wasInRight;

	// Token: 0x040027A8 RID: 10152
	public bool forLocalPlayer;
}
