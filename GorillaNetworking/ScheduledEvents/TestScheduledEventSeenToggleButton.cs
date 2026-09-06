using System;
using UnityEngine;

namespace GorillaNetworking.ScheduledEvents
{
	// Token: 0x02001134 RID: 4404
	public class TestScheduledEventSeenToggleButton : GorillaPressableButton
	{
		// Token: 0x06006E79 RID: 28281 RVA: 0x00239A6C File Offset: 0x00237C6C
		public override void Start()
		{
			base.Start();
			this.RefreshFromPrefs();
			this.nextPollTime = Time.time + 1f;
		}

		// Token: 0x06006E7A RID: 28282 RVA: 0x00239A8B File Offset: 0x00237C8B
		private void Update()
		{
			if (Time.time < this.nextPollTime)
			{
				return;
			}
			this.nextPollTime = Time.time + 1f;
			this.RefreshFromPrefs();
		}

		// Token: 0x06006E7B RID: 28283 RVA: 0x00239AB4 File Offset: 0x00237CB4
		public override void ButtonActivation()
		{
			DateTime dateTime = ((GorillaComputer.instance != null) ? GorillaComputer.instance.GetServerTime() : DateTime.UtcNow);
			if (ScheduledEventMatchmaking.HasSeenScheduledEventRecently(dateTime))
			{
				PlayerPrefs.DeleteKey("lastSawScheduledEventTime");
			}
			else
			{
				ScheduledEventMatchmaking.MarkSeenScheduledEventNow(dateTime);
			}
			this.RefreshFromPrefs();
		}

		// Token: 0x06006E7C RID: 28284 RVA: 0x00239B04 File Offset: 0x00237D04
		private void RefreshFromPrefs()
		{
			DateTime dateTime = ((GorillaComputer.instance != null) ? GorillaComputer.instance.GetServerTime() : DateTime.UtcNow);
			this.isOn = ScheduledEventMatchmaking.HasSeenScheduledEventRecently(dateTime);
			this.UpdateColor();
		}

		// Token: 0x04007EA6 RID: 32422
		private const float PollInterval = 1f;

		// Token: 0x04007EA7 RID: 32423
		private float nextPollTime;
	}
}
