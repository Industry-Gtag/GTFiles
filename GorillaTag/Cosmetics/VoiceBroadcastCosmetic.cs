using System;
using GorillaTag.Audio;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200136C RID: 4972
	[RequireComponent(typeof(LoudSpeakerActivator))]
	public class VoiceBroadcastCosmetic : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06007C8A RID: 31882 RVA: 0x0028B05E File Offset: 0x0028925E
		private void Awake()
		{
			this.loudSpeaker = base.GetComponent<LoudSpeakerActivator>();
			this.animator = base.GetComponent<Animator>();
			this.talkAnimationTrigger = Animator.StringToHash(this.talkAnimationTriggerName);
			this.gsl = base.GetComponentInParent<GorillaSpeakerLoudness>();
		}

		// Token: 0x06007C8B RID: 31883 RVA: 0x0028B095 File Offset: 0x00289295
		public void SetWearable(VoiceBroadcastCosmeticWearable wearable)
		{
			this.wearable = wearable;
		}

		// Token: 0x06007C8C RID: 31884 RVA: 0x0028B0A0 File Offset: 0x002892A0
		private void StartBroadcast()
		{
			this.loudSpeaker.StartLocalBroadcast();
			UnityEvent unityEvent = this.onStartListening;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			UnityEvent unityEvent2 = this.onStartListening;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke();
			}
			this.wearable.OnCosmeticStartListening();
			this.lastSliceUpdateTime = Time.time;
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x06007C8D RID: 31885 RVA: 0x0028B0F7 File Offset: 0x002892F7
		private void StopBroadcast()
		{
			this.loudSpeaker.StopLocalBroadcast();
			UnityEvent unityEvent = this.onStopListening;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.wearable.OnCosmeticStopListening();
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x06007C8E RID: 31886 RVA: 0x0028B128 File Offset: 0x00289328
		public void OnEnable()
		{
			this.isListening = false;
			this.speakingTime = 0f;
		}

		// Token: 0x06007C8F RID: 31887 RVA: 0x0028B13C File Offset: 0x0028933C
		public void OnDisable()
		{
			this.isListening = false;
			this.speakingTime = 0f;
			this.StopBroadcast();
		}

		// Token: 0x06007C90 RID: 31888 RVA: 0x0028B158 File Offset: 0x00289358
		public void SetListenState(bool listening)
		{
			if (this.isListening == listening || !base.enabled || !base.gameObject.activeInHierarchy)
			{
				return;
			}
			this.isListening = listening;
			this.speakingTime = 0f;
			if (listening)
			{
				this.StartBroadcast();
				return;
			}
			this.StopBroadcast();
		}

		// Token: 0x06007C91 RID: 31889 RVA: 0x0028B1A8 File Offset: 0x002893A8
		public void SliceUpdate()
		{
			float num = Time.time - this.lastSliceUpdateTime;
			this.lastSliceUpdateTime = Time.time;
			if (this.gsl != null && this.gsl.IsSpeaking && this.gsl.LoudnessNormalized >= this.minVolume)
			{
				this.speakingTime += num;
				if (this.speakingTime >= this.minSpeakingTime)
				{
					if (this.animator != null)
					{
						this.animator.SetTrigger(this.talkAnimationTrigger);
					}
					if (this.simpleAnimation != null && !this.simpleAnimation.isPlaying)
					{
						this.simpleAnimation.Play();
					}
					if (!this.isSpeaking)
					{
						UnityEvent unityEvent = this.onStartSpeaking;
						if (unityEvent != null)
						{
							unityEvent.Invoke();
						}
						this.isSpeaking = true;
						return;
					}
				}
			}
			else
			{
				this.speakingTime = 0f;
				if (this.isSpeaking)
				{
					UnityEvent unityEvent2 = this.onStopSpeaking;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke();
					}
					this.isSpeaking = false;
				}
			}
		}

		// Token: 0x06007C92 RID: 31890 RVA: 0x0028B2B5 File Offset: 0x002894B5
		private void ResetToFirstFrame()
		{
			this.simpleAnimation.Rewind();
			this.simpleAnimation.Play();
			this.simpleAnimation.Sample();
			this.simpleAnimation.Stop();
		}

		// Token: 0x04008F17 RID: 36631
		public TalkingCosmeticType talkingCosmeticType;

		// Token: 0x04008F18 RID: 36632
		[Tooltip("How loud the Gorilla voice should be before detecting as talking.")]
		[SerializeField]
		public float minVolume = 0.1f;

		// Token: 0x04008F19 RID: 36633
		[Tooltip("How long the initial speaking section needs to last to trigger the talking animation.")]
		[SerializeField]
		public float minSpeakingTime = 0.15f;

		// Token: 0x04008F1A RID: 36634
		[SerializeField]
		private Animation simpleAnimation;

		// Token: 0x04008F1B RID: 36635
		[SerializeField]
		private string talkAnimationTriggerName;

		// Token: 0x04008F1C RID: 36636
		private int talkAnimationTrigger;

		// Token: 0x04008F1D RID: 36637
		private const string EVENTS = "Events";

		// Token: 0x04008F1E RID: 36638
		[SerializeField]
		private UnityEvent onStartListening;

		// Token: 0x04008F1F RID: 36639
		[SerializeField]
		private UnityEvent onStartSpeaking;

		// Token: 0x04008F20 RID: 36640
		[SerializeField]
		private UnityEvent onStopSpeaking;

		// Token: 0x04008F21 RID: 36641
		[SerializeField]
		private UnityEvent onStopListening;

		// Token: 0x04008F22 RID: 36642
		private float speakingTime;

		// Token: 0x04008F23 RID: 36643
		private bool isListening;

		// Token: 0x04008F24 RID: 36644
		private bool isSpeaking;

		// Token: 0x04008F25 RID: 36645
		private VoiceBroadcastCosmeticWearable wearable;

		// Token: 0x04008F26 RID: 36646
		private LoudSpeakerActivator loudSpeaker;

		// Token: 0x04008F27 RID: 36647
		private GorillaSpeakerLoudness gsl;

		// Token: 0x04008F28 RID: 36648
		private Animator animator;

		// Token: 0x04008F29 RID: 36649
		private float lastSliceUpdateTime;
	}
}
