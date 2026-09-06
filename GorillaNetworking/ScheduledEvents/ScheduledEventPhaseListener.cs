using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaNetworking.ScheduledEvents
{
	// Token: 0x02001132 RID: 4402
	public class ScheduledEventPhaseListener : MonoBehaviour
	{
		// Token: 0x06006E6D RID: 28269 RVA: 0x00239900 File Offset: 0x00237B00
		private void OnEnable()
		{
			if (ScheduledEventManager.Instance == null)
			{
				base.StartCoroutine(this.OnEnableDefered());
				return;
			}
			ScheduledEventManager.Instance.OnPhaseChanged += this.OnPhaseChange;
		}

		// Token: 0x06006E6E RID: 28270 RVA: 0x00239933 File Offset: 0x00237B33
		private IEnumerator OnEnableDefered()
		{
			while (ScheduledEventManager.Instance == null)
			{
				yield return null;
			}
			this.OnEnable();
			yield break;
		}

		// Token: 0x06006E6F RID: 28271 RVA: 0x00239944 File Offset: 0x00237B44
		private void OnPhaseChange(ScheduledEventPhase phase)
		{
			switch (phase)
			{
			case ScheduledEventPhase.Before:
			{
				UnityEvent<float> onBefore = this._onBefore;
				if (onBefore == null)
				{
					return;
				}
				onBefore.Invoke((float)ScheduledEventManager.Instance.SecondsUntilEventStart);
				return;
			}
			case ScheduledEventPhase.During:
			{
				UnityEvent<float> onDuring = this._onDuring;
				if (onDuring == null)
				{
					return;
				}
				onDuring.Invoke((float)ScheduledEventManager.Instance.SecondsUntilEventStart);
				return;
			}
			case ScheduledEventPhase.After:
			{
				UnityEvent<float> onAfter = this._onAfter;
				if (onAfter == null)
				{
					return;
				}
				onAfter.Invoke((float)ScheduledEventManager.Instance.SecondsUntilEventStart);
				return;
			}
			case ScheduledEventPhase.NoEvent:
			{
				UnityEvent<float> onNoEvent = this._onNoEvent;
				if (onNoEvent == null)
				{
					return;
				}
				onNoEvent.Invoke((float)ScheduledEventManager.Instance.SecondsUntilEventStart);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x06006E70 RID: 28272 RVA: 0x002399D7 File Offset: 0x00237BD7
		private void OnDisable()
		{
			ScheduledEventManager.Instance.OnPhaseChanged -= this.OnPhaseChange;
		}

		// Token: 0x06006E71 RID: 28273 RVA: 0x002399EF File Offset: 0x00237BEF
		public void DebugStartCountdown()
		{
			ScheduledEventManager.Instance.DebugStartCountdown();
		}

		// Token: 0x04007E9F RID: 32415
		[SerializeField]
		private UnityEvent<float> _onBefore;

		// Token: 0x04007EA0 RID: 32416
		[SerializeField]
		private UnityEvent<float> _onDuring;

		// Token: 0x04007EA1 RID: 32417
		[SerializeField]
		private UnityEvent<float> _onAfter;

		// Token: 0x04007EA2 RID: 32418
		[SerializeField]
		private UnityEvent<float> _onNoEvent;
	}
}
