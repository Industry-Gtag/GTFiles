using System;
using UnityEngine;

// Token: 0x02000AEC RID: 2796
public class DistanceMeasure : MonoBehaviour
{
	// Token: 0x060047B9 RID: 18361 RVA: 0x001839C7 File Offset: 0x00181BC7
	private void Awake()
	{
		if (this.from == null)
		{
			this.from = base.transform;
		}
		if (this.to == null)
		{
			this.to = base.transform;
		}
	}

	// Token: 0x04005A3C RID: 23100
	public Transform from;

	// Token: 0x04005A3D RID: 23101
	public Transform to;
}
