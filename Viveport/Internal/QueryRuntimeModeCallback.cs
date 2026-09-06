using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000ECC RID: 3788
	// (Invoke) Token: 0x06005C2E RID: 23598
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void QueryRuntimeModeCallback(int nResult, int nMode);
}
