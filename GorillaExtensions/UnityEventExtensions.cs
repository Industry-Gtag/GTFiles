using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace GorillaExtensions
{
	// Token: 0x020011C7 RID: 4551
	public static class UnityEventExtensions
	{
		// Token: 0x060072B0 RID: 29360 RVA: 0x00255C54 File Offset: 0x00253E54
		public static void InvokeAll(this IEnumerable<UnityEvent> events)
		{
			foreach (UnityEvent unityEvent in events)
			{
				unityEvent.Invoke();
			}
		}

		// Token: 0x060072B1 RID: 29361 RVA: 0x00255C9C File Offset: 0x00253E9C
		public static void InvokeAll<TArg>(this IEnumerable<UnityEvent<TArg>> events, TArg arg)
		{
			foreach (UnityEvent<TArg> unityEvent in events)
			{
				unityEvent.Invoke(arg);
			}
		}
	}
}
