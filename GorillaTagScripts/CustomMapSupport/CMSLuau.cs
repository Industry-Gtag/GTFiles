using System;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000FB9 RID: 4025
	public class CMSLuau : CMSTrigger
	{
		// Token: 0x0600641A RID: 25626 RVA: 0x00202EDE File Offset: 0x002010DE
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			if (originatedLocally)
			{
				LuauVm.touchEventsQueue.Enqueue(base.gameObject);
			}
		}
	}
}
