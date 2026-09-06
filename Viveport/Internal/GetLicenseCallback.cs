using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000EC9 RID: 3785
	// (Invoke) Token: 0x06005C22 RID: 23586
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void GetLicenseCallback([MarshalAs(UnmanagedType.LPStr)] string message, [MarshalAs(UnmanagedType.LPStr)] string signature);
}
