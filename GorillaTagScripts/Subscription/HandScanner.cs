using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription
{
	// Token: 0x02000FF5 RID: 4085
	public class HandScanner : ObservableBehavior, IClickable
	{
		// Token: 0x0600658A RID: 25994 RVA: 0x0020B1DC File Offset: 0x002093DC
		protected override void ObservableSliceUpdate()
		{
			if (this.scanningRig == null)
			{
				return;
			}
			if (Time.time - this.scanStart > this.scanTime)
			{
				UnityEvent<NetPlayer> unityEvent = this.onHandScanSuccess;
				if (unityEvent != null)
				{
					unityEvent.Invoke(this.scanningRig.creator);
				}
				this.scanningRig = null;
			}
		}

		// Token: 0x0600658B RID: 25995 RVA: 0x0020B22F File Offset: 0x0020942F
		protected override void OnBecameObservable()
		{
			UnityEvent unityEvent = this.onHandScanInRange;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x0600658C RID: 25996 RVA: 0x0020B241 File Offset: 0x00209441
		protected override void OnLostObservable()
		{
			UnityEvent unityEvent = this.onHandScanOutOfRange;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x0600658D RID: 25997 RVA: 0x0020B254 File Offset: 0x00209454
		private void OnTriggerEnter(Collider other)
		{
			SIScannableHand component = other.GetComponent<SIScannableHand>();
			if (component == null)
			{
				return;
			}
			VRRig componentInParent = component.GetComponentInParent<VRRig>();
			if (componentInParent != null && componentInParent.isLocal)
			{
				this.scanningRig = componentInParent;
				this.scanStart = Time.time;
				UnityEvent<NetPlayer> unityEvent = this.onHandScanStart;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(this.scanningRig.creator);
			}
		}

		// Token: 0x0600658E RID: 25998 RVA: 0x0020B2B8 File Offset: 0x002094B8
		private void OnTriggerExit(Collider other)
		{
			SIScannableHand component = other.GetComponent<SIScannableHand>();
			if (component == null)
			{
				return;
			}
			VRRig componentInParent = component.GetComponentInParent<VRRig>();
			if (componentInParent != null && componentInParent == this.scanningRig && componentInParent.isLocal)
			{
				UnityEvent<NetPlayer> unityEvent = this.onHandScanAbort;
				if (unityEvent != null)
				{
					unityEvent.Invoke(this.scanningRig.creator);
				}
				this.scanningRig = null;
			}
		}

		// Token: 0x0600658F RID: 25999 RVA: 0x0020B31F File Offset: 0x0020951F
		public void Click(bool leftHand = false)
		{
			UnityEvent<NetPlayer> unityEvent = this.onHandScanStart;
			if (unityEvent != null)
			{
				unityEvent.Invoke(VRRig.LocalRig.creator);
			}
			UnityEvent<NetPlayer> unityEvent2 = this.onHandScanSuccess;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(VRRig.LocalRig.creator);
		}

		// Token: 0x04007488 RID: 29832
		[SerializeField]
		private float scanTime = 1f;

		// Token: 0x04007489 RID: 29833
		public UnityEvent<NetPlayer> onHandScanStart;

		// Token: 0x0400748A RID: 29834
		public UnityEvent<NetPlayer> onHandScanAbort;

		// Token: 0x0400748B RID: 29835
		public UnityEvent<NetPlayer> onHandScanSuccess;

		// Token: 0x0400748C RID: 29836
		public UnityEvent onHandScanInRange;

		// Token: 0x0400748D RID: 29837
		public UnityEvent onHandScanOutOfRange;

		// Token: 0x0400748E RID: 29838
		private VRRig scanningRig;

		// Token: 0x0400748F RID: 29839
		private float scanStart;
	}
}
