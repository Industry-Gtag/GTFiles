using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000FA4 RID: 4004
	public class PlayerTimerManager : MonoBehaviourPunCallbacks
	{
		// Token: 0x060063B2 RID: 25522 RVA: 0x00200E5C File Offset: 0x001FF05C
		private void Awake()
		{
			if (PlayerTimerManager.instance == null)
			{
				PlayerTimerManager.instance = this;
			}
			else if (PlayerTimerManager.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.callLimiters = new CallLimiter[2];
			this.callLimiters[0] = new CallLimiter(10, 1f, 0.5f);
			this.callLimiters[1] = new CallLimiter(30, 1f, 0.5f);
			this.playerTimerData = new Dictionary<int, PlayerTimerManager.PlayerTimerData>(10);
			this.timerToggleLimiters = new Dictionary<int, CallLimiter>(10);
			this.limiterPool = new List<CallLimiter>(10);
			this.serializedTimerData = new byte[256];
		}

		// Token: 0x060063B3 RID: 25523 RVA: 0x00200F0C File Offset: 0x001FF10C
		private CallLimiter CreateLimiterFromPool()
		{
			if (this.limiterPool.Count > 0)
			{
				CallLimiter callLimiter = this.limiterPool[this.limiterPool.Count - 1];
				this.limiterPool.RemoveAt(this.limiterPool.Count - 1);
				return callLimiter;
			}
			return new CallLimiter(5, 1f, 0.5f);
		}

		// Token: 0x060063B4 RID: 25524 RVA: 0x00200F68 File Offset: 0x001FF168
		private void ReturnCallLimiterToPool(CallLimiter limiter)
		{
			if (limiter == null)
			{
				return;
			}
			limiter.Reset();
			this.limiterPool.Add(limiter);
		}

		// Token: 0x060063B5 RID: 25525 RVA: 0x00200F80 File Offset: 0x001FF180
		public void RegisterTimerBoard(PlayerTimerBoard board)
		{
			if (!PlayerTimerManager.timerBoards.Contains(board))
			{
				PlayerTimerManager.timerBoards.Add(board);
				this.UpdateTimerBoard(board);
			}
		}

		// Token: 0x060063B6 RID: 25526 RVA: 0x00200FA1 File Offset: 0x001FF1A1
		public void UnregisterTimerBoard(PlayerTimerBoard board)
		{
			PlayerTimerManager.timerBoards.Remove(board);
		}

		// Token: 0x060063B7 RID: 25527 RVA: 0x00200FB0 File Offset: 0x001FF1B0
		public bool IsLocalTimerStarted()
		{
			PlayerTimerManager.PlayerTimerData playerTimerData;
			return this.playerTimerData.TryGetValue(NetworkSystem.Instance.LocalPlayer.ActorNumber, out playerTimerData) && playerTimerData.isStarted;
		}

		// Token: 0x060063B8 RID: 25528 RVA: 0x00200FE4 File Offset: 0x001FF1E4
		public float GetTimeForPlayer(int actorNumber)
		{
			PlayerTimerManager.PlayerTimerData playerTimerData;
			if (!this.playerTimerData.TryGetValue(actorNumber, out playerTimerData))
			{
				return 0f;
			}
			if (playerTimerData.isStarted)
			{
				return Mathf.Clamp((PhotonNetwork.ServerTimestamp - playerTimerData.startTimeStamp) / 1000f, 0f, 3599.99f);
			}
			return Mathf.Clamp(playerTimerData.lastTimerDuration / 1000f, 0f, 3599.99f);
		}

		// Token: 0x060063B9 RID: 25529 RVA: 0x00201050 File Offset: 0x001FF250
		public float GetLastDurationForPlayer(int actorNumber)
		{
			PlayerTimerManager.PlayerTimerData playerTimerData;
			if (this.playerTimerData.TryGetValue(actorNumber, out playerTimerData))
			{
				return Mathf.Clamp(playerTimerData.lastTimerDuration / 1000f, 0f, 3599.99f);
			}
			return -1f;
		}

		// Token: 0x060063BA RID: 25530 RVA: 0x00201090 File Offset: 0x001FF290
		[PunRPC]
		private void InitTimersMasterRPC(int numBytes, byte[] bytes, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "InitTimersMasterRPC");
			if (!this.ValidateCallLimits(PlayerTimerManager.RPC.InitTimersMaster, info))
			{
				return;
			}
			if (this.areTimersInitialized)
			{
				return;
			}
			this.DeserializeTimerState(bytes.Length, bytes);
			this.areTimersInitialized = true;
			this.UpdateAllTimerBoards();
		}

		// Token: 0x060063BB RID: 25531 RVA: 0x002010E4 File Offset: 0x001FF2E4
		private int SerializeTimerState()
		{
			Array.Clear(this.serializedTimerData, 0, this.serializedTimerData.Length);
			MemoryStream memoryStream = new MemoryStream(this.serializedTimerData);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			if (this.playerTimerData.Count > 10)
			{
				this.ClearOldPlayerData();
			}
			binaryWriter.Write(this.playerTimerData.Count);
			foreach (KeyValuePair<int, PlayerTimerManager.PlayerTimerData> keyValuePair in this.playerTimerData)
			{
				binaryWriter.Write(keyValuePair.Key);
				binaryWriter.Write(keyValuePair.Value.startTimeStamp);
				binaryWriter.Write(keyValuePair.Value.endTimeStamp);
				binaryWriter.Write(keyValuePair.Value.isStarted ? 1 : 0);
				binaryWriter.Write(keyValuePair.Value.lastTimerDuration);
			}
			return (int)memoryStream.Position;
		}

		// Token: 0x060063BC RID: 25532 RVA: 0x002011E0 File Offset: 0x001FF3E0
		private void DeserializeTimerState(int numBytes, byte[] bytes)
		{
			if (numBytes <= 0 || numBytes > 256)
			{
				return;
			}
			if (bytes == null || bytes.Length < numBytes)
			{
				return;
			}
			MemoryStream memoryStream = new MemoryStream(bytes);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			this.playerTimerData.Clear();
			try
			{
				List<Player> list = PhotonNetwork.PlayerList.ToList<Player>();
				if (bytes.Length < 4)
				{
					this.playerTimerData.Clear();
					return;
				}
				int num = binaryReader.ReadInt32();
				if (num < 0 || num > 10)
				{
					this.playerTimerData.Clear();
					return;
				}
				int num2 = 17;
				if (memoryStream.Position + (long)(num2 * num) > (long)bytes.Length)
				{
					this.playerTimerData.Clear();
					return;
				}
				for (int i = 0; i < num; i++)
				{
					int actorNum = binaryReader.ReadInt32();
					int num3 = binaryReader.ReadInt32();
					int num4 = binaryReader.ReadInt32();
					bool flag = binaryReader.ReadByte() > 0;
					uint num5 = binaryReader.ReadUInt32();
					if (list.FindIndex((Player x) => x.ActorNumber == actorNum) >= 0)
					{
						PlayerTimerManager.PlayerTimerData playerTimerData = new PlayerTimerManager.PlayerTimerData
						{
							startTimeStamp = num3,
							endTimeStamp = num4,
							isStarted = flag,
							lastTimerDuration = num5
						};
						this.playerTimerData.TryAdd(actorNum, playerTimerData);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				this.playerTimerData.Clear();
			}
			if (Time.time - this.requestSendTime < 5f && this.IsLocalTimerStarted() != this.localPlayerRequestedStart)
			{
				this.timerPV.RPC("RequestTimerToggleRPC", RpcTarget.MasterClient, new object[] { this.localPlayerRequestedStart });
			}
		}

		// Token: 0x060063BD RID: 25533 RVA: 0x002013A4 File Offset: 0x001FF5A4
		private void ClearOldPlayerData()
		{
			List<int> list = new List<int>(this.playerTimerData.Count);
			List<Player> list2 = PhotonNetwork.PlayerList.ToList<Player>();
			using (Dictionary<int, PlayerTimerManager.PlayerTimerData>.KeyCollection.Enumerator enumerator = this.playerTimerData.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int actorNum = enumerator.Current;
					if (list2.FindIndex((Player x) => x.ActorNumber == actorNum) < 0)
					{
						list.Add(actorNum);
					}
				}
			}
			foreach (int num in list)
			{
				this.playerTimerData.Remove(num);
			}
		}

		// Token: 0x060063BE RID: 25534 RVA: 0x00201484 File Offset: 0x001FF684
		public void RequestTimerToggle(bool startTimer)
		{
			this.requestSendTime = Time.time;
			this.localPlayerRequestedStart = startTimer;
			this.timerPV.RPC("RequestTimerToggleRPC", RpcTarget.MasterClient, new object[] { startTimer });
		}

		// Token: 0x060063BF RID: 25535 RVA: 0x002014B8 File Offset: 0x001FF6B8
		[PunRPC]
		private void RequestTimerToggleRPC(bool startTimer, PhotonMessageInfo info)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "RequestTimerToggleRPC");
			CallLimiter callLimiter;
			if (this.timerToggleLimiters.TryGetValue(info.Sender.ActorNumber, out callLimiter))
			{
				if (!callLimiter.CheckCallTime(Time.time))
				{
					return;
				}
			}
			else
			{
				CallLimiter callLimiter2 = this.CreateLimiterFromPool();
				this.timerToggleLimiters.Add(info.Sender.ActorNumber, callLimiter2);
				callLimiter2.CheckCallTime(Time.time);
			}
			if (info.Sender == null)
			{
				return;
			}
			PlayerTimerManager.PlayerTimerData playerTimerData;
			bool flag = this.playerTimerData.TryGetValue(info.Sender.ActorNumber, out playerTimerData);
			if (!startTimer && !flag)
			{
				return;
			}
			if (flag && !startTimer && !playerTimerData.isStarted)
			{
				return;
			}
			int num = info.SentServerTimestamp;
			if (PhotonNetwork.ServerTimestamp - num > PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout)
			{
				num = PhotonNetwork.ServerTimestamp - PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout;
			}
			this.timerPV.RPC("TimerToggledMasterRPC", RpcTarget.All, new object[] { startTimer, num, info.Sender });
		}

		// Token: 0x060063C0 RID: 25536 RVA: 0x002015D0 File Offset: 0x001FF7D0
		[PunRPC]
		private void TimerToggledMasterRPC(bool startTimer, int toggleTimeStamp, Player player, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "TimerToggledMasterRPC");
			if (!this.ValidateCallLimits(PlayerTimerManager.RPC.ToggleTimerMaster, info))
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			if (!this.areTimersInitialized)
			{
				return;
			}
			int num = toggleTimeStamp;
			int num2 = info.SentServerTimestamp;
			if (PhotonNetwork.ServerTimestamp - num2 > PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout)
			{
				num2 = PhotonNetwork.ServerTimestamp - PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout;
			}
			if (num2 - num > PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout)
			{
				num = num2 - PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout;
			}
			this.OnToggleTimerForPlayer(startTimer, player, num);
		}

		// Token: 0x060063C1 RID: 25537 RVA: 0x00201678 File Offset: 0x001FF878
		private void OnToggleTimerForPlayer(bool startTimer, Player player, int toggleTime)
		{
			PlayerTimerManager.PlayerTimerData playerTimerData;
			if (this.playerTimerData.TryGetValue(player.ActorNumber, out playerTimerData))
			{
				if (startTimer && !playerTimerData.isStarted)
				{
					playerTimerData.startTimeStamp = toggleTime;
					playerTimerData.isStarted = true;
					UnityEvent<int> onTimerStartedForPlayer = this.OnTimerStartedForPlayer;
					if (onTimerStartedForPlayer != null)
					{
						onTimerStartedForPlayer.Invoke(player.ActorNumber);
					}
					if (player.IsLocal)
					{
						UnityEvent onLocalTimerStarted = this.OnLocalTimerStarted;
						if (onLocalTimerStarted != null)
						{
							onLocalTimerStarted.Invoke();
						}
					}
				}
				else if (!startTimer && playerTimerData.isStarted)
				{
					playerTimerData.endTimeStamp = toggleTime;
					playerTimerData.isStarted = false;
					playerTimerData.lastTimerDuration = (uint)(playerTimerData.endTimeStamp - playerTimerData.startTimeStamp);
					UnityEvent<int, int> onTimerStopped = this.OnTimerStopped;
					if (onTimerStopped != null)
					{
						onTimerStopped.Invoke(player.ActorNumber, playerTimerData.endTimeStamp - playerTimerData.startTimeStamp);
					}
				}
				this.playerTimerData[player.ActorNumber] = playerTimerData;
			}
			else
			{
				PlayerTimerManager.PlayerTimerData playerTimerData2 = new PlayerTimerManager.PlayerTimerData
				{
					startTimeStamp = (startTimer ? toggleTime : 0),
					endTimeStamp = (startTimer ? 0 : toggleTime),
					isStarted = startTimer,
					lastTimerDuration = 0U
				};
				this.playerTimerData.TryAdd(player.ActorNumber, playerTimerData2);
				UnityEvent<int> onTimerStartedForPlayer2 = this.OnTimerStartedForPlayer;
				if (onTimerStartedForPlayer2 != null)
				{
					onTimerStartedForPlayer2.Invoke(player.ActorNumber);
				}
				if (player.IsLocal)
				{
					UnityEvent onLocalTimerStarted2 = this.OnLocalTimerStarted;
					if (onLocalTimerStarted2 != null)
					{
						onLocalTimerStarted2.Invoke();
					}
				}
			}
			this.UpdateAllTimerBoards();
		}

		// Token: 0x060063C2 RID: 25538 RVA: 0x002017D0 File Offset: 0x001FF9D0
		private bool ValidateCallLimits(PlayerTimerManager.RPC rpcCall, PhotonMessageInfo info)
		{
			return rpcCall >= PlayerTimerManager.RPC.InitTimersMaster && rpcCall < PlayerTimerManager.RPC.Count && this.callLimiters[(int)rpcCall].CheckCallTime(Time.time);
		}

		// Token: 0x060063C3 RID: 25539 RVA: 0x00201800 File Offset: 0x001FFA00
		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			base.OnMasterClientSwitched(newMasterClient);
			if (newMasterClient.IsLocal)
			{
				int num = this.SerializeTimerState();
				this.timerPV.RPC("InitTimersMasterRPC", RpcTarget.Others, new object[] { num, this.serializedTimerData });
				return;
			}
			this.playerTimerData.Clear();
			this.areTimersInitialized = false;
		}

		// Token: 0x060063C4 RID: 25540 RVA: 0x00201860 File Offset: 0x001FFA60
		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			base.OnPlayerEnteredRoom(newPlayer);
			if (PhotonNetwork.IsMasterClient && !newPlayer.IsLocal)
			{
				int num = this.SerializeTimerState();
				this.timerPV.RPC("InitTimersMasterRPC", newPlayer, new object[] { num, this.serializedTimerData });
			}
			this.UpdateAllTimerBoards();
		}

		// Token: 0x060063C5 RID: 25541 RVA: 0x002018BC File Offset: 0x001FFABC
		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			base.OnPlayerLeftRoom(otherPlayer);
			this.playerTimerData.Remove(otherPlayer.ActorNumber);
			CallLimiter callLimiter;
			if (this.timerToggleLimiters.TryGetValue(otherPlayer.ActorNumber, out callLimiter))
			{
				this.ReturnCallLimiterToPool(callLimiter);
				this.timerToggleLimiters.Remove(otherPlayer.ActorNumber);
			}
			this.UpdateAllTimerBoards();
		}

		// Token: 0x060063C6 RID: 25542 RVA: 0x00201918 File Offset: 0x001FFB18
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
			this.joinedRoom = true;
			if (PhotonNetwork.IsMasterClient)
			{
				this.playerTimerData.Clear();
				foreach (CallLimiter callLimiter in this.timerToggleLimiters.Values)
				{
					this.ReturnCallLimiterToPool(callLimiter);
				}
				this.timerToggleLimiters.Clear();
				this.areTimersInitialized = true;
				this.UpdateAllTimerBoards();
				return;
			}
			this.requestSendTime = 0f;
			this.areTimersInitialized = false;
		}

		// Token: 0x060063C7 RID: 25543 RVA: 0x002019BC File Offset: 0x001FFBBC
		public override void OnLeftRoom()
		{
			base.OnLeftRoom();
			this.joinedRoom = false;
			this.playerTimerData.Clear();
			foreach (CallLimiter callLimiter in this.timerToggleLimiters.Values)
			{
				this.ReturnCallLimiterToPool(callLimiter);
			}
			this.timerToggleLimiters.Clear();
			this.areTimersInitialized = false;
			this.requestSendTime = 0f;
			this.localPlayerRequestedStart = false;
			this.UpdateAllTimerBoards();
		}

		// Token: 0x060063C8 RID: 25544 RVA: 0x00201A58 File Offset: 0x001FFC58
		private void UpdateAllTimerBoards()
		{
			foreach (PlayerTimerBoard playerTimerBoard in PlayerTimerManager.timerBoards)
			{
				this.UpdateTimerBoard(playerTimerBoard);
			}
		}

		// Token: 0x060063C9 RID: 25545 RVA: 0x00201AAC File Offset: 0x001FFCAC
		private void UpdateTimerBoard(PlayerTimerBoard board)
		{
			board.SetSleepState(this.joinedRoom);
			if (GorillaComputer.instance == null)
			{
				return;
			}
			if (!this.joinedRoom)
			{
				if (board.notInRoomText != null)
				{
					board.notInRoomText.gameObject.SetActive(true);
					board.notInRoomText.text = GorillaComputer.instance.offlineTextInitialString;
				}
				for (int i = 0; i < board.lines.Count; i++)
				{
					board.lines[i].ResetData();
				}
				return;
			}
			if (board.notInRoomText != null)
			{
				board.notInRoomText.gameObject.SetActive(false);
			}
			for (int j = 0; j < board.lines.Count; j++)
			{
				PlayerTimerBoardLine playerTimerBoardLine = board.lines[j];
				if (j < PhotonNetwork.PlayerList.Length)
				{
					playerTimerBoardLine.gameObject.SetActive(true);
					playerTimerBoardLine.SetLineData(NetworkSystem.Instance.GetPlayer(PhotonNetwork.PlayerList[j]));
					playerTimerBoardLine.UpdateLine();
				}
				else
				{
					playerTimerBoardLine.ResetData();
					playerTimerBoardLine.gameObject.SetActive(false);
				}
			}
			board.RedrawPlayerLines();
		}

		// Token: 0x04007264 RID: 29284
		public static PlayerTimerManager instance;

		// Token: 0x04007265 RID: 29285
		public PhotonView timerPV;

		// Token: 0x04007266 RID: 29286
		public UnityEvent OnLocalTimerStarted;

		// Token: 0x04007267 RID: 29287
		public UnityEvent<int> OnTimerStartedForPlayer;

		// Token: 0x04007268 RID: 29288
		public UnityEvent<int, int> OnTimerStopped;

		// Token: 0x04007269 RID: 29289
		public const float MAX_DURATION_SECONDS = 3599.99f;

		// Token: 0x0400726A RID: 29290
		private float requestSendTime;

		// Token: 0x0400726B RID: 29291
		private bool localPlayerRequestedStart;

		// Token: 0x0400726C RID: 29292
		private CallLimiter[] callLimiters;

		// Token: 0x0400726D RID: 29293
		private Dictionary<int, CallLimiter> timerToggleLimiters;

		// Token: 0x0400726E RID: 29294
		private List<CallLimiter> limiterPool;

		// Token: 0x0400726F RID: 29295
		private bool areTimersInitialized;

		// Token: 0x04007270 RID: 29296
		private Dictionary<int, PlayerTimerManager.PlayerTimerData> playerTimerData;

		// Token: 0x04007271 RID: 29297
		private const int MAX_TIMER_INIT_BYTES = 256;

		// Token: 0x04007272 RID: 29298
		private byte[] serializedTimerData;

		// Token: 0x04007273 RID: 29299
		private static List<PlayerTimerBoard> timerBoards = new List<PlayerTimerBoard>(10);

		// Token: 0x04007274 RID: 29300
		private bool joinedRoom;

		// Token: 0x02000FA5 RID: 4005
		private enum RPC
		{
			// Token: 0x04007276 RID: 29302
			InitTimersMaster,
			// Token: 0x04007277 RID: 29303
			ToggleTimerMaster,
			// Token: 0x04007278 RID: 29304
			Count
		}

		// Token: 0x02000FA6 RID: 4006
		public struct PlayerTimerData
		{
			// Token: 0x04007279 RID: 29305
			public int startTimeStamp;

			// Token: 0x0400727A RID: 29306
			public int endTimeStamp;

			// Token: 0x0400727B RID: 29307
			public bool isStarted;

			// Token: 0x0400727C RID: 29308
			public uint lastTimerDuration;
		}
	}
}
