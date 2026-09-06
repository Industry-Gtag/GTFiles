using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000904 RID: 2308
public class FreeHoverboardManager : NetworkSceneObject
{
	// Token: 0x1700057F RID: 1407
	// (get) Token: 0x06003C86 RID: 15494 RVA: 0x0014A56B File Offset: 0x0014876B
	// (set) Token: 0x06003C87 RID: 15495 RVA: 0x0014A572 File Offset: 0x00148772
	public static FreeHoverboardManager instance { get; private set; }

	// Token: 0x06003C88 RID: 15496 RVA: 0x0014A57C File Offset: 0x0014877C
	private FreeHoverboardManager.DataPerPlayer GetOrCreatePlayerData(int actorNumber)
	{
		FreeHoverboardManager.DataPerPlayer dataPerPlayer;
		if (!this.perPlayerData.TryGetValue(actorNumber, out dataPerPlayer))
		{
			dataPerPlayer = default(FreeHoverboardManager.DataPerPlayer);
			dataPerPlayer.Init(actorNumber, this.freeBoardPool);
			this.perPlayerData.Add(actorNumber, dataPerPlayer);
		}
		return dataPerPlayer;
	}

	// Token: 0x06003C89 RID: 15497 RVA: 0x0014A5C0 File Offset: 0x001487C0
	private void Awake()
	{
		FreeHoverboardManager.instance = this;
		for (int i = 0; i < 20; i++)
		{
			FreeHoverboardInstance freeHoverboardInstance = global::UnityEngine.Object.Instantiate<FreeHoverboardInstance>(this.freeHoverboardPrefab);
			freeHoverboardInstance.gameObject.SetActive(false);
			this.freeBoardPool.Push(freeHoverboardInstance);
		}
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnLeftRoom;
	}

	// Token: 0x06003C8A RID: 15498 RVA: 0x0014A648 File Offset: 0x00148848
	private void OnPlayerLeftRoom(NetPlayer netPlayer)
	{
		FreeHoverboardManager.DataPerPlayer dataPerPlayer;
		if (this.perPlayerData.TryGetValue(netPlayer.ActorNumber, out dataPerPlayer))
		{
			dataPerPlayer.ReturnBoards(this.freeBoardPool);
			this.perPlayerData.Remove(netPlayer.ActorNumber);
		}
	}

	// Token: 0x06003C8B RID: 15499 RVA: 0x0014A68C File Offset: 0x0014888C
	private void OnLeftRoom()
	{
		foreach (KeyValuePair<int, FreeHoverboardManager.DataPerPlayer> keyValuePair in this.perPlayerData)
		{
			keyValuePair.Value.ReturnBoards(this.freeBoardPool);
		}
		this.perPlayerData.Clear();
	}

	// Token: 0x06003C8C RID: 15500 RVA: 0x0014A6F8 File Offset: 0x001488F8
	private void SpawnBoard(FreeHoverboardManager.DataPerPlayer playerData, int boardIndex, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 avelocity, Color boardColor)
	{
		FreeHoverboardInstance freeHoverboardInstance = ((boardIndex == 0) ? playerData.board0 : playerData.board1);
		freeHoverboardInstance.transform.position = position;
		freeHoverboardInstance.transform.rotation = rotation;
		freeHoverboardInstance.Rigidbody.linearVelocity = velocity;
		freeHoverboardInstance.Rigidbody.angularVelocity = avelocity;
		freeHoverboardInstance.SetColor(boardColor);
		freeHoverboardInstance.gameObject.SetActive(true);
		int ownerActorNumber = freeHoverboardInstance.ownerActorNumber;
		NetPlayer localPlayer = NetworkSystem.Instance.LocalPlayer;
		int? num = ((localPlayer != null) ? new int?(localPlayer.ActorNumber) : null);
		if ((ownerActorNumber == num.GetValueOrDefault()) & (num != null))
		{
			this.localPlayerLastSpawnedBoardIndex = boardIndex;
		}
	}

	// Token: 0x06003C8D RID: 15501 RVA: 0x0014A7A0 File Offset: 0x001489A0
	public void SendDropBoardRPC(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 avelocity, Color boardColor)
	{
		FreeHoverboardManager.DataPerPlayer orCreatePlayerData = this.GetOrCreatePlayerData(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		int num = ((!orCreatePlayerData.board0.gameObject.activeSelf) ? 0 : ((!orCreatePlayerData.board1.gameObject.activeSelf) ? 1 : (1 - this.localPlayerLastSpawnedBoardIndex)));
		if (PhotonNetwork.InRoom)
		{
			long num2 = BitPackUtils.PackWorldPosForNetwork(position);
			int num3 = BitPackUtils.PackQuaternionForNetwork(rotation);
			long num4 = BitPackUtils.PackWorldPosForNetwork(velocity);
			long num5 = BitPackUtils.PackWorldPosForNetwork(avelocity);
			short num6 = BitPackUtils.PackColorForNetwork(boardColor);
			this.photonView.RPC("DropBoard_RPC", RpcTarget.All, new object[]
			{
				num == 1,
				num2,
				num3,
				num4,
				num5,
				num6
			});
			return;
		}
		this.SpawnBoard(orCreatePlayerData, num, position, rotation, velocity, avelocity, boardColor);
	}

	// Token: 0x06003C8E RID: 15502 RVA: 0x0014A888 File Offset: 0x00148A88
	[PunRPC]
	public void DropBoard_RPC(bool boardIndex1, long positionPacked, int rotationPacked, long velocityPacked, long avelocityPacked, short colorPacked, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "DropBoard_RPC");
		int num = (boardIndex1 ? 1 : 0);
		FreeHoverboardManager.DataPerPlayer orCreatePlayerData = this.GetOrCreatePlayerData(info.Sender.ActorNumber);
		if (info.Sender != PhotonNetwork.LocalPlayer && !orCreatePlayerData.spamCheck.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(positionPacked);
		if (!rigContainer.Rig.IsPositionInRange(vector, 5f))
		{
			return;
		}
		this.SpawnBoard(orCreatePlayerData, num, vector, BitPackUtils.UnpackQuaternionFromNetwork(rotationPacked), BitPackUtils.UnpackWorldPosFromNetwork(velocityPacked), BitPackUtils.UnpackWorldPosFromNetwork(avelocityPacked), BitPackUtils.UnpackColorFromNetwork(colorPacked));
	}

	// Token: 0x06003C8F RID: 15503 RVA: 0x0014A934 File Offset: 0x00148B34
	public void SendGrabBoardRPC(FreeHoverboardInstance board)
	{
		if (PhotonNetwork.InRoom)
		{
			this.photonView.RPC("GrabBoard_RPC", RpcTarget.All, new object[]
			{
				board.ownerActorNumber,
				board.boardIndex == 1
			});
			board.gameObject.SetActive(false);
			return;
		}
		board.gameObject.SetActive(false);
	}

	// Token: 0x06003C90 RID: 15504 RVA: 0x0014A998 File Offset: 0x00148B98
	[PunRPC]
	public void GrabBoard_RPC(int ownerActorNumber, bool boardIndex1, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "GrabBoard_RPC");
		int num = (boardIndex1 ? 1 : 0);
		if (NetworkSystem.Instance.GetNetPlayerByID(ownerActorNumber) == null)
		{
			return;
		}
		FreeHoverboardManager.DataPerPlayer orCreatePlayerData = this.GetOrCreatePlayerData(ownerActorNumber);
		if (info.Sender != PhotonNetwork.LocalPlayer && !orCreatePlayerData.spamCheck.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		FreeHoverboardInstance board = orCreatePlayerData.GetBoard(num);
		if (board.IsNull())
		{
			return;
		}
		if (info.Sender.ActorNumber != ownerActorNumber)
		{
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
			{
				return;
			}
			if (!rigContainer.Rig.IsPositionInRange(board.transform.position, 5f))
			{
				return;
			}
		}
		board.gameObject.SetActive(false);
	}

	// Token: 0x06003C91 RID: 15505 RVA: 0x0014AA50 File Offset: 0x00148C50
	public void PreserveMaxHoverboardsConstraint(int actorNumber)
	{
		FreeHoverboardManager.DataPerPlayer dataPerPlayer;
		if (!this.perPlayerData.TryGetValue(actorNumber, out dataPerPlayer))
		{
			return;
		}
		if (dataPerPlayer.board0.gameObject.activeSelf && dataPerPlayer.board1.gameObject.activeSelf)
		{
			FreeHoverboardInstance board = dataPerPlayer.GetBoard(1 - this.localPlayerLastSpawnedBoardIndex);
			this.SendGrabBoardRPC(board);
		}
	}

	// Token: 0x04004D45 RID: 19781
	[SerializeField]
	private FreeHoverboardInstance freeHoverboardPrefab;

	// Token: 0x04004D46 RID: 19782
	private Stack<FreeHoverboardInstance> freeBoardPool = new Stack<FreeHoverboardInstance>(20);

	// Token: 0x04004D47 RID: 19783
	private const int NumPlayers = 10;

	// Token: 0x04004D48 RID: 19784
	private const int NumFreeBoardsPerPlayer = 2;

	// Token: 0x04004D49 RID: 19785
	private int localPlayerLastSpawnedBoardIndex;

	// Token: 0x04004D4A RID: 19786
	private Dictionary<int, FreeHoverboardManager.DataPerPlayer> perPlayerData = new Dictionary<int, FreeHoverboardManager.DataPerPlayer>();

	// Token: 0x02000905 RID: 2309
	private struct DataPerPlayer
	{
		// Token: 0x06003C93 RID: 15507 RVA: 0x0014AACC File Offset: 0x00148CCC
		public void Init(int actorNumber, Stack<FreeHoverboardInstance> freeBoardPool)
		{
			this.board0 = freeBoardPool.Pop();
			this.board0.ownerActorNumber = actorNumber;
			this.board0.boardIndex = 0;
			this.board1 = freeBoardPool.Pop();
			this.board1.ownerActorNumber = actorNumber;
			this.board1.boardIndex = 1;
			this.spamCheck = new CallLimiterWithCooldown(5f, 10, 1f);
		}

		// Token: 0x06003C94 RID: 15508 RVA: 0x0014AB38 File Offset: 0x00148D38
		public void ReturnBoards(Stack<FreeHoverboardInstance> freeBoardPool)
		{
			this.board0.gameObject.SetActive(false);
			freeBoardPool.Push(this.board0);
			this.board1.gameObject.SetActive(false);
			freeBoardPool.Push(this.board1);
		}

		// Token: 0x06003C95 RID: 15509 RVA: 0x0014AB74 File Offset: 0x00148D74
		public FreeHoverboardInstance GetBoard(int boardIndex)
		{
			if (boardIndex != 1)
			{
				return this.board0;
			}
			return this.board1;
		}

		// Token: 0x04004D4B RID: 19787
		public FreeHoverboardInstance board0;

		// Token: 0x04004D4C RID: 19788
		public FreeHoverboardInstance board1;

		// Token: 0x04004D4D RID: 19789
		public CallLimiterWithCooldown spamCheck;
	}
}
