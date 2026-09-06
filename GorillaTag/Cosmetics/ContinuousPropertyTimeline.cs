using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001313 RID: 4883
	public class ContinuousPropertyTimeline : MonoBehaviour, ITickSystemTick, ISpawnable
	{
		// Token: 0x17000BF9 RID: 3065
		// (get) Token: 0x06007A7C RID: 31356 RVA: 0x0027F6DE File Offset: 0x0027D8DE
		// (set) Token: 0x06007A7D RID: 31357 RVA: 0x0027F6E9 File Offset: 0x0027D8E9
		private bool IsBackward
		{
			get
			{
				return !this.IsForward;
			}
			set
			{
				this.IsForward = !value;
			}
		}

		// Token: 0x17000BFA RID: 3066
		// (get) Token: 0x06007A7E RID: 31358 RVA: 0x0027F6F5 File Offset: 0x0027D8F5
		// (set) Token: 0x06007A7F RID: 31359 RVA: 0x0027F700 File Offset: 0x0027D900
		private bool IsPaused
		{
			get
			{
				return !this.IsPlaying;
			}
			set
			{
				this.IsPlaying = !value;
			}
		}

		// Token: 0x06007A80 RID: 31360 RVA: 0x0027F70C File Offset: 0x0027D90C
		public void TimelinePlay()
		{
			this.IsPlaying = true;
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06007A81 RID: 31361 RVA: 0x0027F71B File Offset: 0x0027D91B
		public void TimelinePause()
		{
			this.IsPaused = true;
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06007A82 RID: 31362 RVA: 0x0027F72A File Offset: 0x0027D92A
		public void TimelineToggleDirection()
		{
			this.IsForward = !this.IsForward;
		}

		// Token: 0x06007A83 RID: 31363 RVA: 0x0027F73B File Offset: 0x0027D93B
		public void TimelineTogglePlay()
		{
			if (this.IsPlaying)
			{
				this.TimelinePause();
				return;
			}
			this.TimelinePlay();
		}

		// Token: 0x06007A84 RID: 31364 RVA: 0x0027F752 File Offset: 0x0027D952
		public void TimelinePlayForward()
		{
			this.IsForward = true;
			this.TimelinePlay();
		}

		// Token: 0x06007A85 RID: 31365 RVA: 0x0027F761 File Offset: 0x0027D961
		public void TimelinePlayBackward()
		{
			this.IsBackward = true;
			this.TimelinePlay();
		}

		// Token: 0x06007A86 RID: 31366 RVA: 0x0027F770 File Offset: 0x0027D970
		public void TimelinePlayFromBeginning()
		{
			this.time = 0f;
			this.TimelinePlayForward();
			this.OnReachedBeginning();
		}

		// Token: 0x06007A87 RID: 31367 RVA: 0x0027F789 File Offset: 0x0027D989
		public void TimelinePlayFromEnd()
		{
			this.time = this.durationSeconds;
			this.TimelinePlayBackward();
			this.OnReachedEnd();
		}

		// Token: 0x06007A88 RID: 31368 RVA: 0x0027F7A3 File Offset: 0x0027D9A3
		public void TimelineScrubToTime(float t)
		{
			if (t <= 0f)
			{
				this.time = 0f;
				this.OnReachedBeginning();
				return;
			}
			if (t >= this.durationSeconds)
			{
				this.time = this.durationSeconds;
				this.OnReachedEnd();
				return;
			}
			this.time = t;
		}

		// Token: 0x06007A89 RID: 31369 RVA: 0x0027F7E2 File Offset: 0x0027D9E2
		public void TimelineScrubToFraction(float f)
		{
			this.TimelineScrubToTime(f * this.durationSeconds);
		}

		// Token: 0x06007A8A RID: 31370 RVA: 0x0027F7F2 File Offset: 0x0027D9F2
		public void TimelineSetDuration(float d)
		{
			this.durationSeconds = d;
			this.inverseDuration = 1f / this.durationSeconds;
			this.backwardDeltaMult = this.durationSeconds / this.backwardDuration;
		}

		// Token: 0x06007A8B RID: 31371 RVA: 0x0027F820 File Offset: 0x0027DA20
		public void TimelineSetBackwardDuration(float d)
		{
			this.separateBackwardDuration = true;
			this.backwardDuration = d;
			this.backwardDeltaMult = this.durationSeconds / this.backwardDuration;
		}

		// Token: 0x06007A8C RID: 31372 RVA: 0x0027F843 File Offset: 0x0027DA43
		private void Awake()
		{
			this.IsPlaying = this.startPlaying;
		}

		// Token: 0x06007A8D RID: 31373 RVA: 0x0027F854 File Offset: 0x0027DA54
		private void OnEnable()
		{
			if (this.myRig == null)
			{
				this.myRig = base.GetComponentInParent<VRRig>();
			}
			this.inverseDuration = 1f / this.durationSeconds;
			this.backwardDeltaMult = this.durationSeconds / this.backwardDuration;
			this.events.InvokeAll(ContinuousPropertyTimeline.TimelineEvent.OnEnable, this.myRig != null && this.myRig.isLocal);
			if (this.IsPlaying)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06007A8E RID: 31374 RVA: 0x0027F8D6 File Offset: 0x0027DAD6
		private void OnDisable()
		{
			this.events.InvokeAll(ContinuousPropertyTimeline.TimelineEvent.OnDisable, this.myRig != null && this.myRig.isLocal);
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06007A8F RID: 31375 RVA: 0x0027F908 File Offset: 0x0027DB08
		private void OnReachedEnd()
		{
			if (this.IsForward)
			{
				switch (this.endBehavior)
				{
				case ContinuousPropertyTimeline.TimelineEndBehavior.Stop:
					this.TimelinePause();
					this.time = this.durationSeconds;
					break;
				case ContinuousPropertyTimeline.TimelineEndBehavior.Loop:
					this.TimelinePlayFromBeginning();
					break;
				case ContinuousPropertyTimeline.TimelineEndBehavior.PingPong:
					this.IsBackward = true;
					this.time = this.durationSeconds;
					break;
				}
			}
			this.continuousProperties.cachedRigIsLocal = this.myRig != null && this.myRig.isLocal;
			this.continuousProperties.ApplyAll(1f);
			this.events.InvokeAll(ContinuousPropertyTimeline.TimelineEvent.OnReachedEnd, this.myRig != null && this.myRig.isLocal);
		}

		// Token: 0x06007A90 RID: 31376 RVA: 0x0027F9C8 File Offset: 0x0027DBC8
		private void OnReachedBeginning()
		{
			if (this.IsBackward)
			{
				switch (this.endBehavior)
				{
				case ContinuousPropertyTimeline.TimelineEndBehavior.Stop:
					this.TimelinePause();
					this.time = 0f;
					break;
				case ContinuousPropertyTimeline.TimelineEndBehavior.Loop:
					this.TimelinePlayFromEnd();
					break;
				case ContinuousPropertyTimeline.TimelineEndBehavior.PingPong:
					this.IsForward = true;
					this.time = 0f;
					break;
				}
			}
			this.continuousProperties.cachedRigIsLocal = this.myRig != null && this.myRig.isLocal;
			this.continuousProperties.ApplyAll(0f);
			this.events.InvokeAll(ContinuousPropertyTimeline.TimelineEvent.OnReachedBeginning, this.myRig != null && this.myRig.isLocal);
		}

		// Token: 0x06007A91 RID: 31377 RVA: 0x0027FA84 File Offset: 0x0027DC84
		private void InBetween()
		{
			float num = this.time * this.inverseDuration;
			this.continuousProperties.cachedRigIsLocal = this.myRig != null && this.myRig.isLocal;
			this.continuousProperties.ApplyAll(num);
		}

		// Token: 0x17000BFB RID: 3067
		// (get) Token: 0x06007A92 RID: 31378 RVA: 0x0027FAD2 File Offset: 0x0027DCD2
		// (set) Token: 0x06007A93 RID: 31379 RVA: 0x0027FADA File Offset: 0x0027DCDA
		public bool TickRunning { get; set; }

		// Token: 0x06007A94 RID: 31380 RVA: 0x0027FAE4 File Offset: 0x0027DCE4
		public void Tick()
		{
			if (this.IsForward)
			{
				this.time += Time.deltaTime;
				if (this.time >= this.durationSeconds)
				{
					this.OnReachedEnd();
					return;
				}
				this.InBetween();
				return;
			}
			else
			{
				this.time -= Time.deltaTime * this.backwardDeltaMult;
				if (this.time <= 0f)
				{
					this.OnReachedBeginning();
					return;
				}
				this.InBetween();
				return;
			}
		}

		// Token: 0x17000BFC RID: 3068
		// (get) Token: 0x06007A95 RID: 31381 RVA: 0x0027FB5A File Offset: 0x0027DD5A
		// (set) Token: 0x06007A96 RID: 31382 RVA: 0x0027FB62 File Offset: 0x0027DD62
		public bool IsSpawned { get; set; }

		// Token: 0x17000BFD RID: 3069
		// (get) Token: 0x06007A97 RID: 31383 RVA: 0x0027FB6B File Offset: 0x0027DD6B
		// (set) Token: 0x06007A98 RID: 31384 RVA: 0x0027FB73 File Offset: 0x0027DD73
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06007A99 RID: 31385 RVA: 0x0027FB7C File Offset: 0x0027DD7C
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x06007A9A RID: 31386 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnDespawn()
		{
		}

		// Token: 0x04008C10 RID: 35856
		[SerializeField]
		private float durationSeconds = 1f;

		// Token: 0x04008C11 RID: 35857
		[SerializeField]
		private float backwardDuration = 1f;

		// Token: 0x04008C12 RID: 35858
		[Tooltip("If true, the the timeline can move at a different speed when playing backwards.")]
		[SerializeField]
		private bool separateBackwardDuration;

		// Token: 0x04008C13 RID: 35859
		[Tooltip("When this object is enabled for the first time, should it immediately start playing from the beginning?")]
		[SerializeField]
		private bool startPlaying;

		// Token: 0x04008C14 RID: 35860
		[Tooltip("Determine what happens when the timeline reaches the end (or beginning while playing backwards).")]
		[SerializeField]
		private ContinuousPropertyTimeline.TimelineEndBehavior endBehavior;

		// Token: 0x04008C15 RID: 35861
		[SerializeField]
		private ContinuousPropertyArray continuousProperties;

		// Token: 0x04008C16 RID: 35862
		[SerializeField]
		private FlagEvents<ContinuousPropertyTimeline.TimelineEvent> events;

		// Token: 0x04008C17 RID: 35863
		private float time;

		// Token: 0x04008C18 RID: 35864
		private float inverseDuration;

		// Token: 0x04008C19 RID: 35865
		private float backwardDeltaMult;

		// Token: 0x04008C1A RID: 35866
		private bool IsForward = true;

		// Token: 0x04008C1B RID: 35867
		private bool IsPlaying;

		// Token: 0x04008C1D RID: 35869
		private VRRig myRig;

		// Token: 0x02001314 RID: 4884
		private enum TimelineEndBehavior
		{
			// Token: 0x04008C21 RID: 35873
			Stop,
			// Token: 0x04008C22 RID: 35874
			Loop,
			// Token: 0x04008C23 RID: 35875
			PingPong
		}

		// Token: 0x02001315 RID: 4885
		[Flags]
		private enum TimelineEvent
		{
			// Token: 0x04008C25 RID: 35877
			OnReachedEnd = 1,
			// Token: 0x04008C26 RID: 35878
			OnReachedBeginning = 2,
			// Token: 0x04008C27 RID: 35879
			OnEnable = 4,
			// Token: 0x04008C28 RID: 35880
			OnDisable = 8
		}
	}
}
