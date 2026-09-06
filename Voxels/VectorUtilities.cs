using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020013A9 RID: 5033
	public static class VectorUtilities
	{
		// Token: 0x06007D7B RID: 32123 RVA: 0x00292043 File Offset: 0x00290243
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int ToVectorInt(this int3 v)
		{
			return new Vector3Int(v.x, v.y, v.z);
		}

		// Token: 0x06007D7C RID: 32124 RVA: 0x0029205C File Offset: 0x0029025C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 ToInt3(this Vector3Int v)
		{
			return new int3(v.x, v.y, v.z);
		}

		// Token: 0x06007D7D RID: 32125 RVA: 0x00292078 File Offset: 0x00290278
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 ToInt3(this Vector3 v)
		{
			return new int3((int)v.x, (int)v.y, (int)v.z);
		}

		// Token: 0x06007D7E RID: 32126 RVA: 0x00292094 File Offset: 0x00290294
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half3 ToHalf3(this Vector3 v)
		{
			return new half3((half)v.x, (half)v.y, (half)v.z);
		}

		// Token: 0x06007D7F RID: 32127 RVA: 0x002920BC File Offset: 0x002902BC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 ToVector3(this half3 h)
		{
			return new Vector3(h.x, h.y, h.z);
		}

		// Token: 0x06007D80 RID: 32128 RVA: 0x002920E4 File Offset: 0x002902E4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 ToInt3(this float3 v)
		{
			return new int3((int)v.x, (int)v.y, (int)v.z);
		}

		// Token: 0x06007D81 RID: 32129 RVA: 0x00292100 File Offset: 0x00290300
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 RoundToInt(this Vector3 v)
		{
			return (int3)math.round(v);
		}

		// Token: 0x06007D82 RID: 32130 RVA: 0x00292112 File Offset: 0x00290312
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int RoundToVectorInt(this Vector3 v)
		{
			return new Vector3Int((int)math.round(v.x), (int)math.round(v.y), (int)math.round(v.z));
		}

		// Token: 0x06007D83 RID: 32131 RVA: 0x0029213D File Offset: 0x0029033D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int FloorToVectorInt(this Vector3 v)
		{
			return new Vector3Int((int)math.floor(v.x), (int)math.floor(v.y), (int)math.floor(v.z));
		}

		// Token: 0x06007D84 RID: 32132 RVA: 0x00292168 File Offset: 0x00290368
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int CeilToVectorInt(this Vector3 v)
		{
			return new Vector3Int((int)math.ceil(v.x), (int)math.ceil(v.y), (int)math.ceil(v.z));
		}

		// Token: 0x06007D85 RID: 32133 RVA: 0x00292193 File Offset: 0x00290393
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 RoundToInt(this float3 v)
		{
			return (int3)math.round(v);
		}

		// Token: 0x06007D86 RID: 32134 RVA: 0x002921A0 File Offset: 0x002903A0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 CeilToInt(this float3 v)
		{
			return new int3(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z));
		}

		// Token: 0x06007D87 RID: 32135 RVA: 0x002921C8 File Offset: 0x002903C8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Ceil(this Vector3 v)
		{
			return new Vector3(math.ceil(v.x), math.ceil(v.y), math.ceil(v.z));
		}

		// Token: 0x06007D88 RID: 32136 RVA: 0x002921F0 File Offset: 0x002903F0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Floor(this Vector3 v)
		{
			return new Vector3(math.floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z));
		}

		// Token: 0x06007D89 RID: 32137 RVA: 0x00292218 File Offset: 0x00290418
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 Floor(this float3 v)
		{
			return new float3(math.floor(v.x), math.floor(v.y), math.floor(v.z));
		}

		// Token: 0x06007D8A RID: 32138 RVA: 0x00292240 File Offset: 0x00290440
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 ToVector3(this int3 v)
		{
			return new Vector3((float)v.x, (float)v.y, (float)v.z);
		}

		// Token: 0x06007D8B RID: 32139 RVA: 0x0029225C File Offset: 0x0029045C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 ToFloat3(this int3 v)
		{
			return new float3((float)v.x, (float)v.y, (float)v.z);
		}

		// Token: 0x06007D8C RID: 32140 RVA: 0x00292278 File Offset: 0x00290478
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 FloorToMultipleOfX(this Vector3 v, int3 x)
		{
			return (int3)(math.floor(new float3(v.x / (float)x.x, v.y / (float)x.y, v.z / (float)x.z)) * x);
		}

		// Token: 0x06007D8D RID: 32141 RVA: 0x002922CC File Offset: 0x002904CC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 FloorToMultipleOfX(this Vector3Int v, int3 x)
		{
			return (int3)(math.floor(new float3((float)v.x / (float)x.x, (float)v.y / (float)x.y, (float)v.z / (float)x.z)) * x);
		}

		// Token: 0x06007D8E RID: 32142 RVA: 0x00292324 File Offset: 0x00290524
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 FloorToMultipleOfX(this int3 v, int3 x)
		{
			return (int3)(math.floor(new float3((float)v.x / (float)x.x, (float)v.y / (float)x.y, (float)v.z / (float)x.z)) * x);
		}

		// Token: 0x06007D8F RID: 32143 RVA: 0x00292378 File Offset: 0x00290578
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 LocalPositionToChunkId(this Vector3 localWorldPosition, int3 chunkSize)
		{
			return localWorldPosition.FloorToMultipleOfX(chunkSize) / chunkSize;
		}

		// Token: 0x06007D90 RID: 32144 RVA: 0x00292387 File Offset: 0x00290587
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 LocalPositionToChunkId(this Vector3Int localWorldPosition, int3 chunkSize)
		{
			return localWorldPosition.FloorToMultipleOfX(chunkSize) / chunkSize;
		}

		// Token: 0x06007D91 RID: 32145 RVA: 0x00292396 File Offset: 0x00290596
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 LocalPositionToChunkId(this int3 localWorldPosition, int3 chunkSize)
		{
			return localWorldPosition.FloorToMultipleOfX(chunkSize) / chunkSize;
		}

		// Token: 0x06007D92 RID: 32146 RVA: 0x002923A5 File Offset: 0x002905A5
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ToByte(this float value)
		{
			return (byte)math.clamp(value * 127f + 128f, 0f, 255f);
		}

		// Token: 0x06007D93 RID: 32147 RVA: 0x002923C4 File Offset: 0x002905C4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToFloat(this byte value)
		{
			return (float)(value - 128) / 127f;
		}

		// Token: 0x06007D94 RID: 32148 RVA: 0x002923D4 File Offset: 0x002905D4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSolid(this byte density)
		{
			return density > 127;
		}

		// Token: 0x06007D95 RID: 32149 RVA: 0x002923DC File Offset: 0x002905DC
		public static int3[] GetCardinalNeighbours(this int3 center)
		{
			return new int3[]
			{
				center + new int3(1, 0, 0),
				center + new int3(-1, 0, 0),
				center + new int3(0, 1, 0),
				center + new int3(0, -1, 0),
				center + new int3(0, 0, 1),
				center + new int3(0, 0, -1)
			};
		}

		// Token: 0x06007D96 RID: 32150 RVA: 0x00292470 File Offset: 0x00290670
		public static int3 GetClosestCardinalNeighbour(this int3 center, Vector3 target)
		{
			int3[] cardinalNeighbours = center.GetCardinalNeighbours();
			int num = 0;
			float num2 = math.distance(cardinalNeighbours[0], target);
			for (int i = 1; i < cardinalNeighbours.Length; i++)
			{
				float num3 = math.distance(cardinalNeighbours[i], target);
				if (num3 < num2)
				{
					num2 = num3;
					num = i;
				}
			}
			return cardinalNeighbours[num];
		}

		// Token: 0x06007D97 RID: 32151 RVA: 0x002924D6 File Offset: 0x002906D6
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int Min(Vector3Int v1, Vector3Int v2)
		{
			return new Vector3Int(math.min(v1.x, v2.x), math.min(v1.y, v2.y), math.min(v1.z, v2.z));
		}

		// Token: 0x06007D98 RID: 32152 RVA: 0x00292516 File Offset: 0x00290716
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3Int Max(Vector3Int v1, Vector3Int v2)
		{
			return new Vector3Int(math.max(v1.x, v2.x), math.max(v1.y, v2.y), math.max(v1.z, v2.z));
		}
	}
}
