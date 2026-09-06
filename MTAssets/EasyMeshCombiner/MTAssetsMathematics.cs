using System;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02001161 RID: 4449
	[AddComponentMenu("")]
	public class MTAssetsMathematics : MonoBehaviour
	{
		// Token: 0x06006FAE RID: 28590 RVA: 0x0023FA7C File Offset: 0x0023DC7C
		public static List<T> RandomizeThisList<T>(List<T> list)
		{
			int count = list.Count;
			int num = count - 1;
			for (int i = 0; i < num; i++)
			{
				int num2 = Random.Range(i, count);
				T t = list[i];
				list[i] = list[num2];
				list[num2] = t;
			}
			return list;
		}

		// Token: 0x06006FAF RID: 28591 RVA: 0x0023FAC9 File Offset: 0x0023DCC9
		public static Vector3 GetHalfPositionBetweenTwoPoints(Vector3 pointA, Vector3 pointB)
		{
			return Vector3.Lerp(pointA, pointB, 0.5f);
		}
	}
}
