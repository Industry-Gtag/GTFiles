using System;
using System.Diagnostics;

namespace BuildSafe
{
	// Token: 0x02001090 RID: 4240
	public static class Callbacks
	{
		// Token: 0x02001091 RID: 4241
		[Conditional("UNITY_EDITOR")]
		public class DidReloadScripts : Attribute
		{
			// Token: 0x060069D7 RID: 27095 RVA: 0x00220DAE File Offset: 0x0021EFAE
			public DidReloadScripts(bool activeOnly = false)
			{
				this.activeOnly = activeOnly;
			}

			// Token: 0x0400798B RID: 31115
			public bool activeOnly;
		}
	}
}
