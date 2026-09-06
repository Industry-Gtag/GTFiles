using System;

namespace PerformanceSystems
{
	// Token: 0x02000F28 RID: 3880
	public interface ITimeSlice
	{
		// Token: 0x06005F1A RID: 24346
		void SliceUpdate();

		// Token: 0x06005F1B RID: 24347
		void SliceUpdateAlways(float deltaTime);

		// Token: 0x06005F1C RID: 24348
		void SliceUpdate(float deltaTime);
	}
}
