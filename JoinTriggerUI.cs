using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000358 RID: 856
public class JoinTriggerUI : MonoBehaviour
{
	// Token: 0x17000216 RID: 534
	// (get) Token: 0x06001500 RID: 5376 RVA: 0x0007007B File Offset: 0x0006E27B
	public bool HasFriendCollider
	{
		get
		{
			return this.friendColliderResolved;
		}
	}

	// Token: 0x17000217 RID: 535
	// (get) Token: 0x06001501 RID: 5377 RVA: 0x00070083 File Offset: 0x0006E283
	public GorillaFriendCollider FriendJoinCollider
	{
		get
		{
			return this.friendCollider;
		}
	}

	// Token: 0x06001502 RID: 5378 RVA: 0x0007008C File Offset: 0x0006E28C
	private void Awake()
	{
		this.friendColliderResolved = this.friendColliderRef.TryResolve<GorillaFriendCollider>(out this.friendCollider) && this.friendCollider != null;
		this.joinTriggerResolved = this.joinTriggerRef.TryResolve<GorillaNetworkJoinTrigger>(out this.joinTrigger) && this.joinTrigger != null;
	}

	// Token: 0x06001503 RID: 5379 RVA: 0x000700E9 File Offset: 0x0006E2E9
	private void Start()
	{
		this.didStart = true;
		this.OnEnable();
	}

	// Token: 0x06001504 RID: 5380 RVA: 0x000700F8 File Offset: 0x0006E2F8
	private void OnEnable()
	{
		if (this.didStart && this.IsValid())
		{
			this.joinTrigger.RegisterUI(this);
			if (this.friendColliderResolved)
			{
				this.friendCollider.RegisterUI(this);
			}
		}
	}

	// Token: 0x06001505 RID: 5381 RVA: 0x0007012A File Offset: 0x0006E32A
	private void OnDisable()
	{
		if (this.IsValid())
		{
			this.joinTrigger.UnregisterUI(this);
			if (this.friendColliderResolved)
			{
				this.friendCollider.UnregisterUI();
			}
		}
	}

	// Token: 0x06001506 RID: 5382 RVA: 0x00070153 File Offset: 0x0006E353
	public void TriggerUpdateUI()
	{
		if (this.IsValid())
		{
			this.joinTrigger.UpdateUI();
		}
	}

	// Token: 0x06001507 RID: 5383 RVA: 0x00070168 File Offset: 0x0006E368
	public void SetState(JoinTriggerVisualState state, Func<string> oldZone, Func<string> newZone, Func<string> oldGameMode, Func<string> newGameMode)
	{
		switch (state)
		{
		case JoinTriggerVisualState.ConnectionError:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_Error;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_Error;
			this.screenText.text = (this.template.showFullErrorMessages ? GorillaScoreboardTotalUpdater.instance.offlineTextErrorString : this.template.ScreenText_Error);
			return;
		case JoinTriggerVisualState.AlreadyInRoom:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_AlreadyInRoom;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_AlreadyInRoom;
			this.screenText.text = this.template.ScreenText_AlreadyInRoom.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.InPrivateRoom:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_InPrivateRoom;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_InPrivateRoom;
			this.screenText.text = this.template.ScreenText_InPrivateRoom.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.NotConnectedSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_NotConnectedSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_NotConnectedSoloJoin;
			this.screenText.text = this.template.ScreenText_NotConnectedSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.LeaveRoomAndSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_LeaveRoomAndSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_LeaveRoomAndSoloJoin;
			this.screenText.text = this.template.ScreenText_LeaveRoomAndSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.LeaveRoomAndPartyJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_LeaveRoomAndGroupJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_LeaveRoomAndGroupJoin;
			this.screenText.text = this.template.ScreenText_LeaveRoomAndGroupJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.AbandonPartyAndSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_AbandonPartyAndSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_AbandonPartyAndSoloJoin;
			this.screenText.text = this.template.ScreenText_AbandonPartyAndSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		case JoinTriggerVisualState.ChangingGameModeSoloJoin:
			this.milestoneRenderer.sharedMaterial = this.template.Milestone_ChangingGameModeSoloJoin;
			this.screenBGRenderer.sharedMaterial = this.template.ScreenBG_ChangingGameModeSoloJoin;
			this.screenText.text = this.template.ScreenText_ChangingGameModeSoloJoin.GetText(oldZone, newZone, oldGameMode, newGameMode);
			return;
		default:
			return;
		}
	}

	// Token: 0x06001508 RID: 5384 RVA: 0x00070419 File Offset: 0x0006E619
	private bool IsValid()
	{
		bool flag = this.joinTriggerResolved;
		return this.joinTriggerResolved;
	}

	// Token: 0x040019BD RID: 6589
	[SerializeField]
	private XSceneRef joinTriggerRef;

	// Token: 0x040019BE RID: 6590
	private GorillaNetworkJoinTrigger joinTrigger;

	// Token: 0x040019BF RID: 6591
	private bool joinTriggerResolved;

	// Token: 0x040019C0 RID: 6592
	[SerializeField]
	private XSceneRef friendColliderRef;

	// Token: 0x040019C1 RID: 6593
	private GorillaFriendCollider friendCollider;

	// Token: 0x040019C2 RID: 6594
	private bool friendColliderResolved;

	// Token: 0x040019C3 RID: 6595
	[SerializeField]
	private MeshRenderer milestoneRenderer;

	// Token: 0x040019C4 RID: 6596
	[SerializeField]
	private MeshRenderer screenBGRenderer;

	// Token: 0x040019C5 RID: 6597
	[SerializeField]
	private TextMeshPro screenText;

	// Token: 0x040019C6 RID: 6598
	[SerializeField]
	private JoinTriggerUITemplate template;

	// Token: 0x040019C7 RID: 6599
	private new bool didStart;
}
