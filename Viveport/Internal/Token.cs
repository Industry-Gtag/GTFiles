using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000EDF RID: 3807
	internal class Token
	{
		// Token: 0x06005CB0 RID: 23728 RVA: 0x001DC457 File Offset: 0x001DA657
		static Token()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x06005CB1 RID: 23729
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportToken_IsReady")]
		internal static extern int IsReady(StatusCallback IsReadyCallback);

		// Token: 0x06005CB2 RID: 23730
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportToken_IsReady")]
		internal static extern int IsReady_64(StatusCallback IsReadyCallback);

		// Token: 0x06005CB3 RID: 23731
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportToken_GetSessionToken")]
		internal static extern int GetSessionToken(StatusCallback2 GetSessionTokenCallback);

		// Token: 0x06005CB4 RID: 23732
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportToken_GetSessionToken")]
		internal static extern int GetSessionToken_64(StatusCallback2 GetSessionTokenCallback);
	}
}
