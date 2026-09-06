using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012F0 RID: 4848
	[RequireComponent(typeof(TransferrableObject))]
	public class SeedPacketHoldable : MonoBehaviour
	{
		// Token: 0x06007978 RID: 31096 RVA: 0x00279F2A File Offset: 0x0027812A
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
			this.flowerEffectHash = PoolUtils.GameObjHashCode(this.flowerEffectPrefab);
		}

		// Token: 0x06007979 RID: 31097 RVA: 0x00279F4C File Offset: 0x0027814C
		private void OnEnable()
		{
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = ((this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? (this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null));
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.SyncTriggerEffect;
			}
		}

		// Token: 0x0600797A RID: 31098 RVA: 0x0027A014 File Offset: 0x00278214
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.SyncTriggerEffect;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x0600797B RID: 31099 RVA: 0x0027A063 File Offset: 0x00278263
		private void OnDestroy()
		{
			this.pooledObjects.Clear();
		}

		// Token: 0x0600797C RID: 31100 RVA: 0x0027A070 File Offset: 0x00278270
		private void Update()
		{
			if (!this.transferrableObject.InHand())
			{
				return;
			}
			if (!this.isPouring && Vector3.Angle(base.transform.up, Vector3.down) <= this.pouringAngle)
			{
				this.StartPouring();
				RaycastHit raycastHit;
				if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, this.pouringRaycastDistance, this.raycastLayerMask))
				{
					this.hitPoint = raycastHit.point;
					base.Invoke("SpawnEffect", raycastHit.distance * this.placeEffectDelayMultiplier);
				}
			}
			if (this.isPouring && Time.time - this.pouringStartedTime >= this.cooldown)
			{
				this.isPouring = false;
			}
		}

		// Token: 0x0600797D RID: 31101 RVA: 0x0027A129 File Offset: 0x00278329
		private void StartPouring()
		{
			if (this.particles)
			{
				this.particles.Play();
			}
			this.isPouring = true;
			this.pouringStartedTime = Time.time;
		}

		// Token: 0x0600797E RID: 31102 RVA: 0x0027A158 File Offset: 0x00278358
		private void SpawnEffect()
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(this.flowerEffectHash, true);
			gameObject.transform.position = this.hitPoint;
			SeedPacketTriggerHandler seedPacketTriggerHandler;
			if (gameObject.TryGetComponent<SeedPacketTriggerHandler>(out seedPacketTriggerHandler))
			{
				this.pooledObjects.Add(seedPacketTriggerHandler);
				seedPacketTriggerHandler.onTriggerEntered.AddListener(new UnityAction<SeedPacketTriggerHandler>(this.SyncTriggerEffectForOthers));
			}
		}

		// Token: 0x0600797F RID: 31103 RVA: 0x0027A1B4 File Offset: 0x002783B4
		private void SyncTriggerEffectForOthers(SeedPacketTriggerHandler seedPacketTriggerHandlerTriggerHandlerEvent)
		{
			int num = this.pooledObjects.IndexOf(seedPacketTriggerHandlerTriggerHandlerEvent);
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { num });
			}
		}

		// Token: 0x06007980 RID: 31104 RVA: 0x0027A218 File Offset: 0x00278418
		private void SyncTriggerEffect(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (args.Length != 1)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "SyncTriggerEffect");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			int num = (int)args[0];
			if (num < 0 && num >= this.pooledObjects.Count)
			{
				return;
			}
			this.pooledObjects[num].ToggleEffects();
		}

		// Token: 0x04008AB7 RID: 35511
		[SerializeField]
		private float cooldown;

		// Token: 0x04008AB8 RID: 35512
		[SerializeField]
		private ParticleSystem particles;

		// Token: 0x04008AB9 RID: 35513
		[SerializeField]
		private float pouringAngle;

		// Token: 0x04008ABA RID: 35514
		[SerializeField]
		private float pouringRaycastDistance = 5f;

		// Token: 0x04008ABB RID: 35515
		[SerializeField]
		private LayerMask raycastLayerMask;

		// Token: 0x04008ABC RID: 35516
		[SerializeField]
		private float placeEffectDelayMultiplier = 10f;

		// Token: 0x04008ABD RID: 35517
		[SerializeField]
		private GameObject flowerEffectPrefab;

		// Token: 0x04008ABE RID: 35518
		private List<SeedPacketTriggerHandler> pooledObjects = new List<SeedPacketTriggerHandler>();

		// Token: 0x04008ABF RID: 35519
		private CallLimiter callLimiter = new CallLimiter(10, 3f, 0.5f);

		// Token: 0x04008AC0 RID: 35520
		private int flowerEffectHash;

		// Token: 0x04008AC1 RID: 35521
		private Vector3 hitPoint;

		// Token: 0x04008AC2 RID: 35522
		private TransferrableObject transferrableObject;

		// Token: 0x04008AC3 RID: 35523
		private bool isPouring = true;

		// Token: 0x04008AC4 RID: 35524
		private float pouringStartedTime;

		// Token: 0x04008AC5 RID: 35525
		private RubberDuckEvents _events;
	}
}
