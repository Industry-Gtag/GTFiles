using System;
using System.Collections;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200135E RID: 4958
	public class SnakeInCanHoldable : TransferrableObject
	{
		// Token: 0x06007C44 RID: 31812 RVA: 0x002899C2 File Offset: 0x00287BC2
		protected override void Awake()
		{
			base.Awake();
			this.topRigPosition = this.topRigObject.transform.position;
		}

		// Token: 0x06007C45 RID: 31813 RVA: 0x002899E0 File Offset: 0x00287BE0
		internal override void OnEnable()
		{
			base.OnEnable();
			this.disableObjectBeforeTrigger.SetActive(false);
			if (this.compressedPoint != null)
			{
				this.topRigObject.transform.position = this.compressedPoint.position;
			}
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
				this._events.Activate += this.OnEnableObject;
			}
		}

		// Token: 0x06007C46 RID: 31814 RVA: 0x00289AD8 File Offset: 0x00287CD8
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= this.OnEnableObject;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06007C47 RID: 31815 RVA: 0x00289B30 File Offset: 0x00287D30
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
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { false });
			}
			this.EnableObjectLocal(false);
			return true;
		}

		// Token: 0x06007C48 RID: 31816 RVA: 0x00289BB8 File Offset: 0x00287DB8
		private void OnEnableObject(int sender, int target, object[] arg, PhotonMessageInfoWrapped info)
		{
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			if (arg.Length != 1 || !(arg[0] is bool))
			{
				return;
			}
			if (sender != target)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnEnableObject");
			if (!this.snakeInCanCallLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			bool flag = (bool)arg[0];
			this.EnableObjectLocal(flag);
		}

		// Token: 0x06007C49 RID: 31817 RVA: 0x00289C24 File Offset: 0x00287E24
		private void EnableObjectLocal(bool enable)
		{
			this.disableObjectBeforeTrigger.SetActive(enable);
			if (!enable)
			{
				if (this.compressedPoint != null)
				{
					this.topRigObject.transform.position = this.compressedPoint.position;
				}
				return;
			}
			if (this.stretchedPoint != null)
			{
				base.StartCoroutine(this.SmoothTransition());
				return;
			}
			this.topRigObject.transform.position = this.topRigPosition;
		}

		// Token: 0x06007C4A RID: 31818 RVA: 0x00289C9C File Offset: 0x00287E9C
		private IEnumerator SmoothTransition()
		{
			while (Vector3.Distance(this.topRigObject.transform.position, this.stretchedPoint.position) > 0.01f)
			{
				this.topRigObject.transform.position = Vector3.MoveTowards(this.topRigObject.transform.position, this.stretchedPoint.position, this.jumpSpeed * Time.deltaTime);
				yield return null;
			}
			this.topRigObject.transform.position = this.stretchedPoint.position;
			yield break;
		}

		// Token: 0x06007C4B RID: 31819 RVA: 0x00289CAB File Offset: 0x00287EAB
		public void OnButtonPressed()
		{
			this.EnableObjectLocal(true);
		}

		// Token: 0x04008E9C RID: 36508
		[SerializeField]
		private float jumpSpeed;

		// Token: 0x04008E9D RID: 36509
		[SerializeField]
		private Transform stretchedPoint;

		// Token: 0x04008E9E RID: 36510
		[SerializeField]
		private Transform compressedPoint;

		// Token: 0x04008E9F RID: 36511
		[SerializeField]
		private GameObject topRigObject;

		// Token: 0x04008EA0 RID: 36512
		[SerializeField]
		private GameObject disableObjectBeforeTrigger;

		// Token: 0x04008EA1 RID: 36513
		private CallLimiter snakeInCanCallLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x04008EA2 RID: 36514
		private Vector3 topRigPosition;

		// Token: 0x04008EA3 RID: 36515
		private Vector3 originalTopRigPosition;

		// Token: 0x04008EA4 RID: 36516
		private RubberDuckEvents _events;
	}
}
