using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200135B RID: 4955
	public class ShakeReactorCosmetic : MonoBehaviour, ISpawnable
	{
		// Token: 0x06007C2E RID: 31790 RVA: 0x00288F88 File Offset: 0x00287188
		private void OnEnable()
		{
			this.lastReversalTime = Time.time;
			this.pathSinceLastReversal = 0f;
			this.recentHalfCycleDurations.Clear();
			this.hasLastDir = false;
			this.lastPosition = ((this.speedTracker != null) ? this.speedTracker.transform.position : base.transform.position);
			this.isShaking = false;
			this.debugCurrentHalfCycleDistance = 0f;
			this.debugCurrentRateHz = 0f;
			this.lastAmplitudeMeters = 0f;
			this.nextAllowedShakeStartTime = Time.time;
			if (this.myRig == null)
			{
				this.myRig = base.GetComponentInParent<VRRig>();
			}
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			}
			NetPlayer netPlayer = ((this.myRig != null) ? (this.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : NetworkSystem.Instance.LocalPlayer);
			if (netPlayer != null)
			{
				this._events.Init(netPlayer);
			}
			if (!this.subscribed && this._events.Activate != null)
			{
				this._events.Activate.reliable = true;
				this._events.Activate += this.OnShake;
				this.subscribed = true;
			}
		}

		// Token: 0x06007C2F RID: 31791 RVA: 0x002890F4 File Offset: 0x002872F4
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.OnShake;
				this.subscribed = false;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06007C30 RID: 31792 RVA: 0x0028914C File Offset: 0x0028734C
		private void Update()
		{
			bool flag = this.myRig == null || this.myRig.isLocal;
			if (this.speedTracker == null)
			{
				if (this.isShaking)
				{
					this.isShaking = false;
					if (flag && PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
					{
						this._events.Activate.RaiseOthers(new object[] { this.isShaking });
					}
					UnityEvent shakeEndShared = this.ShakeEndShared;
					if (shakeEndShared != null)
					{
						shakeEndShared.Invoke();
					}
					UnityEvent shakeEndLocal = this.ShakeEndLocal;
					if (shakeEndLocal != null)
					{
						shakeEndLocal.Invoke();
					}
					this.nextAllowedShakeStartTime = Time.time + Mathf.Max(0f, this.startCooldownSeconds);
				}
				return;
			}
			Vector3 position = this.speedTracker.transform.position;
			float magnitude = (position - this.lastPosition).magnitude;
			if (magnitude > 0f)
			{
				this.pathSinceLastReversal += magnitude;
				this.debugCurrentHalfCycleDistance = this.pathSinceLastReversal;
			}
			Vector3 worldVelocity = this.speedTracker.GetWorldVelocity();
			float magnitude2 = worldVelocity.magnitude;
			Vector3 vector = ((worldVelocity.sqrMagnitude > 1E-06f) ? worldVelocity.normalized : this.lastVelocityDir);
			bool flag2 = false;
			if (this.hasLastDir)
			{
				if (Vector3.Angle(this.lastVelocityDir, vector) >= this.angleToleranceDeg && magnitude2 >= this.minSpeedForReversal)
				{
					float num = Time.time - this.lastReversalTime;
					if (num > 0.0005f)
					{
						this.EnqueueHalfCycle(num);
						this.lastAmplitudeMeters = this.pathSinceLastReversal;
						this.lastReversalTime = Time.time;
						this.pathSinceLastReversal = 0f;
						flag2 = true;
					}
				}
			}
			else
			{
				this.hasLastDir = true;
				this.lastVelocityDir = vector;
				this.lastReversalTime = Time.time;
			}
			this.lastVelocityDir = vector;
			this.lastPosition = position;
			float averageHalfCycleDuration = this.GetAverageHalfCycleDuration();
			float num2 = Time.time - this.lastReversalTime;
			float num3 = Mathf.Max((averageHalfCycleDuration > 1E-05f) ? averageHalfCycleDuration : float.PositiveInfinity, num2);
			float num4 = ((num3 < float.PositiveInfinity) ? (0.5f / num3) : 0f);
			this.debugCurrentRateHz = num4;
			bool flag3 = num4 >= this.shakeRateThreshold;
			bool flag4 = this.lastAmplitudeMeters >= this.shakeAmplitudeThreshold;
			if (flag)
			{
				if (!this.isShaking)
				{
					if (Time.time >= this.nextAllowedShakeStartTime && flag3 && flag4)
					{
						this.isShaking = true;
						if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
						{
							this._events.Activate.RaiseOthers(new object[] { this.isShaking });
						}
						UnityEvent shakeStartLocal = this.ShakeStartLocal;
						if (shakeStartLocal != null)
						{
							shakeStartLocal.Invoke();
						}
						UnityEvent shakeStartShared = this.ShakeStartShared;
						if (shakeStartShared != null)
						{
							shakeStartShared.Invoke();
						}
					}
				}
				else
				{
					float num5 = ((this.shakeRateThreshold > 1E-05f) ? (0.5f / this.shakeRateThreshold) : float.PositiveInfinity);
					float num6 = 1f * num5;
					bool flag5 = Time.time - this.lastReversalTime > num6;
					if ((!flag3 && !flag2) || flag5)
					{
						this.isShaking = false;
						if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
						{
							this._events.Activate.RaiseOthers(new object[] { this.isShaking });
						}
						UnityEvent shakeEndLocal2 = this.ShakeEndLocal;
						if (shakeEndLocal2 != null)
						{
							shakeEndLocal2.Invoke();
						}
						UnityEvent shakeEndShared2 = this.ShakeEndShared;
						if (shakeEndShared2 != null)
						{
							shakeEndShared2.Invoke();
						}
						this.nextAllowedShakeStartTime = Time.time + Mathf.Max(0f, this.startCooldownSeconds);
					}
				}
			}
			if (this.useMaxes && this.isShaking)
			{
				bool flag6 = num4 >= this.maxShakeRate;
				bool flag7 = this.lastAmplitudeMeters >= this.maxShakeAmplitude;
				if (flag6 || flag7)
				{
					UnityEvent maxShake = this.MaxShake;
					if (maxShake != null)
					{
						maxShake.Invoke();
					}
				}
			}
			float num7 = 0f;
			if (this.isShaking)
			{
				float num8 = Mathf.Max(1E-05f, this.shakeAmplitudeThreshold);
				if (this.useMaxes && this.maxShakeAmplitude > num8)
				{
					num7 = Mathf.InverseLerp(num8, this.maxShakeAmplitude, this.lastAmplitudeMeters);
				}
				else
				{
					float num9 = Mathf.Max(num8, this.shakeAmplitudeThreshold * Mathf.Max(1f, this.softMaxMultiplier));
					num7 = Mathf.InverseLerp(num8, num9, this.lastAmplitudeMeters);
				}
			}
			this.ApplyStrength(num7);
		}

		// Token: 0x06007C31 RID: 31793 RVA: 0x00289607 File Offset: 0x00287807
		private void EnqueueHalfCycle(float duration)
		{
			this.recentHalfCycleDurations.Enqueue(duration);
			while (this.recentHalfCycleDurations.Count > Mathf.Max(1, 1))
			{
				this.recentHalfCycleDurations.Dequeue();
			}
		}

		// Token: 0x06007C32 RID: 31794 RVA: 0x00289638 File Offset: 0x00287838
		private float GetAverageHalfCycleDuration()
		{
			if (this.recentHalfCycleDurations.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			foreach (float num2 in this.recentHalfCycleDurations)
			{
				num += num2;
			}
			return num / (float)this.recentHalfCycleDurations.Count;
		}

		// Token: 0x06007C33 RID: 31795 RVA: 0x002896B0 File Offset: 0x002878B0
		private void ApplyStrength(float strength01)
		{
			if (this.continuousProperties != null)
			{
				this.continuousProperties.ApplyAll(strength01);
			}
		}

		// Token: 0x06007C34 RID: 31796 RVA: 0x002896C8 File Offset: 0x002878C8
		private void OnShake(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target || info.senderID != this.myRig.creator.ActorNumber)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnShake");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			if (args.Length != 1)
			{
				return;
			}
			object obj = args[0];
			if (!(obj is bool))
			{
				return;
			}
			bool flag = (bool)obj;
			this.isShaking = flag;
			if (flag)
			{
				UnityEvent shakeStartShared = this.ShakeStartShared;
				if (shakeStartShared == null)
				{
					return;
				}
				shakeStartShared.Invoke();
				return;
			}
			else
			{
				UnityEvent shakeEndShared = this.ShakeEndShared;
				if (shakeEndShared == null)
				{
					return;
				}
				shakeEndShared.Invoke();
				return;
			}
		}

		// Token: 0x17000C25 RID: 3109
		// (get) Token: 0x06007C35 RID: 31797 RVA: 0x0028975A File Offset: 0x0028795A
		// (set) Token: 0x06007C36 RID: 31798 RVA: 0x00289762 File Offset: 0x00287962
		public bool IsSpawned { get; set; }

		// Token: 0x17000C26 RID: 3110
		// (get) Token: 0x06007C37 RID: 31799 RVA: 0x0028976B File Offset: 0x0028796B
		// (set) Token: 0x06007C38 RID: 31800 RVA: 0x00289773 File Offset: 0x00287973
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06007C39 RID: 31801 RVA: 0x0028977C File Offset: 0x0028797C
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x06007C3A RID: 31802 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnDespawn()
		{
		}

		// Token: 0x04008E69 RID: 36457
		[Header("Speed Source")]
		[Tooltip("Speed component provider")]
		[SerializeField]
		private SimpleSpeedTracker speedTracker;

		// Token: 0x04008E6A RID: 36458
		[Header("Settings")]
		[Tooltip("Minimum reversals-per-second required to consider motion a shake - Hz.")]
		[SerializeField]
		private float shakeRateThreshold = 1f;

		// Token: 0x04008E6B RID: 36459
		[Tooltip("Minimum distance traveled between direction reversals to count as a valid half-cycle.")]
		[SerializeField]
		private float shakeAmplitudeThreshold = 0.1f;

		// Token: 0x04008E6C RID: 36460
		[Tooltip("Minimum angle change (degrees) between consecutive lobes to register a reversal. Higher = stricter.")]
		[SerializeField]
		[Range(10f, 170f)]
		private float angleToleranceDeg = 120f;

		// Token: 0x04008E6D RID: 36461
		[Tooltip("Minimum speed required to accept a direction reversal, ignores tiny jitter near stop.")]
		[SerializeField]
		private float minSpeedForReversal = 0.2f;

		// Token: 0x04008E6E RID: 36462
		[Tooltip("After a shake ends, how long to wait before ShakeStartLocal can fire again")]
		[SerializeField]
		private float startCooldownSeconds = 0.2f;

		// Token: 0x04008E6F RID: 36463
		[SerializeField]
		private bool useMaxes;

		// Token: 0x04008E70 RID: 36464
		[Tooltip("If enabled, exceeding this rate is considered a max shake.")]
		[SerializeField]
		private float maxShakeRate = 6f;

		// Token: 0x04008E71 RID: 36465
		[Tooltip("If enabled, exceeding this amplitude per half cycle is considered a max shake.")]
		[SerializeField]
		private float maxShakeAmplitude = 0.3f;

		// Token: 0x04008E72 RID: 36466
		[Header("Continuous Output")]
		[SerializeField]
		private ContinuousPropertyArray continuousProperties;

		// Token: 0x04008E73 RID: 36467
		[Header("Advanced")]
		[Tooltip("When no hard max amplitude is defined, strength is mapped to Threshold × this multiplier.")]
		[SerializeField]
		private float softMaxMultiplier = 3f;

		// Token: 0x04008E74 RID: 36468
		[FormerlySerializedAs("ShakeStart")]
		[Header("Events")]
		public UnityEvent ShakeStartLocal;

		// Token: 0x04008E75 RID: 36469
		public UnityEvent ShakeStartShared;

		// Token: 0x04008E76 RID: 36470
		[FormerlySerializedAs("ShakeEnd")]
		public UnityEvent ShakeEndLocal;

		// Token: 0x04008E77 RID: 36471
		public UnityEvent ShakeEndShared;

		// Token: 0x04008E78 RID: 36472
		public UnityEvent MaxShake;

		// Token: 0x04008E79 RID: 36473
		[Header("Debug")]
		public bool isShaking;

		// Token: 0x04008E7A RID: 36474
		public float lastAmplitudeMeters;

		// Token: 0x04008E7B RID: 36475
		public float debugCurrentHalfCycleDistance;

		// Token: 0x04008E7C RID: 36476
		public float debugCurrentRateHz;

		// Token: 0x04008E7D RID: 36477
		private const int kFrequencyHistoryCount = 1;

		// Token: 0x04008E7E RID: 36478
		private const float kNoReversalGraceMultiplier = 1f;

		// Token: 0x04008E7F RID: 36479
		private readonly Queue<float> recentHalfCycleDurations = new Queue<float>();

		// Token: 0x04008E80 RID: 36480
		private Vector3 lastVelocityDir;

		// Token: 0x04008E81 RID: 36481
		private bool hasLastDir;

		// Token: 0x04008E82 RID: 36482
		private float lastReversalTime;

		// Token: 0x04008E83 RID: 36483
		private Vector3 lastPosition;

		// Token: 0x04008E84 RID: 36484
		private float pathSinceLastReversal;

		// Token: 0x04008E85 RID: 36485
		private float nextAllowedShakeStartTime;

		// Token: 0x04008E86 RID: 36486
		private const float kEpsilon = 1E-05f;

		// Token: 0x04008E87 RID: 36487
		private const float kTinyVelocitySqr = 1E-06f;

		// Token: 0x04008E88 RID: 36488
		private const float kMinHalfCycleDuration = 0.0005f;

		// Token: 0x04008E89 RID: 36489
		private const float kHalfPerCycle = 0.5f;

		// Token: 0x04008E8A RID: 36490
		private RubberDuckEvents _events;

		// Token: 0x04008E8B RID: 36491
		private CallLimiter callLimiter = new CallLimiter(10, 1f, 0.5f);

		// Token: 0x04008E8C RID: 36492
		private VRRig myRig;

		// Token: 0x04008E8D RID: 36493
		private bool subscribed;
	}
}
