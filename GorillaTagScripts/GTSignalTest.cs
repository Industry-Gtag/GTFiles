using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F99 RID: 3993
	public class GTSignalTest : GTSignalListener
	{
		// Token: 0x040071FD RID: 29181
		public MeshRenderer[] targets = new MeshRenderer[0];

		// Token: 0x040071FE RID: 29182
		[Space]
		public MeshRenderer target;

		// Token: 0x040071FF RID: 29183
		public List<GTSignalListener> listeners = new List<GTSignalListener>(12);
	}
}
