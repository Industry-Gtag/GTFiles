using System;
using Photon.Realtime;

namespace GorillaTag
{
	// Token: 0x0200121C RID: 4636
	internal class ReportMuteTimer : TickSystemTimerAbstract, ObjectPoolEvents
	{
		// Token: 0x17000B67 RID: 2919
		// (get) Token: 0x06007588 RID: 30088 RVA: 0x00264770 File Offset: 0x00262970
		// (set) Token: 0x06007589 RID: 30089 RVA: 0x00264778 File Offset: 0x00262978
		public int Muted { get; set; }

		// Token: 0x0600758A RID: 30090 RVA: 0x00264784 File Offset: 0x00262984
		public override void OnTimedEvent()
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				this.Stop();
				return;
			}
			ReportMuteTimer.content[0] = this.m_playerID;
			ReportMuteTimer.content[1] = this.Muted;
			ReportMuteTimer.content[2] = ((this.m_nickName.Length > 12) ? this.m_nickName.Remove(12) : this.m_nickName);
			ReportMuteTimer.content[3] = NetworkSystem.Instance.LocalPlayer.NickName;
			ReportMuteTimer.content[4] = !NetworkSystem.Instance.SessionIsPrivate;
			ReportMuteTimer.content[5] = NetworkSystem.Instance.RoomStringStripped();
			NetworkSystemRaiseEvent.RaiseEvent(51, ReportMuteTimer.content, ReportMuteTimer.netEventOptions, true);
			this.Stop();
		}

		// Token: 0x0600758B RID: 30091 RVA: 0x00264846 File Offset: 0x00262A46
		public void SetReportData(string id, string name, int muted)
		{
			this.Muted = muted;
			this.m_playerID = id;
			this.m_nickName = name;
		}

		// Token: 0x0600758C RID: 30092 RVA: 0x00002C2D File Offset: 0x00000E2D
		void ObjectPoolEvents.OnTaken()
		{
		}

		// Token: 0x0600758D RID: 30093 RVA: 0x0026485D File Offset: 0x00262A5D
		void ObjectPoolEvents.OnReturned()
		{
			if (base.Running)
			{
				this.OnTimedEvent();
			}
			this.m_playerID = string.Empty;
			this.m_nickName = string.Empty;
			this.Muted = 0;
		}

		// Token: 0x0400857D RID: 34173
		private static readonly NetEventOptions netEventOptions = new NetEventOptions
		{
			Flags = new WebFlags(3),
			TargetActors = new int[] { -1 }
		};

		// Token: 0x0400857E RID: 34174
		private static readonly object[] content = new object[6];

		// Token: 0x0400857F RID: 34175
		private const byte evCode = 51;

		// Token: 0x04008581 RID: 34177
		private string m_playerID;

		// Token: 0x04008582 RID: 34178
		private string m_nickName;
	}
}
