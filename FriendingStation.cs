using System;
using Cysharp.Text;
using GorillaLocomotion;
using GorillaTagScripts;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000CC1 RID: 3265
public class FriendingStation : MonoBehaviour
{
	// Token: 0x170007B2 RID: 1970
	// (get) Token: 0x060050FD RID: 20733 RVA: 0x001ADADA File Offset: 0x001ABCDA
	public TextMeshProUGUI Player1Text
	{
		get
		{
			return this.player1Text;
		}
	}

	// Token: 0x170007B3 RID: 1971
	// (get) Token: 0x060050FE RID: 20734 RVA: 0x001ADAE2 File Offset: 0x001ABCE2
	public TextMeshProUGUI Player2Text
	{
		get
		{
			return this.player2Text;
		}
	}

	// Token: 0x170007B4 RID: 1972
	// (get) Token: 0x060050FF RID: 20735 RVA: 0x001ADAEA File Offset: 0x001ABCEA
	public TextMeshProUGUI StatusText
	{
		get
		{
			return this.statusText;
		}
	}

	// Token: 0x170007B5 RID: 1973
	// (get) Token: 0x06005100 RID: 20736 RVA: 0x001ADAF2 File Offset: 0x001ABCF2
	public GTZone Zone
	{
		get
		{
			return this.zone;
		}
	}

	// Token: 0x06005101 RID: 20737 RVA: 0x001ADAFA File Offset: 0x001ABCFA
	private void Awake()
	{
		this.triggerNotifier.TriggerEnterEvent += this.TriggerEntered;
		this.triggerNotifier.TriggerExitEvent += this.TriggerExited;
	}

	// Token: 0x06005102 RID: 20738 RVA: 0x001ADB2C File Offset: 0x001ABD2C
	private void OnEnable()
	{
		FriendingManager.Instance.RegisterFriendingStation(this);
		if (PhotonNetwork.InRoom)
		{
			this.displayedData.actorNumberA = -1;
			this.displayedData.actorNumberB = -1;
			this.displayedData.state = FriendingManager.FriendStationState.WaitingForPlayers;
		}
		else
		{
			this.displayedData.actorNumberA = -2;
			this.displayedData.actorNumberB = -2;
			this.displayedData.state = FriendingManager.FriendStationState.NotInRoom;
		}
		this.UpdatePlayerText(this.player1Text, this.displayedData.actorNumberA);
		this.UpdatePlayerText(this.player2Text, this.displayedData.actorNumberB);
		this.UpdateDisplayedState(this.displayedData.state);
	}

	// Token: 0x06005103 RID: 20739 RVA: 0x001ADBD8 File Offset: 0x001ABDD8
	private void OnDisable()
	{
		FriendingManager.Instance.UnregisterFriendingStation(this);
	}

	// Token: 0x06005104 RID: 20740 RVA: 0x001ADBE8 File Offset: 0x001ABDE8
	private void UpdatePlayerText(TextMeshProUGUI playerText, int playerId)
	{
		if (playerId == -2)
		{
			playerText.text = "";
			return;
		}
		if (playerId == -1)
		{
			playerText.text = "PLAYER:\nNONE";
			return;
		}
		NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(playerId);
		if (netPlayerByID != null)
		{
			playerText.text = "PLAYER:\n" + netPlayerByID.SanitizedNickName;
			return;
		}
		playerText.text = "PLAYER:\nNONE";
	}

	// Token: 0x06005105 RID: 20741 RVA: 0x001ADC48 File Offset: 0x001ABE48
	private void UpdateDisplayedState(FriendingManager.FriendStationState state)
	{
		switch (state)
		{
		case FriendingManager.FriendStationState.NotInRoom:
			this.statusText.text = "JOIN A ROOM TO USE";
			return;
		case FriendingManager.FriendStationState.WaitingForPlayers:
			this.statusText.text = "";
			return;
		case FriendingManager.FriendStationState.WaitingOnFriendStatusBoth:
			this.statusText.text = "LOADING";
			return;
		case FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerA:
			this.statusText.text = "LOADING";
			return;
		case FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerB:
			this.statusText.text = "LOADING";
			return;
		case FriendingManager.FriendStationState.WaitingOnButtonBoth:
			this.statusText.text = "PRESS [       ] PRESS";
			return;
		case FriendingManager.FriendStationState.WaitingOnButtonPlayerA:
			this.statusText.text = "PRESS [       ] READY";
			return;
		case FriendingManager.FriendStationState.WaitingOnButtonPlayerB:
			this.statusText.text = "READY [       ] PRESS";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer0:
			this.statusText.text = "READY [       ] READY";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer1:
			this.statusText.text = "READY [-     -] READY";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer2:
			this.statusText.text = "READY [--   --] READY";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer3:
			this.statusText.text = "READY [--- ---] READY";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer4:
			this.statusText.text = "READY [-------] READY";
			return;
		case FriendingManager.FriendStationState.WaitingOnRequestBoth:
			this.statusText.text = " SENT [-------] SENT ";
			return;
		case FriendingManager.FriendStationState.WaitingOnRequestPlayerA:
			this.statusText.text = " SENT [-------] DONE ";
			return;
		case FriendingManager.FriendStationState.WaitingOnRequestPlayerB:
			this.statusText.text = " DONE [-------] SENT ";
			return;
		case FriendingManager.FriendStationState.RequestFailed:
			this.statusText.text = "FRIEND REQUEST FAILED";
			return;
		case FriendingManager.FriendStationState.Friends:
			this.statusText.text = "\\O/ FRIENDS \\O/";
			return;
		case FriendingManager.FriendStationState.AlreadyFriends:
			this.statusText.text = "ALREADY FRIENDS";
			return;
		default:
			return;
		}
	}

	// Token: 0x06005106 RID: 20742 RVA: 0x001ADDEC File Offset: 0x001ABFEC
	private void UpdateAddFriendButton()
	{
		int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		if ((this.displayedData.state >= FriendingManager.FriendStationState.ButtonConfirmationTimer0 && this.displayedData.state <= FriendingManager.FriendStationState.ButtonConfirmationTimer4) || (this.displayedData.actorNumberA == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB) || (this.displayedData.actorNumberB == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA))
		{
			this.addFriendButton.isOn = true;
		}
		else
		{
			this.addFriendButton.isOn = false;
		}
		this.addFriendButton.UpdateColor();
	}

	// Token: 0x06005107 RID: 20743 RVA: 0x001ADE8C File Offset: 0x001AC08C
	private void UpdateDisplay(ref FriendingManager.FriendStationData data)
	{
		if (this.displayedData.actorNumberA != data.actorNumberA)
		{
			this.UpdatePlayerText(this.player1Text, data.actorNumberA);
		}
		if (this.displayedData.actorNumberB != data.actorNumberB)
		{
			this.UpdatePlayerText(this.player2Text, data.actorNumberB);
		}
		if (this.displayedData.state != data.state)
		{
			this.UpdateDisplayedState(data.state);
		}
		this.displayedData = data;
		this.UpdateAddFriendButton();
	}

	// Token: 0x06005108 RID: 20744 RVA: 0x001ADF14 File Offset: 0x001AC114
	public void UpdateState(FriendingManager.FriendStationData data)
	{
		this.UpdateDisplay(ref data);
	}

	// Token: 0x06005109 RID: 20745 RVA: 0x001ADF20 File Offset: 0x001AC120
	public void TriggerEntered(TriggerEventNotifier notifier, Collider other)
	{
		if (PhotonNetwork.InRoom)
		{
			VRRig component = other.GetComponent<VRRig>();
			if (component != null && component.OwningNetPlayer != null)
			{
				this.addFriendButton.ResetState();
				FriendingManager.Instance.PlayerEnteredStation(this.zone, component.OwningNetPlayer);
				return;
			}
		}
		else if (other == GTPlayer.Instance.headCollider)
		{
			this.displayedData.state = FriendingManager.FriendStationState.NotInRoom;
			this.displayedData.actorNumberA = -2;
			this.displayedData.actorNumberB = -2;
			this.UpdateDisplayedState(this.displayedData.state);
			this.UpdatePlayerText(this.player1Text, this.displayedData.actorNumberA);
			this.UpdatePlayerText(this.player2Text, this.displayedData.actorNumberB);
			this.addFriendButton.ResetState();
		}
	}

	// Token: 0x0600510A RID: 20746 RVA: 0x001ADFF8 File Offset: 0x001AC1F8
	public void TriggerExited(TriggerEventNotifier notifier, Collider other)
	{
		if (PhotonNetwork.InRoom)
		{
			VRRig component = other.GetComponent<VRRig>();
			if (component != null)
			{
				this.addFriendButton.ResetState();
				FriendingManager.Instance.PlayerExitedStation(this.zone, component.OwningNetPlayer);
				return;
			}
		}
		else if (other == GTPlayer.Instance.headCollider)
		{
			this.displayedData.state = FriendingManager.FriendStationState.NotInRoom;
			this.displayedData.actorNumberA = -2;
			this.displayedData.actorNumberB = -2;
			this.UpdateDisplayedState(this.displayedData.state);
			this.UpdatePlayerText(this.player1Text, this.displayedData.actorNumberA);
			this.UpdatePlayerText(this.player2Text, this.displayedData.actorNumberB);
			this.addFriendButton.ResetState();
		}
	}

	// Token: 0x0600510B RID: 20747 RVA: 0x001AE0C8 File Offset: 0x001AC2C8
	public void FriendButtonPressed()
	{
		if (this.displayedData.state == FriendingManager.FriendStationState.WaitingForPlayers || this.displayedData.state == FriendingManager.FriendStationState.Friends)
		{
			return;
		}
		string text;
		if (this.LocalPlayerIsAtCapacity(out text))
		{
			this.statusText.text = text;
			return;
		}
		if (!this.addFriendButton.isOn)
		{
			FriendingManager.Instance.photonView.RPC("FriendButtonPressedRPC", RpcTarget.MasterClient, new object[] { this.zone });
			int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			if (this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonBoth || (this.displayedData.actorNumberA == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA) || (this.displayedData.actorNumberB == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB))
			{
				this.addFriendButton.isOn = true;
				this.addFriendButton.UpdateColor();
			}
		}
	}

	// Token: 0x0600510C RID: 20748 RVA: 0x001AE1B4 File Offset: 0x001AC3B4
	private bool LocalPlayerIsAtCapacity(out string fullMessage)
	{
		fullMessage = null;
		FriendBackendController instance = FriendBackendController.Instance;
		if (instance == null || instance.FriendsList == null)
		{
			return false;
		}
		int configuredFreeExtraPageCount = FriendDisplay.ConfiguredFreeExtraPageCount;
		int configuredVimPageCount = FriendDisplay.ConfiguredVimPageCount;
		int num = 9 * configuredVimPageCount;
		int num2 = 9 + 9 * configuredFreeExtraPageCount;
		int num3 = num2 + num;
		int count = instance.FriendsList.Count;
		if (count < num2)
		{
			return false;
		}
		if (!SubscriptionManager.IsLocalSubscribed() && configuredVimPageCount != 0)
		{
			int num4 = Mathf.Clamp(count - num2, 0, num);
			if (num4 <= 0)
			{
				fullMessage = ZString.Format<int>("FRIEND SLOTS ARE FULL! SUBSCRIBE FOR {0} ADDITIONAL SLOTS!", num);
			}
			else
			{
				int num5 = num - num4;
				fullMessage = ZString.Format<int>("RENEW GTFC SUBSCRIPTION TO UNLOCK YOUR REMAINING {0} SLOTS.", num5);
			}
			return true;
		}
		if (count >= num3)
		{
			fullMessage = "CANNOT ADD FRIEND! ALL FRIEND SLOTS FILLED!";
			return true;
		}
		return false;
	}

	// Token: 0x0600510D RID: 20749 RVA: 0x001AE268 File Offset: 0x001AC468
	public void FriendButtonReleased()
	{
		if (this.displayedData.state == FriendingManager.FriendStationState.WaitingForPlayers || this.displayedData.state == FriendingManager.FriendStationState.Friends)
		{
			return;
		}
		if (this.addFriendButton.isOn)
		{
			FriendingManager.Instance.photonView.RPC("FriendButtonUnpressedRPC", RpcTarget.MasterClient, new object[] { this.zone });
			int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			if ((this.displayedData.state >= FriendingManager.FriendStationState.ButtonConfirmationTimer0 && this.displayedData.state <= FriendingManager.FriendStationState.ButtonConfirmationTimer4) || (this.displayedData.actorNumberA == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB) || (this.displayedData.actorNumberB == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA))
			{
				this.addFriendButton.isOn = false;
				this.addFriendButton.UpdateColor();
			}
		}
	}

	// Token: 0x04006306 RID: 25350
	[SerializeField]
	private TriggerEventNotifier triggerNotifier;

	// Token: 0x04006307 RID: 25351
	[SerializeField]
	private TextMeshProUGUI player1Text;

	// Token: 0x04006308 RID: 25352
	[SerializeField]
	private TextMeshProUGUI player2Text;

	// Token: 0x04006309 RID: 25353
	[SerializeField]
	private TextMeshProUGUI statusText;

	// Token: 0x0400630A RID: 25354
	[SerializeField]
	private GTZone zone;

	// Token: 0x0400630B RID: 25355
	[SerializeField]
	private GorillaPressableButton addFriendButton;

	// Token: 0x0400630C RID: 25356
	private FriendingManager.FriendStationData displayedData;
}
