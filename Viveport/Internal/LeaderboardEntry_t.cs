using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000ED5 RID: 3797
	internal struct LeaderboardEntry_t
	{
		// Token: 0x04006CA2 RID: 27810
		internal int m_nGlobalRank;

		// Token: 0x04006CA3 RID: 27811
		internal int m_nScore;

		// Token: 0x04006CA4 RID: 27812
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		internal string m_pUserName;
	}
}
