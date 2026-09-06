using System;
using UnityEngine;

// Token: 0x02000BCF RID: 3023
public class KIDUI_DebugScreen : MonoBehaviour
{
	// Token: 0x06004C28 RID: 19496 RVA: 0x00196126 File Offset: 0x00194326
	private void Awake()
	{
		Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x06004C29 RID: 19497 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnResetUserAndQuit()
	{
	}

	// Token: 0x06004C2A RID: 19498 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnClose()
	{
	}

	// Token: 0x06004C2B RID: 19499 RVA: 0x00036275 File Offset: 0x00034475
	public static string GetOrCreateUsername()
	{
		return null;
	}

	// Token: 0x06004C2C RID: 19500 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void ResetAll()
	{
	}

	// Token: 0x04005F2E RID: 24366
	public const string KID_ENABLED_KEY = "dbg-kid-enabled";
}
