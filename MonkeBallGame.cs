using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200060C RID: 1548
[NetworkBehaviourWeaved(0)]
public class MonkeBallGame : NetworkComponent, ITickSystemTick
{
	// Token: 0x17000403 RID: 1027
	// (get) Token: 0x0600268A RID: 9866 RVA: 0x000CC0DE File Offset: 0x000CA2DE
	public Transform BallLauncher
	{
		get
		{
			return this._ballLauncher;
		}
	}

	// Token: 0x17000404 RID: 1028
	// (get) Token: 0x0600268B RID: 9867 RVA: 0x000CC0E6 File Offset: 0x000CA2E6
	// (set) Token: 0x0600268C RID: 9868 RVA: 0x000CC0EE File Offset: 0x000CA2EE
	public bool TickRunning { get; set; }

	// Token: 0x0600268D RID: 9869 RVA: 0x000CC0F8 File Offset: 0x000CA2F8
	protected override void Awake()
	{
		base.Awake();
		MonkeBallGame.Instance = this;
		this.gameState = MonkeBallGame.GameState.None;
		this._callLimiters = new CallLimiter[8];
		this._callLimiters[0] = new CallLimiter(20, 1f, 0.5f);
		this._callLimiters[1] = new CallLimiter(20, 10f, 0.5f);
		this._callLimiters[2] = new CallLimiter(20, 10f, 0.5f);
		this._callLimiters[3] = new CallLimiter(20, 1f, 0.5f);
		this._callLimiters[4] = new CallLimiter(20, 1f, 0.5f);
		this._callLimiters[5] = new CallLimiter(20, 1f, 0.5f);
		this._callLimiters[6] = new CallLimiter(20, 1f, 0.5f);
		this._callLimiters[7] = new CallLimiter(5, 10f, 0.5f);
		this.AssignNetworkListeners();
	}

	// Token: 0x0600268E RID: 9870 RVA: 0x000CC1F4 File Offset: 0x000CA3F4
	private bool ValidateCallLimits(MonkeBallGame.RPC rpcCall, PhotonMessageInfo info)
	{
		if (rpcCall < MonkeBallGame.RPC.SetGameState || rpcCall >= MonkeBallGame.RPC.Count)
		{
			return false;
		}
		bool flag = this._callLimiters[(int)rpcCall].CheckCallTime(Time.time);
		if (!flag)
		{
			this.ReportRPCCall(rpcCall, info, "Too many RPC Calls!");
		}
		return flag;
	}

	// Token: 0x0600268F RID: 9871 RVA: 0x000CC22F File Offset: 0x000CA42F
	private void ReportRPCCall(MonkeBallGame.RPC rpcCall, PhotonMessageInfo info, string susReason)
	{
		MonkeAgent.instance.SendReport(string.Format("Reason: {0}   RPC: {1}", susReason, rpcCall), info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x06002690 RID: 9872 RVA: 0x000CC264 File Offset: 0x000CA464
	protected override void Start()
	{
		base.Start();
		for (int i = 0; i < this.startingBalls.Count; i++)
		{
			GameBallManager.Instance.AddGameBall(this.startingBalls[i].gameBall);
		}
		for (int j = 0; j < this.scoreboards.Count; j++)
		{
			this.scoreboards[j].Setup(this);
		}
		this.gameEndTime = -1.0;
	}

	// Token: 0x06002691 RID: 9873 RVA: 0x0000C135 File Offset: 0x0000A335
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06002692 RID: 9874 RVA: 0x0000C149 File Offset: 0x0000A349
	private new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06002693 RID: 9875 RVA: 0x000CC2E2 File Offset: 0x000CA4E2
	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		base.Despawned(runner, hasState);
		this.UnassignNetworkListeners();
	}

	// Token: 0x06002694 RID: 9876 RVA: 0x000CC2F4 File Offset: 0x000CA4F4
	public void OnPlayerDestroy()
	{
		if (this._setStoredLocalPlayerColor)
		{
			PlayerPrefs.SetFloat("redValue", this._storedLocalPlayerColor.r);
			PlayerPrefs.SetFloat("greenValue", this._storedLocalPlayerColor.g);
			PlayerPrefs.SetFloat("blueValue", this._storedLocalPlayerColor.b);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06002695 RID: 9877 RVA: 0x000CC350 File Offset: 0x000CA550
	private void AssignNetworkListeners()
	{
		NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerJoined;
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeft;
		NetworkSystem.Instance.OnMasterClientSwitchedEvent += this.OnMasterClientSwitched;
	}

	// Token: 0x06002696 RID: 9878 RVA: 0x000CC3C0 File Offset: 0x000CA5C0
	private void UnassignNetworkListeners()
	{
		NetworkSystem.Instance.OnPlayerJoined -= this.OnPlayerJoined;
		NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerLeft;
		NetworkSystem.Instance.OnMasterClientSwitchedEvent -= this.OnMasterClientSwitched;
	}

	// Token: 0x06002697 RID: 9879 RVA: 0x000CC430 File Offset: 0x000CA630
	public void Tick()
	{
		if (this.IsMasterClient() && this.gameState != MonkeBallGame.GameState.None && this.gameEndTime >= 0.0 && PhotonNetwork.Time > this.gameEndTime)
		{
			this.gameEndTime = -1.0;
			this.RequestGameState(MonkeBallGame.GameState.PostGame);
		}
		if (!ZoneManagement.IsInZone(GTZone.arena))
		{
			return;
		}
		this.RefreshTime();
		if (this._forceSync)
		{
			this._forceSyncDelay -= Time.deltaTime;
			if (this._forceSyncDelay <= 0f)
			{
				this._forceSync = false;
				this.ForceSyncPlayersVisuals();
				this.RefreshTeamPlayers(false);
			}
		}
		if (this._forceOrigColorFix)
		{
			this._forceOrigColorDelay -= Time.deltaTime;
			if (this._forceOrigColorDelay <= 0f)
			{
				this._forceOrigColorFix = false;
				this.ForceOriginalColorSync();
			}
		}
	}

	// Token: 0x06002698 RID: 9880 RVA: 0x000CC500 File Offset: 0x000CA700
	private void OnPlayerJoined(NetPlayer player)
	{
		this._forceSync = true;
		this._forceSyncDelay = 5f;
		if (!this.IsMasterClient())
		{
			return;
		}
		int[] array;
		int[] array2;
		int[] array3;
		long[] array4;
		long[] array5;
		this.GetCurrentGameState(out array, out array2, out array3, out array4, out array5);
		this.photonView.RPC("RequestSetGameStateRPC", player.GetPlayerRef(), new object[]
		{
			(int)this.gameState,
			this.gameEndTime,
			array,
			array2,
			array3,
			array4,
			array5
		});
	}

	// Token: 0x06002699 RID: 9881 RVA: 0x000CC588 File Offset: 0x000CA788
	private void OnPlayerLeft(NetPlayer player)
	{
		this._forceSync = true;
		this._forceSyncDelay = 5f;
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(player.ActorNumber);
		if (gamePlayer != null)
		{
			gamePlayer.CleanupPlayer();
		}
		if (!this.IsMasterClient())
		{
			return;
		}
		this.photonView.RPC("SetTeamRPC", RpcTarget.All, new object[]
		{
			-1,
			player.GetPlayerRef()
		});
	}

	// Token: 0x0600269A RID: 9882 RVA: 0x000CC5F4 File Offset: 0x000CA7F4
	private void OnMasterClientSwitched(NetPlayer player)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		int[] array;
		int[] array2;
		int[] array3;
		long[] array4;
		long[] array5;
		this.GetCurrentGameState(out array, out array2, out array3, out array4, out array5);
		this.photonView.RPC("RequestSetGameStateRPC", RpcTarget.Others, new object[]
		{
			(int)this.gameState,
			this.gameEndTime,
			array,
			array2,
			array3,
			array4,
			array5
		});
	}

	// Token: 0x0600269B RID: 9883 RVA: 0x000CC668 File Offset: 0x000CA868
	private void GetCurrentGameState(out int[] playerIds, out int[] playerTeams, out int[] scores, out long[] packedBallPosRot, out long[] packedBallVel)
	{
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		playerIds = new int[allNetPlayers.Length];
		playerTeams = new int[allNetPlayers.Length];
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			playerIds[i] = allNetPlayers[i].ActorNumber;
			GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(allNetPlayers[i].ActorNumber);
			if (gamePlayer != null)
			{
				playerTeams[i] = gamePlayer.teamId;
			}
			else
			{
				playerTeams[i] = -1;
			}
		}
		scores = new int[this.team.Count];
		for (int j = 0; j < this.team.Count; j++)
		{
			scores[j] = this.team[j].score;
		}
		packedBallPosRot = new long[this.startingBalls.Count];
		packedBallVel = new long[this.startingBalls.Count];
		for (int k = 0; k < this.startingBalls.Count; k++)
		{
			packedBallPosRot[k] = BitPackUtils.PackHandPosRotForNetwork(this.startingBalls[k].transform.position, this.startingBalls[k].transform.rotation);
			packedBallVel[k] = BitPackUtils.PackWorldPosForNetwork(this.startingBalls[k].gameBall.GetVelocity());
		}
	}

	// Token: 0x0600269C RID: 9884 RVA: 0x000962A7 File Offset: 0x000944A7
	private bool IsMasterClient()
	{
		return PhotonNetwork.IsMasterClient;
	}

	// Token: 0x0600269D RID: 9885 RVA: 0x000CC7AE File Offset: 0x000CA9AE
	public MonkeBallGame.GameState GetGameState()
	{
		return this.gameState;
	}

	// Token: 0x0600269E RID: 9886 RVA: 0x000CC7B6 File Offset: 0x000CA9B6
	public void RequestGameState(MonkeBallGame.GameState newGameState)
	{
		if (!this.IsMasterClient())
		{
			return;
		}
		this.photonView.RPC("SetGameStateRPC", RpcTarget.All, new object[] { (int)newGameState });
	}

	// Token: 0x0600269F RID: 9887 RVA: 0x000CC7E4 File Offset: 0x000CA9E4
	[PunRPC]
	private void SetGameStateRPC(int newGameState, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "SetGameStateRPC");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.SetGameState, info))
		{
			return;
		}
		if (newGameState < 0 || newGameState > 4)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetGameState, info, "newGameState outside of enum range.");
			return;
		}
		this.SetGameState((MonkeBallGame.GameState)newGameState);
		if (newGameState == 1)
		{
			this.gameEndTime = info.SentServerTime + (double)this.gameDuration;
		}
	}

	// Token: 0x060026A0 RID: 9888 RVA: 0x000CC84C File Offset: 0x000CAA4C
	private void SetGameState(MonkeBallGame.GameState newGameState)
	{
		this.gameState = newGameState;
		switch (this.gameState)
		{
		case MonkeBallGame.GameState.PreGame:
			this.OnEnterStatePreGame();
			return;
		case MonkeBallGame.GameState.Playing:
			this.OnEnterStatePlaying();
			return;
		case MonkeBallGame.GameState.PostScore:
			this.OnEnterStatePostScore();
			return;
		case MonkeBallGame.GameState.PostGame:
			this.OnEnterStatePostGame();
			return;
		default:
			return;
		}
	}

	// Token: 0x060026A1 RID: 9889 RVA: 0x000CC89C File Offset: 0x000CAA9C
	private void OnEnterStatePreGame()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].PlayGameStartFx();
		}
	}

	// Token: 0x060026A2 RID: 9890 RVA: 0x000CC8D0 File Offset: 0x000CAAD0
	private void OnEnterStatePlaying()
	{
		this._forceSync = true;
		this._forceSyncDelay = 0.1f;
	}

	// Token: 0x060026A3 RID: 9891 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnEnterStatePostScore()
	{
	}

	// Token: 0x060026A4 RID: 9892 RVA: 0x000CC8E4 File Offset: 0x000CAAE4
	private void OnEnterStatePostGame()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].PlayGameEndFx();
		}
	}

	// Token: 0x060026A5 RID: 9893 RVA: 0x000CC918 File Offset: 0x000CAB18
	[PunRPC]
	private void RequestSetGameStateRPC(int newGameState, double newGameEndTime, int[] playerIds, int[] playerTeams, int[] scores, long[] packedBallPosRot, long[] packedBallVel, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "RequestSetGameStateRPC");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.RequestSetGameState, info))
		{
			return;
		}
		if (playerIds.IsNullOrEmpty<int>() || playerTeams.IsNullOrEmpty<int>() || scores.IsNullOrEmpty<int>() || packedBallPosRot.IsNullOrEmpty<long>() || packedBallVel.IsNullOrEmpty<long>())
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "Array params are null or empty.");
			return;
		}
		if (newGameState < 0 || newGameState > 4)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "newGameState outside of enum range.");
			return;
		}
		if (playerIds.Length != playerTeams.Length)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "playerIDs and playerTeams are not the same length.");
			return;
		}
		if (scores.Length > this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "scores and team are not the same length.");
			return;
		}
		if (packedBallPosRot.Length != this.startingBalls.Count || packedBallPosRot.Length != packedBallVel.Length)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "packedBall arrays are not the same length.");
			return;
		}
		if (double.IsNaN(newGameEndTime) || double.IsInfinity(newGameEndTime))
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "newGameEndTime is not valid.");
			return;
		}
		if (newGameEndTime < -1.0 || newGameEndTime > PhotonNetwork.Time + (double)this.gameDuration)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "newGameEndTime exceeds possible time limits.");
			return;
		}
		this.gameState = (MonkeBallGame.GameState)newGameState;
		this.gameEndTime = newGameEndTime;
		for (int i = 0; i < playerIds.Length; i++)
		{
			if (VRRigCache.Instance.localRig.Creator.ActorNumber != playerIds[i])
			{
				GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(playerIds[i]);
				if (!(gamePlayer == null))
				{
					gamePlayer.teamId = playerTeams[i];
					RigContainer rigContainer;
					if (playerTeams[i] >= 0 && playerTeams[i] < this.team.Count && VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(playerIds[i]), out rigContainer))
					{
						Color color = this.team[playerTeams[i]].color;
						rigContainer.Rig.InitializeNoobMaterialLocal(color.r, color.g, color.b);
						rigContainer.Rig.LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet.EmptySet, CosmeticsController.CosmeticSet.EmptySet, false);
					}
				}
			}
		}
		this.RefreshTeamPlayers(false);
		for (int j = 0; j < scores.Length; j++)
		{
			this.SetScore(j, scores[j], false);
		}
		for (int k = 0; k < packedBallPosRot.Length; k++)
		{
			Vector3 vector;
			Quaternion quaternion;
			BitPackUtils.UnpackHandPosRotFromNetwork(packedBallPosRot[k], out vector, out quaternion);
			float num = 10000f;
			if ((in vector).IsValid(in num) && (in quaternion).IsValid())
			{
				this.startingBalls[k].transform.position = vector;
				this.startingBalls[k].transform.rotation = quaternion;
				if ((this.startingBalls[k].transform.position - base.transform.position).sqrMagnitude > 6400f)
				{
					this.startingBalls[k].transform.position = this._neutralBallStartLocation.transform.position;
				}
				Vector3 vector2 = BitPackUtils.UnpackWorldPosFromNetwork(packedBallVel[k]);
				num = 10000f;
				if ((in vector2).IsValid(in num))
				{
					this.startingBalls[k].gameBall.SetVelocity(vector2);
					this.startingBalls[k].TriggerDelayedResync();
				}
			}
		}
		this._forceSync = true;
		this._forceSyncDelay = 5f;
	}

	// Token: 0x060026A6 RID: 9894 RVA: 0x000CCC82 File Offset: 0x000CAE82
	public void RequestResetGame()
	{
		this.photonView.RPC("RequestResetGameRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060026A7 RID: 9895 RVA: 0x000CCC9C File Offset: 0x000CAE9C
	[PunRPC]
	private void RequestResetGameRPC(PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RequestResetGameRPC");
		if (!this.IsMasterClient())
		{
			return;
		}
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.RequestResetGame, info))
		{
			return;
		}
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(info.Sender.ActorNumber);
		if (gamePlayer == null)
		{
			return;
		}
		if (gamePlayer.teamId != this.resetButton.allowedTeamId)
		{
			return;
		}
		for (int i = 0; i < this.startingBalls.Count; i++)
		{
			this.RequestResetBall(this.startingBalls[i].gameBall.id, -1);
		}
		for (int j = 0; j < this.team.Count; j++)
		{
			this.RequestSetScore(j, 0);
		}
		this.RequestGameState(MonkeBallGame.GameState.PreGame);
		this.resetButton.ToggleReset(false, -1, true);
		if (this.centerResetButton != null)
		{
			this.centerResetButton.ToggleReset(false, -1, true);
		}
	}

	// Token: 0x060026A8 RID: 9896 RVA: 0x000CCD7C File Offset: 0x000CAF7C
	public void ToggleResetButton(bool toggle, int teamId)
	{
		int otherTeam = this.GetOtherTeam(teamId);
		this.photonView.RPC("SetResetButtonRPC", RpcTarget.All, new object[] { toggle, otherTeam });
	}

	// Token: 0x060026A9 RID: 9897 RVA: 0x000CCDBC File Offset: 0x000CAFBC
	[PunRPC]
	private void SetResetButtonRPC(bool toggleReset, int teamId, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "SetResetButtonRPC");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.SetResetButton, info))
		{
			return;
		}
		if (teamId < -1 || teamId >= this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetResetButton, info, "teamID exceeds possible range.");
			return;
		}
		this.resetButton.ToggleReset(toggleReset, teamId, false);
		if (this.centerResetButton != null)
		{
			this.centerResetButton.ToggleReset(toggleReset, teamId, false);
		}
	}

	// Token: 0x060026AA RID: 9898 RVA: 0x000CCE37 File Offset: 0x000CB037
	public void OnBallGrabbed(GameBallId gameBallId)
	{
		if (this.gameState == MonkeBallGame.GameState.PreGame)
		{
			this.SetGameState(MonkeBallGame.GameState.Playing);
		}
		if (this.gameState == MonkeBallGame.GameState.PostScore)
		{
			this.SetGameState(MonkeBallGame.GameState.Playing);
		}
	}

	// Token: 0x060026AB RID: 9899 RVA: 0x000CCE5C File Offset: 0x000CB05C
	private void RefreshTime()
	{
		this._frameIndex++;
		if (this._frameIndex > 2)
		{
			this._frameIndex = 0;
		}
		if (this._frameIndex != 0)
		{
			return;
		}
		float num = (float)(this.gameEndTime - PhotonNetwork.Time);
		if (this.gameEndTime < 0.0)
		{
			num = 0f;
		}
		string text = Mathf.Max(num, 0f).ToString("#00.00");
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].RefreshTime(text);
		}
	}

	// Token: 0x060026AC RID: 9900 RVA: 0x000CCEF8 File Offset: 0x000CB0F8
	public void RequestResetBall(GameBallId gameBallId, int teamId)
	{
		if (!this.IsMasterClient())
		{
			return;
		}
		if (teamId >= 0)
		{
			this.LaunchBallWithTeam(gameBallId, teamId, this.team[teamId].ballLaunchPosition, this.team[teamId].ballLaunchVelocityRange, this.team[teamId].ballLaunchAngleXRange, this.team[teamId].ballLaunchAngleXRange);
			return;
		}
		this.LaunchBallNeutral(gameBallId);
	}

	// Token: 0x060026AD RID: 9901 RVA: 0x000CCF68 File Offset: 0x000CB168
	public void RequestScore(int teamId)
	{
		if (!this.IsMasterClient())
		{
			return;
		}
		if (teamId < 0 || teamId >= this.team.Count)
		{
			return;
		}
		this.photonView.RPC("SetScoreRPC", RpcTarget.All, new object[]
		{
			teamId,
			this.team[teamId].score + 1
		});
		this.RequestGameState(MonkeBallGame.GameState.PostScore);
	}

	// Token: 0x060026AE RID: 9902 RVA: 0x000CCFD3 File Offset: 0x000CB1D3
	public void RequestSetScore(int teamId, int score)
	{
		if (!this.IsMasterClient())
		{
			return;
		}
		this.photonView.RPC("SetScoreRPC", RpcTarget.All, new object[] { teamId, score });
	}

	// Token: 0x060026AF RID: 9903 RVA: 0x000CD008 File Offset: 0x000CB208
	[PunRPC]
	private void SetScoreRPC(int teamId, int score, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "SetScoreRPC");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.SetScore, info))
		{
			return;
		}
		if (teamId < 0 || teamId >= this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetScore, info, "teamID exceeds possible range.");
			return;
		}
		if (score != 0 && score != this.team[teamId].score + 1)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetScore, info, "Score is being set to a non-achievable value.");
			return;
		}
		this.SetScore(teamId, Mathf.Clamp(score, 0, 999), true);
	}

	// Token: 0x060026B0 RID: 9904 RVA: 0x000CD094 File Offset: 0x000CB294
	private void SetScore(int teamId, int score, bool playFX = true)
	{
		if (teamId < 0 || teamId > this.team.Count)
		{
			return;
		}
		int score2 = this.team[teamId].score;
		this.team[teamId].score = score;
		if (playFX && score > score2)
		{
			this.PlayScoreFx();
			Color color = this.team[teamId].color;
			for (int i = 0; i < this.endZoneEffects.Length; i++)
			{
				this.endZoneEffects[i].startColor = color;
				this.endZoneEffects[i].Play();
			}
		}
		this.RefreshScore();
	}

	// Token: 0x060026B1 RID: 9905 RVA: 0x000CD12C File Offset: 0x000CB32C
	private void RefreshScore()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].RefreshScore();
		}
	}

	// Token: 0x060026B2 RID: 9906 RVA: 0x000CD160 File Offset: 0x000CB360
	private void PlayScoreFx()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].PlayScoreFx();
		}
	}

	// Token: 0x060026B3 RID: 9907 RVA: 0x000CD194 File Offset: 0x000CB394
	public MonkeBallTeam GetTeam(int teamId)
	{
		return this.team[teamId];
	}

	// Token: 0x060026B4 RID: 9908 RVA: 0x000CD1A2 File Offset: 0x000CB3A2
	public int GetOtherTeam(int teamId)
	{
		return (teamId + 1) % this.team.Count;
	}

	// Token: 0x060026B5 RID: 9909 RVA: 0x000CD1B4 File Offset: 0x000CB3B4
	public void RequestSetTeam(int teamId)
	{
		if (!ZoneManagement.IsInZone(GTZone.arena))
		{
			return;
		}
		this.photonView.RPC("RequestSetTeamRPC", RpcTarget.MasterClient, new object[] { teamId });
		bool flag = false;
		Color color = Color.white;
		if (teamId >= 0 && teamId < this.team.Count)
		{
			flag = true;
			if (!this._setStoredLocalPlayerColor)
			{
				this._storedLocalPlayerColor = new Color(PlayerPrefs.GetFloat("redValue", 1f), PlayerPrefs.GetFloat("greenValue", 1f), PlayerPrefs.GetFloat("blueValue", 1f));
				this._setStoredLocalPlayerColor = true;
			}
			this._forceOrigColorFix = false;
			color = this.team[teamId].color;
		}
		else
		{
			color = this._storedLocalPlayerColor;
			this._setStoredLocalPlayerColor = false;
		}
		PlayerPrefs.SetFloat("redValue", color.r);
		PlayerPrefs.SetFloat("greenValue", color.g);
		PlayerPrefs.SetFloat("blueValue", color.b);
		PlayerPrefs.Save();
		GorillaTagger.Instance.UpdateColor(color.r, color.g, color.b);
		GorillaComputer.instance.UpdateColor(color.r, color.g, color.b);
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { color.r, color.g, color.b });
			if (flag)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_HideAllCosmetics", RpcTarget.All, Array.Empty<object>());
			}
			else
			{
				this._forceOrigColorFix = true;
				this._forceOrigColorDelay = 3f;
				CosmeticsController.instance.UpdateWornCosmetics(true);
			}
			this.ForceSyncPlayersVisuals();
		}
	}

	// Token: 0x060026B6 RID: 9910 RVA: 0x000CD380 File Offset: 0x000CB580
	private MonkeBall GetMonkeBall(GameBallId gameBallId)
	{
		GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
		if (!(gameBall == null))
		{
			return gameBall.GetComponent<MonkeBall>();
		}
		return null;
	}

	// Token: 0x060026B7 RID: 9911 RVA: 0x000CD3AC File Offset: 0x000CB5AC
	[PunRPC]
	private void RequestSetTeamRPC(int teamId, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RequestSetTeamRPC");
		if (!this.IsMasterClient())
		{
			return;
		}
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.RequestSetTeam, info))
		{
			return;
		}
		if (teamId < -1 || teamId >= this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetTeam, info, "teamID exceeds possible range.");
			return;
		}
		this.photonView.RPC("SetTeamRPC", RpcTarget.All, new object[] { teamId, info.Sender });
	}

	// Token: 0x060026B8 RID: 9912 RVA: 0x000CD424 File Offset: 0x000CB624
	[PunRPC]
	private void SetTeamRPC(int teamId, Player player, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "SetTeamRPC");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.SetTeam, info))
		{
			return;
		}
		if (teamId < -1 || teamId >= this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetTeam, info, "teamID exceeds possible range.");
			return;
		}
		this.SetTeamPlayer(teamId, player);
	}

	// Token: 0x060026B9 RID: 9913 RVA: 0x000CD480 File Offset: 0x000CB680
	private void SetTeamPlayer(int teamId, Player player)
	{
		if (player == null)
		{
			return;
		}
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(player.ActorNumber);
		if (gamePlayer != null)
		{
			gamePlayer.teamId = teamId;
		}
		this.RefreshTeamPlayers(true);
	}

	// Token: 0x060026BA RID: 9914 RVA: 0x000CD4B4 File Offset: 0x000CB6B4
	private void RefreshTeamPlayers(bool playSounds)
	{
		int[] array = new int[this.team.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = 0;
		}
		int num = 0;
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int j = 0; j < allNetPlayers.Length; j++)
		{
			GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(allNetPlayers[j].ActorNumber);
			if (!(gamePlayer == null))
			{
				int teamId = gamePlayer.teamId;
				if (teamId >= 0)
				{
					array[teamId]++;
					num++;
				}
			}
		}
		for (int k = 0; k < this.scoreboards.Count; k++)
		{
			for (int l = 0; l < array.Length; l++)
			{
				this.scoreboards[k].RefreshTeamPlayers(l, array[l]);
			}
			if (playSounds)
			{
				if (this._currentPlayerTotal < num)
				{
					this.scoreboards[k].PlayPlayerJoinFx();
				}
				else if (this._currentPlayerTotal > num)
				{
					this.scoreboards[k].PlayPlayerLeaveFx();
				}
			}
		}
		this._currentPlayerTotal = num;
	}

	// Token: 0x060026BB RID: 9915 RVA: 0x000CD5C4 File Offset: 0x000CB7C4
	private void ForceSyncPlayersVisuals()
	{
		for (int i = 0; i < NetworkSystem.Instance.AllNetPlayers.Length; i++)
		{
			int actorNumber = NetworkSystem.Instance.AllNetPlayers[i].ActorNumber;
			if (VRRigCache.Instance.localRig.Creator.ActorNumber != actorNumber)
			{
				GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(actorNumber);
				RigContainer rigContainer;
				if (!(gamePlayer == null) && gamePlayer.teamId >= 0 && gamePlayer.teamId < this.team.Count && VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out rigContainer))
				{
					Color color = this.team[gamePlayer.teamId].color;
					rigContainer.Rig.InitializeNoobMaterialLocal(color.r, color.g, color.b);
					rigContainer.Rig.LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet.EmptySet, CosmeticsController.CosmeticSet.EmptySet, false);
				}
			}
		}
	}

	// Token: 0x060026BC RID: 9916 RVA: 0x000CD6B0 File Offset: 0x000CB8B0
	private void ForceOriginalColorSync()
	{
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(VRRigCache.Instance.localRig.Creator.ActorNumber);
		if (gamePlayer == null || (gamePlayer.teamId >= 0 && gamePlayer.teamId < this.team.Count))
		{
			return;
		}
		Color storedLocalPlayerColor = this._storedLocalPlayerColor;
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { storedLocalPlayerColor.r, storedLocalPlayerColor.g, storedLocalPlayerColor.b });
		}
	}

	// Token: 0x060026BD RID: 9917 RVA: 0x000CD755 File Offset: 0x000CB955
	public void RequestRestrictBallToTeam(GameBallId gameBallId, int teamId)
	{
		this.RestrictBallToTeam(gameBallId, teamId, this.restrictBallDuration);
	}

	// Token: 0x060026BE RID: 9918 RVA: 0x000CD765 File Offset: 0x000CB965
	public void RequestRestrictBallToTeamOnScore(GameBallId gameBallId, int teamId)
	{
		this.RestrictBallToTeam(gameBallId, teamId, this.restrictBallDurationAfterScore);
	}

	// Token: 0x060026BF RID: 9919 RVA: 0x000CD778 File Offset: 0x000CB978
	private void RestrictBallToTeam(GameBallId gameBallId, int teamId, float restrictDuration)
	{
		if (!this.IsMasterClient())
		{
			return;
		}
		this.photonView.RPC("SetRestrictBallToTeam", RpcTarget.All, new object[] { gameBallId.index, teamId, restrictDuration });
	}

	// Token: 0x060026C0 RID: 9920 RVA: 0x000CD7C8 File Offset: 0x000CB9C8
	[PunRPC]
	private void SetRestrictBallToTeam(int gameBallIndex, int teamId, float restrictDuration, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "SetRestrictBallToTeam");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.SetRestrictBallToTeam, info))
		{
			return;
		}
		if (gameBallIndex < 0 || gameBallIndex >= this.startingBalls.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetRestrictBallToTeam, info, "gameBallIndex exceeds possible range.");
			return;
		}
		if (teamId < -1 || teamId >= this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetRestrictBallToTeam, info, "teamID exceeds possible range.");
			return;
		}
		if (float.IsNaN(restrictDuration) || float.IsInfinity(restrictDuration) || restrictDuration < 0f || restrictDuration > this.restrictBallDurationAfterScore + this.restrictBallDuration)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetRestrictBallToTeam, info, "restrictDuration is not a feasible value.");
			return;
		}
		GameBallId gameBallId = new GameBallId(gameBallIndex);
		MonkeBall monkeBall = this.GetMonkeBall(gameBallId);
		bool flag = false;
		if (monkeBall != null)
		{
			flag = monkeBall.RestrictBallToTeam(teamId, restrictDuration);
		}
		if (flag)
		{
			for (int i = 0; i < this.shotclocks.Count; i++)
			{
				this.shotclocks[i].SetTime(teamId, restrictDuration);
			}
		}
	}

	// Token: 0x060026C1 RID: 9921 RVA: 0x000CD8C8 File Offset: 0x000CBAC8
	public void LaunchBallNeutral(GameBallId gameBallId)
	{
		this.LaunchBall(gameBallId, this._ballLauncher, this.ballLauncherVelocityRange.x, this.ballLauncherVelocityRange.y, this.ballLaunchAngleXRange.x, this.ballLaunchAngleXRange.y, this.ballLaunchAngleYRange.x, this.ballLaunchAngleYRange.y);
	}

	// Token: 0x060026C2 RID: 9922 RVA: 0x000CD924 File Offset: 0x000CBB24
	public void LaunchBallWithTeam(GameBallId gameBallId, int teamId, Transform launcher, Vector2 velocityRange, Vector2 angleXRange, Vector2 angleYRange)
	{
		this.LaunchBall(gameBallId, launcher, velocityRange.x, velocityRange.y, angleXRange.x, angleXRange.y, angleYRange.x, angleYRange.y);
	}

	// Token: 0x060026C3 RID: 9923 RVA: 0x000CD964 File Offset: 0x000CBB64
	private void LaunchBall(GameBallId gameBallId, Transform launcher, float minVelocity, float maxVelocity, float minXAngle, float maxXAngle, float minYAngle, float maxYAngle)
	{
		GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
		if (gameBall == null)
		{
			return;
		}
		gameBall.transform.position = launcher.transform.position;
		Quaternion rotation = launcher.transform.rotation;
		launcher.transform.Rotate(Vector3.up, Random.Range(minXAngle, maxXAngle));
		launcher.transform.Rotate(Vector3.right, Random.Range(minYAngle, maxYAngle));
		gameBall.transform.rotation = launcher.transform.rotation;
		Vector3 vector = launcher.transform.forward * Random.Range(minVelocity, maxVelocity);
		launcher.transform.rotation = rotation;
		GameBallManager.Instance.RequestLaunchBall(gameBallId, vector);
	}

	// Token: 0x060026C4 RID: 9924 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void WriteDataFusion()
	{
	}

	// Token: 0x060026C5 RID: 9925 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void ReadDataFusion()
	{
	}

	// Token: 0x060026C6 RID: 9926 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060026C7 RID: 9927 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060026C9 RID: 9929 RVA: 0x00002E6F File Offset: 0x0000106F
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060026CA RID: 9930 RVA: 0x00002E7B File Offset: 0x0000107B
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x0400320E RID: 12814
	public static MonkeBallGame Instance;

	// Token: 0x0400320F RID: 12815
	public List<MonkeBall> startingBalls;

	// Token: 0x04003210 RID: 12816
	public List<MonkeBallScoreboard> scoreboards;

	// Token: 0x04003211 RID: 12817
	public List<MonkeBallShotclock> shotclocks;

	// Token: 0x04003212 RID: 12818
	public List<MonkeBallGoalZone> goalZones;

	// Token: 0x04003213 RID: 12819
	[Space]
	public MonkeBallResetGame resetButton;

	// Token: 0x04003214 RID: 12820
	public MonkeBallResetGame centerResetButton;

	// Token: 0x04003215 RID: 12821
	[Space]
	public PhotonView photonView;

	// Token: 0x04003216 RID: 12822
	public List<MonkeBallTeam> team;

	// Token: 0x04003217 RID: 12823
	private int _currentPlayerTotal;

	// Token: 0x04003218 RID: 12824
	[Space]
	[Tooltip("The length of the game in seconds.")]
	public float gameDuration;

	// Token: 0x04003219 RID: 12825
	[Space]
	[Tooltip("If the ball should be reset to a team starting position after a score. If not set to true then the will reset back to a neutral starting position.")]
	public bool resetBallPositionOnScore = true;

	// Token: 0x0400321A RID: 12826
	[Tooltip("The duration in which a team is restricted from grabbing the ball after toss.")]
	public float restrictBallDuration = 5f;

	// Token: 0x0400321B RID: 12827
	[Tooltip("The duration in which a team is restricted from grabbing the ball after a score.")]
	public float restrictBallDurationAfterScore = 10f;

	// Token: 0x0400321C RID: 12828
	[Header("Neutral Launcher")]
	[SerializeField]
	private Transform _ballLauncher;

	// Token: 0x0400321D RID: 12829
	[Tooltip("The min/max random velocity of the ball when launched.")]
	public Vector2 ballLauncherVelocityRange = new Vector2(8f, 15f);

	// Token: 0x0400321E RID: 12830
	[Tooltip("The min/max random x-angle of the ball when launched.")]
	public Vector2 ballLaunchAngleXRange = new Vector2(0f, 0f);

	// Token: 0x0400321F RID: 12831
	[Tooltip("The min/max random y-angle of the ball when launched.")]
	public Vector2 ballLaunchAngleYRange = new Vector2(0f, 0f);

	// Token: 0x04003220 RID: 12832
	[Space]
	[SerializeField]
	private Transform _neutralBallStartLocation;

	// Token: 0x04003221 RID: 12833
	[SerializeField]
	private ParticleSystem[] endZoneEffects;

	// Token: 0x04003223 RID: 12835
	private MonkeBallGame.GameState gameState;

	// Token: 0x04003224 RID: 12836
	public double gameEndTime;

	// Token: 0x04003225 RID: 12837
	private int _frameIndex;

	// Token: 0x04003226 RID: 12838
	private bool _forceSync;

	// Token: 0x04003227 RID: 12839
	private float _forceSyncDelay;

	// Token: 0x04003228 RID: 12840
	private bool _forceOrigColorFix;

	// Token: 0x04003229 RID: 12841
	private float _forceOrigColorDelay;

	// Token: 0x0400322A RID: 12842
	private Color _storedLocalPlayerColor;

	// Token: 0x0400322B RID: 12843
	private bool _setStoredLocalPlayerColor;

	// Token: 0x0400322C RID: 12844
	private CallLimiter[] _callLimiters;

	// Token: 0x0200060D RID: 1549
	public enum GameState
	{
		// Token: 0x0400322E RID: 12846
		None,
		// Token: 0x0400322F RID: 12847
		PreGame,
		// Token: 0x04003230 RID: 12848
		Playing,
		// Token: 0x04003231 RID: 12849
		PostScore,
		// Token: 0x04003232 RID: 12850
		PostGame
	}

	// Token: 0x0200060E RID: 1550
	private enum RPC
	{
		// Token: 0x04003234 RID: 12852
		SetGameState,
		// Token: 0x04003235 RID: 12853
		RequestSetGameState,
		// Token: 0x04003236 RID: 12854
		RequestResetGame,
		// Token: 0x04003237 RID: 12855
		SetScore,
		// Token: 0x04003238 RID: 12856
		RequestSetTeam,
		// Token: 0x04003239 RID: 12857
		SetTeam,
		// Token: 0x0400323A RID: 12858
		SetRestrictBallToTeam,
		// Token: 0x0400323B RID: 12859
		SetResetButton,
		// Token: 0x0400323C RID: 12860
		Count
	}
}
