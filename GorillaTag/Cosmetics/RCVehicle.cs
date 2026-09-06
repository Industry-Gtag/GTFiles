using System;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012DB RID: 4827
	public class RCVehicle : MonoBehaviour, ISpawnable
	{
		// Token: 0x17000BBF RID: 3007
		// (get) Token: 0x060078EE RID: 30958 RVA: 0x00276A08 File Offset: 0x00274C08
		public bool HasLocalAuthority
		{
			get
			{
				return !PhotonNetwork.InRoom || (this.networkSync != null && this.networkSync.photonView.IsMine);
			}
		}

		// Token: 0x060078EF RID: 30959 RVA: 0x00276A34 File Offset: 0x00274C34
		public virtual void WakeUpRemote(RCCosmeticNetworkSync sync)
		{
			this.networkSync = sync;
			this.hasNetworkSync = sync != null;
			if (this.HasLocalAuthority)
			{
				return;
			}
			if (!base.enabled || !base.gameObject.activeSelf)
			{
				this.localStatePrev = RCVehicle.State.Disabled;
				base.enabled = true;
				base.gameObject.SetActive(true);
				this.RemoteUpdate(Time.deltaTime);
			}
		}

		// Token: 0x060078F0 RID: 30960 RVA: 0x00276A98 File Offset: 0x00274C98
		public virtual void StartConnection(RCRemoteHoldable remote, RCCosmeticNetworkSync sync)
		{
			this.connectedRemote = remote;
			this.networkSync = sync;
			this.hasNetworkSync = sync != null;
			base.enabled = true;
			base.gameObject.SetActive(true);
			this.useLeftDock = remote.XRNode == XRNode.LeftHand;
			if (this.HasLocalAuthority && this.localState != RCVehicle.State.Mobilized)
			{
				this.AuthorityBeginDocked();
			}
		}

		// Token: 0x060078F1 RID: 30961 RVA: 0x00276AF9 File Offset: 0x00274CF9
		public virtual void EndConnection()
		{
			this.connectedRemote = null;
			this.activeInput = default(RCRemoteHoldable.RCInput);
			this.disconnectionTime = Time.time;
		}

		// Token: 0x060078F2 RID: 30962 RVA: 0x00276B1C File Offset: 0x00274D1C
		protected virtual void ResetToSpawnPosition()
		{
			if (this.rb == null)
			{
				this.rb = base.GetComponent<Rigidbody>();
			}
			if (this.rb != null)
			{
				this.rb.isKinematic = true;
			}
			base.transform.parent = (this.useLeftDock ? this.leftDockParent : this.rightDockParent);
			base.transform.SetLocalPositionAndRotation(this.useLeftDock ? this.dockLeftOffset.pos : this.dockRightOffset.pos, this.useLeftDock ? this.dockLeftOffset.rot : this.dockRightOffset.rot);
			base.transform.localScale = (this.useLeftDock ? this.dockLeftOffset.scale : this.dockRightOffset.scale);
		}

		// Token: 0x060078F3 RID: 30963 RVA: 0x00276BF4 File Offset: 0x00274DF4
		protected virtual void AuthorityBeginDocked()
		{
			this.localState = (this.useLeftDock ? RCVehicle.State.DockedLeft : RCVehicle.State.DockedRight);
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.stateStartTime = Time.time;
			this.waitingForTriggerRelease = true;
			this.ResetToSpawnPosition();
			if (this.connectedRemote == null)
			{
				this.SetDisabledState();
			}
		}

		// Token: 0x060078F4 RID: 30964 RVA: 0x00276C64 File Offset: 0x00274E64
		protected virtual void AuthorityBeginMobilization()
		{
			this.localState = RCVehicle.State.Mobilized;
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.stateStartTime = Time.time;
			base.transform.parent = null;
			this.rb.isKinematic = false;
		}

		// Token: 0x060078F5 RID: 30965 RVA: 0x00276CC0 File Offset: 0x00274EC0
		protected virtual void AuthorityBeginCrash()
		{
			this.localState = RCVehicle.State.Crashed;
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.stateStartTime = Time.time;
		}

		// Token: 0x060078F6 RID: 30966 RVA: 0x00276CFC File Offset: 0x00274EFC
		protected virtual void SetDisabledState()
		{
			this.localState = RCVehicle.State.Disabled;
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.state = (byte)this.localState;
			}
			this.ResetToSpawnPosition();
			base.enabled = false;
			base.gameObject.SetActive(false);
		}

		// Token: 0x060078F7 RID: 30967 RVA: 0x00276D4E File Offset: 0x00274F4E
		protected virtual void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
		}

		// Token: 0x060078F8 RID: 30968 RVA: 0x00002C2D File Offset: 0x00000E2D
		protected virtual void OnEnable()
		{
		}

		// Token: 0x17000BC0 RID: 3008
		// (get) Token: 0x060078F9 RID: 30969 RVA: 0x00276D5C File Offset: 0x00274F5C
		// (set) Token: 0x060078FA RID: 30970 RVA: 0x00276D64 File Offset: 0x00274F64
		bool ISpawnable.IsSpawned { get; set; }

		// Token: 0x17000BC1 RID: 3009
		// (get) Token: 0x060078FB RID: 30971 RVA: 0x00276D6D File Offset: 0x00274F6D
		// (set) Token: 0x060078FC RID: 30972 RVA: 0x00276D75 File Offset: 0x00274F75
		ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

		// Token: 0x060078FD RID: 30973 RVA: 0x00276D80 File Offset: 0x00274F80
		void ISpawnable.OnSpawn(VRRig rig)
		{
			if (rig == null)
			{
				GTDev.LogError<string>("RCVehicle: Could not find VRRig in parents. If you are trying to make this a world item rather than a cosmetic then you'll have to refactor how it teleports back to the arms.", this, null);
				return;
			}
			string text;
			if (!GTHardCodedBones.TryGetBoneXforms(rig, out this._vrRigBones, out text))
			{
				Debug.LogError("RCVehicle: " + text, this);
				return;
			}
			if (this.leftDockParent == null && !GTHardCodedBones.TryGetBoneXform(this._vrRigBones, this.dockLeftOffset.bone, out this.leftDockParent))
			{
				GTDev.LogError<string>("RCVehicle: Could not find left dock transform.", this, null);
			}
			if (this.rightDockParent == null && !GTHardCodedBones.TryGetBoneXform(this._vrRigBones, this.dockRightOffset.bone, out this.rightDockParent))
			{
				GTDev.LogError<string>("RCVehicle: Could not find right dock transform.", this, null);
			}
		}

		// Token: 0x060078FE RID: 30974 RVA: 0x00002C2D File Offset: 0x00000E2D
		void ISpawnable.OnDespawn()
		{
		}

		// Token: 0x060078FF RID: 30975 RVA: 0x00276E3B File Offset: 0x0027503B
		protected virtual void OnDisable()
		{
			this.localState = RCVehicle.State.Disabled;
			this.localStatePrev = RCVehicle.State.Disabled;
		}

		// Token: 0x06007900 RID: 30976 RVA: 0x00276E4C File Offset: 0x0027504C
		public void ApplyRemoteControlInput(RCRemoteHoldable.RCInput rcInput)
		{
			this.activeInput.joystick.y = Mathf.Sign(rcInput.joystick.y) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.joystickDeadzone, 1f, Mathf.Abs(rcInput.joystick.y)));
			this.activeInput.joystick.x = Mathf.Sign(rcInput.joystick.x) * Mathf.Lerp(0f, 1f, Mathf.InverseLerp(this.joystickDeadzone, 1f, Mathf.Abs(rcInput.joystick.x)));
			this.activeInput.trigger = Mathf.Clamp(rcInput.trigger, -1f, 1f);
			this.activeInput.buttons = rcInput.buttons;
		}

		// Token: 0x06007901 RID: 30977 RVA: 0x00276F2C File Offset: 0x0027512C
		private void Update()
		{
			float deltaTime = Time.deltaTime;
			if (this.HasLocalAuthority)
			{
				this.AuthorityUpdate(deltaTime);
			}
			else
			{
				this.RemoteUpdate(deltaTime);
			}
			this.SharedUpdate(deltaTime);
			this.localStatePrev = this.localState;
		}

		// Token: 0x06007902 RID: 30978 RVA: 0x00276F6C File Offset: 0x0027516C
		protected virtual void AuthorityUpdate(float dt)
		{
			switch (this.localState)
			{
			default:
				this.ResetToSpawnPosition();
				if (this.connectedRemote == null)
				{
					this.SetDisabledState();
					return;
				}
				if (this.waitingForTriggerRelease && this.activeInput.trigger < 0.25f)
				{
					this.waitingForTriggerRelease = false;
				}
				if (!this.waitingForTriggerRelease && this.activeInput.trigger > 0.25f)
				{
					this.AuthorityBeginMobilization();
					return;
				}
				break;
			case RCVehicle.State.Mobilized:
			{
				if (this.networkSync != null)
				{
					this.networkSync.syncedState.position = base.transform.position;
					this.networkSync.syncedState.rotation = base.transform.rotation;
				}
				bool flag = (base.transform.position - this.leftDockParent.position).sqrMagnitude > this.maxRange * this.maxRange;
				bool flag2 = this.connectedRemote == null && Time.time - this.disconnectionTime > this.maxDisconnectionTime;
				if (flag || flag2)
				{
					this.AuthorityBeginCrash();
					return;
				}
				break;
			}
			case RCVehicle.State.Crashed:
				if (Time.time > this.stateStartTime + this.crashRespawnDelay)
				{
					this.AuthorityBeginDocked();
				}
				break;
			}
		}

		// Token: 0x06007903 RID: 30979 RVA: 0x002770BC File Offset: 0x002752BC
		protected virtual void RemoteUpdate(float dt)
		{
			if (this.networkSync == null)
			{
				this.SetDisabledState();
				return;
			}
			this.localState = (RCVehicle.State)this.networkSync.syncedState.state;
			switch (this.localState)
			{
			case RCVehicle.State.Disabled:
				this.SetDisabledState();
				break;
			default:
				this.useLeftDock = this.localState != RCVehicle.State.DockedRight;
				this.ResetToSpawnPosition();
				return;
			case RCVehicle.State.Mobilized:
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.rb.isKinematic = true;
					base.transform.parent = null;
				}
				base.transform.position = Vector3.Lerp(this.networkSync.syncedState.position, base.transform.position, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
				base.transform.rotation = Quaternion.Slerp(this.networkSync.syncedState.rotation, base.transform.rotation, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
				return;
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.rb.isKinematic = false;
					base.transform.parent = null;
					if (this.localStatePrev != RCVehicle.State.Mobilized)
					{
						base.transform.position = this.networkSync.syncedState.position;
						base.transform.rotation = this.networkSync.syncedState.rotation;
						return;
					}
				}
				break;
			}
		}

		// Token: 0x06007904 RID: 30980 RVA: 0x00002C2D File Offset: 0x00000E2D
		protected virtual void SharedUpdate(float dt)
		{
		}

		// Token: 0x06007905 RID: 30981 RVA: 0x0027722C File Offset: 0x0027542C
		public virtual void AuthorityApplyImpact(Vector3 hitVelocity, bool isProjectile)
		{
			if (this.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				float num = (isProjectile ? this.projectileVelocityTransfer : this.hitVelocityTransfer);
				this.rb.AddForce(Vector3.ClampMagnitude(hitVelocity * num, this.hitMaxHitSpeed) * this.rb.mass, ForceMode.Impulse);
				if (isProjectile || (this.crashOnHit && hitVelocity.sqrMagnitude > this.crashOnHitSpeedThreshold * this.crashOnHitSpeedThreshold))
				{
					this.AuthorityBeginCrash();
				}
			}
			UnityEvent onHitImpact = this.OnHitImpact;
			if (onHitImpact == null)
			{
				return;
			}
			onHitImpact.Invoke();
		}

		// Token: 0x06007906 RID: 30982 RVA: 0x001B28F9 File Offset: 0x001B0AF9
		protected float NormalizeAngle180(float angle)
		{
			angle = (angle + 180f) % 360f;
			if (angle < 0f)
			{
				angle += 360f;
			}
			return angle - 180f;
		}

		// Token: 0x06007907 RID: 30983 RVA: 0x002772C4 File Offset: 0x002754C4
		protected static void AddScaledGravityCompensationForce(Rigidbody rb, float scaleFactor, float gravityCompensation)
		{
			Vector3 gravity = Physics.gravity;
			Vector3 vector = -gravity * gravityCompensation;
			Vector3 vector2 = gravity + vector;
			Vector3 vector3 = vector2 * scaleFactor - vector2;
			rb.AddForce((vector + vector3) * rb.mass, ForceMode.Force);
		}

		// Token: 0x040089E3 RID: 35299
		[SerializeField]
		private Transform leftDockParent;

		// Token: 0x040089E4 RID: 35300
		[SerializeField]
		private Transform rightDockParent;

		// Token: 0x040089E5 RID: 35301
		[SerializeField]
		private float maxRange = 100f;

		// Token: 0x040089E6 RID: 35302
		[SerializeField]
		private float maxDisconnectionTime = 10f;

		// Token: 0x040089E7 RID: 35303
		[SerializeField]
		private float crashRespawnDelay = 3f;

		// Token: 0x040089E8 RID: 35304
		[SerializeField]
		private bool crashOnHit;

		// Token: 0x040089E9 RID: 35305
		[SerializeField]
		private float crashOnHitSpeedThreshold = 5f;

		// Token: 0x040089EA RID: 35306
		[SerializeField]
		[Range(0f, 1f)]
		private float hitVelocityTransfer = 0.5f;

		// Token: 0x040089EB RID: 35307
		[SerializeField]
		[Range(0f, 1f)]
		private float projectileVelocityTransfer = 0.1f;

		// Token: 0x040089EC RID: 35308
		[SerializeField]
		private float hitMaxHitSpeed = 4f;

		// Token: 0x040089ED RID: 35309
		[SerializeField]
		[Range(0f, 1f)]
		private float joystickDeadzone = 0.1f;

		// Token: 0x040089EE RID: 35310
		[Header("RCVehicle - Shared Event")]
		public UnityEvent OnHitImpact;

		// Token: 0x040089EF RID: 35311
		protected RCVehicle.State localState;

		// Token: 0x040089F0 RID: 35312
		protected RCVehicle.State localStatePrev;

		// Token: 0x040089F1 RID: 35313
		protected float stateStartTime;

		// Token: 0x040089F2 RID: 35314
		protected RCRemoteHoldable connectedRemote;

		// Token: 0x040089F3 RID: 35315
		protected RCCosmeticNetworkSync networkSync;

		// Token: 0x040089F4 RID: 35316
		protected bool hasNetworkSync;

		// Token: 0x040089F5 RID: 35317
		protected RCRemoteHoldable.RCInput activeInput;

		// Token: 0x040089F6 RID: 35318
		protected Rigidbody rb;

		// Token: 0x040089F7 RID: 35319
		private bool waitingForTriggerRelease;

		// Token: 0x040089F8 RID: 35320
		private float disconnectionTime;

		// Token: 0x040089F9 RID: 35321
		private bool useLeftDock;

		// Token: 0x040089FA RID: 35322
		private BoneOffset dockLeftOffset = new BoneOffset(GTHardCodedBones.EBone.forearm_L, new Vector3(-0.062f, 0.283f, -0.136f), new Vector3(275f, 0f, 25f));

		// Token: 0x040089FB RID: 35323
		private BoneOffset dockRightOffset = new BoneOffset(GTHardCodedBones.EBone.forearm_R, new Vector3(0.069f, 0.265f, -0.128f), new Vector3(275f, 0f, 335f));

		// Token: 0x040089FC RID: 35324
		private float networkSyncFollowRateExp = 2f;

		// Token: 0x040089FD RID: 35325
		private Transform[] _vrRigBones;

		// Token: 0x020012DC RID: 4828
		protected enum State
		{
			// Token: 0x04008A01 RID: 35329
			Disabled,
			// Token: 0x04008A02 RID: 35330
			DockedLeft,
			// Token: 0x04008A03 RID: 35331
			DockedRight,
			// Token: 0x04008A04 RID: 35332
			Mobilized,
			// Token: 0x04008A05 RID: 35333
			Crashed
		}
	}
}
