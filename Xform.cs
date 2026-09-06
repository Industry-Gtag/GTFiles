using System;
using Drawing;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000B20 RID: 2848
[ExecuteAlways]
public class Xform : MonoBehaviour
{
	// Token: 0x170006ED RID: 1773
	// (get) Token: 0x06004928 RID: 18728 RVA: 0x001877A3 File Offset: 0x001859A3
	public float3 localExtents
	{
		get
		{
			return this.localScale * 0.5f;
		}
	}

	// Token: 0x06004929 RID: 18729 RVA: 0x001877B5 File Offset: 0x001859B5
	public Matrix4x4 LocalTRS()
	{
		return Matrix4x4.TRS(this.localPosition, this.localRotation, this.localScale);
	}

	// Token: 0x0600492A RID: 18730 RVA: 0x001877D8 File Offset: 0x001859D8
	public Matrix4x4 TRS()
	{
		if (this.parent.AsNull<Transform>() == null)
		{
			return this.LocalTRS();
		}
		return this.parent.localToWorldMatrix * this.LocalTRS();
	}

	// Token: 0x0600492B RID: 18731 RVA: 0x0018780C File Offset: 0x00185A0C
	private unsafe void Update()
	{
		Matrix4x4 matrix4x = this.TRS();
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithMatrix(matrix4x))
		{
			using (commandBuilder.WithLineWidth(2f, true))
			{
				commandBuilder.PlaneWithNormal(Xform.AXIS_XR_RT * 0.5f, Xform.AXIS_XR_RT, Xform.F2_ONE, Xform.CR);
				commandBuilder.PlaneWithNormal(Xform.AXIS_YG_UP * 0.5f, Xform.AXIS_YG_UP, Xform.F2_ONE, Xform.CG);
				commandBuilder.PlaneWithNormal(Xform.AXIS_ZB_FW * 0.5f, Xform.AXIS_ZB_FW, Xform.F2_ONE, Xform.CB);
				commandBuilder.WireBox(float3.zero, quaternion.identity, 1f, this.displayColor);
			}
		}
	}

	// Token: 0x04005B58 RID: 23384
	public Transform parent;

	// Token: 0x04005B59 RID: 23385
	[Space]
	public Color displayColor = SRand.New().NextColor();

	// Token: 0x04005B5A RID: 23386
	[Space]
	public float3 localPosition = float3.zero;

	// Token: 0x04005B5B RID: 23387
	public float3 localScale = Vector3.one;

	// Token: 0x04005B5C RID: 23388
	public Quaternion localRotation = quaternion.identity;

	// Token: 0x04005B5D RID: 23389
	private static readonly float3 F3_ONE = 1f;

	// Token: 0x04005B5E RID: 23390
	private static readonly float2 F2_ONE = 1f;

	// Token: 0x04005B5F RID: 23391
	private static readonly float3 AXIS_ZB_FW = new float3(0f, 0f, 1f);

	// Token: 0x04005B60 RID: 23392
	private static readonly float3 AXIS_YG_UP = new float3(0f, 1f, 0f);

	// Token: 0x04005B61 RID: 23393
	private static readonly float3 AXIS_XR_RT = new float3(1f, 0f, 0f);

	// Token: 0x04005B62 RID: 23394
	private static readonly Color CR = new Color(1f, 0f, 0f, 0.24f);

	// Token: 0x04005B63 RID: 23395
	private static readonly Color CG = new Color(0f, 1f, 0f, 0.24f);

	// Token: 0x04005B64 RID: 23396
	private static readonly Color CB = new Color(0f, 0f, 1f, 0.24f);
}
