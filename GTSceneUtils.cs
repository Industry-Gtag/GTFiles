using System;
using System.Diagnostics;
using UnityEngine.SceneManagement;

// Token: 0x02000E12 RID: 3602
public static class GTSceneUtils
{
	// Token: 0x0600582B RID: 22571 RVA: 0x00002C2D File Offset: 0x00000E2D
	[Conditional("UNITY_EDITOR")]
	public static void AddToBuild(GTScene scene)
	{
	}

	// Token: 0x0600582C RID: 22572 RVA: 0x001CB115 File Offset: 0x001C9315
	public static bool Equals(GTScene x, Scene y)
	{
		return !(x == null) && y.IsValid() && x.Equals(y);
	}

	// Token: 0x0600582D RID: 22573 RVA: 0x001CB139 File Offset: 0x001C9339
	public static GTScene[] ScenesInBuild()
	{
		return Array.Empty<GTScene>();
	}

	// Token: 0x0600582E RID: 22574 RVA: 0x00002C2D File Offset: 0x00000E2D
	[Conditional("UNITY_EDITOR")]
	public static void SyncBuildScenes()
	{
	}
}
