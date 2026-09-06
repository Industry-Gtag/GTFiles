using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000ECB RID: 3787
	// (Invoke) Token: 0x06005C2A RID: 23594
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void StatusCallback2(int nResult, [MarshalAs(UnmanagedType.LPStr)] string message);
}
