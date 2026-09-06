using System;
using UnityEngine;

// Token: 0x02000431 RID: 1073
[Serializable]
public struct SerializableVector3
{
	// Token: 0x06001973 RID: 6515 RVA: 0x0008F0EB File Offset: 0x0008D2EB
	public SerializableVector3(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	// Token: 0x06001974 RID: 6516 RVA: 0x0008F102 File Offset: 0x0008D302
	public static implicit operator SerializableVector3(Vector3 v)
	{
		return new SerializableVector3(v.x, v.y, v.z);
	}

	// Token: 0x06001975 RID: 6517 RVA: 0x0008F11B File Offset: 0x0008D31B
	public static implicit operator Vector3(SerializableVector3 v)
	{
		return new Vector3(v.x, v.y, v.z);
	}

	// Token: 0x04002478 RID: 9336
	public float x;

	// Token: 0x04002479 RID: 9337
	public float y;

	// Token: 0x0400247A RID: 9338
	public float z;
}
