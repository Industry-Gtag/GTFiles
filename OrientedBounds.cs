using System;
using UnityEngine;

// Token: 0x02000B10 RID: 2832
[Serializable]
public struct OrientedBounds
{
	// Token: 0x170006DD RID: 1757
	// (get) Token: 0x0600487A RID: 18554 RVA: 0x00185E2C File Offset: 0x0018402C
	public static OrientedBounds Empty { get; } = new OrientedBounds
	{
		size = Vector3.zero,
		center = Vector3.zero,
		rotation = Quaternion.identity
	};

	// Token: 0x170006DE RID: 1758
	// (get) Token: 0x0600487B RID: 18555 RVA: 0x00185E33 File Offset: 0x00184033
	public static OrientedBounds Identity { get; } = new OrientedBounds
	{
		size = Vector3.one,
		center = Vector3.zero,
		rotation = Quaternion.identity
	};

	// Token: 0x0600487C RID: 18556 RVA: 0x00185E3A File Offset: 0x0018403A
	public Matrix4x4 TRS()
	{
		return Matrix4x4.TRS(this.center, this.rotation, this.size);
	}

	// Token: 0x04005B08 RID: 23304
	public Vector3 size;

	// Token: 0x04005B09 RID: 23305
	public Vector3 center;

	// Token: 0x04005B0A RID: 23306
	public Quaternion rotation;
}
