using System;
using UnityEngine;

// Token: 0x02000DA5 RID: 3493
public static class EchoUtils
{
	// Token: 0x060055E3 RID: 21987 RVA: 0x001C1B97 File Offset: 0x001BFD97
	[HideInCallstack]
	public static T Echo<T>(this T message)
	{
		Debug.Log(message);
		return message;
	}

	// Token: 0x060055E4 RID: 21988 RVA: 0x001C1BA5 File Offset: 0x001BFDA5
	[HideInCallstack]
	public static T Echo<T>(this T message, Object context)
	{
		Debug.Log(message, context);
		return message;
	}
}
