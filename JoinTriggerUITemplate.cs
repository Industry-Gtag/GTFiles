using System;
using UnityEngine;

// Token: 0x02000359 RID: 857
[CreateAssetMenu(fileName = "JoinTriggerUITemplate", menuName = "ScriptableObjects/JoinTriggerUITemplate")]
public class JoinTriggerUITemplate : ScriptableObject
{
	// Token: 0x040019C8 RID: 6600
	public Material Milestone_Error;

	// Token: 0x040019C9 RID: 6601
	public Material Milestone_AlreadyInRoom;

	// Token: 0x040019CA RID: 6602
	public Material Milestone_InPrivateRoom;

	// Token: 0x040019CB RID: 6603
	public Material Milestone_NotConnectedSoloJoin;

	// Token: 0x040019CC RID: 6604
	public Material Milestone_LeaveRoomAndSoloJoin;

	// Token: 0x040019CD RID: 6605
	public Material Milestone_LeaveRoomAndGroupJoin;

	// Token: 0x040019CE RID: 6606
	public Material Milestone_AbandonPartyAndSoloJoin;

	// Token: 0x040019CF RID: 6607
	public Material Milestone_ChangingGameModeSoloJoin;

	// Token: 0x040019D0 RID: 6608
	public Material ScreenBG_Error;

	// Token: 0x040019D1 RID: 6609
	public Material ScreenBG_AlreadyInRoom;

	// Token: 0x040019D2 RID: 6610
	public Material ScreenBG_InPrivateRoom;

	// Token: 0x040019D3 RID: 6611
	public Material ScreenBG_NotConnectedSoloJoin;

	// Token: 0x040019D4 RID: 6612
	public Material ScreenBG_LeaveRoomAndSoloJoin;

	// Token: 0x040019D5 RID: 6613
	public Material ScreenBG_LeaveRoomAndGroupJoin;

	// Token: 0x040019D6 RID: 6614
	public Material ScreenBG_AbandonPartyAndSoloJoin;

	// Token: 0x040019D7 RID: 6615
	public Material ScreenBG_ChangingGameModeSoloJoin;

	// Token: 0x040019D8 RID: 6616
	public string ScreenText_Error;

	// Token: 0x040019D9 RID: 6617
	public bool showFullErrorMessages;

	// Token: 0x040019DA RID: 6618
	public JoinTriggerUITemplate.FormattedString ScreenText_AlreadyInRoom;

	// Token: 0x040019DB RID: 6619
	public JoinTriggerUITemplate.FormattedString ScreenText_InPrivateRoom;

	// Token: 0x040019DC RID: 6620
	public JoinTriggerUITemplate.FormattedString ScreenText_NotConnectedSoloJoin;

	// Token: 0x040019DD RID: 6621
	public JoinTriggerUITemplate.FormattedString ScreenText_LeaveRoomAndSoloJoin;

	// Token: 0x040019DE RID: 6622
	public JoinTriggerUITemplate.FormattedString ScreenText_LeaveRoomAndGroupJoin;

	// Token: 0x040019DF RID: 6623
	public JoinTriggerUITemplate.FormattedString ScreenText_AbandonPartyAndSoloJoin;

	// Token: 0x040019E0 RID: 6624
	public JoinTriggerUITemplate.FormattedString ScreenText_ChangingGameModeSoloJoin;

	// Token: 0x0200035A RID: 858
	[Serializable]
	public struct FormattedString
	{
		// Token: 0x0600150B RID: 5387 RVA: 0x00070428 File Offset: 0x0006E628
		public string GetText(string oldZone, string newZone, string oldGameType, string newGameType)
		{
			if (this.formatter == null)
			{
				this.formatter = StringFormatter.Parse(this.formatText);
			}
			return this.formatter.Format(new string[] { oldZone, newZone, oldGameType, newGameType });
		}

		// Token: 0x0600150C RID: 5388 RVA: 0x00070465 File Offset: 0x0006E665
		public string GetText(Func<string> oldZone, Func<string> newZone, Func<string> oldGameType, Func<string> newGameType)
		{
			if (this.formatter == null)
			{
				this.formatter = StringFormatter.Parse(this.formatText);
			}
			return this.formatter.Format(oldZone, newZone, oldGameType, newGameType);
		}

		// Token: 0x040019E1 RID: 6625
		[TextArea]
		[SerializeField]
		private string formatText;

		// Token: 0x040019E2 RID: 6626
		[NonSerialized]
		private StringFormatter formatter;
	}
}
