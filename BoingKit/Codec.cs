using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001452 RID: 5202
	public class Codec
	{
		// Token: 0x06008302 RID: 33538 RVA: 0x002AF28C File Offset: 0x002AD48C
		public static float PackSaturated(float a, float b)
		{
			a = Mathf.Floor(a * 4095f);
			b = Mathf.Floor(b * 4095f);
			return a * 4096f + b;
		}

		// Token: 0x06008303 RID: 33539 RVA: 0x002AF2B3 File Offset: 0x002AD4B3
		public static float PackSaturated(Vector2 v)
		{
			return Codec.PackSaturated(v.x, v.y);
		}

		// Token: 0x06008304 RID: 33540 RVA: 0x002AF2C6 File Offset: 0x002AD4C6
		public static Vector2 UnpackSaturated(float f)
		{
			return new Vector2(Mathf.Floor(f / 4096f), Mathf.Repeat(f, 4096f)) / 4095f;
		}

		// Token: 0x06008305 RID: 33541 RVA: 0x002AF2F0 File Offset: 0x002AD4F0
		public static Vector2 OctWrap(Vector2 v)
		{
			return (Vector2.one - new Vector2(Mathf.Abs(v.y), Mathf.Abs(v.x))) * new Vector2(Mathf.Sign(v.x), Mathf.Sign(v.y));
		}

		// Token: 0x06008306 RID: 33542 RVA: 0x002AF344 File Offset: 0x002AD544
		public static float PackNormal(Vector3 n)
		{
			n /= Mathf.Abs(n.x) + Mathf.Abs(n.y) + Mathf.Abs(n.z);
			return Codec.PackSaturated(((n.z >= 0f) ? new Vector2(n.x, n.y) : Codec.OctWrap(new Vector2(n.x, n.y))) * 0.5f + 0.5f * Vector2.one);
		}

		// Token: 0x06008307 RID: 33543 RVA: 0x002AF3D8 File Offset: 0x002AD5D8
		public static Vector3 UnpackNormal(float f)
		{
			Vector2 vector = Codec.UnpackSaturated(f);
			vector = vector * 2f - Vector2.one;
			Vector3 vector2 = new Vector3(vector.x, vector.y, 1f - Mathf.Abs(vector.x) - Mathf.Abs(vector.y));
			float num = Mathf.Clamp01(-vector2.z);
			vector2.x += ((vector2.x >= 0f) ? (-num) : num);
			vector2.y += ((vector2.y >= 0f) ? (-num) : num);
			return vector2.normalized;
		}

		// Token: 0x06008308 RID: 33544 RVA: 0x002AF480 File Offset: 0x002AD680
		public static uint PackRgb(Color color)
		{
			return ((uint)(color.b * 255f) << 16) | ((uint)(color.g * 255f) << 8) | (uint)(color.r * 255f);
		}

		// Token: 0x06008309 RID: 33545 RVA: 0x002AF4B0 File Offset: 0x002AD6B0
		public static Color UnpackRgb(uint i)
		{
			return new Color((i & 255U) / 255f, ((i & 65280U) >> 8) / 255f, ((i & 16711680U) >> 16) / 255f);
		}

		// Token: 0x0600830A RID: 33546 RVA: 0x002AF4EC File Offset: 0x002AD6EC
		public static uint PackRgba(Color color)
		{
			return ((uint)(color.a * 255f) << 24) | ((uint)(color.b * 255f) << 16) | ((uint)(color.g * 255f) << 8) | (uint)(color.r * 255f);
		}

		// Token: 0x0600830B RID: 33547 RVA: 0x002AF538 File Offset: 0x002AD738
		public static Color UnpackRgba(uint i)
		{
			return new Color((i & 255U) / 255f, ((i & 65280U) >> 8) / 255f, ((i & 16711680U) >> 16) / 255f, ((i & 4278190080U) >> 24) / 255f);
		}

		// Token: 0x0600830C RID: 33548 RVA: 0x002AF58E File Offset: 0x002AD78E
		public static uint Pack8888(uint x, uint y, uint z, uint w)
		{
			return ((x & 255U) << 24) | ((y & 255U) << 16) | ((z & 255U) << 8) | (w & 255U);
		}

		// Token: 0x0600830D RID: 33549 RVA: 0x002AF5B7 File Offset: 0x002AD7B7
		public static void Unpack8888(uint i, out uint x, out uint y, out uint z, out uint w)
		{
			x = (i >> 24) & 255U;
			y = (i >> 16) & 255U;
			z = (i >> 8) & 255U;
			w = i & 255U;
		}

		// Token: 0x0600830E RID: 33550 RVA: 0x002AF5E8 File Offset: 0x002AD7E8
		private static int IntReinterpret(float f)
		{
			return new Codec.IntFloat
			{
				FloatValue = f
			}.IntValue;
		}

		// Token: 0x0600830F RID: 33551 RVA: 0x002AF60B File Offset: 0x002AD80B
		public static int HashConcat(int hash, int i)
		{
			return (hash ^ i) * Codec.FnvPrime;
		}

		// Token: 0x06008310 RID: 33552 RVA: 0x002AF616 File Offset: 0x002AD816
		public static int HashConcat(int hash, long i)
		{
			hash = Codec.HashConcat(hash, (int)(i & (long)((ulong)(-1))));
			hash = Codec.HashConcat(hash, (int)(i >> 32));
			return hash;
		}

		// Token: 0x06008311 RID: 33553 RVA: 0x002AF633 File Offset: 0x002AD833
		public static int HashConcat(int hash, float f)
		{
			return Codec.HashConcat(hash, Codec.IntReinterpret(f));
		}

		// Token: 0x06008312 RID: 33554 RVA: 0x002AF641 File Offset: 0x002AD841
		public static int HashConcat(int hash, bool b)
		{
			return Codec.HashConcat(hash, b ? 1 : 0);
		}

		// Token: 0x06008313 RID: 33555 RVA: 0x002AF650 File Offset: 0x002AD850
		public static int HashConcat(int hash, params int[] ints)
		{
			foreach (int num in ints)
			{
				hash = Codec.HashConcat(hash, num);
			}
			return hash;
		}

		// Token: 0x06008314 RID: 33556 RVA: 0x002AF67C File Offset: 0x002AD87C
		public static int HashConcat(int hash, params float[] floats)
		{
			foreach (float num in floats)
			{
				hash = Codec.HashConcat(hash, num);
			}
			return hash;
		}

		// Token: 0x06008315 RID: 33557 RVA: 0x002AF6A7 File Offset: 0x002AD8A7
		public static int HashConcat(int hash, Vector2 v)
		{
			return Codec.HashConcat(hash, new float[] { v.x, v.y });
		}

		// Token: 0x06008316 RID: 33558 RVA: 0x002AF6C7 File Offset: 0x002AD8C7
		public static int HashConcat(int hash, Vector3 v)
		{
			return Codec.HashConcat(hash, new float[] { v.x, v.y, v.z });
		}

		// Token: 0x06008317 RID: 33559 RVA: 0x002AF6F0 File Offset: 0x002AD8F0
		public static int HashConcat(int hash, Vector4 v)
		{
			return Codec.HashConcat(hash, new float[] { v.x, v.y, v.z, v.w });
		}

		// Token: 0x06008318 RID: 33560 RVA: 0x002AF722 File Offset: 0x002AD922
		public static int HashConcat(int hash, Quaternion q)
		{
			return Codec.HashConcat(hash, new float[] { q.x, q.y, q.z, q.w });
		}

		// Token: 0x06008319 RID: 33561 RVA: 0x002AF754 File Offset: 0x002AD954
		public static int HashConcat(int hash, Color c)
		{
			return Codec.HashConcat(hash, new float[] { c.r, c.g, c.b, c.a });
		}

		// Token: 0x0600831A RID: 33562 RVA: 0x002AF786 File Offset: 0x002AD986
		public static int HashConcat(int hash, Transform t)
		{
			return Codec.HashConcat(hash, t.GetHashCode());
		}

		// Token: 0x0600831B RID: 33563 RVA: 0x002AF794 File Offset: 0x002AD994
		public static int Hash(int i)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, i);
		}

		// Token: 0x0600831C RID: 33564 RVA: 0x002AF7A1 File Offset: 0x002AD9A1
		public static int Hash(long i)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, i);
		}

		// Token: 0x0600831D RID: 33565 RVA: 0x002AF7AE File Offset: 0x002AD9AE
		public static int Hash(float f)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, f);
		}

		// Token: 0x0600831E RID: 33566 RVA: 0x002AF7BB File Offset: 0x002AD9BB
		public static int Hash(bool b)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, b);
		}

		// Token: 0x0600831F RID: 33567 RVA: 0x002AF7C8 File Offset: 0x002AD9C8
		public static int Hash(params int[] ints)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, ints);
		}

		// Token: 0x06008320 RID: 33568 RVA: 0x002AF7D5 File Offset: 0x002AD9D5
		public static int Hash(params float[] floats)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, floats);
		}

		// Token: 0x06008321 RID: 33569 RVA: 0x002AF7E2 File Offset: 0x002AD9E2
		public static int Hash(Vector2 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x06008322 RID: 33570 RVA: 0x002AF7EF File Offset: 0x002AD9EF
		public static int Hash(Vector3 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x06008323 RID: 33571 RVA: 0x002AF7FC File Offset: 0x002AD9FC
		public static int Hash(Vector4 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x06008324 RID: 33572 RVA: 0x002AF809 File Offset: 0x002ADA09
		public static int Hash(Quaternion q)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, q);
		}

		// Token: 0x06008325 RID: 33573 RVA: 0x002AF816 File Offset: 0x002ADA16
		public static int Hash(Color c)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, c);
		}

		// Token: 0x06008326 RID: 33574 RVA: 0x002AF824 File Offset: 0x002ADA24
		private static int HashTransformHierarchyRecurvsive(int hash, Transform t)
		{
			hash = Codec.HashConcat(hash, t);
			hash = Codec.HashConcat(hash, t.childCount);
			for (int i = 0; i < t.childCount; i++)
			{
				hash = Codec.HashTransformHierarchyRecurvsive(hash, t.GetChild(i));
			}
			return hash;
		}

		// Token: 0x06008327 RID: 33575 RVA: 0x002AF869 File Offset: 0x002ADA69
		public static int HashTransformHierarchy(Transform t)
		{
			return Codec.HashTransformHierarchyRecurvsive(Codec.FnvDefaultBasis, t);
		}

		// Token: 0x0400944B RID: 37963
		public static readonly int FnvDefaultBasis = -2128831035;

		// Token: 0x0400944C RID: 37964
		public static readonly int FnvPrime = 16777619;

		// Token: 0x02001453 RID: 5203
		[StructLayout(LayoutKind.Explicit)]
		private struct IntFloat
		{
			// Token: 0x0400944D RID: 37965
			[FieldOffset(0)]
			public int IntValue;

			// Token: 0x0400944E RID: 37966
			[FieldOffset(0)]
			public float FloatValue;
		}
	}
}
