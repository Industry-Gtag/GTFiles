using System;
using GorillaTag.Cosmetics;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000F3B RID: 3899
	public class GorillaIntervalTimer : MonoBehaviourPun
	{
		// Token: 0x06005FBE RID: 24510 RVA: 0x001E6023 File Offset: 0x001E4223
		private void Awake()
		{
			if (this.networkProvider == null)
			{
				this.networkProvider = base.GetComponentInParent<NetworkedRandomProvider>();
			}
			this.ResetElapsed();
			this.ResetRun();
		}

		// Token: 0x06005FBF RID: 24511 RVA: 0x001E604B File Offset: 0x001E424B
		private void OnEnable()
		{
			if (this.runOnEnable)
			{
				if (!this.isRegistered)
				{
					GorillaIntervalTimerManager.RegisterGorillaTimer(this);
					this.isRegistered = true;
				}
				this.StartTimer();
			}
		}

		// Token: 0x06005FC0 RID: 24512 RVA: 0x001E6070 File Offset: 0x001E4270
		private void OnDisable()
		{
			if (this.isRegistered)
			{
				GorillaIntervalTimerManager.UnregisterGorillaTimer(this);
				this.isRegistered = false;
			}
			this.StopTimer();
		}

		// Token: 0x06005FC1 RID: 24513 RVA: 0x001E6090 File Offset: 0x001E4290
		public void StartTimer()
		{
			if (!this.isRegistered)
			{
				GorillaIntervalTimerManager.RegisterGorillaTimer(this);
				this.isRegistered = true;
			}
			this.ResetRun();
			this.elapsed = 0f;
			this.isInPostFireDelay = false;
			if (this.useInitialDelay && this.initialDelay > 0f)
			{
				this.currentIntervalSeconds = Mathf.Max(0.001f, this.ToSeconds(this.initialDelay));
			}
			else
			{
				this.RollNextInterval();
			}
			this.isRunning = true;
			this.isPaused = false;
			UnityEvent unityEvent = this.onTimerStarted;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x06005FC2 RID: 24514 RVA: 0x001E6124 File Offset: 0x001E4324
		public void StopTimer()
		{
			this.isRunning = false;
			this.isPaused = false;
			this.elapsed = 0f;
			this.isInPostFireDelay = false;
			UnityEvent unityEvent = this.onTimerStopped;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			if (this.isRegistered)
			{
				GorillaIntervalTimerManager.UnregisterGorillaTimer(this);
				this.isRegistered = false;
			}
		}

		// Token: 0x06005FC3 RID: 24515 RVA: 0x001E6177 File Offset: 0x001E4377
		public void Pause()
		{
			this.isPaused = true;
		}

		// Token: 0x06005FC4 RID: 24516 RVA: 0x001E6180 File Offset: 0x001E4380
		public void Resume()
		{
			this.isPaused = false;
		}

		// Token: 0x06005FC5 RID: 24517 RVA: 0x001E618C File Offset: 0x001E438C
		public void SetFixedIntervalSeconds(float seconds)
		{
			this.useRandomDuration = false;
			this.fixedInterval = Mathf.Max(0f, seconds);
			this.currentIntervalSeconds = Mathf.Max(0.001f, this.ToSeconds(this.fixedInterval));
			this.elapsed = 0f;
		}

		// Token: 0x06005FC6 RID: 24518 RVA: 0x001E61D8 File Offset: 0x001E43D8
		public void OverrideNextIntervalSeconds(float seconds)
		{
			this.currentIntervalSeconds = Mathf.Max(0.001f, seconds);
			this.elapsed = 0f;
		}

		// Token: 0x06005FC7 RID: 24519 RVA: 0x001E61F6 File Offset: 0x001E43F6
		public void ResetRun()
		{
			this.runFiredSoFar = 0;
		}

		// Token: 0x06005FC8 RID: 24520 RVA: 0x001E6200 File Offset: 0x001E4400
		public void InvokeUpdate()
		{
			if (!this.isRunning || this.isPaused)
			{
				return;
			}
			this.elapsed += Time.deltaTime;
			if (this.elapsed >= this.currentIntervalSeconds)
			{
				if (this.isInPostFireDelay)
				{
					this.isInPostFireDelay = false;
					this.elapsed = 0f;
					this.RollNextInterval();
					return;
				}
				UnityEvent unityEvent = this.onIntervalFired;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
				this.runFiredSoFar++;
				if (this.runLength == GorillaIntervalTimer.RunLength.Finite && this.runFiredSoFar >= Mathf.Max(1, this.maxFiresPerRun))
				{
					if (this.requireManualReset)
					{
						this.StopTimer();
						return;
					}
					this.runFiredSoFar = 0;
				}
				if (this.usePostIntervalDelay && this.postIntervalDelay > 0f)
				{
					this.isInPostFireDelay = true;
					this.elapsed = 0f;
					this.currentIntervalSeconds = Mathf.Max(0.001f, this.ToSeconds(this.postIntervalDelay));
					return;
				}
				this.elapsed = 0f;
				this.RollNextInterval();
			}
		}

		// Token: 0x06005FC9 RID: 24521 RVA: 0x001E6309 File Offset: 0x001E4509
		private void ResetElapsed()
		{
			this.elapsed = 0f;
		}

		// Token: 0x06005FCA RID: 24522 RVA: 0x001E6318 File Offset: 0x001E4518
		private void RollNextInterval()
		{
			if (!this.useRandomDuration)
			{
				this.currentIntervalSeconds = Mathf.Max(0.001f, this.ToSeconds(this.fixedInterval));
				return;
			}
			float num = Mathf.Max(0f, this.ToSeconds(this.randTimeMin));
			float num2 = Mathf.Max(num, this.ToSeconds(this.randTimeMax));
			float num3;
			if (this.intervalSource == GorillaIntervalTimer.IntervalSource.NetworkedRandom && this.networkProvider != null)
			{
				switch (this.distribution)
				{
				default:
					num3 = this.networkProvider.NextFloat(num, num2);
					break;
				case GorillaIntervalTimer.RandomDistribution.Normal:
				{
					double num4 = Math.Max(double.Epsilon, 1.0 - this.networkProvider.NextDouble(0.0, 1.0));
					double num5 = Math.Max(double.Epsilon, 1.0 - (double)this.networkProvider.NextFloat01());
					double num6 = Math.Sqrt(-2.0 * Math.Log(num4)) * Math.Sin(6.283185307179586 * num5);
					float num7 = 0.5f * (num + num2);
					float num8 = (num2 - num) / 6f;
					num3 = Mathf.Clamp(num7 + (float)(num6 * (double)num8), num, num2);
					break;
				}
				case GorillaIntervalTimer.RandomDistribution.Exponential:
				{
					double num9 = Math.Max(double.Epsilon, 1.0 - this.networkProvider.NextDouble(0.0, 1.0));
					double num10 = 0.5 * (double)(num + num2);
					double num11 = ((num10 > 0.0) ? (1.0 / num10) : 1.0);
					num3 = Mathf.Clamp((float)(-(float)Math.Log(num9) / num11), num, num2);
					break;
				}
				}
				this.currentIntervalSeconds = Mathf.Max(0.001f, num3);
				return;
			}
			switch (this.distribution)
			{
			default:
				num3 = Random.Range(num, num2);
				break;
			case GorillaIntervalTimer.RandomDistribution.Normal:
			{
				float num12 = Mathf.Max(float.Epsilon, 1f - Random.value);
				float num13 = 1f - Random.value;
				float num14 = Mathf.Sqrt(-2f * Mathf.Log(num12)) * Mathf.Sin(6.2831855f * num13);
				float num15 = 0.5f * (num + num2);
				float num16 = (num2 - num) / 6f;
				num3 = Mathf.Clamp(num15 + num14 * num16, num, num2);
				break;
			}
			case GorillaIntervalTimer.RandomDistribution.Exponential:
			{
				float num17 = 0.5f * (num + num2);
				float num18 = ((num17 > 0f) ? (1f / num17) : 1f);
				num3 = Mathf.Clamp(-Mathf.Log(Mathf.Max(float.Epsilon, 1f - Random.value)) / num18, num, num2);
				break;
			}
			}
			this.currentIntervalSeconds = Mathf.Max(0.001f, num3);
		}

		// Token: 0x06005FCB RID: 24523 RVA: 0x001E65E8 File Offset: 0x001E47E8
		private float ToSeconds(float value)
		{
			switch (this.unit)
			{
			default:
				return value;
			case GorillaIntervalTimer.TimeUnit.Minutes:
				return value * 60f;
			case GorillaIntervalTimer.TimeUnit.Hours:
				return value * 3600f;
			}
		}

		// Token: 0x06005FCC RID: 24524 RVA: 0x001E661F File Offset: 0x001E481F
		public void RestartTimer()
		{
			this.ResetElapsed();
			this.RollNextInterval();
			this.StartTimer();
		}

		// Token: 0x06005FCD RID: 24525 RVA: 0x001E6633 File Offset: 0x001E4833
		public float GetPassedTime()
		{
			return this.elapsed;
		}

		// Token: 0x06005FCE RID: 24526 RVA: 0x001E663B File Offset: 0x001E483B
		public float GetRemainingTime()
		{
			return Mathf.Max(0f, this.currentIntervalSeconds - this.elapsed);
		}

		// Token: 0x04006E19 RID: 28185
		[Header("Scheduling")]
		[Tooltip("If true, the timer will automatically start when this component is enabled.")]
		[SerializeField]
		private bool runOnEnable = true;

		// Token: 0x04006E1A RID: 28186
		[Tooltip("If true, apply an initial delay before the first interval is fired.")]
		[SerializeField]
		private bool useInitialDelay;

		// Token: 0x04006E1B RID: 28187
		[Tooltip("Delay (in seconds or minutes depending on Unit) before the first fire if 'Use Initial Delay' is enabled.")]
		[SerializeField]
		private float initialDelay;

		// Token: 0x04006E1C RID: 28188
		[Header("Interval")]
		[Tooltip("Unit of time for Fixed Interval, Min and Max values.")]
		[SerializeField]
		private GorillaIntervalTimer.TimeUnit unit;

		// Token: 0x04006E1D RID: 28189
		[Tooltip("Distribution type used for generating random intervals when Interval Source = LocalRandom.")]
		[SerializeField]
		private GorillaIntervalTimer.RandomDistribution distribution;

		// Token: 0x04006E1E RID: 28190
		[Tooltip("Fixed interval duration (interpreted by Unit) when Use Random Duration = false.")]
		[SerializeField]
		private float fixedInterval = 1f;

		// Token: 0x04006E1F RID: 28191
		[Space]
		[Tooltip("If false, 'Fixed Interval' is used. If true, a random interval is sampled each cycle.")]
		[SerializeField]
		private bool useRandomDuration;

		// Token: 0x04006E20 RID: 28192
		[Tooltip("Minimum interval time (in selected Unit).")]
		[SerializeField]
		private float randTimeMin = 0.5f;

		// Token: 0x04006E21 RID: 28193
		[Tooltip("Maximum interval time (in selected Unit).")]
		[SerializeField]
		private float randTimeMax = 2f;

		// Token: 0x04006E22 RID: 28194
		[Tooltip("Determines whether to use a local random generator or a networked random source.")]
		[SerializeField]
		private GorillaIntervalTimer.IntervalSource intervalSource;

		// Token: 0x04006E23 RID: 28195
		[Header("Networked Interval (optional)")]
		[Tooltip("If Interval Source = NetworkedRandom, the timer queries this component for the next interval")]
		[SerializeField]
		private NetworkedRandomProvider networkProvider;

		// Token: 0x04006E24 RID: 28196
		[Space]
		[Tooltip("If true, wait this additional delay after onIntervalFired() before starting the next interval.")]
		[SerializeField]
		private bool usePostIntervalDelay;

		// Token: 0x04006E25 RID: 28197
		[Tooltip("Additional delay (in selected Unit) to wait after onIntervalFired(), before the next interval begins.")]
		[SerializeField]
		private float postIntervalDelay;

		// Token: 0x04006E26 RID: 28198
		[Header("Run Length")]
		[Tooltip("Infinite runs forever. Finite stops after Max Fires Per Run.")]
		[SerializeField]
		private GorillaIntervalTimer.RunLength runLength;

		// Token: 0x04006E27 RID: 28199
		[Tooltip("Number of times the timer fires before the run completes (when Run Length = Finite).")]
		[SerializeField]
		private int maxFiresPerRun = 3;

		// Token: 0x04006E28 RID: 28200
		[Tooltip("If true, the timer stops at the end of a finite run and requires ResetRun() / StartTimer() to continue. If false, the run counter auto-resets and continues.")]
		[SerializeField]
		private bool requireManualReset = true;

		// Token: 0x04006E29 RID: 28201
		[Header("Events")]
		public UnityEvent onIntervalFired;

		// Token: 0x04006E2A RID: 28202
		public UnityEvent onTimerStarted;

		// Token: 0x04006E2B RID: 28203
		public UnityEvent onTimerStopped;

		// Token: 0x04006E2C RID: 28204
		private const float minIntervalEpsilon = 0.001f;

		// Token: 0x04006E2D RID: 28205
		private float currentIntervalSeconds = 1f;

		// Token: 0x04006E2E RID: 28206
		private float elapsed;

		// Token: 0x04006E2F RID: 28207
		private bool isRunning;

		// Token: 0x04006E30 RID: 28208
		private bool isPaused;

		// Token: 0x04006E31 RID: 28209
		private bool isRegistered;

		// Token: 0x04006E32 RID: 28210
		private int runFiredSoFar;

		// Token: 0x04006E33 RID: 28211
		private bool isInPostFireDelay;

		// Token: 0x02000F3C RID: 3900
		private enum TimeUnit
		{
			// Token: 0x04006E35 RID: 28213
			Seconds,
			// Token: 0x04006E36 RID: 28214
			Minutes,
			// Token: 0x04006E37 RID: 28215
			Hours
		}

		// Token: 0x02000F3D RID: 3901
		private enum RandomDistribution
		{
			// Token: 0x04006E39 RID: 28217
			Uniform,
			// Token: 0x04006E3A RID: 28218
			Normal,
			// Token: 0x04006E3B RID: 28219
			Exponential
		}

		// Token: 0x02000F3E RID: 3902
		private enum IntervalSource
		{
			// Token: 0x04006E3D RID: 28221
			LocalRandom,
			// Token: 0x04006E3E RID: 28222
			NetworkedRandom
		}

		// Token: 0x02000F3F RID: 3903
		private enum RunLength
		{
			// Token: 0x04006E40 RID: 28224
			Infinite,
			// Token: 0x04006E41 RID: 28225
			Finite
		}
	}
}
