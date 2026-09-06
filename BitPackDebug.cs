using System;
using UnityEngine;

// Token: 0x02000D8D RID: 3469
public class BitPackDebug : MonoBehaviour
{
	// Token: 0x040066E2 RID: 26338
	public bool debugPos;

	// Token: 0x040066E3 RID: 26339
	public Vector3 pos;

	// Token: 0x040066E4 RID: 26340
	public Vector3 min = Vector3.one * -2f;

	// Token: 0x040066E5 RID: 26341
	public Vector3 max = Vector3.one * 2f;

	// Token: 0x040066E6 RID: 26342
	public float rad = 4f;

	// Token: 0x040066E7 RID: 26343
	[Space]
	public bool debug32;

	// Token: 0x040066E8 RID: 26344
	public uint packed;

	// Token: 0x040066E9 RID: 26345
	public Vector3 unpacked;

	// Token: 0x040066EA RID: 26346
	[Space]
	public bool debug16;

	// Token: 0x040066EB RID: 26347
	public ushort packed16;

	// Token: 0x040066EC RID: 26348
	public Vector3 unpacked16;
}
