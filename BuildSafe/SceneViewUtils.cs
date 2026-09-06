using System;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x020010A0 RID: 4256
	public static class SceneViewUtils
	{
		// Token: 0x06006A0D RID: 27149 RVA: 0x00221109 File Offset: 0x0021F309
		private static bool RaycastWorldSafe(Vector2 screenPos, out RaycastHit hit)
		{
			hit = default(RaycastHit);
			return false;
		}

		// Token: 0x040079A3 RID: 31139
		public static readonly SceneViewUtils.FuncRaycastWorld RaycastWorld = new SceneViewUtils.FuncRaycastWorld(SceneViewUtils.RaycastWorldSafe);

		// Token: 0x020010A1 RID: 4257
		// (Invoke) Token: 0x06006A10 RID: 27152
		public delegate bool FuncRaycastWorld(Vector2 screenPos, out RaycastHit hit);

		// Token: 0x020010A2 RID: 4258
		// (Invoke) Token: 0x06006A14 RID: 27156
		public delegate GameObject FuncPickClosestGameObject(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex);
	}
}
