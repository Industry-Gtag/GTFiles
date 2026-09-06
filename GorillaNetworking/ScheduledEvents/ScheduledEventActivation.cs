using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaNetworking.ScheduledEvents
{
	// Token: 0x02001127 RID: 4391
	public class ScheduledEventActivation : MonoBehaviour
	{
		// Token: 0x06006E13 RID: 28179 RVA: 0x00238489 File Offset: 0x00236689
		private void OnEnable()
		{
			if (ScheduledEventManager.Instance == null)
			{
				base.StartCoroutine(this.SubscribeWhenReady());
				return;
			}
			this.Subscribe();
		}

		// Token: 0x06006E14 RID: 28180 RVA: 0x002384AC File Offset: 0x002366AC
		private IEnumerator SubscribeWhenReady()
		{
			while (ScheduledEventManager.Instance == null)
			{
				yield return null;
			}
			if (base.isActiveAndEnabled)
			{
				this.Subscribe();
			}
			yield break;
		}

		// Token: 0x06006E15 RID: 28181 RVA: 0x002384BC File Offset: 0x002366BC
		private void Subscribe()
		{
			if (this.subscribed)
			{
				return;
			}
			this.subscribed = true;
			ScheduledEventManager.Instance.OnPhaseChanged += this.OnPhaseChanged;
			ScheduledEventManager.Instance.OnSubphaseChanged += this.OnSubphaseChanged;
			this.currentPhase = ScheduledEventManager.Instance.CurrentPhase;
			this.currentSubphase = ScheduledEventManager.Instance.EventSubphase;
			this.ApplyAll();
		}

		// Token: 0x06006E16 RID: 28182 RVA: 0x0023852C File Offset: 0x0023672C
		private void OnDisable()
		{
			if (this.subscribed && ScheduledEventManager.Instance != null)
			{
				ScheduledEventManager.Instance.OnPhaseChanged -= this.OnPhaseChanged;
				ScheduledEventManager.Instance.OnSubphaseChanged -= this.OnSubphaseChanged;
			}
			this.subscribed = false;
		}

		// Token: 0x06006E17 RID: 28183 RVA: 0x00238581 File Offset: 0x00236781
		private void OnPhaseChanged(ScheduledEventPhase phase)
		{
			this.currentPhase = phase;
			this.ApplyAll();
		}

		// Token: 0x06006E18 RID: 28184 RVA: 0x00238590 File Offset: 0x00236790
		private void OnSubphaseChanged(int subphase)
		{
			this.currentSubphase = subphase;
			if (this.currentPhase == ScheduledEventPhase.During)
			{
				this.ApplyAll();
			}
		}

		// Token: 0x06006E19 RID: 28185 RVA: 0x002385A8 File Offset: 0x002367A8
		private void ApplyAll()
		{
			if (this.nodes == null)
			{
				return;
			}
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i].Apply(this.currentPhase, this.currentSubphase);
			}
		}

		// Token: 0x04007E5E RID: 32350
		[SerializeField]
		private ScheduledEventActivation.ScheduledEventActivationTarget[] nodes;

		// Token: 0x04007E5F RID: 32351
		private bool subscribed;

		// Token: 0x04007E60 RID: 32352
		private ScheduledEventPhase currentPhase = ScheduledEventPhase.None;

		// Token: 0x04007E61 RID: 32353
		private int currentSubphase = -1;

		// Token: 0x02001128 RID: 4392
		[Serializable]
		private class ScheduledEventActivationTarget
		{
			// Token: 0x06006E1B RID: 28187 RVA: 0x00238600 File Offset: 0x00236800
			public bool MatchesPhase(ScheduledEventPhase phase)
			{
				switch (phase)
				{
				case ScheduledEventPhase.Before:
					return this.enableBefore;
				case ScheduledEventPhase.During:
					return this.enableDuring;
				case ScheduledEventPhase.After:
					return this.enableAfter;
				case ScheduledEventPhase.NoEvent:
					return this.enableIfNoEvent;
				default:
					return false;
				}
			}

			// Token: 0x06006E1C RID: 28188 RVA: 0x00238637 File Offset: 0x00236837
			public bool IsActive(ScheduledEventPhase phase, int subphase)
			{
				return this.MatchesPhase(phase) && (phase != ScheduledEventPhase.During || this.duringSubphases == null || this.duringSubphases.Length == 0 || Array.IndexOf<int>(this.duringSubphases, subphase) >= 0);
			}

			// Token: 0x06006E1D RID: 28189 RVA: 0x00238670 File Offset: 0x00236870
			public void Apply(ScheduledEventPhase phase, int subphase)
			{
				if (this.gameObject == null)
				{
					return;
				}
				bool flag = this.IsActive(phase, subphase);
				if (this.gameObject.activeSelf == flag)
				{
					return;
				}
				this.gameObject.SetActive(flag);
				if (flag)
				{
					UnityEvent unityEvent = this.onActivate;
					if (unityEvent == null)
					{
						return;
					}
					unityEvent.Invoke();
					return;
				}
				else
				{
					UnityEvent unityEvent2 = this.onDeactivate;
					if (unityEvent2 == null)
					{
						return;
					}
					unityEvent2.Invoke();
					return;
				}
			}

			// Token: 0x04007E62 RID: 32354
			[SerializeField]
			private GameObject gameObject;

			// Token: 0x04007E63 RID: 32355
			[SerializeField]
			private bool enableBefore;

			// Token: 0x04007E64 RID: 32356
			[SerializeField]
			private bool enableDuring;

			// Token: 0x04007E65 RID: 32357
			[SerializeField]
			private bool enableAfter;

			// Token: 0x04007E66 RID: 32358
			[SerializeField]
			private bool enableIfNoEvent;

			// Token: 0x04007E67 RID: 32359
			[Tooltip("Subphases (ScheduledEventManager.EventSubphase) in which this object is active during the event. Leave empty to be active for the entire During phase regardless of subphase.")]
			[SerializeField]
			private int[] duringSubphases;

			// Token: 0x04007E68 RID: 32360
			[SerializeField]
			private UnityEvent onActivate;

			// Token: 0x04007E69 RID: 32361
			[SerializeField]
			private UnityEvent onDeactivate;
		}
	}
}
