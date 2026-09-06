using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000ED7 RID: 3799
	internal struct IAPCurrency_t
	{
		// Token: 0x04006CA5 RID: 27813
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		internal string m_pName;

		// Token: 0x04006CA6 RID: 27814
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		internal string m_pSymbol;
	}
}
