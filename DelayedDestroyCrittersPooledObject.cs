using System;
using UnityEngine;

// Token: 0x02000084 RID: 132
public class DelayedDestroyCrittersPooledObject : MonoBehaviour
{
	// Token: 0x06000338 RID: 824 RVA: 0x00013857 File Offset: 0x00011A57
	protected void OnEnable()
	{
		if (ObjectPools.instance == null || !ObjectPools.instance.initialized)
		{
			return;
		}
		this.timeToDie = Time.time + this.destroyDelay;
	}

	// Token: 0x06000339 RID: 825 RVA: 0x00013885 File Offset: 0x00011A85
	protected void LateUpdate()
	{
		if (Time.time >= this.timeToDie)
		{
			CrittersPool.Return(base.gameObject);
		}
	}

	// Token: 0x040003D8 RID: 984
	public float destroyDelay = 1f;

	// Token: 0x040003D9 RID: 985
	private float timeToDie = -1f;
}
