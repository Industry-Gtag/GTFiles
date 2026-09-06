using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000E1E RID: 3614
public class ServerTimeEvent : TimeEvent
{
	// Token: 0x06005882 RID: 22658 RVA: 0x001CBDB1 File Offset: 0x001C9FB1
	private void Awake()
	{
		this.eventTimes = new HashSet<ServerTimeEvent.EventTime>(this.times);
	}

	// Token: 0x06005883 RID: 22659 RVA: 0x001CBDC4 File Offset: 0x001C9FC4
	private void Update()
	{
		if (GorillaComputer.instance == null || Time.time - this.lastQueryTime < this.queryTime)
		{
			return;
		}
		ServerTimeEvent.EventTime eventTime = new ServerTimeEvent.EventTime(GorillaComputer.instance.GetServerTime().Hour, GorillaComputer.instance.GetServerTime().Minute);
		bool flag = this.eventTimes.Contains(eventTime);
		if (!this._ongoing && flag)
		{
			base.StartEvent();
		}
		if (this._ongoing && !flag)
		{
			base.StopEvent();
		}
		this.lastQueryTime = Time.time;
	}

	// Token: 0x040068B0 RID: 26800
	[SerializeField]
	private ServerTimeEvent.EventTime[] times;

	// Token: 0x040068B1 RID: 26801
	[SerializeField]
	private float queryTime = 60f;

	// Token: 0x040068B2 RID: 26802
	private float lastQueryTime;

	// Token: 0x040068B3 RID: 26803
	private HashSet<ServerTimeEvent.EventTime> eventTimes;

	// Token: 0x02000E1F RID: 3615
	[Serializable]
	public struct EventTime
	{
		// Token: 0x06005885 RID: 22661 RVA: 0x001CBE73 File Offset: 0x001CA073
		public EventTime(int h, int m)
		{
			this.hour = h;
			this.minute = m;
		}

		// Token: 0x040068B4 RID: 26804
		public int hour;

		// Token: 0x040068B5 RID: 26805
		public int minute;
	}
}
