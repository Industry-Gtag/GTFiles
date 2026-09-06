using System;

// Token: 0x02000D29 RID: 3369
[Serializable]
public class CallLimitType<T> where T : CallLimiter
{
	// Token: 0x0600536A RID: 21354 RVA: 0x001B7933 File Offset: 0x001B5B33
	public static implicit operator CallLimitType<CallLimiter>(CallLimitType<T> clt)
	{
		return new CallLimitType<CallLimiter>
		{
			Key = clt.Key,
			UseNetWorkTime = clt.UseNetWorkTime,
			CallLimitSettings = clt.CallLimitSettings
		};
	}

	// Token: 0x04006503 RID: 25859
	public FXType Key;

	// Token: 0x04006504 RID: 25860
	public bool UseNetWorkTime;

	// Token: 0x04006505 RID: 25861
	public T CallLimitSettings;
}
