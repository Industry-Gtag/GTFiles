using System;

namespace GorillaNetworking.ScheduledEvents
{
	// Token: 0x0200112F RID: 4399
	public struct ScheduledEventInfo
	{
		// Token: 0x17000A8A RID: 2698
		// (get) Token: 0x06006E66 RID: 28262 RVA: 0x00239784 File Offset: 0x00237984
		public static ScheduledEventInfo None
		{
			get
			{
				return new ScheduledEventInfo
				{
					isActive = false
				};
			}
		}

		// Token: 0x04007E94 RID: 32404
		public bool isActive;

		// Token: 0x04007E95 RID: 32405
		public DateTime scheduledStart;
	}
}
