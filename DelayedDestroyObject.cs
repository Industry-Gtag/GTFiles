using System;
using UnityEngine;

// Token: 0x02000085 RID: 133
public class DelayedDestroyObject : MonoBehaviour
{
	// Token: 0x0600033B RID: 827 RVA: 0x000138BD File Offset: 0x00011ABD
	private void Start()
	{
		this._timeToDie = Time.time + this.lifetime;
	}

	// Token: 0x0600033C RID: 828 RVA: 0x000138D1 File Offset: 0x00011AD1
	private void LateUpdate()
	{
		if (Time.time >= this._timeToDie)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040003DA RID: 986
	public float lifetime = 10f;

	// Token: 0x040003DB RID: 987
	private float _timeToDie;
}
