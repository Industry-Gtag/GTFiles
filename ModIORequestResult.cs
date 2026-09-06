using System;
using Modio;

// Token: 0x02000A7B RID: 2683
public struct ModIORequestResult
{
	// Token: 0x06004521 RID: 17697 RVA: 0x00170DDC File Offset: 0x0016EFDC
	public static ModIORequestResult CreateFailureResult(string inMessage)
	{
		ModIORequestResult modIORequestResult;
		modIORequestResult.success = false;
		modIORequestResult.message = inMessage;
		return modIORequestResult;
	}

	// Token: 0x06004522 RID: 17698 RVA: 0x00170DFC File Offset: 0x0016EFFC
	public static ModIORequestResult CreateSuccessResult()
	{
		ModIORequestResult modIORequestResult;
		modIORequestResult.success = true;
		modIORequestResult.message = "";
		return modIORequestResult;
	}

	// Token: 0x06004523 RID: 17699 RVA: 0x00170E20 File Offset: 0x0016F020
	public static ModIORequestResult CreateFromError(Error error)
	{
		ModIORequestResult modIORequestResult;
		if (error)
		{
			modIORequestResult.success = false;
			modIORequestResult.message = error.GetMessage();
		}
		else
		{
			modIORequestResult.success = true;
			modIORequestResult.message = "";
		}
		return modIORequestResult;
	}

	// Token: 0x040056FD RID: 22269
	public bool success;

	// Token: 0x040056FE RID: 22270
	public string message;
}
