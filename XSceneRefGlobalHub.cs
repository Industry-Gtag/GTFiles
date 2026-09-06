using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003C7 RID: 967
public static class XSceneRefGlobalHub
{
	// Token: 0x0600173B RID: 5947 RVA: 0x000868B0 File Offset: 0x00084AB0
	public static void Register(int ID, XSceneRefTarget obj)
	{
		if (ID > 0)
		{
			int sceneIndex = (int)obj.GetSceneIndex();
			if (sceneIndex >= 0)
			{
				XSceneRefGlobalHub.registry[sceneIndex][ID] = obj;
			}
		}
	}

	// Token: 0x0600173C RID: 5948 RVA: 0x000868E0 File Offset: 0x00084AE0
	public static void Unregister(int ID, XSceneRefTarget obj)
	{
		int sceneIndex = (int)obj.GetSceneIndex();
		if (ID > 0 && sceneIndex >= 0)
		{
			if (sceneIndex < 0 || sceneIndex >= XSceneRefGlobalHub.registry.Count)
			{
				Debug.LogErrorFormat(obj, "Invalid scene index {0} cannot remove ID {1}", new object[] { sceneIndex, ID });
			}
			XSceneRefGlobalHub.registry[sceneIndex].Remove(ID);
		}
	}

	// Token: 0x0600173D RID: 5949 RVA: 0x00086942 File Offset: 0x00084B42
	public static bool TryResolve(SceneIndex sceneIndex, int ID, out XSceneRefTarget result)
	{
		return XSceneRefGlobalHub.registry[(int)sceneIndex].TryGetValue(ID, out result);
	}

	// Token: 0x0400227B RID: 8827
	private static List<Dictionary<int, XSceneRefTarget>> registry = new List<Dictionary<int, XSceneRefTarget>>
	{
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } }
	};
}
