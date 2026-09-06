using System;
using UnityEngine;

namespace Cosmetics
{
	// Token: 0x020011DE RID: 4574
	public interface ICreatorCodeProvider
	{
		// Token: 0x17000B40 RID: 2880
		// (get) Token: 0x0600744B RID: 29771
		GameObject GameObject { get; }

		// Token: 0x17000B41 RID: 2881
		// (get) Token: 0x0600744C RID: 29772
		string TerminalId { get; }

		// Token: 0x0600744D RID: 29773
		void GetCreatorCode(out string code, out NexusGroupId[] groups);
	}
}
