using System;
using System.IO;
using UnityEngine;

// Token: 0x02000640 RID: 1600
[Serializable]
public struct SnapBounds
{
	// Token: 0x060027CC RID: 10188 RVA: 0x000D2D96 File Offset: 0x000D0F96
	public SnapBounds(Vector2Int min, Vector2Int max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x060027CD RID: 10189 RVA: 0x000D2DA6 File Offset: 0x000D0FA6
	public SnapBounds(int minX, int minY, int maxX, int maxY)
	{
		this.min = new Vector2Int(minX, minY);
		this.max = new Vector2Int(maxX, maxY);
	}

	// Token: 0x060027CE RID: 10190 RVA: 0x000D2DC3 File Offset: 0x000D0FC3
	public void Clear()
	{
		this.min = new Vector2Int(int.MinValue, int.MinValue);
		this.max = new Vector2Int(int.MinValue, int.MinValue);
	}

	// Token: 0x060027CF RID: 10191 RVA: 0x000D2DF0 File Offset: 0x000D0FF0
	public void Write(BinaryWriter writer)
	{
		writer.Write(this.min.x);
		writer.Write(this.min.y);
		writer.Write(this.max.x);
		writer.Write(this.max.y);
	}

	// Token: 0x060027D0 RID: 10192 RVA: 0x000D2E44 File Offset: 0x000D1044
	public void Read(BinaryReader reader)
	{
		this.min.x = reader.ReadInt32();
		this.min.y = reader.ReadInt32();
		this.max.x = reader.ReadInt32();
		this.max.y = reader.ReadInt32();
	}

	// Token: 0x0400337F RID: 13183
	public Vector2Int min;

	// Token: 0x04003380 RID: 13184
	public Vector2Int max;
}
