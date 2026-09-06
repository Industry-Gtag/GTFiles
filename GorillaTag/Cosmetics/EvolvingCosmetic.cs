using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200132F RID: 4911
	[Obsolete]
	public class EvolvingCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000C0C RID: 3084
		// (get) Token: 0x06007B46 RID: 31558 RVA: 0x00284831 File Offset: 0x00282A31
		private int LoopMaxValue
		{
			get
			{
				return this.stages.Length;
			}
		}

		// Token: 0x06007B47 RID: 31559 RVA: 0x0028483C File Offset: 0x00282A3C
		private void Awake()
		{
			base.gameObject.GetOrAddComponent(ref this.networkEvents);
			this.myRig = base.GetComponentInParent<VRRig>();
			for (int i = 0; i < this.stages.Length; i++)
			{
				this.totalDuration += this.stages[i].Duration;
				if (this.enableLooping)
				{
					if (i < this.loopToStageOnComplete - 1)
					{
						this.timeAtLoopStart += this.stages[i].Duration;
					}
					else
					{
						this.loopDuration += this.stages[i].Duration;
					}
				}
			}
		}

		// Token: 0x06007B48 RID: 31560 RVA: 0x002848E0 File Offset: 0x00282AE0
		private void OnEnable()
		{
			if (this.stages.Length == 0)
			{
				return;
			}
			NetPlayer netPlayer = this.myRig.creator ?? NetworkSystem.Instance.LocalPlayer;
			if (netPlayer != null)
			{
				this.networkEvents.Init(netPlayer);
				TickSystem<object>.AddTickCallback(this);
				NetworkSystem.Instance.OnPlayerJoined += this.SendElapsedTime;
				this.networkEvents.Activate += this.ReceiveElapsedTime;
				this.FirstStage();
				return;
			}
			Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
		}

		// Token: 0x06007B49 RID: 31561 RVA: 0x0028497C File Offset: 0x00282B7C
		private void OnDisable()
		{
			if (this.networkEvents != null)
			{
				TickSystem<object>.RemoveTickCallback(this);
				NetworkSystem.Instance.OnPlayerJoined -= this.SendElapsedTime;
				this.networkEvents.Activate -= this.ReceiveElapsedTime;
				this.FirstStage();
			}
			CallLimiter callLimiter = this.callLimiter;
			if (callLimiter == null)
			{
				return;
			}
			callLimiter.Reset();
		}

		// Token: 0x06007B4A RID: 31562 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void Log(bool isComplete, bool isEvent)
		{
		}

		// Token: 0x06007B4B RID: 31563 RVA: 0x002849F8 File Offset: 0x00282BF8
		private void FirstStage()
		{
			this.activeStageIndex = 0;
			this.activeStage = this.stages[0];
			this.nextEventIndex = 0;
			this.nextEvent = this.activeStage.GetEventOrNull(0);
			this.totalElapsedTime = 0f;
			this.totalTimeOfPreviousStages = 0f;
			this.HandleStages();
		}

		// Token: 0x06007B4C RID: 31564 RVA: 0x00284A50 File Offset: 0x00282C50
		private void HandleStages()
		{
			for (;;)
			{
				float num = this.totalElapsedTime - this.totalTimeOfPreviousStages;
				float num2 = Mathf.Min(num / this.activeStage.Duration, 1f);
				this.activeStage.continuousProperties.ApplyAll(num2);
				while (this.nextEvent != null && num >= this.nextEvent.absoluteTime)
				{
					UnityEvent onTimeReached = this.nextEvent.onTimeReached;
					if (onTimeReached != null)
					{
						onTimeReached.Invoke();
					}
					this.Log(false, true);
					EvolvingCosmetic.EvolutionStage evolutionStage = this.activeStage;
					int num3 = this.nextEventIndex + 1;
					this.nextEventIndex = num3;
					this.nextEvent = evolutionStage.GetEventOrNull(num3);
				}
				if (num < this.activeStage.Duration)
				{
					break;
				}
				this.activeStageIndex++;
				if (this.activeStageIndex >= this.stages.Length && !this.enableLooping)
				{
					goto Block_4;
				}
				if (this.activeStageIndex >= this.stages.Length)
				{
					this.activeStageIndex = this.loopToStageOnComplete - 1;
					this.totalTimeOfPreviousStages = this.timeAtLoopStart;
					this.totalElapsedTime -= this.loopDuration;
				}
				else
				{
					this.totalTimeOfPreviousStages += this.activeStage.Duration;
				}
				this.activeStage = this.stages[this.activeStageIndex];
				this.nextEventIndex = 0;
				this.nextEvent = this.activeStage.GetEventOrNull(0);
				if (!this.activeStage.HasDuration)
				{
					this.totalElapsedTime = this.totalTimeOfPreviousStages + this.activeStage.Duration * 0.5f;
					TickSystem<object>.RemoveTickCallback(this);
				}
				else
				{
					TickSystem<object>.AddTickCallback(this);
				}
				this.Log(false, false);
			}
			return;
			Block_4:
			this.totalElapsedTime = this.totalDuration;
			TickSystem<object>.RemoveTickCallback(this);
			this.Log(true, false);
		}

		// Token: 0x17000C0D RID: 3085
		// (get) Token: 0x06007B4D RID: 31565 RVA: 0x00284C04 File Offset: 0x00282E04
		// (set) Token: 0x06007B4E RID: 31566 RVA: 0x00284C0C File Offset: 0x00282E0C
		public bool TickRunning { get; set; }

		// Token: 0x06007B4F RID: 31567 RVA: 0x00284C18 File Offset: 0x00282E18
		public void Tick()
		{
			this.totalElapsedTime = Mathf.Clamp(this.totalElapsedTime + Mathf.Max(this.activeStage.DeltaTime(Time.deltaTime), 0f), 0f, this.totalDuration * 1.01f);
			this.HandleStages();
		}

		// Token: 0x06007B50 RID: 31568 RVA: 0x00284C68 File Offset: 0x00282E68
		public void CompleteManualStage()
		{
			if (!this.activeStage.HasDuration)
			{
				this.ForceNextStage();
			}
		}

		// Token: 0x06007B51 RID: 31569 RVA: 0x00284C7D File Offset: 0x00282E7D
		public void ForceNextStage()
		{
			this.totalElapsedTime = this.totalTimeOfPreviousStages + this.activeStage.Duration;
			this.HandleStages();
		}

		// Token: 0x06007B52 RID: 31570 RVA: 0x00284C9D File Offset: 0x00282E9D
		private void SendElapsedTime(NetPlayer player)
		{
			if (this.sendProgressDelayCoroutine != null)
			{
				base.StopCoroutine(this.sendProgressDelayCoroutine);
			}
			this.sendProgressDelayCoroutine = base.StartCoroutine(this.SendElapsedTimeDelayed());
		}

		// Token: 0x06007B53 RID: 31571 RVA: 0x00284CC5 File Offset: 0x00282EC5
		private IEnumerator SendElapsedTimeDelayed()
		{
			yield return new WaitForSeconds(1f);
			this.sendProgressDelayCoroutine = null;
			this.networkEvents.Activate.RaiseOthers(new object[] { this.totalElapsedTime });
			yield break;
		}

		// Token: 0x06007B54 RID: 31572 RVA: 0x00284CD4 File Offset: 0x00282ED4
		private void ReceiveElapsedTime(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "ReceiveElapsedTime");
			if (info.senderID == this.myRig.creator.ActorNumber && this.callLimiter.CheckCallServerTime((double)Time.unscaledTime) && args.Length == 1)
			{
				object obj = args[0];
				if (obj is float)
				{
					float num = (float)obj;
					if (float.IsFinite(num) && num <= this.totalDuration && num >= 0f)
					{
						this.totalElapsedTime = num;
						this.HandleStages();
						return;
					}
				}
			}
		}

		// Token: 0x06007B55 RID: 31573 RVA: 0x00284D60 File Offset: 0x00282F60
		private void SetStage(int targetIndex)
		{
			if (this.stages == null || this.stages.Length == 0)
			{
				return;
			}
			if (this.enableLooping)
			{
				if (targetIndex < 0)
				{
					targetIndex = this.stages.Length - 1;
				}
				else if (targetIndex >= this.stages.Length)
				{
					targetIndex = 0;
				}
			}
			else
			{
				targetIndex = Mathf.Clamp(targetIndex, 0, this.stages.Length - 1);
			}
			this.activeStageIndex = targetIndex;
			this.activeStage = this.stages[targetIndex];
			float num = 0f;
			for (int i = 0; i < targetIndex; i++)
			{
				num += this.stages[i].Duration;
			}
			this.totalTimeOfPreviousStages = num;
			this.totalElapsedTime = num + Mathf.Epsilon;
			this.nextEventIndex = 0;
			this.nextEvent = this.activeStage.GetEventOrNull(0);
			if (this.activeStage.HasDuration)
			{
				TickSystem<object>.AddTickCallback(this);
			}
			else
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
			int num2 = 0;
			for (EvolvingCosmetic.EvolutionStage.EventAtTime eventAtTime = this.activeStage.GetEventOrNull(num2); eventAtTime != null; eventAtTime = this.activeStage.GetEventOrNull(num2))
			{
				UnityEvent onTimeReached = eventAtTime.onTimeReached;
				if (onTimeReached != null)
				{
					onTimeReached.Invoke();
				}
				num2++;
			}
			this.HandleStages();
		}

		// Token: 0x06007B56 RID: 31574 RVA: 0x00284E77 File Offset: 0x00283077
		private void RestartStageInternal()
		{
			this.SetStage(this.activeStageIndex);
		}

		// Token: 0x06007B57 RID: 31575 RVA: 0x00284E85 File Offset: 0x00283085
		public void IncrementStage()
		{
			this.SetStage(this.activeStageIndex + 1);
		}

		// Token: 0x06007B58 RID: 31576 RVA: 0x00284E95 File Offset: 0x00283095
		public void DecrementStage()
		{
			this.SetStage(this.activeStageIndex - 1);
		}

		// Token: 0x06007B59 RID: 31577 RVA: 0x00284EA5 File Offset: 0x002830A5
		public void JumpToFirstStage()
		{
			this.SetStage(0);
		}

		// Token: 0x06007B5A RID: 31578 RVA: 0x00284EAE File Offset: 0x002830AE
		public void JumpToLastStage()
		{
			if (this.stages == null || this.stages.Length == 0)
			{
				return;
			}
			this.SetStage(this.stages.Length - 1);
		}

		// Token: 0x06007B5B RID: 31579 RVA: 0x00284ED2 File Offset: 0x002830D2
		public void RestartCurrentStage()
		{
			this.RestartStageInternal();
		}

		// Token: 0x06007B5C RID: 31580 RVA: 0x00284EDA File Offset: 0x002830DA
		public void JumpToStageIndex(int index)
		{
			this.SetStage(index);
		}

		// Token: 0x04008D1E RID: 36126
		[SerializeField]
		private bool enableLooping;

		// Token: 0x04008D1F RID: 36127
		[SerializeField]
		private int loopToStageOnComplete = 1;

		// Token: 0x04008D20 RID: 36128
		[SerializeField]
		private EvolvingCosmetic.EvolutionStage[] stages;

		// Token: 0x04008D21 RID: 36129
		private RubberDuckEvents networkEvents;

		// Token: 0x04008D22 RID: 36130
		private VRRig myRig;

		// Token: 0x04008D23 RID: 36131
		private CallLimiter callLimiter = new CallLimiter(5, 10f, 0.5f);

		// Token: 0x04008D24 RID: 36132
		private int activeStageIndex;

		// Token: 0x04008D25 RID: 36133
		private EvolvingCosmetic.EvolutionStage activeStage;

		// Token: 0x04008D26 RID: 36134
		private int nextEventIndex;

		// Token: 0x04008D27 RID: 36135
		private EvolvingCosmetic.EvolutionStage.EventAtTime nextEvent;

		// Token: 0x04008D28 RID: 36136
		private float totalElapsedTime;

		// Token: 0x04008D29 RID: 36137
		private float totalTimeOfPreviousStages;

		// Token: 0x04008D2A RID: 36138
		private float totalDuration;

		// Token: 0x04008D2B RID: 36139
		private float timeAtLoopStart;

		// Token: 0x04008D2C RID: 36140
		private float loopDuration;

		// Token: 0x04008D2D RID: 36141
		private Coroutine sendProgressDelayCoroutine;

		// Token: 0x02001330 RID: 4912
		[Serializable]
		private class EvolutionStage
		{
			// Token: 0x06007B5E RID: 31582 RVA: 0x00284F08 File Offset: 0x00283108
			private bool HasAnyFlag(EvolvingCosmetic.EvolutionStage.ProgressionFlags flag)
			{
				return (this.progressionFlags & flag) > EvolvingCosmetic.EvolutionStage.ProgressionFlags.None;
			}

			// Token: 0x17000C0E RID: 3086
			// (get) Token: 0x06007B5F RID: 31583 RVA: 0x00284F15 File Offset: 0x00283115
			public bool HasDuration
			{
				get
				{
					return this.HasAnyFlag(EvolvingCosmetic.EvolutionStage.ProgressionFlags.Time | EvolvingCosmetic.EvolutionStage.ProgressionFlags.Temperature);
				}
			}

			// Token: 0x17000C0F RID: 3087
			// (get) Token: 0x06007B60 RID: 31584 RVA: 0x00284F1E File Offset: 0x0028311E
			public bool HasTime
			{
				get
				{
					return this.HasAnyFlag(EvolvingCosmetic.EvolutionStage.ProgressionFlags.Time);
				}
			}

			// Token: 0x17000C10 RID: 3088
			// (get) Token: 0x06007B61 RID: 31585 RVA: 0x00284F27 File Offset: 0x00283127
			public bool HasTemperature
			{
				get
				{
					return this.HasAnyFlag(EvolvingCosmetic.EvolutionStage.ProgressionFlags.Temperature);
				}
			}

			// Token: 0x17000C11 RID: 3089
			// (get) Token: 0x06007B62 RID: 31586 RVA: 0x00284F30 File Offset: 0x00283130
			public float Duration
			{
				get
				{
					if (!this.HasDuration)
					{
						return 1f;
					}
					return this.durationSeconds;
				}
			}

			// Token: 0x06007B63 RID: 31587 RVA: 0x00284F46 File Offset: 0x00283146
			public float DeltaTime(float deltaTime)
			{
				return (this.HasTime ? deltaTime : 0f) + (this.HasTemperature ? (deltaTime * this.celsiusSpeedupMult.Evaluate(this.thermalReceiver.celsius)) : 0f);
			}

			// Token: 0x06007B64 RID: 31588 RVA: 0x00284F80 File Offset: 0x00283180
			public EvolvingCosmetic.EvolutionStage.EventAtTime GetEventOrNull(int index)
			{
				if (this.events == null || index < 0 || index >= this.events.Length)
				{
					return null;
				}
				return this.events[index];
			}

			// Token: 0x04008D2F RID: 36143
			private const float MIN_STAGE_TIME = 0.01f;

			// Token: 0x04008D30 RID: 36144
			public string debugName;

			// Token: 0x04008D31 RID: 36145
			public EvolvingCosmetic.EvolutionStage.ProgressionFlags progressionFlags = EvolvingCosmetic.EvolutionStage.ProgressionFlags.Time;

			// Token: 0x04008D32 RID: 36146
			[SerializeField]
			private float durationSeconds = float.NaN;

			// Token: 0x04008D33 RID: 36147
			public ThermalReceiver thermalReceiver;

			// Token: 0x04008D34 RID: 36148
			public AnimationCurve celsiusSpeedupMult = AnimationCurve.Linear(0f, 0f, 100f, 2f);

			// Token: 0x04008D35 RID: 36149
			public ContinuousPropertyArray continuousProperties;

			// Token: 0x04008D36 RID: 36150
			[SerializeField]
			private EvolvingCosmetic.EvolutionStage.EventAtTime[] events;

			// Token: 0x02001331 RID: 4913
			[Flags]
			public enum ProgressionFlags
			{
				// Token: 0x04008D38 RID: 36152
				None = 0,
				// Token: 0x04008D39 RID: 36153
				Time = 1,
				// Token: 0x04008D3A RID: 36154
				Temperature = 2
			}

			// Token: 0x02001332 RID: 4914
			[Serializable]
			public class EventAtTime : IComparable<EvolvingCosmetic.EvolutionStage.EventAtTime>
			{
				// Token: 0x17000C12 RID: 3090
				// (get) Token: 0x06007B66 RID: 31590 RVA: 0x00284FDC File Offset: 0x002831DC
				private string DynamicTimeLabel
				{
					get
					{
						if (this.type != EvolvingCosmetic.EvolutionStage.EventAtTime.Type.DurationFraction)
						{
							return "Time";
						}
						return "Fraction";
					}
				}

				// Token: 0x06007B67 RID: 31591 RVA: 0x00284FF2 File Offset: 0x002831F2
				public int CompareTo(EvolvingCosmetic.EvolutionStage.EventAtTime other)
				{
					return this.absoluteTime.CompareTo(other.absoluteTime);
				}

				// Token: 0x04008D3B RID: 36155
				public string debugName;

				// Token: 0x04008D3C RID: 36156
				public float time;

				// Token: 0x04008D3D RID: 36157
				public EvolvingCosmetic.EvolutionStage.EventAtTime.Type type;

				// Token: 0x04008D3E RID: 36158
				public float absoluteTime;

				// Token: 0x04008D3F RID: 36159
				public UnityEvent onTimeReached;

				// Token: 0x02001333 RID: 4915
				public enum Type
				{
					// Token: 0x04008D41 RID: 36161
					SecondsFromBeginning,
					// Token: 0x04008D42 RID: 36162
					SecondsBeforeEnd,
					// Token: 0x04008D43 RID: 36163
					DurationFraction
				}
			}
		}
	}
}
