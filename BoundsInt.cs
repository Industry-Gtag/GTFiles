using System;
using UnityEngine;

// Token: 0x02000E8C RID: 3724
[Serializable]
public struct BoundsInt
{
	// Token: 0x06005A6B RID: 23147 RVA: 0x001D5B65 File Offset: 0x001D3D65
	public BoundsInt(Vector3Int min, Vector3Int max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x06005A6C RID: 23148 RVA: 0x001D5B78 File Offset: 0x001D3D78
	public BoundsInt(Vector3 center, Vector3 size)
	{
		Vector3 vector = size * 0.5f;
		this.min = global::BoundsInt.FloatToInt(center - vector);
		this.max = global::BoundsInt.FloatToInt(center + vector);
	}

	// Token: 0x170008A0 RID: 2208
	// (get) Token: 0x06005A6D RID: 23149 RVA: 0x001D5BB5 File Offset: 0x001D3DB5
	public Vector3Int center
	{
		get
		{
			return (this.min + this.max) / 2;
		}
	}

	// Token: 0x170008A1 RID: 2209
	// (get) Token: 0x06005A6E RID: 23150 RVA: 0x001D5BCE File Offset: 0x001D3DCE
	public Vector3Int size
	{
		get
		{
			return this.max - this.min;
		}
	}

	// Token: 0x170008A2 RID: 2210
	// (get) Token: 0x06005A6F RID: 23151 RVA: 0x001D5BE1 File Offset: 0x001D3DE1
	public Vector3 centerFloat
	{
		get
		{
			return global::BoundsInt.IntToFloat(this.center);
		}
	}

	// Token: 0x170008A3 RID: 2211
	// (get) Token: 0x06005A70 RID: 23152 RVA: 0x001D5BEE File Offset: 0x001D3DEE
	public Vector3 sizeFloat
	{
		get
		{
			return global::BoundsInt.IntToFloat(this.size);
		}
	}

	// Token: 0x06005A71 RID: 23153 RVA: 0x001D5BFB File Offset: 0x001D3DFB
	public static Vector3Int FloatToInt(Vector3 v)
	{
		return new Vector3Int(Mathf.RoundToInt(v.x * 1000f), Mathf.RoundToInt(v.y * 1000f), Mathf.RoundToInt(v.z * 1000f));
	}

	// Token: 0x06005A72 RID: 23154 RVA: 0x001D5C35 File Offset: 0x001D3E35
	public static Vector3 IntToFloat(Vector3Int v)
	{
		return new Vector3((float)v.x / 1000f, (float)v.y / 1000f, (float)v.z / 1000f);
	}

	// Token: 0x06005A73 RID: 23155 RVA: 0x001D5C66 File Offset: 0x001D3E66
	public static global::BoundsInt FromBounds(Bounds bounds)
	{
		return new global::BoundsInt(bounds.center, bounds.size);
	}

	// Token: 0x06005A74 RID: 23156 RVA: 0x001D5C7B File Offset: 0x001D3E7B
	public Bounds ToBounds()
	{
		return new Bounds(this.centerFloat, this.sizeFloat);
	}

	// Token: 0x06005A75 RID: 23157 RVA: 0x001D5B65 File Offset: 0x001D3D65
	public void SetMinMax(Vector3Int min, Vector3Int max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x06005A76 RID: 23158 RVA: 0x001D5C8E File Offset: 0x001D3E8E
	public void SetMinMax(Vector3 min, Vector3 max)
	{
		this.min = global::BoundsInt.FloatToInt(min);
		this.max = global::BoundsInt.FloatToInt(max);
	}

	// Token: 0x06005A77 RID: 23159 RVA: 0x001D5CA8 File Offset: 0x001D3EA8
	public void Encapsulate(global::BoundsInt other)
	{
		this.min = new Vector3Int(Mathf.Min(this.min.x, other.min.x), Mathf.Min(this.min.y, other.min.y), Mathf.Min(this.min.z, other.min.z));
		this.max = new Vector3Int(Mathf.Max(this.max.x, other.max.x), Mathf.Max(this.max.y, other.max.y), Mathf.Max(this.max.z, other.max.z));
	}

	// Token: 0x06005A78 RID: 23160 RVA: 0x001D5D74 File Offset: 0x001D3F74
	public void Expand(float amount)
	{
		int num = Mathf.RoundToInt(amount * 1000f);
		Vector3Int vector3Int = new Vector3Int(num, num, num);
		this.min -= vector3Int;
		this.max += vector3Int;
	}

	// Token: 0x06005A79 RID: 23161 RVA: 0x001D5DBC File Offset: 0x001D3FBC
	public bool Intersects(global::BoundsInt other)
	{
		return this.min.x < other.max.x && this.max.x > other.min.x && this.min.y < other.max.y && this.max.y > other.min.y && this.min.z < other.max.z && this.max.z > other.min.z;
	}

	// Token: 0x06005A7A RID: 23162 RVA: 0x001D5E68 File Offset: 0x001D4068
	public bool Contains(global::BoundsInt other)
	{
		return this.min.x <= other.min.x && this.min.y <= other.min.y && this.min.z <= other.min.z && this.max.x >= other.max.x && this.max.y >= other.max.y && this.max.z >= other.max.z;
	}

	// Token: 0x06005A7B RID: 23163 RVA: 0x001D5F14 File Offset: 0x001D4114
	public bool Contains(Vector3 point)
	{
		Vector3Int vector3Int = global::BoundsInt.FloatToInt(point);
		return vector3Int.x >= this.min.x && vector3Int.x <= this.max.x && vector3Int.y >= this.min.y && vector3Int.y <= this.max.y && vector3Int.z >= this.min.z && vector3Int.z <= this.max.z;
	}

	// Token: 0x06005A7C RID: 23164 RVA: 0x001D5FA8 File Offset: 0x001D41A8
	public global::BoundsInt GetIntersection(global::BoundsInt other)
	{
		Vector3Int vector3Int = new Vector3Int(Mathf.Max(this.min.x, other.min.x), Mathf.Max(this.min.y, other.min.y), Mathf.Max(this.min.z, other.min.z));
		Vector3Int vector3Int2 = new Vector3Int(Mathf.Min(this.max.x, other.max.x), Mathf.Min(this.max.y, other.max.y), Mathf.Min(this.max.z, other.max.z));
		if (vector3Int.x > vector3Int2.x || vector3Int.y > vector3Int2.y || vector3Int.z > vector3Int2.z)
		{
			return new global::BoundsInt(Vector3Int.zero, Vector3Int.zero);
		}
		return new global::BoundsInt(vector3Int, vector3Int2);
	}

	// Token: 0x06005A7D RID: 23165 RVA: 0x001D60B4 File Offset: 0x001D42B4
	public long Volume()
	{
		Vector3Int size = this.size;
		return (long)size.x * (long)size.y * (long)size.z;
	}

	// Token: 0x06005A7E RID: 23166 RVA: 0x001D60E2 File Offset: 0x001D42E2
	public float VolumeFloat()
	{
		return (float)this.Volume() / 1E+09f;
	}

	// Token: 0x06005A7F RID: 23167 RVA: 0x001D60F1 File Offset: 0x001D42F1
	public static bool operator ==(global::BoundsInt a, global::BoundsInt b)
	{
		return a.min == b.min && a.max == b.max;
	}

	// Token: 0x06005A80 RID: 23168 RVA: 0x001D6119 File Offset: 0x001D4319
	public static bool operator !=(global::BoundsInt a, global::BoundsInt b)
	{
		return !(a == b);
	}

	// Token: 0x06005A81 RID: 23169 RVA: 0x001D6128 File Offset: 0x001D4328
	public override bool Equals(object obj)
	{
		if (obj is global::BoundsInt)
		{
			global::BoundsInt boundsInt = (global::BoundsInt)obj;
			return this == boundsInt;
		}
		return false;
	}

	// Token: 0x06005A82 RID: 23170 RVA: 0x001D6152 File Offset: 0x001D4352
	public override int GetHashCode()
	{
		return this.min.GetHashCode() ^ (this.max.GetHashCode() << 2);
	}

	// Token: 0x06005A83 RID: 23171 RVA: 0x001D6179 File Offset: 0x001D4379
	public override string ToString()
	{
		return string.Format("BoundsInt(min: {0}, max: {1})", this.min, this.max);
	}

	// Token: 0x04006B96 RID: 27542
	private const int SCALE_FACTOR = 1000;

	// Token: 0x04006B97 RID: 27543
	public Vector3Int min;

	// Token: 0x04006B98 RID: 27544
	public Vector3Int max;
}
