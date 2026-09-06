using System;
using UnityEngine;

namespace GorillaExtensions
{
	// Token: 0x020011C2 RID: 4546
	public static class GameObjectExtensions
	{
		// Token: 0x0600729D RID: 29341 RVA: 0x002557FF File Offset: 0x002539FF
		public static bool TryGetComponentInParent<T>(this GameObject obj, out T component) where T : MonoBehaviour
		{
			while (!obj.TryGetComponent<T>(out component))
			{
				obj = ((obj.transform.parent != null) ? obj.transform.parent.gameObject : null);
				if (!(obj != null))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600729E RID: 29342 RVA: 0x00002076 File Offset: 0x00000276
		public static bool IsPrefab(this GameObject go)
		{
			return false;
		}
	}
}
