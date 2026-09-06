using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x02000B06 RID: 2822
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 64)]
public struct m4x4
{
	// Token: 0x0600483E RID: 18494 RVA: 0x00184E3C File Offset: 0x0018303C
	public m4x4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33)
	{
		this = default(m4x4);
		this.m00 = m00;
		this.m01 = m01;
		this.m02 = m02;
		this.m03 = m03;
		this.m10 = m10;
		this.m11 = m11;
		this.m12 = m12;
		this.m13 = m13;
		this.m20 = m20;
		this.m21 = m21;
		this.m22 = m22;
		this.m23 = m23;
		this.m30 = m30;
		this.m31 = m31;
		this.m32 = m32;
		this.m33 = m33;
	}

	// Token: 0x0600483F RID: 18495 RVA: 0x00184ECD File Offset: 0x001830CD
	public m4x4(Vector4 row0, Vector4 row1, Vector4 row2, Vector4 row3)
	{
		this = default(m4x4);
		this.r0 = row0;
		this.r1 = row1;
		this.r2 = row2;
		this.r3 = row3;
	}

	// Token: 0x06004840 RID: 18496 RVA: 0x00184EF4 File Offset: 0x001830F4
	public void Clear()
	{
		this.m00 = 0f;
		this.m01 = 0f;
		this.m02 = 0f;
		this.m03 = 0f;
		this.m10 = 0f;
		this.m11 = 0f;
		this.m12 = 0f;
		this.m13 = 0f;
		this.m20 = 0f;
		this.m21 = 0f;
		this.m22 = 0f;
		this.m23 = 0f;
		this.m30 = 0f;
		this.m31 = 0f;
		this.m32 = 0f;
		this.m33 = 0f;
	}

	// Token: 0x06004841 RID: 18497 RVA: 0x00184FB1 File Offset: 0x001831B1
	public void SetRow0(ref Vector4 v)
	{
		this.m00 = v.x;
		this.m01 = v.y;
		this.m02 = v.z;
		this.m03 = v.w;
	}

	// Token: 0x06004842 RID: 18498 RVA: 0x00184FE3 File Offset: 0x001831E3
	public void SetRow1(ref Vector4 v)
	{
		this.m10 = v.x;
		this.m11 = v.y;
		this.m12 = v.z;
		this.m13 = v.w;
	}

	// Token: 0x06004843 RID: 18499 RVA: 0x00185015 File Offset: 0x00183215
	public void SetRow2(ref Vector4 v)
	{
		this.m20 = v.x;
		this.m21 = v.y;
		this.m22 = v.z;
		this.m23 = v.w;
	}

	// Token: 0x06004844 RID: 18500 RVA: 0x00185047 File Offset: 0x00183247
	public void SetRow3(ref Vector4 v)
	{
		this.m30 = v.x;
		this.m31 = v.y;
		this.m32 = v.z;
		this.m33 = v.w;
	}

	// Token: 0x06004845 RID: 18501 RVA: 0x0018507C File Offset: 0x0018327C
	public void Transpose()
	{
		float num = this.m01;
		float num2 = this.m02;
		float num3 = this.m03;
		float num4 = this.m10;
		float num5 = this.m12;
		float num6 = this.m13;
		float num7 = this.m20;
		float num8 = this.m21;
		float num9 = this.m23;
		float num10 = this.m30;
		float num11 = this.m31;
		float num12 = this.m32;
		this.m01 = num4;
		this.m02 = num7;
		this.m03 = num10;
		this.m10 = num;
		this.m12 = num8;
		this.m13 = num11;
		this.m20 = num2;
		this.m21 = num5;
		this.m23 = num12;
		this.m30 = num3;
		this.m31 = num6;
		this.m32 = num9;
	}

	// Token: 0x06004846 RID: 18502 RVA: 0x00185141 File Offset: 0x00183341
	public void Set(ref Vector4 row0, ref Vector4 row1, ref Vector4 row2, ref Vector4 row3)
	{
		this.r0 = row0;
		this.r1 = row1;
		this.r2 = row2;
		this.r3 = row3;
	}

	// Token: 0x06004847 RID: 18503 RVA: 0x00185174 File Offset: 0x00183374
	public void SetTransposed(ref Vector4 row0, ref Vector4 row1, ref Vector4 row2, ref Vector4 row3)
	{
		this.m00 = row0.x;
		this.m01 = row1.x;
		this.m02 = row2.x;
		this.m03 = row3.x;
		this.m10 = row0.y;
		this.m11 = row1.y;
		this.m12 = row2.y;
		this.m13 = row3.y;
		this.m20 = row0.z;
		this.m21 = row1.z;
		this.m22 = row2.z;
		this.m23 = row3.z;
		this.m30 = row0.w;
		this.m31 = row1.w;
		this.m32 = row2.w;
		this.m33 = row3.w;
	}

	// Token: 0x06004848 RID: 18504 RVA: 0x00185248 File Offset: 0x00183448
	public void Set(ref Matrix4x4 x)
	{
		this.m00 = x.m00;
		this.m01 = x.m01;
		this.m02 = x.m02;
		this.m03 = x.m03;
		this.m10 = x.m10;
		this.m11 = x.m11;
		this.m12 = x.m12;
		this.m13 = x.m13;
		this.m20 = x.m20;
		this.m21 = x.m21;
		this.m22 = x.m22;
		this.m23 = x.m23;
		this.m30 = x.m30;
		this.m31 = x.m31;
		this.m32 = x.m32;
		this.m33 = x.m33;
	}

	// Token: 0x06004849 RID: 18505 RVA: 0x00185318 File Offset: 0x00183518
	public void SetTransposed(ref Matrix4x4 x)
	{
		this.m00 = x.m00;
		this.m01 = x.m10;
		this.m02 = x.m20;
		this.m03 = x.m30;
		this.m10 = x.m01;
		this.m11 = x.m11;
		this.m12 = x.m21;
		this.m13 = x.m31;
		this.m20 = x.m02;
		this.m21 = x.m12;
		this.m22 = x.m22;
		this.m23 = x.m32;
		this.m30 = x.m03;
		this.m31 = x.m13;
		this.m32 = x.m23;
		this.m33 = x.m33;
	}

	// Token: 0x0600484A RID: 18506 RVA: 0x001853E8 File Offset: 0x001835E8
	public void Push(ref Matrix4x4 x)
	{
		x.m00 = this.m00;
		x.m01 = this.m01;
		x.m02 = this.m02;
		x.m03 = this.m03;
		x.m10 = this.m10;
		x.m11 = this.m11;
		x.m12 = this.m12;
		x.m13 = this.m13;
		x.m20 = this.m20;
		x.m21 = this.m21;
		x.m22 = this.m22;
		x.m23 = this.m23;
		x.m30 = this.m30;
		x.m31 = this.m31;
		x.m32 = this.m32;
		x.m33 = this.m33;
	}

	// Token: 0x0600484B RID: 18507 RVA: 0x001854B8 File Offset: 0x001836B8
	public void PushTransposed(ref Matrix4x4 x)
	{
		x.m00 = this.m00;
		x.m01 = this.m10;
		x.m02 = this.m20;
		x.m03 = this.m30;
		x.m10 = this.m01;
		x.m11 = this.m11;
		x.m12 = this.m21;
		x.m13 = this.m31;
		x.m20 = this.m02;
		x.m21 = this.m12;
		x.m22 = this.m22;
		x.m23 = this.m32;
		x.m30 = this.m03;
		x.m31 = this.m13;
		x.m32 = this.m23;
		x.m33 = this.m33;
	}

	// Token: 0x0600484C RID: 18508 RVA: 0x00185585 File Offset: 0x00183785
	public static ref m4x4 From(ref Matrix4x4 src)
	{
		return Unsafe.As<Matrix4x4, m4x4>(ref src);
	}

	// Token: 0x04005AA9 RID: 23209
	[FixedBuffer(typeof(float), 16)]
	[NonSerialized]
	[FieldOffset(0)]
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public m4x4.<data_f>e__FixedBuffer data_f;

	// Token: 0x04005AAA RID: 23210
	[FixedBuffer(typeof(int), 16)]
	[NonSerialized]
	[FieldOffset(0)]
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public m4x4.<data_i>e__FixedBuffer data_i;

	// Token: 0x04005AAB RID: 23211
	[FixedBuffer(typeof(ushort), 32)]
	[NonSerialized]
	[FieldOffset(0)]
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
	public m4x4.<data_h>e__FixedBuffer data_h;

	// Token: 0x04005AAC RID: 23212
	[NonSerialized]
	[FieldOffset(0)]
	public Vector4 r0;

	// Token: 0x04005AAD RID: 23213
	[NonSerialized]
	[FieldOffset(16)]
	public Vector4 r1;

	// Token: 0x04005AAE RID: 23214
	[NonSerialized]
	[FieldOffset(32)]
	public Vector4 r2;

	// Token: 0x04005AAF RID: 23215
	[NonSerialized]
	[FieldOffset(48)]
	public Vector4 r3;

	// Token: 0x04005AB0 RID: 23216
	[NonSerialized]
	[FieldOffset(0)]
	public float m00;

	// Token: 0x04005AB1 RID: 23217
	[NonSerialized]
	[FieldOffset(4)]
	public float m01;

	// Token: 0x04005AB2 RID: 23218
	[NonSerialized]
	[FieldOffset(8)]
	public float m02;

	// Token: 0x04005AB3 RID: 23219
	[NonSerialized]
	[FieldOffset(12)]
	public float m03;

	// Token: 0x04005AB4 RID: 23220
	[NonSerialized]
	[FieldOffset(16)]
	public float m10;

	// Token: 0x04005AB5 RID: 23221
	[NonSerialized]
	[FieldOffset(20)]
	public float m11;

	// Token: 0x04005AB6 RID: 23222
	[NonSerialized]
	[FieldOffset(24)]
	public float m12;

	// Token: 0x04005AB7 RID: 23223
	[NonSerialized]
	[FieldOffset(28)]
	public float m13;

	// Token: 0x04005AB8 RID: 23224
	[NonSerialized]
	[FieldOffset(32)]
	public float m20;

	// Token: 0x04005AB9 RID: 23225
	[NonSerialized]
	[FieldOffset(36)]
	public float m21;

	// Token: 0x04005ABA RID: 23226
	[NonSerialized]
	[FieldOffset(40)]
	public float m22;

	// Token: 0x04005ABB RID: 23227
	[NonSerialized]
	[FieldOffset(44)]
	public float m23;

	// Token: 0x04005ABC RID: 23228
	[NonSerialized]
	[FieldOffset(48)]
	public float m30;

	// Token: 0x04005ABD RID: 23229
	[NonSerialized]
	[FieldOffset(52)]
	public float m31;

	// Token: 0x04005ABE RID: 23230
	[NonSerialized]
	[FieldOffset(56)]
	public float m32;

	// Token: 0x04005ABF RID: 23231
	[NonSerialized]
	[FieldOffset(60)]
	public float m33;

	// Token: 0x04005AC0 RID: 23232
	[HideInInspector]
	[FieldOffset(0)]
	public int i00;

	// Token: 0x04005AC1 RID: 23233
	[HideInInspector]
	[FieldOffset(4)]
	public int i01;

	// Token: 0x04005AC2 RID: 23234
	[HideInInspector]
	[FieldOffset(8)]
	public int i02;

	// Token: 0x04005AC3 RID: 23235
	[HideInInspector]
	[FieldOffset(12)]
	public int i03;

	// Token: 0x04005AC4 RID: 23236
	[HideInInspector]
	[FieldOffset(16)]
	public int i10;

	// Token: 0x04005AC5 RID: 23237
	[HideInInspector]
	[FieldOffset(20)]
	public int i11;

	// Token: 0x04005AC6 RID: 23238
	[HideInInspector]
	[FieldOffset(24)]
	public int i12;

	// Token: 0x04005AC7 RID: 23239
	[HideInInspector]
	[FieldOffset(28)]
	public int i13;

	// Token: 0x04005AC8 RID: 23240
	[HideInInspector]
	[FieldOffset(32)]
	public int i20;

	// Token: 0x04005AC9 RID: 23241
	[HideInInspector]
	[FieldOffset(36)]
	public int i21;

	// Token: 0x04005ACA RID: 23242
	[HideInInspector]
	[FieldOffset(40)]
	public int i22;

	// Token: 0x04005ACB RID: 23243
	[HideInInspector]
	[FieldOffset(44)]
	public int i23;

	// Token: 0x04005ACC RID: 23244
	[HideInInspector]
	[FieldOffset(48)]
	public int i30;

	// Token: 0x04005ACD RID: 23245
	[HideInInspector]
	[FieldOffset(52)]
	public int i31;

	// Token: 0x04005ACE RID: 23246
	[HideInInspector]
	[FieldOffset(56)]
	public int i32;

	// Token: 0x04005ACF RID: 23247
	[HideInInspector]
	[FieldOffset(60)]
	public int i33;

	// Token: 0x04005AD0 RID: 23248
	[NonSerialized]
	[FieldOffset(0)]
	public ushort h00_a;

	// Token: 0x04005AD1 RID: 23249
	[NonSerialized]
	[FieldOffset(2)]
	public ushort h00_b;

	// Token: 0x04005AD2 RID: 23250
	[NonSerialized]
	[FieldOffset(4)]
	public ushort h01_a;

	// Token: 0x04005AD3 RID: 23251
	[NonSerialized]
	[FieldOffset(6)]
	public ushort h01_b;

	// Token: 0x04005AD4 RID: 23252
	[NonSerialized]
	[FieldOffset(8)]
	public ushort h02_a;

	// Token: 0x04005AD5 RID: 23253
	[NonSerialized]
	[FieldOffset(10)]
	public ushort h02_b;

	// Token: 0x04005AD6 RID: 23254
	[NonSerialized]
	[FieldOffset(12)]
	public ushort h03_a;

	// Token: 0x04005AD7 RID: 23255
	[NonSerialized]
	[FieldOffset(14)]
	public ushort h03_b;

	// Token: 0x04005AD8 RID: 23256
	[NonSerialized]
	[FieldOffset(16)]
	public ushort h10_a;

	// Token: 0x04005AD9 RID: 23257
	[NonSerialized]
	[FieldOffset(18)]
	public ushort h10_b;

	// Token: 0x04005ADA RID: 23258
	[NonSerialized]
	[FieldOffset(20)]
	public ushort h11_a;

	// Token: 0x04005ADB RID: 23259
	[NonSerialized]
	[FieldOffset(22)]
	public ushort h11_b;

	// Token: 0x04005ADC RID: 23260
	[NonSerialized]
	[FieldOffset(24)]
	public ushort h12_a;

	// Token: 0x04005ADD RID: 23261
	[NonSerialized]
	[FieldOffset(26)]
	public ushort h12_b;

	// Token: 0x04005ADE RID: 23262
	[NonSerialized]
	[FieldOffset(28)]
	public ushort h13_a;

	// Token: 0x04005ADF RID: 23263
	[NonSerialized]
	[FieldOffset(30)]
	public ushort h13_b;

	// Token: 0x04005AE0 RID: 23264
	[NonSerialized]
	[FieldOffset(32)]
	public ushort h20_a;

	// Token: 0x04005AE1 RID: 23265
	[NonSerialized]
	[FieldOffset(34)]
	public ushort h20_b;

	// Token: 0x04005AE2 RID: 23266
	[NonSerialized]
	[FieldOffset(36)]
	public ushort h21_a;

	// Token: 0x04005AE3 RID: 23267
	[NonSerialized]
	[FieldOffset(38)]
	public ushort h21_b;

	// Token: 0x04005AE4 RID: 23268
	[NonSerialized]
	[FieldOffset(40)]
	public ushort h22_a;

	// Token: 0x04005AE5 RID: 23269
	[NonSerialized]
	[FieldOffset(42)]
	public ushort h22_b;

	// Token: 0x04005AE6 RID: 23270
	[NonSerialized]
	[FieldOffset(44)]
	public ushort h23_a;

	// Token: 0x04005AE7 RID: 23271
	[NonSerialized]
	[FieldOffset(46)]
	public ushort h23_b;

	// Token: 0x04005AE8 RID: 23272
	[NonSerialized]
	[FieldOffset(48)]
	public ushort h30_a;

	// Token: 0x04005AE9 RID: 23273
	[NonSerialized]
	[FieldOffset(50)]
	public ushort h30_b;

	// Token: 0x04005AEA RID: 23274
	[NonSerialized]
	[FieldOffset(52)]
	public ushort h31_a;

	// Token: 0x04005AEB RID: 23275
	[NonSerialized]
	[FieldOffset(54)]
	public ushort h31_b;

	// Token: 0x04005AEC RID: 23276
	[NonSerialized]
	[FieldOffset(56)]
	public ushort h32_a;

	// Token: 0x04005AED RID: 23277
	[NonSerialized]
	[FieldOffset(58)]
	public ushort h32_b;

	// Token: 0x04005AEE RID: 23278
	[NonSerialized]
	[FieldOffset(60)]
	public ushort h33_a;

	// Token: 0x04005AEF RID: 23279
	[NonSerialized]
	[FieldOffset(62)]
	public ushort h33_b;

	// Token: 0x02000B07 RID: 2823
	[CompilerGenerated]
	[UnsafeValueType]
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	public struct <data_f>e__FixedBuffer
	{
		// Token: 0x04005AF0 RID: 23280
		public float FixedElementField;
	}

	// Token: 0x02000B08 RID: 2824
	[CompilerGenerated]
	[UnsafeValueType]
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	public struct <data_h>e__FixedBuffer
	{
		// Token: 0x04005AF1 RID: 23281
		public ushort FixedElementField;
	}

	// Token: 0x02000B09 RID: 2825
	[CompilerGenerated]
	[UnsafeValueType]
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	public struct <data_i>e__FixedBuffer
	{
		// Token: 0x04005AF2 RID: 23282
		public int FixedElementField;
	}
}
