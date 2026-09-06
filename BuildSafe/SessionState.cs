using System;

namespace BuildSafe
{
	// Token: 0x020010A3 RID: 4259
	public class SessionState
	{
		// Token: 0x17000A18 RID: 2584
		public string this[string key]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x040079A4 RID: 31140
		public static readonly SessionState Shared = new SessionState();
	}
}
