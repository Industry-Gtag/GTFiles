using System;
using System.Collections.Generic;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02001162 RID: 4450
	public static class ListMethodsExtensions
	{
		// Token: 0x06006FB1 RID: 28593 RVA: 0x0023FAD8 File Offset: 0x0023DCD8
		public static void RemoveAllNullItems<T>(this List<T> list)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i] == null)
				{
					list.RemoveAt(i);
				}
			}
		}
	}
}
