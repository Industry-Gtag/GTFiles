using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000ED6 RID: 3798
	// (Invoke) Token: 0x06005C32 RID: 23602
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void IAPurchaseCallback(int code, [MarshalAs(UnmanagedType.LPStr)] string message);
}
