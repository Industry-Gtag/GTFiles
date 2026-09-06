using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001369 RID: 4969
	[RequireComponent(typeof(TransferrableObject))]
	public class VenusFlyTrapHoldable : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000C2A RID: 3114
		// (get) Token: 0x06007C7E RID: 31870 RVA: 0x0028AA48 File Offset: 0x00288C48
		// (set) Token: 0x06007C7F RID: 31871 RVA: 0x0028AA50 File Offset: 0x00288C50
		public bool TickRunning { get; set; }

		// Token: 0x06007C80 RID: 31872 RVA: 0x0028AA59 File Offset: 0x00288C59
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
		}

		// Token: 0x06007C81 RID: 31873 RVA: 0x0028AA68 File Offset: 0x00288C68
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.triggerEventNotifier.TriggerEnterEvent += this.TriggerEntered;
			this.state = VenusFlyTrapHoldable.VenusState.Open;
			this.localRotA = this.lipA.transform.localRotation;
			this.localRotB = this.lipB.transform.localRotation;
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
				this._events.Activate += this.OnTriggerEvent;
			}
		}

		// Token: 0x06007C82 RID: 31874 RVA: 0x0028AB80 File Offset: 0x00288D80
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
			this.triggerEventNotifier.TriggerEnterEvent -= this.TriggerEntered;
			if (this._events != null)
			{
				this._events.Activate -= this.OnTriggerEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06007C83 RID: 31875 RVA: 0x0028ABEC File Offset: 0x00288DEC
		public void Tick()
		{
			if (this.transferrableObject.InHand() && this.audioSource && !this.audioSource.isPlaying && this.flyLoopingAudio != null)
			{
				this.audioSource.clip = this.flyLoopingAudio;
				this.audioSource.GTPlay();
			}
			if (!this.transferrableObject.InHand() && this.audioSource && this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Open)
			{
				return;
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Closed && Time.time - this.closedStartedTime >= this.closedDuration)
			{
				this.UpdateState(VenusFlyTrapHoldable.VenusState.Opening);
				if (this.audioSource && this.openingAudio != null)
				{
					this.audioSource.GTPlayOneShot(this.openingAudio, 1f);
				}
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Closing)
			{
				this.SmoothRotation(true);
				return;
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Opening)
			{
				this.SmoothRotation(false);
			}
		}

		// Token: 0x06007C84 RID: 31876 RVA: 0x0028ACFC File Offset: 0x00288EFC
		private void SmoothRotation(bool isClosing)
		{
			if (isClosing)
			{
				Quaternion quaternion = Quaternion.Euler(this.targetRotationB);
				this.lipB.transform.localRotation = Quaternion.Lerp(this.lipB.transform.localRotation, quaternion, Time.deltaTime * this.speed);
				Quaternion quaternion2 = Quaternion.Euler(this.targetRotationA);
				this.lipA.transform.localRotation = Quaternion.Lerp(this.lipA.transform.localRotation, quaternion2, Time.deltaTime * this.speed);
				if (Quaternion.Angle(this.lipB.transform.localRotation, quaternion) < 1f && Quaternion.Angle(this.lipA.transform.localRotation, quaternion2) < 1f)
				{
					this.lipB.transform.localRotation = quaternion;
					this.lipA.transform.localRotation = quaternion2;
					this.UpdateState(VenusFlyTrapHoldable.VenusState.Closed);
					return;
				}
			}
			else
			{
				this.lipB.transform.localRotation = Quaternion.Lerp(this.lipB.transform.localRotation, this.localRotB, Time.deltaTime * this.speed / 2f);
				this.lipA.transform.localRotation = Quaternion.Lerp(this.lipA.transform.localRotation, this.localRotA, Time.deltaTime * this.speed / 2f);
				if (Quaternion.Angle(this.lipB.transform.localRotation, this.localRotB) < 1f && Quaternion.Angle(this.lipA.transform.localRotation, this.localRotA) < 1f)
				{
					this.lipB.transform.localRotation = this.localRotB;
					this.lipA.transform.localRotation = this.localRotA;
					this.UpdateState(VenusFlyTrapHoldable.VenusState.Open);
				}
			}
		}

		// Token: 0x06007C85 RID: 31877 RVA: 0x0028AEE6 File Offset: 0x002890E6
		private void UpdateState(VenusFlyTrapHoldable.VenusState newState)
		{
			this.state = newState;
			if (this.state == VenusFlyTrapHoldable.VenusState.Closed)
			{
				this.closedStartedTime = Time.time;
			}
		}

		// Token: 0x06007C86 RID: 31878 RVA: 0x0028AF04 File Offset: 0x00289104
		private void TriggerEntered(TriggerEventNotifier notifier, Collider other)
		{
			if (this.state != VenusFlyTrapHoldable.VenusState.Open)
			{
				return;
			}
			if (!other.gameObject.IsOnLayer(this.layers))
			{
				return;
			}
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(Array.Empty<object>());
			}
			this.OnTriggerLocal();
			GorillaTriggerColliderHandIndicator componentInChildren = other.GetComponentInChildren<GorillaTriggerColliderHandIndicator>();
			if (componentInChildren == null)
			{
				return;
			}
			GorillaTagger.Instance.StartVibration(componentInChildren.isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06007C87 RID: 31879 RVA: 0x0028AF9F File Offset: 0x0028919F
		private void OnTriggerEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnTriggerEvent");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			this.OnTriggerLocal();
		}

		// Token: 0x06007C88 RID: 31880 RVA: 0x0028AFCB File Offset: 0x002891CB
		private void OnTriggerLocal()
		{
			this.UpdateState(VenusFlyTrapHoldable.VenusState.Closing);
			if (this.audioSource && this.closingAudio != null)
			{
				this.audioSource.GTPlayOneShot(this.closingAudio, 1f);
			}
		}

		// Token: 0x04008EF4 RID: 36596
		[SerializeField]
		private GameObject lipA;

		// Token: 0x04008EF5 RID: 36597
		[SerializeField]
		private GameObject lipB;

		// Token: 0x04008EF6 RID: 36598
		[SerializeField]
		private Vector3 targetRotationA;

		// Token: 0x04008EF7 RID: 36599
		[SerializeField]
		private Vector3 targetRotationB;

		// Token: 0x04008EF8 RID: 36600
		[SerializeField]
		private float closedDuration = 3f;

		// Token: 0x04008EF9 RID: 36601
		[SerializeField]
		private float speed = 2f;

		// Token: 0x04008EFA RID: 36602
		[SerializeField]
		private UnityLayer layers;

		// Token: 0x04008EFB RID: 36603
		[SerializeField]
		private TriggerEventNotifier triggerEventNotifier;

		// Token: 0x04008EFC RID: 36604
		[SerializeField]
		private float hapticStrength = 0.5f;

		// Token: 0x04008EFD RID: 36605
		[SerializeField]
		private float hapticDuration = 0.1f;

		// Token: 0x04008EFE RID: 36606
		[SerializeField]
		private GameObject bug;

		// Token: 0x04008EFF RID: 36607
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04008F00 RID: 36608
		[SerializeField]
		private AudioClip closingAudio;

		// Token: 0x04008F01 RID: 36609
		[SerializeField]
		private AudioClip openingAudio;

		// Token: 0x04008F02 RID: 36610
		[SerializeField]
		private AudioClip flyLoopingAudio;

		// Token: 0x04008F03 RID: 36611
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x04008F04 RID: 36612
		private float closedStartedTime;

		// Token: 0x04008F05 RID: 36613
		private VenusFlyTrapHoldable.VenusState state;

		// Token: 0x04008F06 RID: 36614
		private Quaternion localRotA;

		// Token: 0x04008F07 RID: 36615
		private Quaternion localRotB;

		// Token: 0x04008F08 RID: 36616
		private RubberDuckEvents _events;

		// Token: 0x04008F09 RID: 36617
		private TransferrableObject transferrableObject;

		// Token: 0x0200136A RID: 4970
		private enum VenusState
		{
			// Token: 0x04008F0C RID: 36620
			Closed,
			// Token: 0x04008F0D RID: 36621
			Open,
			// Token: 0x04008F0E RID: 36622
			Closing,
			// Token: 0x04008F0F RID: 36623
			Opening
		}
	}
}
