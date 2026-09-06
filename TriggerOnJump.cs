using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000301 RID: 769
public class TriggerOnJump : MonoBehaviour, ITickSystemTick
{
	// Token: 0x0600139C RID: 5020 RVA: 0x000678FC File Offset: 0x00065AFC
	private void OnEnable()
	{
		if (this.myRig.IsNull())
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
		if (this._events == null && this.myRig != null && this.myRig.Creator != null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			this._events.Init(this.myRig.creator);
		}
		if (this._events != null)
		{
			this._events.Activate += this.OnActivate;
		}
		bool flag = !PhotonNetwork.InRoom && this.myRig != null && this.myRig.isOfflineVRRig;
		RigContainer rigContainer;
		bool flag2 = PhotonNetwork.InRoom && this.myRig != null && VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer) && rigContainer != null && rigContainer.Rig != null && rigContainer.Rig == this.myRig;
		if (flag || flag2)
		{
			TickSystem<object>.AddCallbackTarget(this);
		}
	}

	// Token: 0x0600139D RID: 5021 RVA: 0x00067A24 File Offset: 0x00065C24
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
		this.playerOnGround = false;
		this.jumpStartTime = 0f;
		this.lastActivationTime = 0f;
		this.waitingForGrounding = false;
		if (this._events != null)
		{
			this._events.Activate -= this.OnActivate;
			Object.Destroy(this._events);
			this._events = null;
		}
	}

	// Token: 0x0600139E RID: 5022 RVA: 0x00067A9D File Offset: 0x00065C9D
	private void OnActivate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "OnJumpActivate");
		if (info.senderID != this.myRig.creator.ActorNumber)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		this.onJumping.Invoke();
	}

	// Token: 0x0600139F RID: 5023 RVA: 0x00067AD8 File Offset: 0x00065CD8
	public void Tick()
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null)
		{
			bool flag = this.playerOnGround;
			this.playerOnGround = instance.BodyOnGround || instance.IsHandTouching(true) || instance.IsHandTouching(false);
			float time = Time.time;
			if (this.playerOnGround)
			{
				this.waitingForGrounding = false;
			}
			if (!this.playerOnGround && flag)
			{
				this.jumpStartTime = time;
			}
			if (!this.playerOnGround && !this.waitingForGrounding && instance.RigidbodyVelocity.sqrMagnitude > this.minJumpStrength * this.minJumpStrength && instance.RigidbodyVelocity.y > this.minJumpVertical && time > this.jumpStartTime + this.minJumpTime)
			{
				this.waitingForGrounding = true;
				if (time > this.lastActivationTime + this.cooldownTime)
				{
					this.lastActivationTime = time;
					if (PhotonNetwork.InRoom)
					{
						this._events.Activate.RaiseAll(Array.Empty<object>());
						return;
					}
					this.onJumping.Invoke();
				}
			}
		}
	}

	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x060013A0 RID: 5024 RVA: 0x00067BE4 File Offset: 0x00065DE4
	// (set) Token: 0x060013A1 RID: 5025 RVA: 0x00067BEC File Offset: 0x00065DEC
	public bool TickRunning { get; set; }

	// Token: 0x04001808 RID: 6152
	[SerializeField]
	private float minJumpStrength = 1f;

	// Token: 0x04001809 RID: 6153
	[SerializeField]
	private float minJumpVertical = 1f;

	// Token: 0x0400180A RID: 6154
	[SerializeField]
	private float cooldownTime = 1f;

	// Token: 0x0400180B RID: 6155
	[SerializeField]
	private UnityEvent onJumping;

	// Token: 0x0400180C RID: 6156
	private RubberDuckEvents _events;

	// Token: 0x0400180D RID: 6157
	private bool playerOnGround;

	// Token: 0x0400180E RID: 6158
	private float minJumpTime = 0.05f;

	// Token: 0x0400180F RID: 6159
	private bool waitingForGrounding;

	// Token: 0x04001810 RID: 6160
	private float jumpStartTime;

	// Token: 0x04001811 RID: 6161
	private float lastActivationTime;

	// Token: 0x04001812 RID: 6162
	private VRRig myRig;
}
