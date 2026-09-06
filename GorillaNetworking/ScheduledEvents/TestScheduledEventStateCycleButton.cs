using System;
using ExitGames.Client.Photon;
using Photon.Pun;

namespace GorillaNetworking.ScheduledEvents
{
	// Token: 0x02001135 RID: 4405
	public class TestScheduledEventStateCycleButton : GorillaPressableButton
	{
		// Token: 0x06006E7E RID: 28286 RVA: 0x00239B48 File Offset: 0x00237D48
		public override void ButtonActivation()
		{
			if (PhotonNetwork.CurrentRoom == null)
			{
				base.SetText("(no room)");
				return;
			}
			string text = TestScheduledEventStateCycleButton.NextState(TestScheduledEventStateCycleButton.ReadState());
			Hashtable hashtable = new Hashtable { { "scheduledEventState", text } };
			PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
			this.RenderState(text);
		}

		// Token: 0x06006E7F RID: 28287 RVA: 0x00239B9C File Offset: 0x00237D9C
		private void Update()
		{
			string text = TestScheduledEventStateCycleButton.ReadState();
			if (text != this.lastRenderedState)
			{
				this.RenderState(text);
			}
		}

		// Token: 0x06006E80 RID: 28288 RVA: 0x00239BC4 File Offset: 0x00237DC4
		private static string ReadState()
		{
			if (PhotonNetwork.CurrentRoom == null)
			{
				return null;
			}
			object obj;
			PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("scheduledEventState", out obj);
			return obj as string;
		}

		// Token: 0x06006E81 RID: 28289 RVA: 0x00239BF7 File Offset: 0x00237DF7
		private static string NextState(string current)
		{
			if (current == "regular")
			{
				return "event-in-progress";
			}
			if (current == "event-in-progress")
			{
				return "post-event";
			}
			return "regular";
		}

		// Token: 0x06006E82 RID: 28290 RVA: 0x00239C24 File Offset: 0x00237E24
		private void RenderState(string state)
		{
			this.lastRenderedState = state;
			base.SetText(state ?? "(unset)");
		}

		// Token: 0x04007EA8 RID: 32424
		private string lastRenderedState;
	}
}
