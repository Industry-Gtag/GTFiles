using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x0200033E RID: 830
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public class GTContactPoint
{
	// Token: 0x0400192F RID: 6447
	[NonSerialized]
	[FieldOffset(0)]
	public Matrix4x4 data;

	// Token: 0x04001930 RID: 6448
	[NonSerialized]
	[FieldOffset(0)]
	public Vector4 data0;

	// Token: 0x04001931 RID: 6449
	[NonSerialized]
	[FieldOffset(16)]
	public Vector4 data1;

	// Token: 0x04001932 RID: 6450
	[NonSerialized]
	[FieldOffset(32)]
	public Vector4 data2;

	// Token: 0x04001933 RID: 6451
	[NonSerialized]
	[FieldOffset(48)]
	public Vector4 data3;

	// Token: 0x04001934 RID: 6452
	[FieldOffset(0)]
	public Vector3 contactPoint;

	// Token: 0x04001935 RID: 6453
	[FieldOffset(12)]
	public float radius;

	// Token: 0x04001936 RID: 6454
	[FieldOffset(16)]
	public Vector3 counterVelocity;

	// Token: 0x04001937 RID: 6455
	[FieldOffset(28)]
	public float timestamp;

	// Token: 0x04001938 RID: 6456
	[FieldOffset(32)]
	public Color color;

	// Token: 0x04001939 RID: 6457
	[FieldOffset(48)]
	public GTContactType contactType;

	// Token: 0x0400193A RID: 6458
	[FieldOffset(52)]
	public float lifetime = 1f;

	// Token: 0x0400193B RID: 6459
	[FieldOffset(56)]
	public uint free = 1U;
}
