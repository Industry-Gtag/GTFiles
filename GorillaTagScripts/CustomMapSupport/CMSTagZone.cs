using System;
using GorillaGameModes;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000FBE RID: 4030
	public class CMSTagZone : CMSTrigger
	{
		// Token: 0x0600643B RID: 25659 RVA: 0x00203CAB File Offset: 0x00201EAB
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			if (originatedLocally)
			{
				GameMode.ReportHit();
			}
		}
	}
}
