using System;

namespace GorillaTagScripts.AI
{
	// Token: 0x0200107A RID: 4218
	public interface IState
	{
		// Token: 0x0600694E RID: 26958
		void Tick();

		// Token: 0x0600694F RID: 26959
		void OnEnter();

		// Token: 0x06006950 RID: 26960
		void OnExit();
	}
}
