using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001335 RID: 4917
	public class FartBagThrowable : MonoBehaviour, IProjectile
	{
		// Token: 0x17000C15 RID: 3093
		// (get) Token: 0x06007B6F RID: 31599 RVA: 0x00285093 File Offset: 0x00283293
		// (set) Token: 0x06007B70 RID: 31600 RVA: 0x0028509B File Offset: 0x0028329B
		public TransferrableObject ParentTransferable { get; set; }

		// Token: 0x140000CB RID: 203
		// (add) Token: 0x06007B71 RID: 31601 RVA: 0x002850A4 File Offset: 0x002832A4
		// (remove) Token: 0x06007B72 RID: 31602 RVA: 0x002850DC File Offset: 0x002832DC
		public event Action<IProjectile> OnDeflated;

		// Token: 0x06007B73 RID: 31603 RVA: 0x00285114 File Offset: 0x00283314
		private void OnEnable()
		{
			this.placedOnFloor = false;
			this.deflated = false;
			this.handContactPoint = Vector3.negativeInfinity;
			this.handNormalVector = Vector3.zero;
			this.timeCreated = float.PositiveInfinity;
			this.placedOnFloorTime = float.PositiveInfinity;
			if (this.updateBlendShapeCosmetic)
			{
				this.updateBlendShapeCosmetic.ResetBlend();
			}
		}

		// Token: 0x06007B74 RID: 31604 RVA: 0x00285173 File Offset: 0x00283373
		private void Update()
		{
			if (Time.time - this.timeCreated > this.forceDestroyAfterSec)
			{
				this.DeflateLocal();
			}
		}

		// Token: 0x06007B75 RID: 31605 RVA: 0x00285190 File Offset: 0x00283390
		public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
		{
			base.transform.position = startPosition;
			base.transform.rotation = startRotation;
			base.transform.localScale = Vector3.one * ownerRig.scaleFactor;
			this.rigidbody.linearVelocity = velocity;
			this.timeCreated = Time.time;
			this.InitialPhotonEvent();
		}

		// Token: 0x06007B76 RID: 31606 RVA: 0x002851F0 File Offset: 0x002833F0
		private void InitialPhotonEvent()
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			if (this.ParentTransferable)
			{
				NetPlayer netPlayer = ((this.ParentTransferable.myOnlineRig != null) ? this.ParentTransferable.myOnlineRig.creator : ((this.ParentTransferable.myRig != null) ? (this.ParentTransferable.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null));
				if (this._events != null && netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.DeflateEvent;
			}
		}

		// Token: 0x06007B77 RID: 31607 RVA: 0x002852C4 File Offset: 0x002834C4
		private void OnTriggerEnter(Collider other)
		{
			if ((this.handLayerMask.value & (1 << other.gameObject.layer)) != 0)
			{
				if (!this.placedOnFloor)
				{
					return;
				}
				this.handContactPoint = other.ClosestPoint(base.transform.position);
				this.handNormalVector = (this.handContactPoint - base.transform.position).normalized;
				if (Time.time - this.placedOnFloorTime > 0.3f)
				{
					this.Deflate();
				}
			}
		}

		// Token: 0x06007B78 RID: 31608 RVA: 0x0028534C File Offset: 0x0028354C
		private void OnCollisionEnter(Collision other)
		{
			if ((this.floorLayerMask.value & (1 << other.gameObject.layer)) != 0)
			{
				this.placedOnFloor = true;
				this.placedOnFloorTime = Time.time;
				Vector3 normal = other.contacts[0].normal;
				base.transform.position = other.contacts[0].point + normal * this.placementOffset;
				Quaternion quaternion = Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, normal).normalized, normal);
				base.transform.rotation = quaternion;
			}
		}

		// Token: 0x06007B79 RID: 31609 RVA: 0x002853F4 File Offset: 0x002835F4
		private void Deflate()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { this.handContactPoint, this.handNormalVector });
			}
			this.DeflateLocal();
		}

		// Token: 0x06007B7A RID: 31610 RVA: 0x00285464 File Offset: 0x00283664
		private void DeflateEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (args.Length != 2)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "DeflateEvent");
			if (this.callLimiter.CheckCallTime(Time.time))
			{
				object obj = args[0];
				if (obj is Vector3)
				{
					Vector3 vector = (Vector3)obj;
					obj = args[1];
					if (obj is Vector3)
					{
						Vector3 vector2 = (Vector3)obj;
						float num = 10000f;
						if (!(in vector2).IsValid(in num))
						{
							return;
						}
						num = 10000f;
						if (!(in vector).IsValid(in num) || !this.ParentTransferable.targetRig.IsPositionInRange(vector, 4f))
						{
							return;
						}
						this.handNormalVector = vector2;
						this.handContactPoint = vector;
						this.DeflateLocal();
						return;
					}
				}
			}
		}

		// Token: 0x06007B7B RID: 31611 RVA: 0x00285514 File Offset: 0x00283714
		private void DeflateLocal()
		{
			if (this.deflated)
			{
				return;
			}
			GameObject gameObject = ObjectPools.instance.Instantiate(this.deflationEffect, this.handContactPoint, true);
			gameObject.transform.up = this.handNormalVector;
			gameObject.transform.position = base.transform.position;
			SoundBankPlayer componentInChildren = gameObject.GetComponentInChildren<SoundBankPlayer>();
			if (componentInChildren.soundBank)
			{
				componentInChildren.Play();
			}
			this.placedOnFloor = false;
			this.timeCreated = float.PositiveInfinity;
			if (this.updateBlendShapeCosmetic)
			{
				this.updateBlendShapeCosmetic.FullyBlend();
			}
			this.deflated = true;
			base.Invoke("DisableObject", this.destroyWhenDeflateDelay);
		}

		// Token: 0x06007B7C RID: 31612 RVA: 0x002855C3 File Offset: 0x002837C3
		private void DisableObject()
		{
			Action<IProjectile> onDeflated = this.OnDeflated;
			if (onDeflated != null)
			{
				onDeflated(this);
			}
			this.deflated = false;
		}

		// Token: 0x06007B7D RID: 31613 RVA: 0x002855E0 File Offset: 0x002837E0
		private void OnDestroy()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.DeflateEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x04008D47 RID: 36167
		[SerializeField]
		private GameObject deflationEffect;

		// Token: 0x04008D48 RID: 36168
		[SerializeField]
		private float destroyWhenDeflateDelay = 3f;

		// Token: 0x04008D49 RID: 36169
		[SerializeField]
		private float forceDestroyAfterSec = 10f;

		// Token: 0x04008D4A RID: 36170
		[SerializeField]
		private float placementOffset = 0.2f;

		// Token: 0x04008D4B RID: 36171
		[SerializeField]
		private UpdateBlendShapeCosmetic updateBlendShapeCosmetic;

		// Token: 0x04008D4C RID: 36172
		[SerializeField]
		private LayerMask floorLayerMask;

		// Token: 0x04008D4D RID: 36173
		[SerializeField]
		private LayerMask handLayerMask;

		// Token: 0x04008D4E RID: 36174
		[SerializeField]
		private Rigidbody rigidbody;

		// Token: 0x04008D4F RID: 36175
		private bool placedOnFloor;

		// Token: 0x04008D50 RID: 36176
		private float placedOnFloorTime;

		// Token: 0x04008D51 RID: 36177
		private float timeCreated;

		// Token: 0x04008D52 RID: 36178
		private bool deflated;

		// Token: 0x04008D53 RID: 36179
		private Vector3 handContactPoint;

		// Token: 0x04008D54 RID: 36180
		private Vector3 handNormalVector;

		// Token: 0x04008D55 RID: 36181
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x04008D58 RID: 36184
		private RubberDuckEvents _events;
	}
}
