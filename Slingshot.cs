using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020004BA RID: 1210
public class Slingshot : ProjectileWeapon
{
	// Token: 0x06001D8A RID: 7562 RVA: 0x0009F9EC File Offset: 0x0009DBEC
	private void DestroyDummyProjectile()
	{
		if (this.hasDummyProjectile)
		{
			this.dummyProjectile.transform.localScale = Vector3.one * this.dummyProjectileInitialScale;
			this.dummyProjectile.GetComponent<SphereCollider>().enabled = true;
			ObjectPools.instance.Destroy(this.dummyProjectile);
			this.dummyProjectile = null;
			this.hasDummyProjectile = false;
		}
	}

	// Token: 0x06001D8B RID: 7563 RVA: 0x0009FA50 File Offset: 0x0009DC50
	protected override void Awake()
	{
		base.Awake();
		if (this.elasticLeft)
		{
			this._elasticIntialWidthMultiplier = this.elasticLeft.widthMultiplier;
		}
	}

	// Token: 0x06001D8C RID: 7564 RVA: 0x0009FA76 File Offset: 0x0009DC76
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.myRig = rig;
		this.OnEnable();
	}

	// Token: 0x06001D8D RID: 7565 RVA: 0x0009FA8C File Offset: 0x0009DC8C
	internal override void OnEnable()
	{
		if (!base.IsSpawned)
		{
			return;
		}
		this.leftHandSnap = this.myRig.cosmeticReferences.Get(CosmeticRefID.SlingshotSnapLeft).transform;
		this.rightHandSnap = this.myRig.cosmeticReferences.Get(CosmeticRefID.SlingshotSnapRight).transform;
		this.currentState = TransferrableObject.PositionState.OnChest;
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.elasticLeft)
		{
			this.elasticLeft.positionCount = 2;
		}
		if (this.elasticRight)
		{
			this.elasticRight.positionCount = 2;
		}
		this.dummyProjectile = null;
		base.OnEnable();
	}

	// Token: 0x06001D8E RID: 7566 RVA: 0x0009FB28 File Offset: 0x0009DD28
	internal override void OnDisable()
	{
		this.DestroyDummyProjectile();
		base.OnDisable();
	}

	// Token: 0x06001D8F RID: 7567 RVA: 0x0009FB38 File Offset: 0x0009DD38
	protected override void LateUpdateShared()
	{
		if (!base.IsSpawned)
		{
			return;
		}
		base.LateUpdateShared();
		float num = Mathf.Abs(base.transform.lossyScale.x);
		Vector3 vector;
		if (this.InDrawingState())
		{
			if (!this.hasDummyProjectile)
			{
				this.dummyProjectile = ObjectPools.instance.Instantiate(this.projectilePrefab, true);
				this.hasDummyProjectile = true;
				SphereCollider component = this.dummyProjectile.GetComponent<SphereCollider>();
				component.enabled = false;
				this.dummyProjectileColliderRadius = component.radius;
				this.dummyProjectileInitialScale = this.dummyProjectile.transform.localScale.x;
				bool flag;
				bool flag2;
				bool flag3;
				base.GetIsOnTeams(out flag, out flag2, out flag3);
				this.dummyProjectile.GetComponent<SlingshotProjectile>().ApplyTeamModelAndColor(flag, flag2, flag3 && this.targetRig, this.targetRig ? this.targetRig.playerColor : default(Color));
			}
			if (this.disableInDraw != null)
			{
				this.disableInDraw.SetActive(false);
			}
			if (this.disableInDraw != null)
			{
				this.disableInDraw.SetActive(false);
			}
			float num2 = this.dummyProjectileInitialScale * num;
			this.dummyProjectile.transform.localScale = Vector3.one * num2;
			Vector3 position = this.drawingHand.transform.position;
			Vector3 position2 = this.centerOrigin.position;
			Vector3 normalized = (position2 - position).normalized;
			float num3 = (EquipmentInteractor.instance.grabRadius - this.dummyProjectileColliderRadius) * num;
			vector = position + normalized * num3;
			this.dummyProjectile.transform.position = vector;
			this.dummyProjectile.transform.rotation = Quaternion.LookRotation(position2 - vector, Vector3.up);
			if (!this.wasStretching)
			{
				UnityEvent<bool> stretchStartShared = this.StretchStartShared;
				if (stretchStartShared != null)
				{
					stretchStartShared.Invoke(!this.ForLeftHandSlingshot());
				}
				this.wasStretching = true;
			}
		}
		else
		{
			this.DestroyDummyProjectile();
			if (this.disableInDraw != null)
			{
				this.disableInDraw.SetActive(true);
			}
			vector = this.centerOrigin.position;
			if (this.wasStretching)
			{
				UnityEvent<bool> stretchEndShared = this.StretchEndShared;
				if (stretchEndShared != null)
				{
					stretchEndShared.Invoke(!this.ForLeftHandSlingshot());
				}
				this.wasStretching = false;
			}
		}
		this.center.position = vector;
		if (!this.disableLineRenderer)
		{
			this.elasticLeftPoints[0] = this.leftArm.position;
			this.elasticLeftPoints[1] = (this.elasticRightPoints[1] = vector);
			this.elasticRightPoints[0] = this.rightArm.position;
			this.elasticLeft.SetPositions(this.elasticLeftPoints);
			this.elasticRight.SetPositions(this.elasticRightPoints);
			this.elasticLeft.widthMultiplier = this._elasticIntialWidthMultiplier * num;
			this.elasticRight.widthMultiplier = this._elasticIntialWidthMultiplier * num;
		}
		if (!NetworkSystem.Instance.InRoom && this.disableWhenNotInRoom)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001D90 RID: 7568 RVA: 0x0009FE60 File Offset: 0x0009E060
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.InDrawingState())
		{
			if (this.ForLeftHandSlingshot())
			{
				this.drawingHand = EquipmentInteractor.instance.rightHand;
			}
			else
			{
				this.drawingHand = EquipmentInteractor.instance.leftHand;
			}
			GorillaTagger.Instance.StartVibration(!this.ForLeftHandSlingshot(), this.hapticsStrength, this.hapticsLength);
			if (!this.wasStretchingLocal)
			{
				UnityEvent<bool> stretchStartLocal = this.StretchStartLocal;
				if (stretchStartLocal != null)
				{
					stretchStartLocal.Invoke(!this.ForLeftHandSlingshot());
				}
				this.wasStretchingLocal = true;
				return;
			}
		}
		else if (this.wasStretchingLocal)
		{
			UnityEvent<bool> stretchEndLocal = this.StretchEndLocal;
			if (stretchEndLocal != null)
			{
				stretchEndLocal.Invoke(!this.ForLeftHandSlingshot());
			}
			this.wasStretchingLocal = false;
		}
	}

	// Token: 0x06001D91 RID: 7569 RVA: 0x0009FF1B File Offset: 0x0009E11B
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.InDrawingState())
		{
			if (this.ForLeftHandSlingshot())
			{
				this.drawingHand = this.rightHandSnap.gameObject;
				return;
			}
			this.drawingHand = this.leftHandSnap.gameObject;
		}
	}

	// Token: 0x06001D92 RID: 7570 RVA: 0x0009FF56 File Offset: 0x0009E156
	public static bool IsSlingShotEnabled()
	{
		return !(GorillaTagger.Instance == null) && !(GorillaTagger.Instance.offlineVRRig == null) && GorillaTagger.Instance.offlineVRRig.cosmeticSet.HasItemOfCategory(CosmeticsController.CosmeticCategory.Chest);
	}

	// Token: 0x06001D93 RID: 7571 RVA: 0x0009FF90 File Offset: 0x0009E190
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!this.IsMyItem())
		{
			return;
		}
		bool flag = pointGrabbed == this.nock;
		if (flag && !base.InHand())
		{
			return;
		}
		base.OnGrab(pointGrabbed, grabbingHand);
		if (this.InDrawingState() || base.OnChest())
		{
			return;
		}
		if (flag)
		{
			if (grabbingHand == EquipmentInteractor.instance.leftHand)
			{
				EquipmentInteractor.instance.disableLeftGrab = true;
			}
			else
			{
				EquipmentInteractor.instance.disableRightGrab = true;
			}
			if (this.ForLeftHandSlingshot())
			{
				this.itemState = TransferrableObject.ItemStates.State2;
			}
			else
			{
				this.itemState = TransferrableObject.ItemStates.State3;
			}
			this.minTimeToLaunch = Time.time + this.delayLaunchTime;
			GorillaTagger.Instance.StartVibration(!this.ForLeftHandSlingshot(), GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration * 1.5f);
		}
	}

	// Token: 0x06001D94 RID: 7572 RVA: 0x000A006C File Offset: 0x0009E26C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		if (this.InDrawingState() && releasingHand == this.drawingHand)
		{
			if (releasingHand == EquipmentInteractor.instance.leftHand)
			{
				EquipmentInteractor.instance.disableLeftGrab = false;
			}
			else
			{
				EquipmentInteractor.instance.disableRightGrab = false;
			}
			if (this.ForLeftHandSlingshot())
			{
				this.currentState = TransferrableObject.PositionState.InLeftHand;
			}
			else
			{
				this.currentState = TransferrableObject.PositionState.InRightHand;
			}
			this.itemState = TransferrableObject.ItemStates.State0;
			GorillaTagger.Instance.StartVibration(this.ForLeftHandSlingshot(), GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 1.5f);
			if (Time.time > this.minTimeToLaunch && (releasingHand.transform.position - this.centerOrigin.transform.position).sqrMagnitude > this.minDrawDistanceToRelease * this.minDrawDistanceToRelease)
			{
				base.LaunchProjectile();
			}
		}
		else
		{
			EquipmentInteractor.instance.disableLeftGrab = false;
			EquipmentInteractor.instance.disableRightGrab = false;
		}
		return true;
	}

	// Token: 0x06001D95 RID: 7573 RVA: 0x000A0184 File Offset: 0x0009E384
	public override void DropItemCleanup()
	{
		base.DropItemCleanup();
		this.currentState = TransferrableObject.PositionState.OnChest;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001D96 RID: 7574 RVA: 0x00023F0C File Offset: 0x0002210C
	public override bool AutoGrabTrue(bool leftGrabbingHand)
	{
		return true;
	}

	// Token: 0x06001D97 RID: 7575 RVA: 0x000A019B File Offset: 0x0009E39B
	private bool ForLeftHandSlingshot()
	{
		return this.itemState == TransferrableObject.ItemStates.State2 || this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	// Token: 0x06001D98 RID: 7576 RVA: 0x000A01B1 File Offset: 0x0009E3B1
	private bool InDrawingState()
	{
		return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State3;
	}

	// Token: 0x06001D99 RID: 7577 RVA: 0x000A01C7 File Offset: 0x0009E3C7
	protected override Vector3 GetLaunchPosition()
	{
		return this.dummyProjectile.transform.position;
	}

	// Token: 0x06001D9A RID: 7578 RVA: 0x000A01DC File Offset: 0x0009E3DC
	protected override Vector3 GetLaunchVelocity()
	{
		float num = Mathf.Abs(base.transform.lossyScale.x);
		Vector3 vector = this.centerOrigin.position - this.center.position;
		vector /= num;
		Vector3 vector2 = Mathf.Min(this.springConstant * this.maxDraw, vector.magnitude * this.springConstant) * vector.normalized * num;
		Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
		return vector2 + averagedVelocity;
	}

	// Token: 0x040027CC RID: 10188
	[SerializeField]
	private bool disableLineRenderer;

	// Token: 0x040027CD RID: 10189
	[FormerlySerializedAs("elastic")]
	public LineRenderer elasticLeft;

	// Token: 0x040027CE RID: 10190
	public LineRenderer elasticRight;

	// Token: 0x040027CF RID: 10191
	public Transform leftArm;

	// Token: 0x040027D0 RID: 10192
	public Transform rightArm;

	// Token: 0x040027D1 RID: 10193
	public Transform center;

	// Token: 0x040027D2 RID: 10194
	public Transform centerOrigin;

	// Token: 0x040027D3 RID: 10195
	private GameObject dummyProjectile;

	// Token: 0x040027D4 RID: 10196
	public GameObject drawingHand;

	// Token: 0x040027D5 RID: 10197
	public InteractionPoint nock;

	// Token: 0x040027D6 RID: 10198
	public InteractionPoint grip;

	// Token: 0x040027D7 RID: 10199
	public float springConstant;

	// Token: 0x040027D8 RID: 10200
	public float maxDraw;

	// Token: 0x040027D9 RID: 10201
	[SerializeField]
	private GameObject disableInDraw;

	// Token: 0x040027DA RID: 10202
	[SerializeField]
	private float minDrawDistanceToRelease;

	// Token: 0x040027DB RID: 10203
	[Header("Stretching Haptics")]
	[Space]
	[SerializeField]
	private bool playStretchingHaptics;

	// Token: 0x040027DC RID: 10204
	[SerializeField]
	private float hapticsStrength = 0.1f;

	// Token: 0x040027DD RID: 10205
	[SerializeField]
	private float hapticsLength = 0.1f;

	// Token: 0x040027DE RID: 10206
	[Header("Stretching Events")]
	[Space]
	public UnityEvent<bool> StretchStartShared;

	// Token: 0x040027DF RID: 10207
	public UnityEvent<bool> StretchEndShared;

	// Token: 0x040027E0 RID: 10208
	[Space]
	public UnityEvent<bool> StretchStartLocal;

	// Token: 0x040027E1 RID: 10209
	public UnityEvent<bool> StretchEndLocal;

	// Token: 0x040027E2 RID: 10210
	private bool wasStretching;

	// Token: 0x040027E3 RID: 10211
	private bool wasStretchingLocal;

	// Token: 0x040027E4 RID: 10212
	private Transform leftHandSnap;

	// Token: 0x040027E5 RID: 10213
	private Transform rightHandSnap;

	// Token: 0x040027E6 RID: 10214
	public bool disableWhenNotInRoom;

	// Token: 0x040027E7 RID: 10215
	private bool hasDummyProjectile;

	// Token: 0x040027E8 RID: 10216
	private float delayLaunchTime = 0.07f;

	// Token: 0x040027E9 RID: 10217
	private float minTimeToLaunch = -1f;

	// Token: 0x040027EA RID: 10218
	private float dummyProjectileColliderRadius;

	// Token: 0x040027EB RID: 10219
	private float dummyProjectileInitialScale;

	// Token: 0x040027EC RID: 10220
	private int projectileCount;

	// Token: 0x040027ED RID: 10221
	private Vector3[] elasticLeftPoints = new Vector3[2];

	// Token: 0x040027EE RID: 10222
	private Vector3[] elasticRightPoints = new Vector3[2];

	// Token: 0x040027EF RID: 10223
	private float _elasticIntialWidthMultiplier;

	// Token: 0x040027F0 RID: 10224
	private new VRRig myRig;

	// Token: 0x020004BB RID: 1211
	public enum SlingshotState
	{
		// Token: 0x040027F2 RID: 10226
		NoState = 1,
		// Token: 0x040027F3 RID: 10227
		OnChest,
		// Token: 0x040027F4 RID: 10228
		LeftHandDrawing = 4,
		// Token: 0x040027F5 RID: 10229
		RightHandDrawing = 8
	}

	// Token: 0x020004BC RID: 1212
	public enum SlingshotActions
	{
		// Token: 0x040027F7 RID: 10231
		Grab,
		// Token: 0x040027F8 RID: 10232
		Release
	}
}
