using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ArcadeMachine : NetworkComponent
{
	[SerializeField]
	private CallLimiter soundCallLimit;

	private int buttonsStateValue;

	private AudioSource audioSource;

	private int audioSourcePriority;

	private ArcadeGame arcadeGameInstance;

	private IArcadeSimulator gameSimulator;

	[SerializeField]
	private float broadcastInterval;

	private float nextBroadcastTimestamp;

	protected override void Awake()
	{
		base.Awake();
		audioSource = GetComponent<AudioSource>();
		gameSimulator = GetComponent<IArcadeSimulator>();
	}

	protected override void Start()
	{
		base.Start();
		RoomSystem.JoinedRoomEvent += new Action(OnJoinedRoom);
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(OnPlayerEnteredRoom);
	}

	internal override void OnEnable()
	{
		base.OnEnable();
	}

	internal override void OnDisable()
	{
		base.OnDisable();
	}

	[PunRPC]
	private void ArcadeGame_JoinGameData_RPC(double anchorFrameTimestamp, int anchorFrame, object[] frameSerialized, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "ArcadeGame_JoinGameData_RPC");
		if (info.Sender.IsMasterClient && double.IsFinite(anchorFrameTimestamp) && frameSerialized != null)
		{
			gameSimulator.JoinGameInProgress(anchorFrameTimestamp, anchorFrame, frameSerialized);
		}
	}

	private void OnJoinedRoom()
	{
		gameSimulator.StartGame();
	}

	private void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			gameSimulator.FinalizeJoinGameData(out var anchorFrameTimestamp, out var anchorFrame, out var stateData);
			SendRPC("ArcadeGame_JoinGameData_RPC", newPlayer, anchorFrameTimestamp, anchorFrame, stateData);
		}
	}

	public void BroadcastGameState()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		gameSimulator.FinalizeJoinGameData(out var anchorFrameTimestamp, out var anchorFrame, out var stateData);
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player player in playerList)
		{
			if (!player.IsLocal)
			{
				SendRPC("ArcadeGame_JoinGameData_RPC", player, anchorFrameTimestamp, anchorFrame, stateData);
			}
		}
	}

	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	private void Update()
	{
		if (Time.time > nextBroadcastTimestamp)
		{
			bool num = nextBroadcastTimestamp != 0f && PhotonNetwork.IsMasterClient;
			nextBroadcastTimestamp = Time.time + broadcastInterval;
			if (num)
			{
				BroadcastGameState();
			}
		}
	}
}
