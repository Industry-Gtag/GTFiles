using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000F96 RID: 3990
	public class GorillaTimer : MonoBehaviourPun
	{
		// Token: 0x06006346 RID: 25414 RVA: 0x001FF175 File Offset: 0x001FD375
		private void Awake()
		{
			this.ResetTimer();
		}

		// Token: 0x06006347 RID: 25415 RVA: 0x001FF17D File Offset: 0x001FD37D
		public void StartTimer()
		{
			this.startTimer = true;
			UnityEvent<GorillaTimer> unityEvent = this.onTimerStarted;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}

		// Token: 0x06006348 RID: 25416 RVA: 0x001FF197 File Offset: 0x001FD397
		public IEnumerator DelayedReStartTimer(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			this.RestartTimer();
			yield break;
		}

		// Token: 0x06006349 RID: 25417 RVA: 0x001FF1AD File Offset: 0x001FD3AD
		private void StopTimer()
		{
			this.startTimer = false;
			UnityEvent<GorillaTimer> unityEvent = this.onTimerStopped;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}

		// Token: 0x0600634A RID: 25418 RVA: 0x001FF1C7 File Offset: 0x001FD3C7
		private void ResetTimer()
		{
			this.passedTime = 0f;
		}

		// Token: 0x0600634B RID: 25419 RVA: 0x001FF1D4 File Offset: 0x001FD3D4
		public void RestartTimer()
		{
			if (this.useRandomDuration)
			{
				this.SetTimerDuration(Random.Range(this.randTimeMin, this.randTimeMax));
			}
			this.ResetTimer();
			this.StartTimer();
		}

		// Token: 0x0600634C RID: 25420 RVA: 0x001FF201 File Offset: 0x001FD401
		public void SetTimerDuration(float timer)
		{
			this.timerDuration = timer;
		}

		// Token: 0x0600634D RID: 25421 RVA: 0x001FF20A File Offset: 0x001FD40A
		public void InvokeUpdate()
		{
			if (this.startTimer)
			{
				this.passedTime += Time.deltaTime;
			}
			if (this.startTimer && this.passedTime >= this.timerDuration)
			{
				this.StopTimer();
				this.ResetTimer();
			}
		}

		// Token: 0x0600634E RID: 25422 RVA: 0x001FF248 File Offset: 0x001FD448
		public float GetPassedTime()
		{
			return this.passedTime;
		}

		// Token: 0x0600634F RID: 25423 RVA: 0x001FF250 File Offset: 0x001FD450
		public void SetPassedTime(float time)
		{
			this.passedTime = time;
		}

		// Token: 0x06006350 RID: 25424 RVA: 0x001FF259 File Offset: 0x001FD459
		public float GetRemainingTime()
		{
			return this.timerDuration - this.passedTime;
		}

		// Token: 0x06006351 RID: 25425 RVA: 0x001FF268 File Offset: 0x001FD468
		public void OnEnable()
		{
			GorillaTimerManager.RegisterGorillaTimer(this);
		}

		// Token: 0x06006352 RID: 25426 RVA: 0x001FF270 File Offset: 0x001FD470
		public void OnDisable()
		{
			GorillaTimerManager.UnregisterGorillaTimer(this);
		}

		// Token: 0x040071ED RID: 29165
		[SerializeField]
		private float timerDuration;

		// Token: 0x040071EE RID: 29166
		[SerializeField]
		private bool useRandomDuration;

		// Token: 0x040071EF RID: 29167
		[SerializeField]
		private float randTimeMin;

		// Token: 0x040071F0 RID: 29168
		[SerializeField]
		private float randTimeMax;

		// Token: 0x040071F1 RID: 29169
		private float passedTime;

		// Token: 0x040071F2 RID: 29170
		private bool startTimer;

		// Token: 0x040071F3 RID: 29171
		private bool resetTimer;

		// Token: 0x040071F4 RID: 29172
		public UnityEvent<GorillaTimer> onTimerStarted;

		// Token: 0x040071F5 RID: 29173
		public UnityEvent<GorillaTimer> onTimerStopped;
	}
}
