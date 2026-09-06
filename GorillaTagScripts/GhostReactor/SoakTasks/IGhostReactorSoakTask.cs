using System;

namespace GorillaTagScripts.GhostReactor.SoakTasks
{
	// Token: 0x02001025 RID: 4133
	public interface IGhostReactorSoakTask
	{
		// Token: 0x170009D8 RID: 2520
		// (get) Token: 0x060066D4 RID: 26324
		bool Complete { get; }

		// Token: 0x060066D5 RID: 26325
		bool Update();

		// Token: 0x060066D6 RID: 26326
		void Reset();
	}
}
