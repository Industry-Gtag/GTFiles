using System;
using GorillaExtensions;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000528 RID: 1320
public class EdibleHoldable : TransferrableObject
{
	// Token: 0x17000393 RID: 915
	// (get) Token: 0x06002120 RID: 8480 RVA: 0x000B141E File Offset: 0x000AF61E
	// (set) Token: 0x06002121 RID: 8481 RVA: 0x000B1426 File Offset: 0x000AF626
	public int lastBiterActorID { get; private set; } = -1;

	// Token: 0x06002122 RID: 8482 RVA: 0x000B142F File Offset: 0x000AF62F
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.previousEdibleState = (EdibleHoldable.EdibleHoldableStates)this.itemState;
		this.lastFullyEatenTime = -this.respawnTime;
		this.iResettableItems = base.GetComponentsInChildren<IResettableItem>(true);
	}

	// Token: 0x06002123 RID: 8483 RVA: 0x000B1464 File Offset: 0x000AF664
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		this.lastEatTime = Time.time - this.eatMinimumCooldown;
	}

	// Token: 0x06002124 RID: 8484 RVA: 0x000B1480 File Offset: 0x000AF680
	public override void OnActivate()
	{
		base.OnActivate();
	}

	// Token: 0x06002125 RID: 8485 RVA: 0x000B1488 File Offset: 0x000AF688
	internal override void OnEnable()
	{
		base.OnEnable();
	}

	// Token: 0x06002126 RID: 8486 RVA: 0x0004C208 File Offset: 0x0004A408
	internal override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x06002127 RID: 8487 RVA: 0x000B1490 File Offset: 0x000AF690
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
	}

	// Token: 0x06002128 RID: 8488 RVA: 0x000B1498 File Offset: 0x000AF698
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return base.OnRelease(zoneReleased, releasingHand) && !base.InHand();
	}

	// Token: 0x06002129 RID: 8489 RVA: 0x000B14B4 File Offset: 0x000AF6B4
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.itemState == TransferrableObject.ItemStates.State3)
		{
			if (Time.time > this.lastFullyEatenTime + this.respawnTime)
			{
				this.itemState = TransferrableObject.ItemStates.State0;
				return;
			}
		}
		else if (Time.time > this.lastEatTime + this.eatMinimumCooldown)
		{
			bool flag = false;
			bool flag2 = false;
			float num = this.biteDistance * this.biteDistance;
			if (!GorillaParent.hasInstance)
			{
				return;
			}
			VRRig vrrig = null;
			VRRig vrrig2 = null;
			for (int i = 0; i < VRRigCache.ActiveRigContainers.Count; i++)
			{
				VRRig rig = VRRigCache.ActiveRigContainers[i].Rig;
				if (!rig.isOfflineVRRig)
				{
					if (rig.head == null || rig.head.rigTarget.IsNull())
					{
						break;
					}
					Transform transform = rig.head.rigTarget.transform;
					if ((transform.position + transform.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude < num)
					{
						flag = true;
						vrrig2 = rig;
					}
				}
			}
			Transform transform2 = GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform;
			if ((transform2.position + transform2.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude < num)
			{
				flag = true;
				flag2 = true;
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (flag && !this.inBiteZone && (!flag2 || base.InHand()) && this.itemState != TransferrableObject.ItemStates.State3)
			{
				if (this.itemState == TransferrableObject.ItemStates.State0)
				{
					this.itemState = TransferrableObject.ItemStates.State1;
				}
				else if (this.itemState == TransferrableObject.ItemStates.State1)
				{
					this.itemState = TransferrableObject.ItemStates.State2;
				}
				else if (this.itemState == TransferrableObject.ItemStates.State2)
				{
					this.itemState = TransferrableObject.ItemStates.State3;
				}
				this.lastEatTime = Time.time;
				this.lastFullyEatenTime = Time.time;
			}
			if (flag)
			{
				if (flag2)
				{
					int num2;
					if (!vrrig)
					{
						num2 = -1;
					}
					else
					{
						NetPlayer owningNetPlayer = vrrig.OwningNetPlayer;
						num2 = ((owningNetPlayer != null) ? owningNetPlayer.ActorNumber : (-1));
					}
					this.lastBiterActorID = num2;
					EdibleHoldable.BiteEvent biteEvent = this.onBiteView;
					if (biteEvent != null)
					{
						biteEvent.Invoke(vrrig, (int)this.itemState);
					}
				}
				else
				{
					int num3;
					if (!vrrig2)
					{
						num3 = -1;
					}
					else
					{
						NetPlayer owningNetPlayer2 = vrrig2.OwningNetPlayer;
						num3 = ((owningNetPlayer2 != null) ? owningNetPlayer2.ActorNumber : (-1));
					}
					this.lastBiterActorID = num3;
					EdibleHoldable.BiteEvent biteEvent2 = this.onBiteWorld;
					if (biteEvent2 != null)
					{
						biteEvent2.Invoke(vrrig2, (int)this.itemState);
					}
				}
			}
			this.inBiteZone = flag;
		}
	}

	// Token: 0x0600212A RID: 8490 RVA: 0x000B1724 File Offset: 0x000AF924
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		EdibleHoldable.EdibleHoldableStates itemState = (EdibleHoldable.EdibleHoldableStates)this.itemState;
		if (itemState != this.previousEdibleState)
		{
			this.OnEdibleHoldableStateChange();
		}
		this.previousEdibleState = itemState;
	}

	// Token: 0x0600212B RID: 8491 RVA: 0x000B1754 File Offset: 0x000AF954
	protected virtual void OnEdibleHoldableStateChange()
	{
		float num = GorillaTagger.Instance.tapHapticStrength / 4f;
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num2 = 0.08f;
		int num3 = 0;
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			num3 = 0;
			if (this.iResettableItems != null)
			{
				foreach (IResettableItem resettableItem in this.iResettableItems)
				{
					if (resettableItem != null)
					{
						resettableItem.ResetToDefaultState();
					}
				}
			}
		}
		else if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			num3 = 1;
		}
		else if (this.itemState == TransferrableObject.ItemStates.State2)
		{
			num3 = 2;
		}
		else if (this.itemState == TransferrableObject.ItemStates.State3)
		{
			num3 = 3;
		}
		int num4 = num3 - 1;
		if (num4 < 0)
		{
			num4 = this.edibleMeshObjects.Length - 1;
		}
		this.edibleMeshObjects[num4].SetActive(false);
		this.edibleMeshObjects[num3].SetActive(true);
		if ((this.itemState != TransferrableObject.ItemStates.State0 && this.onBiteView != null) || this.onBiteWorld != null)
		{
			VRRig vrrig = null;
			float num5 = float.PositiveInfinity;
			for (int j = 0; j < VRRigCache.ActiveRigContainers.Count; j++)
			{
				VRRig rig = VRRigCache.ActiveRigContainers[j].Rig;
				if (rig.head == null || rig.head.rigTarget.IsNull())
				{
					break;
				}
				Transform transform = rig.head.rigTarget.transform;
				float sqrMagnitude = (transform.position + transform.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude;
				if (sqrMagnitude < num5)
				{
					num5 = sqrMagnitude;
					vrrig = rig;
				}
			}
			if (vrrig.IsNotNull())
			{
				EdibleHoldable.BiteEvent biteEvent = (vrrig.isOfflineVRRig ? this.onBiteView : this.onBiteWorld);
				if (biteEvent != null)
				{
					biteEvent.Invoke(vrrig, (int)this.itemState);
				}
				if (vrrig.isOfflineVRRig && this.itemState != TransferrableObject.ItemStates.State0)
				{
					PlayerGameEvents.EatObject(this.interactEventName);
				}
			}
		}
		this.eatSoundSource.GTPlayOneShot(this.eatSounds[num3], num2);
		if (this.IsMyItem())
		{
			if (base.InHand())
			{
				GorillaTagger.Instance.StartVibration(base.InLeftHand(), num, fixedDeltaTime);
				return;
			}
			GorillaTagger.Instance.StartVibration(false, num, fixedDeltaTime);
			GorillaTagger.Instance.StartVibration(true, num, fixedDeltaTime);
		}
	}

	// Token: 0x0600212C RID: 8492 RVA: 0x00023F0C File Offset: 0x0002210C
	public override bool CanActivate()
	{
		return true;
	}

	// Token: 0x0600212D RID: 8493 RVA: 0x00023F0C File Offset: 0x0002210C
	public override bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x04002BEB RID: 11243
	public AudioClip[] eatSounds;

	// Token: 0x04002BEC RID: 11244
	public GameObject[] edibleMeshObjects;

	// Token: 0x04002BEE RID: 11246
	public EdibleHoldable.BiteEvent onBiteView;

	// Token: 0x04002BEF RID: 11247
	public EdibleHoldable.BiteEvent onBiteWorld;

	// Token: 0x04002BF0 RID: 11248
	[DebugReadout]
	public float lastEatTime;

	// Token: 0x04002BF1 RID: 11249
	[DebugReadout]
	public float lastFullyEatenTime;

	// Token: 0x04002BF2 RID: 11250
	public float eatMinimumCooldown = 1f;

	// Token: 0x04002BF3 RID: 11251
	public float respawnTime = 7f;

	// Token: 0x04002BF4 RID: 11252
	public float biteDistance = 0.1666667f;

	// Token: 0x04002BF5 RID: 11253
	public Vector3 biteOffset = new Vector3(0f, 0.0208f, 0.171f);

	// Token: 0x04002BF6 RID: 11254
	public Transform biteSpot;

	// Token: 0x04002BF7 RID: 11255
	public bool inBiteZone;

	// Token: 0x04002BF8 RID: 11256
	public AudioSource eatSoundSource;

	// Token: 0x04002BF9 RID: 11257
	private EdibleHoldable.EdibleHoldableStates previousEdibleState;

	// Token: 0x04002BFA RID: 11258
	private IResettableItem[] iResettableItems;

	// Token: 0x02000529 RID: 1321
	private enum EdibleHoldableStates
	{
		// Token: 0x04002BFC RID: 11260
		EatingState0 = 1,
		// Token: 0x04002BFD RID: 11261
		EatingState1,
		// Token: 0x04002BFE RID: 11262
		EatingState2 = 4,
		// Token: 0x04002BFF RID: 11263
		EatingState3 = 8
	}

	// Token: 0x0200052A RID: 1322
	[Serializable]
	public class BiteEvent : UnityEvent<VRRig, int>
	{
	}
}
