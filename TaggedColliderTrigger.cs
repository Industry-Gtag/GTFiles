using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000B1A RID: 2842
public class TaggedColliderTrigger : MonoBehaviour
{
	// Token: 0x060048EF RID: 18671 RVA: 0x00186F0E File Offset: 0x0018510E
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag(this.tag))
		{
			return;
		}
		if (this._sinceLastEnter.HasElapsed(this.enterHysteresis, true))
		{
			UnityEvent<Collider> unityEvent = this.onEnter;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(other);
		}
	}

	// Token: 0x060048F0 RID: 18672 RVA: 0x00186F44 File Offset: 0x00185144
	private void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag(this.tag))
		{
			return;
		}
		if (this._sinceLastExit.HasElapsed(this.exitHysteresis, true))
		{
			UnityEvent<Collider> unityEvent = this.onExit;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(other);
		}
	}

	// Token: 0x04005B32 RID: 23346
	public new UnityTag tag;

	// Token: 0x04005B33 RID: 23347
	public UnityEvent<Collider> onEnter = new UnityEvent<Collider>();

	// Token: 0x04005B34 RID: 23348
	public UnityEvent<Collider> onExit = new UnityEvent<Collider>();

	// Token: 0x04005B35 RID: 23349
	public float enterHysteresis = 0.125f;

	// Token: 0x04005B36 RID: 23350
	public float exitHysteresis = 0.125f;

	// Token: 0x04005B37 RID: 23351
	private TimeSince _sinceLastEnter;

	// Token: 0x04005B38 RID: 23352
	private TimeSince _sinceLastExit;
}
