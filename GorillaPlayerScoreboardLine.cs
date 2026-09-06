using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000A31 RID: 2609
public class GorillaPlayerScoreboardLine : MonoBehaviour
{
	// Token: 0x060042D7 RID: 17111 RVA: 0x00163D7A File Offset: 0x00161F7A
	public void Start()
	{
		this.emptyRigCount = 0;
		this.reportedCheating = false;
		this.reportedHateSpeech = false;
		this.reportedToxicity = false;
	}

	// Token: 0x060042D8 RID: 17112 RVA: 0x00163D98 File Offset: 0x00161F98
	public void InitializeLine()
	{
		this.currentNickname = string.Empty;
		this.UpdatePlayerText();
		if (this.linePlayer == NetworkSystem.Instance.LocalPlayer)
		{
			this.muteButton.gameObject.SetActive(false);
			this.reportButton.gameObject.SetActive(false);
			this.reportButtons.SetActive(false);
			return;
		}
		this.muteButton.gameObject.SetActive(true);
		if (GorillaScoreboardTotalUpdater.instance != null && GorillaScoreboardTotalUpdater.instance.reportDict.ContainsKey(this.playerActorNumber))
		{
			GorillaScoreboardTotalUpdater.PlayerReports playerReports = GorillaScoreboardTotalUpdater.instance.reportDict[this.playerActorNumber];
			this.reportedCheating = playerReports.cheating;
			this.reportedHateSpeech = playerReports.hateSpeech;
			this.reportedToxicity = playerReports.toxicity;
			this.reportInProgress = playerReports.pressedReport;
		}
		else
		{
			this.reportedCheating = false;
			this.reportedHateSpeech = false;
			this.reportedToxicity = false;
			this.reportInProgress = false;
		}
		this.reportButton.isOn = this.reportedCheating || this.reportedHateSpeech || this.reportedToxicity;
		this.reportButton.UpdateColor();
		this.SwapToReportState(this.reportInProgress);
		this.muteButton.gameObject.SetActive(true);
		this.isMuteManual = PlayerPrefs.HasKey(this.linePlayer.UserId);
		this.mute = PlayerPrefs.GetInt(this.linePlayer.UserId, 0);
		this.muteButton.isOn = this.mute != 0;
		this.muteButton.isAutoOn = false;
		this.muteButton.UpdateColor();
		if (this.rigContainer != null)
		{
			this.rigContainer.hasManualMute = this.isMuteManual;
			this.rigContainer.SetMuted(RigContainer.MuteReason.Manual, this.mute != 0);
		}
		this._parentConfirmButton = null;
		this._attemptingKick = false;
		this._attemptingBan = false;
	}

	// Token: 0x060042D9 RID: 17113 RVA: 0x00163F84 File Offset: 0x00162184
	public void SetLineData(NetPlayer netPlayer)
	{
		if (!netPlayer.InRoom || netPlayer == this.linePlayer)
		{
			return;
		}
		if (this.playerActorNumber != netPlayer.ActorNumber)
		{
			this.initTime = Time.time;
		}
		this.playerActorNumber = netPlayer.ActorNumber;
		this.linePlayer = netPlayer;
		this.playerNameValue = netPlayer.NickName ?? "";
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
		{
			this.rigContainer = rigContainer;
			this.playerVRRig = rigContainer.Rig;
		}
		this.InitializeLine();
	}

	// Token: 0x060042DA RID: 17114 RVA: 0x0016400C File Offset: 0x0016220C
	public void UpdateLine()
	{
		if (this.linePlayer == null)
		{
			return;
		}
		if (this.playerNameVisible != this.playerVRRig.playerNameVisible)
		{
			this.UpdatePlayerText();
			this.parentScoreboard.SetDirty();
			if (this.playerVRRig.creator.IsMasterClient && GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				CustomMapModeSelector.RefreshHostName();
			}
		}
		if (this.rigContainer == null)
		{
			return;
		}
		if (Time.time > this.initTime + this.emptyRigCooldown)
		{
			if (this.playerVRRig.netView != null)
			{
				this.emptyRigCount = 0;
			}
			else
			{
				this.emptyRigCount++;
				if (this.emptyRigCount > 30)
				{
					MonkeAgent.instance.SendReport("empty rig", this.linePlayer.UserId, this.linePlayer.NickName);
				}
			}
		}
		Material material = ((this.playerVRRig.setMatIndex == 0) ? this.playerVRRig.scoreboardMaterial : this.playerVRRig.materialsToChangeTo[this.playerVRRig.setMatIndex]);
		if (this.playerSwatch.material != material)
		{
			this.playerSwatch.material = material;
		}
		if (this.playerSwatch.color != this.playerVRRig.materialsToChangeTo[0].color)
		{
			this.playerSwatch.color = this.playerVRRig.materialsToChangeTo[0].color;
		}
		if (this.myRecorder == null)
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
		}
		if (this.playerVRRig != null)
		{
			if (this.playerVRRig.remoteUseReplacementVoice || this.playerVRRig.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE")
			{
				this.speakerIcon.enabled = this.playerVRRig.SpeakingLoudness > this.playerVRRig.replacementVoiceLoudnessThreshold && !this.rigContainer.IsMutedFor(~RigContainer.MuteReason.Auto);
			}
			else if ((this.rigContainer.Voice != null && this.rigContainer.Voice.IsSpeaking) || (this.playerVRRig.rigSerializer != null && this.playerVRRig.rigSerializer.IsLocallyOwned && this.myRecorder != null && this.myRecorder.IsCurrentlyTransmitting))
			{
				this.speakerIcon.enabled = true;
			}
			else
			{
				this.speakerIcon.enabled = false;
			}
		}
		else
		{
			this.speakerIcon.enabled = false;
		}
		if (!this.isMuteManual)
		{
			bool isPlayerAutoMuted = this.rigContainer.GetIsPlayerAutoMuted();
			if (this.muteButton.isAutoOn != isPlayerAutoMuted)
			{
				this.muteButton.isAutoOn = isPlayerAutoMuted;
				this.muteButton.UpdateColor();
			}
		}
		bool flag = RoomControls.MutedPlayers.ContainsKey(this.linePlayer.UserId);
		if (this.muteForRoomButton.isOn != flag)
		{
			this.muteForRoomButton.isOn = flag;
			this.muteForRoomButton.UpdateColor();
		}
	}

	// Token: 0x060042DB RID: 17115 RVA: 0x00164320 File Offset: 0x00162520
	private void UpdatePlayerText()
	{
		try
		{
			if (this.rigContainer.IsNull() || this.playerVRRig.IsNull())
			{
				this.playerNameVisible = this.NormalizeName(this.linePlayer.NickName != this.currentNickname, this.linePlayer.NickName);
				this.currentNickname = this.linePlayer.NickName;
			}
			else if (this.rigContainer.Initialized)
			{
				this.playerNameVisible = this.playerVRRig.playerNameVisible;
			}
			else if (this.currentNickname.IsNullOrEmpty() || GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(this.linePlayer.UserId))
			{
				this.playerNameVisible = this.NormalizeName(this.linePlayer.NickName != this.currentNickname, this.linePlayer.NickName);
			}
			this.currentNickname = this.linePlayer.NickName;
			this.playerName.text = (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags) ? this.playerNameVisible : this.linePlayer.DefaultName);
		}
		catch (Exception)
		{
			this.playerNameVisible = this.linePlayer.DefaultName;
			MonkeAgent.instance.SendReport("NmError", this.linePlayer.UserId, this.linePlayer.NickName);
		}
	}

	// Token: 0x060042DC RID: 17116 RVA: 0x00164494 File Offset: 0x00162694
	public bool IsLineActive()
	{
		return base.gameObject.activeInHierarchy;
	}

	// Token: 0x060042DD RID: 17117 RVA: 0x001644A1 File Offset: 0x001626A1
	public bool IsPlayerInRoom()
	{
		return this.linePlayer != null && this.linePlayer.InRoom;
	}

	// Token: 0x060042DE RID: 17118 RVA: 0x001644B8 File Offset: 0x001626B8
	public bool IsReportButtonActive()
	{
		return this.reportButton.isActiveAndEnabled;
	}

	// Token: 0x060042DF RID: 17119 RVA: 0x001644C5 File Offset: 0x001626C5
	public bool IsConfirmButtonsActive()
	{
		return this.confirmRoomControlButtons.activeInHierarchy;
	}

	// Token: 0x060042E0 RID: 17120 RVA: 0x001644D2 File Offset: 0x001626D2
	public bool IsConfirmParentKick()
	{
		return this._parentConfirmButton == this.kickButton;
	}

	// Token: 0x060042E1 RID: 17121 RVA: 0x001644E5 File Offset: 0x001626E5
	public bool IsConfirmParentBan()
	{
		return this._parentConfirmButton == this.banButton;
	}

	// Token: 0x060042E2 RID: 17122 RVA: 0x001644F8 File Offset: 0x001626F8
	public void PressButton(bool isOn, GorillaPlayerLineButton.ButtonType buttonType)
	{
		switch (buttonType)
		{
		case GorillaPlayerLineButton.ButtonType.Mute:
			if (this.linePlayer != null && this.playerVRRig != null)
			{
				this.isMuteManual = true;
				this.muteButton.isAutoOn = false;
				this.mute = (isOn ? 1 : 0);
				PlayerPrefs.SetInt(this.linePlayer.UserId, this.mute);
				if (this.rigContainer != null)
				{
					this.rigContainer.hasManualMute = this.isMuteManual;
					this.rigContainer.SetMuted(RigContainer.MuteReason.Manual, this.mute != 0);
				}
				PlayerPrefs.Save();
				this.muteButton.UpdateColor();
				GorillaScoreboardTotalUpdater.ReportMute(this.linePlayer, this.mute);
				return;
			}
			break;
		case GorillaPlayerLineButton.ButtonType.Report:
			this.SetReportState(true, buttonType);
			return;
		case GorillaPlayerLineButton.ButtonType.Cancel:
			if (this._parentConfirmButton == null)
			{
				this.SetReportState(false, buttonType);
				return;
			}
			this.HideConfirmButtons();
			return;
		case GorillaPlayerLineButton.ButtonType.MuteAllRoom:
			Debug.Log("Pressed Mute Room Control button for player " + this.linePlayer.UserId);
			this.AttemptRoomControlMute();
			return;
		case GorillaPlayerLineButton.ButtonType.KickRoom:
			if (this._attemptingKick || this._attemptingBan)
			{
				Debug.Log("Ban or kick for player " + this.linePlayer.UserId + " is already in progress!");
				return;
			}
			Debug.Log("Pressed Kick Room Control button for player " + this.linePlayer.UserId);
			this.ShowConfirmButtons(this.kickButton);
			return;
		case GorillaPlayerLineButton.ButtonType.BanRoom:
			if (this._attemptingKick || this._attemptingBan)
			{
				Debug.Log("Ban or kick for player " + this.linePlayer.UserId + " is already in progress!");
				return;
			}
			Debug.Log("Pressed Ban Room Control button for player " + this.linePlayer.UserId);
			this.ShowConfirmButtons(this.banButton);
			return;
		case GorillaPlayerLineButton.ButtonType.Confirm:
			if (this.IsConfirmParentKick())
			{
				if (this.AttemptRoomControlKick())
				{
					this.HideConfirmButtons();
					return;
				}
			}
			else if (this.IsConfirmParentBan() && this.AttemptRoomControlBan())
			{
				this.HideConfirmButtons();
				return;
			}
			break;
		default:
			this.SetReportState(false, buttonType);
			break;
		}
	}

	// Token: 0x060042E3 RID: 17123 RVA: 0x00164708 File Offset: 0x00162908
	public void SetReportState(bool reportState, GorillaPlayerLineButton.ButtonType buttonType)
	{
		this.canPressNextReportButton = buttonType != GorillaPlayerLineButton.ButtonType.Toxicity && buttonType != GorillaPlayerLineButton.ButtonType.Report;
		this.reportInProgress = reportState;
		this.SwapToReportState(reportState);
		if (!reportState && this.linePlayer != null && buttonType != GorillaPlayerLineButton.ButtonType.Cancel)
		{
			if ((!this.reportedHateSpeech && buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech) || (!this.reportedToxicity && buttonType == GorillaPlayerLineButton.ButtonType.Toxicity) || (!this.reportedCheating && buttonType == GorillaPlayerLineButton.ButtonType.Cheating))
			{
				GorillaPlayerScoreboardLine.ReportPlayer(this.linePlayer.UserId, buttonType, this.playerNameVisible);
				this.doneReporting = true;
			}
			this.reportedCheating = this.reportedCheating || buttonType == GorillaPlayerLineButton.ButtonType.Cheating;
			this.reportedToxicity = this.reportedToxicity || buttonType == GorillaPlayerLineButton.ButtonType.Toxicity;
			this.reportedHateSpeech = this.reportedHateSpeech || buttonType == GorillaPlayerLineButton.ButtonType.HateSpeech;
			this.reportButton.isOn = true;
			this.reportButton.UpdateColor();
		}
		if (GorillaScoreboardTotalUpdater.instance != null)
		{
			GorillaScoreboardTotalUpdater.instance.UpdateLineState(this);
		}
		this.parentScoreboard.RedrawPlayerLines();
	}

	// Token: 0x060042E4 RID: 17124 RVA: 0x0016480C File Offset: 0x00162A0C
	public static void ReportPlayer(string PlayerID, GorillaPlayerLineButton.ButtonType buttonType, string OtherPlayerNickName)
	{
		if (OtherPlayerNickName.Length > 12)
		{
			OtherPlayerNickName.Remove(12);
		}
		WebFlags webFlags = new WebFlags(3);
		NetEventOptions netEventOptions = new NetEventOptions
		{
			Flags = webFlags,
			TargetActors = GorillaPlayerScoreboardLine.targetActors
		};
		byte b = 50;
		object[] array = new object[]
		{
			PlayerID,
			buttonType,
			OtherPlayerNickName,
			NetworkSystem.Instance.LocalPlayer.NickName,
			!NetworkSystem.Instance.SessionIsPrivate,
			NetworkSystem.Instance.RoomStringStripped()
		};
		NetworkSystemRaiseEvent.RaiseEvent(b, array, netEventOptions, true);
	}

	// Token: 0x060042E5 RID: 17125 RVA: 0x001648A3 File Offset: 0x00162AA3
	public void ToggleRoomControlButtons(bool toggle, bool hideConfirm = true)
	{
		this.roomControlButtons.gameObject.SetActive(toggle);
		if (hideConfirm && !toggle)
		{
			this.HideConfirmButtons();
		}
	}

	// Token: 0x060042E6 RID: 17126 RVA: 0x001648C4 File Offset: 0x00162AC4
	public void ShowConfirmButtons(GorillaPlayerLineButton parentButton)
	{
		this.kickButton.gameObject.SetActive(false);
		this.banButton.gameObject.SetActive(false);
		this.confirmRoomControlButtons.SetActive(true);
		this.confirmRoomControlButton.SetTouchTime(0.5f);
		this.cancelRoomControlButton.SetTouchTime(0.5f);
		this.confirmRoomControlButtons.transform.localPosition = new Vector2(parentButton.transform.localPosition.x, 0f);
		this._parentConfirmButton = parentButton;
		this.parentScoreboard.RedrawPlayerLines();
	}

	// Token: 0x060042E7 RID: 17127 RVA: 0x00164960 File Offset: 0x00162B60
	public void HideConfirmButtons()
	{
		this.kickButton.gameObject.SetActive(true);
		this.banButton.gameObject.SetActive(true);
		this.confirmRoomControlButtons.SetActive(false);
		this._parentConfirmButton = null;
		this.parentScoreboard.RedrawPlayerLines();
	}

	// Token: 0x060042E8 RID: 17128 RVA: 0x001649B0 File Offset: 0x00162BB0
	private string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			int length = text.Length;
			text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
			int length2 = text.Length;
			if (length2 > 0 && length == length2 && GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				if (text.Length > 12)
				{
					text = text.Substring(0, 12);
				}
				text = text.ToUpper();
			}
			else
			{
				text = "BADGORILLA";
				MonkeAgent.instance.SendReport("evading the name ban", this.linePlayer.UserId, this.linePlayer.NickName);
			}
		}
		return text;
	}

	// Token: 0x060042E9 RID: 17129 RVA: 0x00164A67 File Offset: 0x00162C67
	public void ResetData()
	{
		this.emptyRigCount = 0;
		this.playerActorNumber = -1;
		this.linePlayer = null;
		this.playerNameValue = string.Empty;
		this.currentNickname = string.Empty;
	}

	// Token: 0x060042EA RID: 17130 RVA: 0x00164A94 File Offset: 0x00162C94
	private void OnEnable()
	{
		GorillaScoreboardTotalUpdater.RegisterSL(this);
	}

	// Token: 0x060042EB RID: 17131 RVA: 0x00164A9C File Offset: 0x00162C9C
	private void OnDisable()
	{
		GorillaScoreboardTotalUpdater.UnregisterSL(this);
	}

	// Token: 0x060042EC RID: 17132 RVA: 0x00164AA4 File Offset: 0x00162CA4
	private void SwapToReportState(bool reportInProgress)
	{
		this.reportButton.gameObject.SetActive(!reportInProgress);
		this.reportButtons.SetActive(reportInProgress);
	}

	// Token: 0x060042ED RID: 17133 RVA: 0x00164AC8 File Offset: 0x00162CC8
	private bool AttemptRoomControlMute()
	{
		string text;
		if (!RoomControls.CanModerate(out text))
		{
			Debug.LogWarning("Cannot mute player in room: " + text + ".");
			return false;
		}
		if (RoomControls.MutedPlayers.ContainsKey(this.linePlayer.UserId))
		{
			Debug.Log("Attempting UNMUTE in room for player " + this.linePlayer.UserId);
			RoomControls.UnmutePlayer(this.linePlayer.UserId);
		}
		else
		{
			Debug.Log("Attempting MUTE in room for player " + this.linePlayer.UserId);
			RoomControls.MutePlayer(this.linePlayer.ActorNumber, -1);
		}
		return true;
	}

	// Token: 0x060042EE RID: 17134 RVA: 0x00164B64 File Offset: 0x00162D64
	private bool AttemptRoomControlKick()
	{
		string text;
		if (!RoomControls.CanModerate(out text))
		{
			Debug.LogWarning("Cannot kick player from room: " + text + ".");
			return false;
		}
		Debug.Log("Attempting KICK from room for player " + this.linePlayer.UserId);
		RoomControls.KickPlayer(this.linePlayer.ActorNumber);
		this._attemptingKick = true;
		return true;
	}

	// Token: 0x060042EF RID: 17135 RVA: 0x00164BC4 File Offset: 0x00162DC4
	private bool AttemptRoomControlBan()
	{
		string text;
		if (!RoomControls.CanModerate(out text))
		{
			Debug.LogWarning("Cannot ban player from room: " + text + ".");
			return false;
		}
		Debug.Log("Attempting BAN from room for player " + this.linePlayer.UserId);
		RoomControls.KickAndBlockPlayer(this.linePlayer.ActorNumber, -1);
		this._attemptingBan = true;
		return true;
	}

	// Token: 0x040054A6 RID: 21670
	private static int[] targetActors = new int[] { -1 };

	// Token: 0x040054A7 RID: 21671
	public Text playerName;

	// Token: 0x040054A8 RID: 21672
	public Text playerLevel;

	// Token: 0x040054A9 RID: 21673
	public Text playerMMR;

	// Token: 0x040054AA RID: 21674
	public Image playerSwatch;

	// Token: 0x040054AB RID: 21675
	public Texture infectedTexture;

	// Token: 0x040054AC RID: 21676
	public NetPlayer linePlayer;

	// Token: 0x040054AD RID: 21677
	public VRRig playerVRRig;

	// Token: 0x040054AE RID: 21678
	public string playerLevelValue;

	// Token: 0x040054AF RID: 21679
	public string playerMMRValue;

	// Token: 0x040054B0 RID: 21680
	public string playerNameValue;

	// Token: 0x040054B1 RID: 21681
	public string playerNameVisible;

	// Token: 0x040054B2 RID: 21682
	public int playerActorNumber;

	// Token: 0x040054B3 RID: 21683
	public GorillaPlayerLineButton muteButton;

	// Token: 0x040054B4 RID: 21684
	public GorillaPlayerLineButton reportButton;

	// Token: 0x040054B5 RID: 21685
	[Space]
	public GameObject reportButtons;

	// Token: 0x040054B6 RID: 21686
	public GameObject hateSpeechButton;

	// Token: 0x040054B7 RID: 21687
	public GameObject toxicityButton;

	// Token: 0x040054B8 RID: 21688
	public GameObject cheatingButton;

	// Token: 0x040054B9 RID: 21689
	public GameObject cancelButton;

	// Token: 0x040054BA RID: 21690
	[Space]
	public GameObject roomControlButtons;

	// Token: 0x040054BB RID: 21691
	public GorillaPlayerLineButton muteForRoomButton;

	// Token: 0x040054BC RID: 21692
	public GorillaPlayerLineButton kickButton;

	// Token: 0x040054BD RID: 21693
	public GorillaPlayerLineButton banButton;

	// Token: 0x040054BE RID: 21694
	[Space]
	public GameObject confirmRoomControlButtons;

	// Token: 0x040054BF RID: 21695
	public GorillaPlayerLineButton confirmRoomControlButton;

	// Token: 0x040054C0 RID: 21696
	public GorillaPlayerLineButton cancelRoomControlButton;

	// Token: 0x040054C1 RID: 21697
	[Space]
	public SpriteRenderer speakerIcon;

	// Token: 0x040054C2 RID: 21698
	public bool canPressNextReportButton = true;

	// Token: 0x040054C3 RID: 21699
	public Text[] texts;

	// Token: 0x040054C4 RID: 21700
	public SpriteRenderer[] sprites;

	// Token: 0x040054C5 RID: 21701
	public MeshRenderer[] meshes;

	// Token: 0x040054C6 RID: 21702
	public Image[] images;

	// Token: 0x040054C7 RID: 21703
	private Recorder myRecorder;

	// Token: 0x040054C8 RID: 21704
	private bool isMuteManual;

	// Token: 0x040054C9 RID: 21705
	private int mute;

	// Token: 0x040054CA RID: 21706
	private int emptyRigCount;

	// Token: 0x040054CB RID: 21707
	public GameObject myRig;

	// Token: 0x040054CC RID: 21708
	public bool reportedCheating;

	// Token: 0x040054CD RID: 21709
	public bool reportedToxicity;

	// Token: 0x040054CE RID: 21710
	public bool reportedHateSpeech;

	// Token: 0x040054CF RID: 21711
	public bool reportInProgress;

	// Token: 0x040054D0 RID: 21712
	private string currentNickname;

	// Token: 0x040054D1 RID: 21713
	public bool doneReporting;

	// Token: 0x040054D2 RID: 21714
	public bool lastVisible = true;

	// Token: 0x040054D3 RID: 21715
	private GorillaPlayerLineButton _parentConfirmButton;

	// Token: 0x040054D4 RID: 21716
	private bool _attemptingKick;

	// Token: 0x040054D5 RID: 21717
	private bool _attemptingBan;

	// Token: 0x040054D6 RID: 21718
	public GorillaScoreBoard parentScoreboard;

	// Token: 0x040054D7 RID: 21719
	public float initTime;

	// Token: 0x040054D8 RID: 21720
	public float emptyRigCooldown = 10f;

	// Token: 0x040054D9 RID: 21721
	internal RigContainer rigContainer;
}
