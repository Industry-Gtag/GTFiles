using System;
using UnityEngine;

// Token: 0x02000014 RID: 20
public class ParentedObjectStressTestMain : MonoBehaviour
{
	// Token: 0x06000058 RID: 88 RVA: 0x0000306C File Offset: 0x0000126C
	public void Start()
	{
		for (int i = 0; i < (int)this.NumObjects.x; i++)
		{
			for (int j = 0; j < (int)this.NumObjects.y; j++)
			{
				for (int k = 0; k < (int)this.NumObjects.z; k++)
				{
					global::UnityEngine.Object.Instantiate<GameObject>(this.Object).transform.position = new Vector3(2f * ((float)i / (this.NumObjects.x - 1f) - 0.5f) * this.NumObjects.x * this.Spacing.x, 2f * ((float)j / (this.NumObjects.y - 1f) - 0.5f) * this.NumObjects.y * this.Spacing.y, 2f * ((float)k / (this.NumObjects.z - 1f) - 0.5f) * this.NumObjects.z * this.Spacing.z);
				}
			}
		}
	}

	// Token: 0x0400003D RID: 61
	public GameObject Object;

	// Token: 0x0400003E RID: 62
	public Vector3 NumObjects;

	// Token: 0x0400003F RID: 63
	public Vector3 Spacing;
}
