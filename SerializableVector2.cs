using System;
using UnityEngine;

// Token: 0x02000430 RID: 1072
[Serializable]
public struct SerializableVector2
{
	// Token: 0x06001970 RID: 6512 RVA: 0x0008F0B5 File Offset: 0x0008D2B5
	public SerializableVector2(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	// Token: 0x06001971 RID: 6513 RVA: 0x0008F0C5 File Offset: 0x0008D2C5
	public static implicit operator SerializableVector2(Vector2 v)
	{
		return new SerializableVector2(v.x, v.y);
	}

	// Token: 0x06001972 RID: 6514 RVA: 0x0008F0D8 File Offset: 0x0008D2D8
	public static implicit operator Vector2(SerializableVector2 v)
	{
		return new Vector2(v.x, v.y);
	}

	// Token: 0x04002476 RID: 9334
	public float x;

	// Token: 0x04002477 RID: 9335
	public float y;
}
