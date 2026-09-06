using System;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012FF RID: 4863
	public class ChickenSword : MonoBehaviour
	{
		// Token: 0x06007A16 RID: 31254 RVA: 0x0027D35C File Offset: 0x0027B55C
		private void Awake()
		{
			this.lastHitTime = float.PositiveInfinity;
			this.SwitchState(ChickenSword.SwordState.Ready);
		}

		// Token: 0x06007A17 RID: 31255 RVA: 0x0027D370 File Offset: 0x0027B570
		internal void OnEnable()
		{
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = ((this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? ((this.transferrableObject.myRig.creator != null) ? this.transferrableObject.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null));
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
				else
				{
					Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnReachedLastTransformationStep;
			}
		}

		// Token: 0x06007A18 RID: 31256 RVA: 0x0027D454 File Offset: 0x0027B654
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.OnReachedLastTransformationStep;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06007A19 RID: 31257 RVA: 0x0027D4A4 File Offset: 0x0027B6A4
		private void Update()
		{
			ChickenSword.SwordState swordState = this.currentState;
			if (swordState != ChickenSword.SwordState.Ready)
			{
				if (swordState != ChickenSword.SwordState.Deflated)
				{
					return;
				}
				if (Time.time - this.lastHitTime > this.rechargeCooldown)
				{
					this.lastHitTime = float.PositiveInfinity;
					this.SwitchState(ChickenSword.SwordState.Ready);
					UnityEvent onRechargedShared = this.OnRechargedShared;
					if (onRechargedShared != null)
					{
						onRechargedShared.Invoke();
					}
					if (this.transferrableObject && this.transferrableObject.IsMyItem())
					{
						UnityEvent<bool> onRechargedLocal = this.OnRechargedLocal;
						if (onRechargedLocal == null)
						{
							return;
						}
						onRechargedLocal.Invoke(this.transferrableObject.InLeftHand());
					}
				}
			}
			else if (this.hitReceievd)
			{
				this.hitReceievd = false;
				this.lastHitTime = Time.time;
				this.SwitchState(ChickenSword.SwordState.Deflated);
				UnityEvent onDeflatedShared = this.OnDeflatedShared;
				if (onDeflatedShared != null)
				{
					onDeflatedShared.Invoke();
				}
				if (this.transferrableObject && this.transferrableObject.IsMyItem())
				{
					UnityEvent<bool> onDeflatedLocal = this.OnDeflatedLocal;
					if (onDeflatedLocal == null)
					{
						return;
					}
					onDeflatedLocal.Invoke(this.transferrableObject.InLeftHand());
					return;
				}
			}
		}

		// Token: 0x06007A1A RID: 31258 RVA: 0x0027D5A0 File Offset: 0x0027B7A0
		public void OnHitTargetSync(VRRig playerRig)
		{
			if (this.velocityTracker == null)
			{
				return;
			}
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			if (this.currentState == ChickenSword.SwordState.Ready && averageVelocity.magnitude > this.hitVelocityThreshold)
			{
				this.hitReceievd = true;
				UnityEvent<VRRig> onHitTargetShared = this.OnHitTargetShared;
				if (onHitTargetShared != null)
				{
					onHitTargetShared.Invoke(playerRig);
				}
				if (this.transferrableObject && this.transferrableObject.IsMyItem())
				{
					bool flag = this.transferrableObject.InLeftHand();
					UnityEvent<bool> onHitTargetLocal = this.OnHitTargetLocal;
					if (onHitTargetLocal != null)
					{
						onHitTargetLocal.Invoke(flag);
					}
				}
				if (this.cosmeticSwapper != null && playerRig == GorillaTagger.Instance.offlineVRRig && this.cosmeticSwapper.GetCurrentStepIndex(playerRig) >= this.cosmeticSwapper.GetNumberOfSteps() && PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					this._events.Activate.RaiseAll(Array.Empty<object>());
				}
			}
		}

		// Token: 0x06007A1B RID: 31259 RVA: 0x0027D6B4 File Offset: 0x0027B8B4
		private void OnReachedLastTransformationStep(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnReachedLastTransformationStep");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender.ActorNumber), out rigContainer) && rigContainer.Rig.IsPositionInRange(base.transform.position, 6f))
			{
				UnityEvent<VRRig> onReachedLastTransformationStepShared = this.OnReachedLastTransformationStepShared;
				if (onReachedLastTransformationStepShared == null)
				{
					return;
				}
				onReachedLastTransformationStepShared.Invoke(rigContainer.Rig);
			}
		}

		// Token: 0x06007A1C RID: 31260 RVA: 0x0027D73C File Offset: 0x0027B93C
		private void SwitchState(ChickenSword.SwordState newState)
		{
			this.currentState = newState;
		}

		// Token: 0x04008B75 RID: 35701
		[SerializeField]
		private float rechargeCooldown;

		// Token: 0x04008B76 RID: 35702
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04008B77 RID: 35703
		[SerializeField]
		private float hitVelocityThreshold;

		// Token: 0x04008B78 RID: 35704
		[SerializeField]
		private TransferrableObject transferrableObject;

		// Token: 0x04008B79 RID: 35705
		[SerializeField]
		private CosmeticSwapper cosmeticSwapper;

		// Token: 0x04008B7A RID: 35706
		[Space]
		[Space]
		public UnityEvent OnDeflatedShared;

		// Token: 0x04008B7B RID: 35707
		public UnityEvent<bool> OnDeflatedLocal;

		// Token: 0x04008B7C RID: 35708
		public UnityEvent OnRechargedShared;

		// Token: 0x04008B7D RID: 35709
		public UnityEvent<bool> OnRechargedLocal;

		// Token: 0x04008B7E RID: 35710
		public UnityEvent<VRRig> OnHitTargetShared;

		// Token: 0x04008B7F RID: 35711
		public UnityEvent<bool> OnHitTargetLocal;

		// Token: 0x04008B80 RID: 35712
		public UnityEvent<VRRig> OnReachedLastTransformationStepShared;

		// Token: 0x04008B81 RID: 35713
		private float lastHitTime;

		// Token: 0x04008B82 RID: 35714
		private ChickenSword.SwordState currentState;

		// Token: 0x04008B83 RID: 35715
		private bool hitReceievd;

		// Token: 0x04008B84 RID: 35716
		private RubberDuckEvents _events;

		// Token: 0x04008B85 RID: 35717
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x02001300 RID: 4864
		private enum SwordState
		{
			// Token: 0x04008B87 RID: 35719
			Ready,
			// Token: 0x04008B88 RID: 35720
			Deflated
		}
	}
}
