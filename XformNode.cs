using System;
using UnityEngine;

// Token: 0x02000B21 RID: 2849
[Serializable]
public class XformNode
{
	// Token: 0x170006EE RID: 1774
	// (get) Token: 0x0600492E RID: 18734 RVA: 0x00187A30 File Offset: 0x00185C30
	public Vector4 worldPosition
	{
		get
		{
			if (!this.parent)
			{
				return this.localPosition;
			}
			Matrix4x4 localToWorldMatrix = this.parent.localToWorldMatrix;
			Vector4 vector = this.localPosition;
			MatrixUtils.MultiplyXYZ3x4(ref localToWorldMatrix, ref vector);
			return vector;
		}
	}

	// Token: 0x170006EF RID: 1775
	// (get) Token: 0x0600492F RID: 18735 RVA: 0x00187A6E File Offset: 0x00185C6E
	// (set) Token: 0x06004930 RID: 18736 RVA: 0x00187A7B File Offset: 0x00185C7B
	public float radius
	{
		get
		{
			return this.localPosition.w;
		}
		set
		{
			this.localPosition.w = value;
		}
	}

	// Token: 0x06004931 RID: 18737 RVA: 0x00187A89 File Offset: 0x00185C89
	public Matrix4x4 LocalTRS()
	{
		return Matrix4x4.TRS(this.localPosition, Quaternion.identity, Vector3.one);
	}

	// Token: 0x04005B65 RID: 23397
	public Vector4 localPosition;

	// Token: 0x04005B66 RID: 23398
	public Transform parent;
}
