using System;
using UnityEngine;

// Token: 0x02000D9F RID: 3487
public class DelayedDestroyPooledObj : MonoBehaviour
{
	// Token: 0x060055D6 RID: 21974 RVA: 0x001C1A4A File Offset: 0x001BFC4A
	protected void OnEnable()
	{
		if (ObjectPools.instance == null || !ObjectPools.instance.initialized)
		{
			return;
		}
		this.timeToDie = Time.time + this.destroyDelay;
	}

	// Token: 0x060055D7 RID: 21975 RVA: 0x001C1A78 File Offset: 0x001BFC78
	protected void LateUpdate()
	{
		if (Time.time > this.timeToDie)
		{
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x04006741 RID: 26433
	[Tooltip("Return to the object pool after this many seconds.")]
	public float destroyDelay;

	// Token: 0x04006742 RID: 26434
	private float timeToDie = -1f;
}
