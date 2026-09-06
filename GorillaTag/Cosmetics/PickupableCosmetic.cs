using System;
using System.Collections.Generic;
using System.Threading;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012EB RID: 4843
	public class PickupableCosmetic : PickupableVariant
	{
		// Token: 0x06007956 RID: 31062 RVA: 0x00279110 File Offset: 0x00277310
		private void Awake()
		{
			this.rigOwnedPhysicsBody = base.GetComponent<RigOwnedPhysicsBody>();
			this.bodyCollider = base.GetComponent<Collider>();
		}

		// Token: 0x06007957 RID: 31063 RVA: 0x0025D67F File Offset: 0x0025B87F
		private void Start()
		{
			base.enabled = false;
		}

		// Token: 0x06007958 RID: 31064 RVA: 0x0027912A File Offset: 0x0027732A
		private void OnEnable()
		{
			if (this.rigOwnedPhysicsBody != null)
			{
				this.rigOwnedPhysicsBody.enabled = true;
			}
		}

		// Token: 0x06007959 RID: 31065 RVA: 0x00279146 File Offset: 0x00277346
		private void OnDisable()
		{
			if (this.rigOwnedPhysicsBody != null)
			{
				this.rigOwnedPhysicsBody.enabled = false;
			}
		}

		// Token: 0x0600795A RID: 31066 RVA: 0x00279164 File Offset: 0x00277364
		protected internal override void Pickup(bool isAutoPickup = false)
		{
			if (!isAutoPickup)
			{
				UnityEvent onPickupShared = this.OnPickupShared;
				if (onPickupShared != null)
				{
					onPickupShared.Invoke();
				}
			}
			this.rb.linearVelocity = Vector3.zero;
			this.rb.isKinematic = true;
			if (this.holdableParent != null)
			{
				base.transform.parent = this.holdableParent.transform;
			}
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			base.transform.localScale = Vector3.one;
			this.scale = 1f;
			this.placedOnFloorTime = -1f;
			this.placedOnFloor = false;
			this.broken = false;
			this.brokenTime = -1f;
			if (this.isBreakable && this.transferrableParent != null && this.transferrableParent.IsLocalObject())
			{
				int num = (int)this.transferrableParent.itemState;
				num &= ~PickupableCosmetic.breakableBitmask;
				this.transferrableParent.itemState = (TransferrableObject.ItemStates)num;
				if (this.breakEffect != null && this.breakEffect.isPlaying)
				{
					this.breakEffect.Stop();
				}
			}
			this.ShowRenderers(true);
			if (this.interactionPoint != null)
			{
				this.interactionPoint.enabled = true;
			}
			if (this.bodyCollider != null)
			{
				this.bodyCollider.enabled = true;
			}
			base.enabled = false;
		}

		// Token: 0x0600795B RID: 31067 RVA: 0x002792D0 File Offset: 0x002774D0
		protected internal override void DelayedPickup()
		{
			this.DelayedPickup_Internal();
		}

		// Token: 0x0600795C RID: 31068 RVA: 0x002792D8 File Offset: 0x002774D8
		private async void DelayedPickup_Internal()
		{
			await Awaitable.WaitForSecondsAsync(1f, default(CancellationToken));
			this.Pickup(false);
		}

		// Token: 0x0600795D RID: 31069 RVA: 0x00279310 File Offset: 0x00277510
		protected internal override void Release(HoldableObject holdable, Vector3 startPosition, Vector3 velocity, float playerScale)
		{
			this.holdableParent = holdable;
			base.transform.parent = null;
			base.transform.position = startPosition;
			base.transform.localScale = Vector3.one * playerScale;
			this.rb.isKinematic = false;
			this.rb.useGravity = true;
			this.rb.linearVelocity = velocity;
			this.rb.detectCollisions = true;
			if (this.bodyCollider != null)
			{
				this.bodyCollider.enabled = true;
			}
			if (!this.allowPickupFromGround && this.interactionPoint != null)
			{
				this.interactionPoint.enabled = false;
			}
			this.scale = playerScale;
			base.enabled = true;
			this.transferrableParent = this.holdableParent as TransferrableObject;
			this.currentRayIndex = 0;
			this.frameCounter = 0;
		}

		// Token: 0x0600795E RID: 31070 RVA: 0x002793F0 File Offset: 0x002775F0
		private void FixedUpdate()
		{
			if (this.isBreakable && this.broken)
			{
				if (Time.time > this.respawnDelay + this.brokenTime)
				{
					this.Pickup(false);
				}
				return;
			}
			if (this.isBreakable && this.placedOnFloor)
			{
				bool flag = (this.transferrableParent.itemState & (TransferrableObject.ItemStates)PickupableCosmetic.breakableBitmask) > (TransferrableObject.ItemStates)0;
				if (flag != this.broken && flag)
				{
					this.OnBreakReplicated();
				}
			}
			if (this.autoPickupAfterSeconds > 0f && this.placedOnFloor && Time.time - this.placedOnFloorTime > this.autoPickupAfterSeconds)
			{
				this.Pickup(true);
				ThrowablePickupableCosmetic throwablePickupableCosmetic = this.transferrableParent as ThrowablePickupableCosmetic;
				if (throwablePickupableCosmetic)
				{
					UnityEvent onReturnToDockPositionShared = throwablePickupableCosmetic.OnReturnToDockPositionShared;
					if (onReturnToDockPositionShared != null)
					{
						onReturnToDockPositionShared.Invoke();
					}
				}
			}
			if (this.autoPickupDistance > 0f && this.transferrableParent != null && (this.transferrableParent.ownerRig.transform.position - base.transform.position).IsLongerThan(this.autoPickupDistance))
			{
				this.Pickup(false);
			}
			if (!this.placedOnFloor && base.enabled)
			{
				this.frameCounter++;
				if (this.frameCounter % this.stepEveryNFrames != 0)
				{
					return;
				}
				float num = this.RaycastCheckDist * this.scale;
				int value = this.floorLayerMask.value;
				Vector3[] cachedDirections = this.GetCachedDirections(this.RaycastChecksMax);
				int num2 = 0;
				while (num2 < this.raysPerStep && this.currentRayIndex < cachedDirections.Length)
				{
					Vector3 vector = cachedDirections[this.currentRayIndex];
					this.currentRayIndex++;
					num2++;
					RaycastHit raycastHit;
					if (Physics.Raycast(this.GetSafeRayOrigin(this.raycastOrigin.position, vector), vector, out raycastHit, num, value, QueryTriggerInteraction.Ignore) && (!this.dontStickToWall || Vector3.Angle(raycastHit.normal, Vector3.up) < 40f))
					{
						this.SettleBanner(raycastHit);
						UnityEvent onPlacedShared = this.OnPlacedShared;
						if (onPlacedShared != null)
						{
							onPlacedShared.Invoke();
						}
						this.placedOnFloor = true;
						this.placedOnFloorTime = Time.time;
						break;
					}
				}
				if (this.currentRayIndex >= cachedDirections.Length)
				{
					this.currentRayIndex = 0;
				}
			}
		}

		// Token: 0x0600795F RID: 31071 RVA: 0x00279630 File Offset: 0x00277830
		private void SettleBanner(RaycastHit hitInfo)
		{
			this.rb.isKinematic = true;
			this.rb.useGravity = false;
			if (this.bodyCollider != null)
			{
				this.bodyCollider.enabled = false;
			}
			Vector3 normal = hitInfo.normal;
			base.transform.position = hitInfo.point + normal * this.placementOffset;
			Quaternion quaternion = Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, normal).normalized, normal);
			base.transform.rotation = quaternion;
		}

		// Token: 0x06007960 RID: 31072 RVA: 0x002796C8 File Offset: 0x002778C8
		private Vector3 GetFibonacciSphereDirection(int index, int total)
		{
			float num = Mathf.Acos(1f - 2f * ((float)index + 0.5f) / (float)total);
			float num2 = 3.1415927f * (1f + Mathf.Sqrt(5f)) * ((float)index + 0.5f);
			float num3 = Mathf.Sin(num) * Mathf.Cos(num2);
			float num4 = Mathf.Sin(num) * Mathf.Sin(num2);
			float num5 = Mathf.Cos(num);
			return new Vector3(num3, num4, num5).normalized;
		}

		// Token: 0x06007961 RID: 31073 RVA: 0x00279744 File Offset: 0x00277944
		private Vector3[] GetCachedDirections(int count)
		{
			if (count <= 0)
			{
				return PickupableCosmetic.tmpEmpty;
			}
			Vector3[] array;
			if (PickupableCosmetic.directionCache.TryGetValue(count, out array))
			{
				return array;
			}
			array = new Vector3[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = this.GetFibonacciSphereDirection(i, count);
			}
			PickupableCosmetic.directionCache[count] = array;
			return array;
		}

		// Token: 0x06007962 RID: 31074 RVA: 0x0027979C File Offset: 0x0027799C
		private Vector3 GetSafeRayOrigin(Vector3 rawOrigin, Vector3 dir)
		{
			float num = this.selfSkinOffset;
			if (this.bodyCollider != null)
			{
				float magnitude = this.bodyCollider.bounds.extents.magnitude;
				num = Mathf.Max(this.selfSkinOffset, magnitude * 0.05f);
			}
			return rawOrigin - dir.normalized * num;
		}

		// Token: 0x06007963 RID: 31075 RVA: 0x00279800 File Offset: 0x00277A00
		public void BreakPlaceable()
		{
			if (!this.isBreakable || !this.placedOnFloor)
			{
				return;
			}
			if (this.transferrableParent != null && this.transferrableParent.IsLocalObject())
			{
				int num = (int)this.transferrableParent.itemState;
				num |= PickupableCosmetic.breakableBitmask;
				this.transferrableParent.itemState = (TransferrableObject.ItemStates)num;
				return;
			}
			GTDev.LogError<string>("PickupableCosmetic " + base.gameObject.name + " has no TransferrableObject parent. Break effects cannot be replicated", null);
		}

		// Token: 0x06007964 RID: 31076 RVA: 0x0027987A File Offset: 0x00277A7A
		private void OnBreakReplicated()
		{
			this.PlayBreakEffects();
		}

		// Token: 0x06007965 RID: 31077 RVA: 0x00279884 File Offset: 0x00277A84
		protected virtual void PlayBreakEffects()
		{
			if (!this.isBreakable || !this.placedOnFloor || this.broken)
			{
				return;
			}
			this.broken = true;
			this.brokenTime = Time.time;
			if (this.breakEffect != null)
			{
				if (this.breakEffect.isPlaying)
				{
					this.breakEffect.Stop();
				}
				this.breakEffect.Play();
			}
			if (this.interactionPoint != null)
			{
				this.interactionPoint.enabled = false;
			}
			this.ShowRenderers(false);
			UnityEvent onBrokenShared = this.OnBrokenShared;
			if (onBrokenShared == null)
			{
				return;
			}
			onBrokenShared.Invoke();
		}

		// Token: 0x06007966 RID: 31078 RVA: 0x00279920 File Offset: 0x00277B20
		protected virtual void ShowRenderers(bool visible)
		{
			if (this.hideOnBreak.IsNullOrEmpty<Renderer>())
			{
				return;
			}
			for (int i = 0; i < this.hideOnBreak.Length; i++)
			{
				Renderer renderer = this.hideOnBreak[i];
				if (!(renderer == null))
				{
					renderer.forceRenderingOff = !visible;
				}
			}
		}

		// Token: 0x04008A73 RID: 35443
		[SerializeField]
		private InteractionPoint interactionPoint;

		// Token: 0x04008A74 RID: 35444
		[SerializeField]
		private Rigidbody rb;

		// Token: 0x04008A75 RID: 35445
		[SerializeField]
		private Transform raycastOrigin;

		// Token: 0x04008A76 RID: 35446
		[Tooltip("Allow player to grab the placed object")]
		[SerializeField]
		private bool allowPickupFromGround = true;

		// Token: 0x04008A77 RID: 35447
		[SerializeField]
		private float autoPickupAfterSeconds;

		// Token: 0x04008A78 RID: 35448
		[SerializeField]
		private float autoPickupDistance;

		// Token: 0x04008A79 RID: 35449
		[Tooltip("Amount to offset the placed object from the hit position in the hit normal direction")]
		[SerializeField]
		private float placementOffset;

		// Token: 0x04008A7A RID: 35450
		[Tooltip("Prevent sticking if the hit surface normal is not within 40 degrees of world up")]
		[SerializeField]
		private bool dontStickToWall;

		// Token: 0x04008A7B RID: 35451
		[Tooltip("Layers to raycast against for placement")]
		[SerializeField]
		private LayerMask floorLayerMask = 134218241;

		// Token: 0x04008A7C RID: 35452
		[Tooltip("The distance to check if the banner is close to the floor (from a raycast check).")]
		public float RaycastCheckDist = 0.2f;

		// Token: 0x04008A7D RID: 35453
		[Tooltip("How many checks should we attempt for a raycast.")]
		public int RaycastChecksMax = 12;

		// Token: 0x04008A7E RID: 35454
		[FormerlySerializedAs("OnPickup")]
		[Space]
		public UnityEvent OnPickupShared;

		// Token: 0x04008A7F RID: 35455
		[FormerlySerializedAs("OnPlaced")]
		public UnityEvent OnPlacedShared;

		// Token: 0x04008A80 RID: 35456
		[SerializeField]
		private bool isBreakable;

		// Token: 0x04008A81 RID: 35457
		[Tooltip("Particle system played OnBrokenShared")]
		[SerializeField]
		private ParticleSystem breakEffect;

		// Token: 0x04008A82 RID: 35458
		[Tooltip("Renderers disabled OnBrokenShared and enabled OnPickupShared")]
		[SerializeField]
		private Renderer[] hideOnBreak = new Renderer[0];

		// Token: 0x04008A83 RID: 35459
		[Tooltip("Time after BreakPlaceable to reset item")]
		[SerializeField]
		private float respawnDelay = 0.5f;

		// Token: 0x04008A84 RID: 35460
		[FormerlySerializedAs("OnBroken")]
		[Space]
		public UnityEvent OnBrokenShared;

		// Token: 0x04008A85 RID: 35461
		private static int breakableBitmask = 32;

		// Token: 0x04008A86 RID: 35462
		private bool placedOnFloor;

		// Token: 0x04008A87 RID: 35463
		private float placedOnFloorTime = -1f;

		// Token: 0x04008A88 RID: 35464
		private bool broken;

		// Token: 0x04008A89 RID: 35465
		private float brokenTime = -1f;

		// Token: 0x04008A8A RID: 35466
		private VRRig cachedLocalRig;

		// Token: 0x04008A8B RID: 35467
		private HoldableObject holdableParent;

		// Token: 0x04008A8C RID: 35468
		private TransferrableObject transferrableParent;

		// Token: 0x04008A8D RID: 35469
		private RigOwnedPhysicsBody rigOwnedPhysicsBody;

		// Token: 0x04008A8E RID: 35470
		private double throwSettledTime = -1.0;

		// Token: 0x04008A8F RID: 35471
		private int landingSide;

		// Token: 0x04008A90 RID: 35472
		private float scale;

		// Token: 0x04008A91 RID: 35473
		private Collider bodyCollider;

		// Token: 0x04008A92 RID: 35474
		[Tooltip("How many directions to test per physics tick (spreads work across frames).")]
		[SerializeField]
		[Min(1f)]
		private int raysPerStep = 3;

		// Token: 0x04008A93 RID: 35475
		[Tooltip("Run a raycast step only every N physics ticks (1 = every FixedUpdate).")]
		[SerializeField]
		[Min(1f)]
		private int stepEveryNFrames = 2;

		// Token: 0x04008A94 RID: 35476
		[Tooltip("Small skin so rays start just outside our own collider volume.")]
		[SerializeField]
		[Range(0.005f, 0.1f)]
		private float selfSkinOffset = 0.02f;

		// Token: 0x04008A95 RID: 35477
		[SerializeField]
		private bool debugPlacementRays;

		// Token: 0x04008A96 RID: 35478
		private int currentRayIndex;

		// Token: 0x04008A97 RID: 35479
		private int frameCounter;

		// Token: 0x04008A98 RID: 35480
		private static readonly Dictionary<int, Vector3[]> directionCache = new Dictionary<int, Vector3[]>();

		// Token: 0x04008A99 RID: 35481
		private static readonly Vector3[] tmpEmpty = Array.Empty<Vector3>();
	}
}
