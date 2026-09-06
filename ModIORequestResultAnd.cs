using System;

// Token: 0x02000A7C RID: 2684
public struct ModIORequestResultAnd<T>
{
	// Token: 0x06004524 RID: 17700 RVA: 0x00170E64 File Offset: 0x0016F064
	public static ModIORequestResultAnd<T> CreateFailureResult(string inMessage)
	{
		return new ModIORequestResultAnd<T>
		{
			result = ModIORequestResult.CreateFailureResult(inMessage)
		};
	}

	// Token: 0x06004525 RID: 17701 RVA: 0x00170E88 File Offset: 0x0016F088
	public static ModIORequestResultAnd<T> CreateSuccessResult(T payload)
	{
		return new ModIORequestResultAnd<T>
		{
			result = ModIORequestResult.CreateSuccessResult(),
			data = payload
		};
	}

	// Token: 0x040056FF RID: 22271
	public ModIORequestResult result;

	// Token: 0x04005700 RID: 22272
	public T data;
}
