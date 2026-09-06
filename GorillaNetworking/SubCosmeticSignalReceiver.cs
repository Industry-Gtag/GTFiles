using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaNetworking
{
	// Token: 0x020010D1 RID: 4305
	public class SubCosmeticSignalReceiver : MonoBehaviour
	{
		// Token: 0x06006BA1 RID: 27553 RVA: 0x0022B0A4 File Offset: 0x002292A4
		public void ReceiveSignal(int signal)
		{
			for (int i = 0; i < this.triggers.Count; i++)
			{
				SubCosmeticSignalReceiver.SignalTrigger signalTrigger = this.triggers[i];
				if (signalTrigger != null && signalTrigger.signal == signal && (!signalTrigger.triggerOnce || !signalTrigger.hasTriggered))
				{
					UnityEvent onSignalReceived = signalTrigger.onSignalReceived;
					if (onSignalReceived != null)
					{
						onSignalReceived.Invoke();
					}
					if (signalTrigger.triggerOnce)
					{
						signalTrigger.hasTriggered = true;
					}
				}
			}
			UnityEvent<int> unityEvent = this.onAnySignal;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(signal);
		}

		// Token: 0x06006BA2 RID: 27554 RVA: 0x0022B124 File Offset: 0x00229324
		[Tooltip("Resets all 'triggerOnce' flags so single-fire triggers can fire again.")]
		public void ResetTriggers()
		{
			for (int i = 0; i < this.triggers.Count; i++)
			{
				if (this.triggers[i] != null)
				{
					this.triggers[i].hasTriggered = false;
				}
			}
		}

		// Token: 0x04007B11 RID: 31505
		[Header("Signal Triggers")]
		[SerializeField]
		private List<SubCosmeticSignalReceiver.SignalTrigger> triggers = new List<SubCosmeticSignalReceiver.SignalTrigger>();

		// Token: 0x04007B12 RID: 31506
		[Header("Catch-All")]
		[Tooltip("[Optional] event fired for every received signal regardless of value, after any specific triggers above run")]
		public UnityEvent<int> onAnySignal;

		// Token: 0x020010D2 RID: 4306
		[Serializable]
		public class SignalTrigger
		{
			// Token: 0x04007B13 RID: 31507
			[Tooltip("Integer signal value that fires this trigger")]
			public int signal;

			// Token: 0x04007B14 RID: 31508
			[Tooltip("Events invoked when this signal is received")]
			public UnityEvent onSignalReceived;

			// Token: 0x04007B15 RID: 31509
			[Tooltip("Fire only once per session, ignoring subsequent broadcasts of the same signal")]
			public bool triggerOnce;

			// Token: 0x04007B16 RID: 31510
			[NonSerialized]
			public bool hasTriggered;
		}
	}
}
