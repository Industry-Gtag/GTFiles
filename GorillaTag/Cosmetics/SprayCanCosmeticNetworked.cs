using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001360 RID: 4960
	public class SprayCanCosmeticNetworked : MonoBehaviour
	{
		// Token: 0x06007C53 RID: 31827 RVA: 0x00289DB0 File Offset: 0x00287FB0
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
				this._events.Activate += this.OnShakeEvent;
			}
		}

		// Token: 0x06007C54 RID: 31828 RVA: 0x00289E78 File Offset: 0x00288078
		private void OnDisable()
		{
			if (this._events != null)
			{
				if (this._events.Activate != null)
				{
					this._events.Activate -= this.OnShakeEvent;
				}
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06007C55 RID: 31829 RVA: 0x00289EDC File Offset: 0x002880DC
		private void OnShakeEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnShakeEvent");
			NetPlayer sender2 = info.Sender;
			VRRig myOnlineRig = this.transferrableObject.myOnlineRig;
			if (sender2 != ((myOnlineRig != null) ? myOnlineRig.creator : null))
			{
				return;
			}
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			object obj = args[0];
			if (!(obj is bool))
			{
				return;
			}
			bool flag = (bool)obj;
			if (flag)
			{
				UnityEvent handleOnShakeStart = this.HandleOnShakeStart;
				if (handleOnShakeStart == null)
				{
					return;
				}
				handleOnShakeStart.Invoke();
				return;
			}
			else
			{
				UnityEvent handleOnShakeEnd = this.HandleOnShakeEnd;
				if (handleOnShakeEnd == null)
				{
					return;
				}
				handleOnShakeEnd.Invoke();
				return;
			}
		}

		// Token: 0x06007C56 RID: 31830 RVA: 0x00289F68 File Offset: 0x00288168
		public void OnShakeStart()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { true });
			}
			UnityEvent handleOnShakeStart = this.HandleOnShakeStart;
			if (handleOnShakeStart == null)
			{
				return;
			}
			handleOnShakeStart.Invoke();
		}

		// Token: 0x06007C57 RID: 31831 RVA: 0x00289FCC File Offset: 0x002881CC
		public void OnShakeEnd()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { false });
			}
			UnityEvent handleOnShakeEnd = this.HandleOnShakeEnd;
			if (handleOnShakeEnd == null)
			{
				return;
			}
			handleOnShakeEnd.Invoke();
		}

		// Token: 0x04008EA8 RID: 36520
		[SerializeField]
		private TransferrableObject transferrableObject;

		// Token: 0x04008EA9 RID: 36521
		private RubberDuckEvents _events;

		// Token: 0x04008EAA RID: 36522
		private CallLimiter callLimiter = new CallLimiter(10, 1f, 0.5f);

		// Token: 0x04008EAB RID: 36523
		public UnityEvent HandleOnShakeStart;

		// Token: 0x04008EAC RID: 36524
		public UnityEvent HandleOnShakeEnd;
	}
}
