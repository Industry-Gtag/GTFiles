using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000BE0 RID: 3040
public static class UnityWebRequestExtensions
{
	// Token: 0x06004C8D RID: 19597 RVA: 0x0019836C File Offset: 0x0019656C
	public static TaskAwaiter<UnityWebRequest> GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
	{
		TaskCompletionSource<UnityWebRequest> tcs = new TaskCompletionSource<UnityWebRequest>();
		asyncOp.completed += delegate(AsyncOperation operation)
		{
			tcs.TrySetResult(asyncOp.webRequest);
		};
		return tcs.Task.GetAwaiter();
	}
}
