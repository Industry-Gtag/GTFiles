using System;
using UnityEngine;

// Token: 0x02000DD4 RID: 3540
public class PredicatableRandomRotation : MonoBehaviour
{
	// Token: 0x060056B5 RID: 22197 RVA: 0x001C46FC File Offset: 0x001C28FC
	private void Start()
	{
		if (this.source == null)
		{
			this.source = base.transform;
		}
	}

	// Token: 0x060056B6 RID: 22198 RVA: 0x001C4718 File Offset: 0x001C2918
	private void Update()
	{
		float num = (this.source.position.x * this.source.position.x + this.source.position.y * this.source.position.y + this.source.position.z * this.source.position.z) % 1f;
		base.transform.Rotate(this.rot * num);
	}

	// Token: 0x0400679C RID: 26524
	[SerializeField]
	private Vector3 rot = Vector3.zero;

	// Token: 0x0400679D RID: 26525
	[SerializeField]
	private Transform source;
}
