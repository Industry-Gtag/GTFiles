using System;
using System.Collections;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.Shared.Scripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012F4 RID: 4852
	public class ThrowableHoldableCosmetic : TransferrableObject
	{
		// Token: 0x0600799F RID: 31135 RVA: 0x0027A8D4 File Offset: 0x00278AD4
		internal override void OnEnable()
		{
			base.OnEnable();
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = ((base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null));
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnThrowEvent;
			}
			this.forceBackToDock = false;
		}

		// Token: 0x060079A0 RID: 31136 RVA: 0x0027A9A0 File Offset: 0x00278BA0
		protected override void Awake()
		{
			base.Awake();
			this.projectileHash = PoolUtils.GameObjHashCode(this.projectilePrefab);
			if (this.alternativeProjectilePrefab != null)
			{
				this.alternativeProjectileHash = PoolUtils.GameObjHashCode(this.alternativeProjectilePrefab);
			}
			this.currentProjectileHash = this.projectileHash;
			this.playersEffect = base.GetComponentInChildren<CosmeticEffectsOnPlayers>();
			this.respawnWait = new WaitForSeconds(this.respawnCooldown);
		}

		// Token: 0x060079A1 RID: 31137 RVA: 0x0027AA0C File Offset: 0x00278C0C
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (!this.disableWhenThrown.gameObject.activeSelf)
			{
				return;
			}
			base.OnGrab(pointGrabbed, grabbingHand);
		}

		// Token: 0x060079A2 RID: 31138 RVA: 0x0027AA2C File Offset: 0x00278C2C
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
			{
				return false;
			}
			Vector3 position = base.transform.position;
			Quaternion rotation = base.transform.rotation;
			bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
			Vector3 averageVelocity = GTPlayer.Instance.GetInteractPointVelocityTracker(flag).GetAverageVelocity(true, 0.15f, false);
			float scale = GTPlayer.Instance.scale;
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { position, rotation, averageVelocity, scale });
			}
			this.OnThrowLocal(position, rotation, averageVelocity, this.ownerRig);
			return true;
		}

		// Token: 0x060079A3 RID: 31139 RVA: 0x0027AB28 File Offset: 0x00278D28
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= this.OnThrowEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x060079A4 RID: 31140 RVA: 0x0027AB7D File Offset: 0x00278D7D
		public void UseAlternativeProjectile()
		{
			if (this.alternativeProjectilePrefab != null)
			{
				this.currentProjectileHash = this.alternativeProjectileHash;
			}
		}

		// Token: 0x060079A5 RID: 31141 RVA: 0x0027AB99 File Offset: 0x00278D99
		public void ForceBackToDock()
		{
			this.forceBackToDock = true;
		}

		// Token: 0x060079A6 RID: 31142 RVA: 0x0027ABA2 File Offset: 0x00278DA2
		private IEnumerator ReEnableAfterDelay(GameObject obj)
		{
			yield return this.respawnWait;
			obj.SetActive(true);
			yield break;
		}

		// Token: 0x060079A7 RID: 31143 RVA: 0x0027ABB8 File Offset: 0x00278DB8
		private void OnThrowEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (args.Length != 4)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnThrowEvent");
			if (this.firecrackerCallLimiter.CheckCallTime(Time.time))
			{
				object obj = args[0];
				if (obj is Vector3)
				{
					Vector3 vector = (Vector3)obj;
					obj = args[1];
					if (obj is Quaternion)
					{
						Quaternion quaternion = (Quaternion)obj;
						obj = args[2];
						if (obj is Vector3)
						{
							Vector3 vector2 = (Vector3)obj;
							obj = args[3];
							if (obj is float)
							{
								float num = (float)obj;
								vector2 = this.targetRig.ClampVelocityRelativeToPlayerSafe(vector2, 40f, 100f);
								num.ClampSafe(0.01f, 1f);
								if (!(in quaternion).IsValid())
								{
									return;
								}
								float num2 = 10000f;
								if (!(in vector).IsValid(in num2) || !this.targetRig.IsPositionInRange(vector, 4f))
								{
									return;
								}
								this.OnThrowLocal(vector, quaternion, vector2, this.ownerRig);
								return;
							}
						}
					}
				}
			}
		}

		// Token: 0x060079A8 RID: 31144 RVA: 0x0027ACCC File Offset: 0x00278ECC
		private void OnThrowLocal(Vector3 startPos, Quaternion rotation, Vector3 velocity, VRRig ownerRig)
		{
			this.disableWhenThrown.SetActive(false);
			if (this.forceBackToDock)
			{
				this.forceBackToDock = false;
				base.StartCoroutine(this.ReEnableAfterDelay(this.disableWhenThrown));
				return;
			}
			IProjectile component = ObjectPools.instance.Instantiate(this.currentProjectileHash, true).GetComponent<IProjectile>();
			FirecrackerProjectile firecrackerProjectile = component as FirecrackerProjectile;
			if (firecrackerProjectile != null)
			{
				if (this.networkedStateEvents != TransferrableObject.SyncOptions.None)
				{
					int num = (int)(this.itemState & (TransferrableObject.ItemStates)(-65));
					firecrackerProjectile.SetTransferrableState(this.networkedStateEvents, num);
				}
				firecrackerProjectile.OnDetonationComplete.AddListener(new UnityAction<FirecrackerProjectile>(this.HitComplete));
				firecrackerProjectile.OnDetonationStart.AddListener(new UnityAction<FirecrackerProjectile, Vector3>(this.HitStart));
			}
			else
			{
				FartBagThrowable fartBagThrowable = component as FartBagThrowable;
				if (fartBagThrowable != null)
				{
					fartBagThrowable.OnDeflated += this.HitComplete;
					fartBagThrowable.ParentTransferable = this;
				}
			}
			component.Launch(startPos, rotation, velocity, 1f, ownerRig, -1);
			this.currentProjectileHash = this.projectileHash;
		}

		// Token: 0x060079A9 RID: 31145 RVA: 0x0027ADB8 File Offset: 0x00278FB8
		private void HitStart(FirecrackerProjectile firecracker, Vector3 contactPos)
		{
			if (firecracker == null)
			{
				return;
			}
			if (this.playersEffect == null)
			{
				return;
			}
			this.playersEffect.ApplyAllEffectsByDistance(contactPos);
		}

		// Token: 0x060079AA RID: 31146 RVA: 0x0027ADE0 File Offset: 0x00278FE0
		private void HitComplete(IProjectile projectile)
		{
			if (projectile == null)
			{
				return;
			}
			if (base.IsLocalObject() && this.networkedStateEvents != TransferrableObject.SyncOptions.None && this.resetOnDocked)
			{
				TransferrableObject.SyncOptions networkedStateEvents = this.networkedStateEvents;
				if (networkedStateEvents != TransferrableObject.SyncOptions.Bool)
				{
					if (networkedStateEvents == TransferrableObject.SyncOptions.Int)
					{
						base.SetItemStateInt(0);
					}
				}
				else
				{
					base.ResetStateBools();
				}
			}
			FirecrackerProjectile firecrackerProjectile = projectile as FirecrackerProjectile;
			if (firecrackerProjectile != null)
			{
				firecrackerProjectile.OnDetonationStart.RemoveListener(new UnityAction<FirecrackerProjectile, Vector3>(this.HitStart));
				firecrackerProjectile.OnDetonationComplete.RemoveListener(new UnityAction<FirecrackerProjectile>(this.HitComplete));
				ObjectPools.instance.Destroy(firecrackerProjectile.gameObject);
			}
			else
			{
				FartBagThrowable fartBagThrowable = projectile as FartBagThrowable;
				if (fartBagThrowable != null)
				{
					fartBagThrowable.OnDeflated -= this.HitComplete;
					ObjectPools.instance.Destroy(fartBagThrowable.gameObject);
				}
			}
			base.StartCoroutine(this.ReEnableAfterDelay(this.disableWhenThrown));
		}

		// Token: 0x04008AEA RID: 35562
		[Tooltip("Projectile prefab from the global object pool that gets spawned when this object is thrown")]
		[FormerlySerializedAs("firecrackerProjectilePrefab")]
		[SerializeField]
		private GameObject projectilePrefab;

		// Token: 0x04008AEB RID: 35563
		[Tooltip(" A second projectile prefab that will be spawned if UseAlternativeProjectile is called")]
		[SerializeField]
		private GameObject alternativeProjectilePrefab;

		// Token: 0x04008AEC RID: 35564
		[Tooltip("Objects on the body that should be hidden when the projectile is spawned")]
		[SerializeField]
		private GameObject disableWhenThrown;

		// Token: 0x04008AED RID: 35565
		private CallLimiter firecrackerCallLimiter = new CallLimiter(10, 3f, 0.5f);

		// Token: 0x04008AEE RID: 35566
		[SerializeField]
		private float respawnCooldown = 1f;

		// Token: 0x04008AEF RID: 35567
		private CosmeticEffectsOnPlayers playersEffect;

		// Token: 0x04008AF0 RID: 35568
		private int projectileHash;

		// Token: 0x04008AF1 RID: 35569
		private int alternativeProjectileHash;

		// Token: 0x04008AF2 RID: 35570
		private int currentProjectileHash;

		// Token: 0x04008AF3 RID: 35571
		private bool forceBackToDock;

		// Token: 0x04008AF4 RID: 35572
		private WaitForSeconds respawnWait;

		// Token: 0x04008AF5 RID: 35573
		private RubberDuckEvents _events;
	}
}
