using System;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x02000210 RID: 528
public class ScheduledTimelinePlayer : MonoBehaviour
{
	// Token: 0x06000DEF RID: 3567 RVA: 0x0004C702 File Offset: 0x0004A902
	protected void OnEnable()
	{
		this.scheduledEventID = BetterDayNightManager.RegisterScheduledEvent(this.eventHour, new Action(this.HandleScheduledEvent));
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x0004C721 File Offset: 0x0004A921
	protected void OnDisable()
	{
		BetterDayNightManager.UnregisterScheduledEvent(this.scheduledEventID);
	}

	// Token: 0x06000DF1 RID: 3569 RVA: 0x0004C72E File Offset: 0x0004A92E
	private void HandleScheduledEvent()
	{
		this.timeline.Play();
	}

	// Token: 0x0400109C RID: 4252
	public PlayableDirector timeline;

	// Token: 0x0400109D RID: 4253
	public int eventHour = 7;

	// Token: 0x0400109E RID: 4254
	private int scheduledEventID;
}
