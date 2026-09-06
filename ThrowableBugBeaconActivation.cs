using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E4E RID: 3662
public class ThrowableBugBeaconActivation : MonoBehaviour
{
	// Token: 0x06005971 RID: 22897 RVA: 0x001D0DF4 File Offset: 0x001CEFF4
	private void Awake()
	{
		this.tbb = base.GetComponent<ThrowableBugBeacon>();
	}

	// Token: 0x06005972 RID: 22898 RVA: 0x001D0E02 File Offset: 0x001CF002
	private void OnEnable()
	{
		base.StartCoroutine(this.SendSignals());
	}

	// Token: 0x06005973 RID: 22899 RVA: 0x00005879 File Offset: 0x00003A79
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06005974 RID: 22900 RVA: 0x001D0E11 File Offset: 0x001CF011
	private IEnumerator SendSignals()
	{
		uint count = 0U;
		while (this.signalCount == 0U || count < this.signalCount)
		{
			yield return new WaitForSeconds(Random.Range(this.minCallTime, this.maxCallTime));
			switch (this.mode)
			{
			case ThrowableBugBeaconActivation.ActivationMode.CALL:
				this.tbb.Call();
				break;
			case ThrowableBugBeaconActivation.ActivationMode.DISMISS:
				this.tbb.Dismiss();
				break;
			case ThrowableBugBeaconActivation.ActivationMode.LOCK:
				this.tbb.Lock();
				break;
			}
			uint num = count;
			count = num + 1U;
		}
		yield break;
	}

	// Token: 0x040069C3 RID: 27075
	[SerializeField]
	private float minCallTime = 1f;

	// Token: 0x040069C4 RID: 27076
	[SerializeField]
	private float maxCallTime = 5f;

	// Token: 0x040069C5 RID: 27077
	[SerializeField]
	private uint signalCount;

	// Token: 0x040069C6 RID: 27078
	[SerializeField]
	private ThrowableBugBeaconActivation.ActivationMode mode;

	// Token: 0x040069C7 RID: 27079
	private ThrowableBugBeacon tbb;

	// Token: 0x02000E4F RID: 3663
	private enum ActivationMode
	{
		// Token: 0x040069C9 RID: 27081
		CALL,
		// Token: 0x040069CA RID: 27082
		DISMISS,
		// Token: 0x040069CB RID: 27083
		LOCK
	}
}
