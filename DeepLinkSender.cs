using System;
using UnityEngine;

// Token: 0x020005C3 RID: 1475
public static class DeepLinkSender
{
	// Token: 0x0600253D RID: 9533 RVA: 0x000C77AC File Offset: 0x000C59AC
	public static bool SendDeepLink(ulong deepLinkAppID, string deepLinkMessage, Action<string> onSent)
	{
		Debug.LogError("[DeepLinkSender::SendDeepLink] Called on non-oculus platform!");
		return false;
	}

	// Token: 0x040030CB RID: 12491
	private static Action<string> currentDeepLinkSentCallback;
}
