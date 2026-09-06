using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Fusion;
using GorillaExtensions;
using K4os.Compression.LZ4;
using Photon.Pun;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

namespace Voxels
{
	// Token: 0x020013BE RID: 5054
	[NetworkBehaviourWeaved(0)]
	public class VoxelManager : NetworkComponent
	{
		// Token: 0x17000C4F RID: 3151
		// (get) Token: 0x06007E3D RID: 32317 RVA: 0x00295D7E File Offset: 0x00293F7E
		public static bool HasAuthority
		{
			get
			{
				return !VoxelManager.InRoom || VoxelManager._instance.IsLocallyOwned;
			}
		}

		// Token: 0x17000C50 RID: 3152
		// (get) Token: 0x06007E3E RID: 32318 RVA: 0x00295D93 File Offset: 0x00293F93
		public static bool InRoom
		{
			get
			{
				return NetworkSystem.Instance.InRoom;
			}
		}

		// Token: 0x06007E3F RID: 32319 RVA: 0x00295D9F File Offset: 0x00293F9F
		protected override void Start()
		{
			base.Start();
			VoxelManager._instance = this;
		}

		// Token: 0x06007E40 RID: 32320 RVA: 0x00295DB0 File Offset: 0x00293FB0
		internal override void OnEnable()
		{
			NetworkBehaviourUtils.InternalOnEnable(this);
			base.OnEnable();
			RoomSystem.JoinedRoomEvent += new Action(this.OnNetworkJoinedRoom);
			RoomSystem.LeftRoomEvent += new Action(this.OnNetworkLeftRoom);
			NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeft;
			Application.lowMemory += this.OnLowMemory;
		}

		// Token: 0x06007E41 RID: 32321 RVA: 0x00295E34 File Offset: 0x00294034
		internal override void OnDisable()
		{
			NetworkBehaviourUtils.InternalOnDisable(this);
			base.OnDisable();
			RoomSystem.JoinedRoomEvent -= new Action(this.OnNetworkJoinedRoom);
			RoomSystem.LeftRoomEvent -= new Action(this.OnNetworkLeftRoom);
			NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerLeft;
			Application.lowMemory -= this.OnLowMemory;
		}

		// Token: 0x06007E42 RID: 32322 RVA: 0x00295EB8 File Offset: 0x002940B8
		private void Update()
		{
			if (VoxelManager._processInitQueus)
			{
				VoxelManager._processInitQueus = false;
				if (!PhotonNetwork.IsMasterClient)
				{
					VoxelManager._initQueues.Clear();
					return;
				}
				if (VoxelManager._worlds.Count == 0)
				{
					return;
				}
				VoxelManager.UpdateTransferLog();
				foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
				{
					VoxelManager.StateInitQueue stateInitQueue;
					if (!netPlayer.IsLocal && VoxelManager._initQueues.TryGetValue(netPlayer.ActorNumber, out stateInitQueue) && !stateInitQueue.IsEmpty)
					{
						if (VoxelManager._sendRate < 10000)
						{
							VoxelManager.SendNextChunk(stateInitQueue);
						}
						if (!stateInitQueue.IsEmpty)
						{
							VoxelManager._processInitQueus = true;
						}
					}
				}
			}
			if (VoxelManager._mineCommandsQueued && Time.realtimeSinceStartup >= VoxelManager._nextMineCommandTime)
			{
				if (!PhotonNetwork.IsMasterClient)
				{
					VoxelManager._mineCommandsQueued = false;
					VoxelManager._mineOpQueues.Clear();
				}
				VoxelManager.SendQueuedMineCommands();
			}
		}

		// Token: 0x06007E43 RID: 32323 RVA: 0x00295FAC File Offset: 0x002941AC
		private void LateUpdate()
		{
			if (VoxelManager._localOperationQueue.Count > 0 && Time.realtimeSinceStartup >= VoxelManager._nextLocalOpTime)
			{
				VoxelManager.ExecuteQueuedLocalOperations();
			}
		}

		// Token: 0x06007E44 RID: 32324 RVA: 0x00295FCC File Offset: 0x002941CC
		private void OnLowMemory()
		{
			long num = 1048576L;
			long num2 = Profiler.GetMonoUsedSizeLong() / num;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			long num3 = Profiler.GetMonoUsedSizeLong() / num;
			PersistLog.Log(string.Format("Performing Memory Cleanup.  Used Memory: {0}M->{1}M ({2}M)", num2, num3, num3 - num2));
		}

		// Token: 0x06007E45 RID: 32325 RVA: 0x00296020 File Offset: 0x00294220
		private static void UpdateTransferLog()
		{
			float num = Time.realtimeSinceStartup - 1f;
			while (VoxelManager._sendHistory.Count > 0 && VoxelManager._sendHistory.Peek().Item1 <= num)
			{
				int item = VoxelManager._sendHistory.Dequeue().Item2;
				VoxelManager._sendRate -= item;
			}
		}

		// Token: 0x06007E46 RID: 32326 RVA: 0x00296076 File Offset: 0x00294276
		private static void EnqueueTransferLog(int bytes)
		{
			VoxelManager._sendHistory.Enqueue(new ValueTuple<float, int>(Time.realtimeSinceStartup, bytes));
			VoxelManager._sendRate += bytes;
		}

		// Token: 0x06007E47 RID: 32327 RVA: 0x0029609C File Offset: 0x0029429C
		public static int[] GetIntArray(int length)
		{
			int[] staticArray = VoxelManager._intArrayBag.GetStaticArray(length);
			for (int i = 0; i < length; i++)
			{
				staticArray[i] = 0;
			}
			return staticArray;
		}

		// Token: 0x06007E48 RID: 32328 RVA: 0x002960C8 File Offset: 0x002942C8
		public static void Register(VoxelWorld world)
		{
			VoxelManager._worlds[world.Id] = world;
			Debug.Log(string.Format("Registered voxel world {0}[{1}]", world.name, world.Id), world);
			if (!VoxelManager.HasAuthority)
			{
				VoxelManager.SendWorldStateRequest(world.Id);
			}
		}

		// Token: 0x06007E49 RID: 32329 RVA: 0x00296119 File Offset: 0x00294319
		public static void Unregister(VoxelWorld world)
		{
			VoxelManager._worlds.Remove(world.Id);
			Debug.Log(string.Format("Unregistered voxel world {0}[{1}]", world.name, world.Id), world);
		}

		// Token: 0x06007E4A RID: 32330 RVA: 0x00296150 File Offset: 0x00294350
		public static void ReplicateState(VoxelWorld world)
		{
			if (!VoxelManager.InRoom || !VoxelManager.HasAuthority)
			{
				return;
			}
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				if (!netPlayer.IsLocal)
				{
					VoxelManager.SendWorldStateToPlayer(world, netPlayer);
				}
			}
		}

		// Token: 0x06007E4B RID: 32331 RVA: 0x002961BC File Offset: 0x002943BC
		private void OnNetworkJoinedRoom()
		{
			if (!VoxelManager.HasAuthority)
			{
				VoxelManager.RequestVoxelWorldStates();
			}
		}

		// Token: 0x06007E4C RID: 32332 RVA: 0x002961CA File Offset: 0x002943CA
		private void OnNetworkLeftRoom()
		{
			VoxelManager._spamChecks.Clear();
		}

		// Token: 0x06007E4D RID: 32333 RVA: 0x002961D6 File Offset: 0x002943D6
		private void OnPlayerLeft(NetPlayer player)
		{
			if (!VoxelManager.HasAuthority || !VoxelManager.InRoom)
			{
				return;
			}
			VoxelManager.ClearQueuesForPlayer(player);
			VoxelManager._spamChecks.Remove(player.ActorNumber);
		}

		// Token: 0x06007E4E RID: 32334 RVA: 0x002961FE File Offset: 0x002943FE
		protected override void OnOwnerSwitched(NetPlayer newOwningPlayer)
		{
			this._owner = newOwningPlayer;
			VoxelManager._mineOpQueues.Clear();
			VoxelManager._localOperationQueue.Clear();
			if (!VoxelManager.HasAuthority)
			{
				VoxelManager.RequestVoxelWorldStates();
			}
		}

		// Token: 0x06007E4F RID: 32335 RVA: 0x00002C2D File Offset: 0x00000E2D
		public override void WriteDataFusion()
		{
		}

		// Token: 0x06007E50 RID: 32336 RVA: 0x00002C2D File Offset: 0x00000E2D
		public override void ReadDataFusion()
		{
		}

		// Token: 0x06007E51 RID: 32337 RVA: 0x00002C2D File Offset: 0x00000E2D
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
		}

		// Token: 0x06007E52 RID: 32338 RVA: 0x00002C2D File Offset: 0x00000E2D
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
		}

		// Token: 0x06007E53 RID: 32339 RVA: 0x00296228 File Offset: 0x00294428
		private static void RequestVoxelWorldStates()
		{
			foreach (int num in VoxelManager._worlds.Keys)
			{
				VoxelManager.SendWorldStateRequest(num);
			}
		}

		// Token: 0x06007E54 RID: 32340 RVA: 0x0029627C File Offset: 0x0029447C
		public static void RequestWorldState(VoxelWorld world)
		{
			VoxelManager.SendWorldStateRequest(world.Id);
		}

		// Token: 0x06007E55 RID: 32341 RVA: 0x0029628C File Offset: 0x0029448C
		private static void OnWorldStateRequestReceived(int worldId, PhotonMessageInfoWrapped info)
		{
			VoxelWorld voxelWorld;
			if (VoxelManager._worlds.TryGetValue(worldId, out voxelWorld))
			{
				VoxelManager.SendWorldStateToPlayer(voxelWorld, info.Sender);
			}
		}

		// Token: 0x06007E56 RID: 32342 RVA: 0x002962B4 File Offset: 0x002944B4
		private static void SendWorldStateToPlayer(VoxelWorld world, NetPlayer player)
		{
			VoxelManager.StateInitQueue stateInitQueue;
			if (VoxelManager._initQueues.TryGetValue(player.ActorNumber, out stateInitQueue))
			{
				for (int i = 0; i < stateInitQueue.chunks.Count; i++)
				{
					if (stateInitQueue.chunks[i].worldId == world.Id)
					{
						stateInitQueue.chunks.RemoveAt(i--);
					}
				}
			}
			foreach (Chunk chunk in world.Chunks)
			{
				VoxelManager.QueueChunkForPlayer(chunk, player);
			}
		}

		// Token: 0x06007E57 RID: 32343 RVA: 0x00296354 File Offset: 0x00294554
		private static bool WorldIsQueuedForPlayer(VoxelWorld world, NetPlayer player)
		{
			VoxelManager.StateInitQueue stateInitQueue;
			if (!VoxelManager._initQueues.TryGetValue(player.ActorNumber, out stateInitQueue))
			{
				return false;
			}
			using (List<VoxelManager.ChunkInitState>.Enumerator enumerator = stateInitQueue.chunks.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.worldId == world.Id)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06007E58 RID: 32344 RVA: 0x002963CC File Offset: 0x002945CC
		private static void ClearQueuesForPlayer(NetPlayer player)
		{
			VoxelManager.StateInitQueue stateInitQueue;
			if (VoxelManager._initQueues.TryGetValue(player.ActorNumber, out stateInitQueue))
			{
				stateInitQueue.chunks.Clear();
				stateInitQueue.mineOps.Clear();
				stateInitQueue.operations.Clear();
			}
			List<VoxelManager.VoxelMineOperation> list;
			if (VoxelManager._mineOpQueues.TryGetValue(player.ActorNumber, out list))
			{
				list.Clear();
			}
		}

		// Token: 0x06007E59 RID: 32345 RVA: 0x00296428 File Offset: 0x00294628
		private static VoxelManager.StateInitQueue GetOrCreateQueueForPlayer(NetPlayer player)
		{
			VoxelManager.StateInitQueue stateInitQueue;
			if (!VoxelManager._initQueues.TryGetValue(player.ActorNumber, out stateInitQueue))
			{
				stateInitQueue = new VoxelManager.StateInitQueue(player);
				VoxelManager._initQueues[player.ActorNumber] = stateInitQueue;
			}
			return stateInitQueue;
		}

		// Token: 0x06007E5A RID: 32346 RVA: 0x00296464 File Offset: 0x00294664
		private static void QueueChunkForPlayer(Chunk chunk, NetPlayer player)
		{
			VoxelManager.StateInitQueue orCreateQueueForPlayer = VoxelManager.GetOrCreateQueueForPlayer(player);
			byte[] array = null;
			int num = 0;
			if (chunk.IsDataChanged)
			{
				ChunkDTO chunkDTO = new ChunkDTO(chunk);
				array = ChunkIO.SerializeChunk(in chunkDTO);
				num = array.Length;
			}
			VoxelManager.ChunkInitState chunkInitState = new VoxelManager.ChunkInitState
			{
				numSerializedBytes = 0,
				totalSerializedBytes = num,
				worldId = chunk.World.Id,
				chunkId = chunk.Id,
				hash = (chunk.World.Id ^ chunk.Id.GetHashCode()),
				serializedChunkState = array
			};
			orCreateQueueForPlayer.chunks.Add(chunkInitState);
			VoxelManager._processInitQueus = true;
		}

		// Token: 0x06007E5B RID: 32347 RVA: 0x00296504 File Offset: 0x00294704
		private static void QueueOperationForPlayer(VoxelWorld world, NetPlayer player, global::UnityEngine.BoundsInt bounds, byte[] data)
		{
			VoxelManager.StateInitQueue stateInitQueue;
			if (!VoxelManager._initQueues.TryGetValue(player.ActorNumber, out stateInitQueue))
			{
				stateInitQueue = new VoxelManager.StateInitQueue();
				VoxelManager._initQueues[player.ActorNumber] = stateInitQueue;
			}
			VoxelManager.VoxelOperationResult voxelOperationResult = new VoxelManager.VoxelOperationResult
			{
				worldId = world.Id,
				bounds = bounds,
				data = data
			};
			stateInitQueue.operations.Add(voxelOperationResult);
			VoxelManager._processInitQueus = true;
		}

		// Token: 0x06007E5C RID: 32348 RVA: 0x00296578 File Offset: 0x00294778
		private static void QueueMineOperationForPlayer(NetPlayer player, VoxelManager.VoxelMineOperation op)
		{
			VoxelManager.StateInitQueue stateInitQueue;
			if (!VoxelManager._initQueues.TryGetValue(player.ActorNumber, out stateInitQueue))
			{
				stateInitQueue = new VoxelManager.StateInitQueue();
				VoxelManager._initQueues[player.ActorNumber] = stateInitQueue;
			}
			stateInitQueue.mineOps.Add(op);
			VoxelManager._processInitQueus = true;
		}

		// Token: 0x06007E5D RID: 32349 RVA: 0x002965C4 File Offset: 0x002947C4
		private static void QueueMineCommand(NetPlayer player, VoxelManager.VoxelMineOperation operation)
		{
			List<VoxelManager.VoxelMineOperation> list;
			if (!VoxelManager._mineOpQueues.TryGetValue(player.ActorNumber, out list))
			{
				list = new List<VoxelManager.VoxelMineOperation>();
				VoxelManager._mineOpQueues[player.ActorNumber] = list;
			}
			list.Add(operation);
			VoxelManager._mineCommandsQueued = true;
		}

		// Token: 0x06007E5E RID: 32350 RVA: 0x0029660C File Offset: 0x0029480C
		private static void SendQueuedMineCommands()
		{
			int num = 0;
			int num2 = 0;
			VoxelManager._mineCommandsQueued = false;
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				List<VoxelManager.VoxelMineOperation> list;
				if (!netPlayer.IsLocal && VoxelManager._mineOpQueues.TryGetValue(netPlayer.ActorNumber, out list) && list.Count != 0)
				{
					num += list.Count;
					int num3 = Mathf.Min(20, list.Count);
					VoxelManager.SendMineCommand(netPlayer, VoxelManager.GetSpanFor(list, 0, num3));
					list.RemoveRange(0, num3);
					num2 += list.Count;
					if (list.Count > 0)
					{
						VoxelManager._mineCommandsQueued = true;
					}
				}
			}
			VoxelManager._nextMineCommandTime = Time.realtimeSinceStartup + VoxelManager._mineCommandInterval;
		}

		// Token: 0x06007E5F RID: 32351 RVA: 0x002966E8 File Offset: 0x002948E8
		private static void QueueMineOperation(VoxelManager.VoxelMineOperation op, NetPlayer sender)
		{
			VoxelManager._localOperationQueue.Add(new ValueTuple<NetPlayer, VoxelManager.VoxelMineOperation>(sender, op));
		}

		// Token: 0x06007E60 RID: 32352 RVA: 0x002966FC File Offset: 0x002948FC
		private static void ExecuteQueuedLocalOperations()
		{
			bool hasAuthority = VoxelManager.HasAuthority;
			VoxelManager._nextLocalOpTime = Time.realtimeSinceStartup + VoxelManager._localOpInterval;
			int count = VoxelManager._localOperationQueue.Count;
			for (int i = 0; i < VoxelManager._localOperationQueue.Count; i++)
			{
				ValueTuple<NetPlayer, VoxelManager.VoxelMineOperation> valueTuple = VoxelManager._localOperationQueue[i];
				NetPlayer item = valueTuple.Item1;
				VoxelManager.VoxelMineOperation item2 = valueTuple.Item2;
				VoxelWorld voxelWorld;
				if (!VoxelManager._worlds.TryGetValue(item2.worldId, out voxelWorld))
				{
					Debug.LogError(string.Format("[VOX] Unable to perform queued local operation on invalid world {0}", item2.worldId));
					VoxelManager._localOperationQueue.RemoveAtSwapBack(i--);
				}
				else
				{
					global::UnityEngine.BoundsInt bounds = voxelWorld.GetBounds(item2.op.origin, (int)item2.op.radius);
					if (!voxelWorld.ChunksHaveJobs(bounds))
					{
						int[] array = voxelWorld.PerformLocalMiningOperation(item2, false);
						if (hasAuthority)
						{
							VoxelManager.ProcessMiningResult(item, voxelWorld, array);
						}
						VoxelManager._localOperationQueue.RemoveAtSwapBack(i--);
					}
				}
			}
		}

		// Token: 0x06007E61 RID: 32353 RVA: 0x002967F0 File Offset: 0x002949F0
		private static void ProcessMiningResult(NetPlayer player, VoxelWorld world, int[] amounts)
		{
			for (int i = 0; i < amounts.Length; i++)
			{
				if (amounts[i] != 0)
				{
					VoxelEvents.HandleResourceMinedAuthority(player, world, amounts);
					return;
				}
			}
		}

		// Token: 0x06007E62 RID: 32354 RVA: 0x0029681C File Offset: 0x00294A1C
		private static void SendNextChunk(VoxelManager.StateInitQueue queue)
		{
			NetPlayer player = queue.player;
			if (queue.currentChunk != null)
			{
				VoxelManager.ChunkInitState currentChunk = queue.currentChunk;
				VoxelManager.SendNextPacketForChunk(currentChunk, player);
				if (currentChunk.numSerializedBytes == currentChunk.totalSerializedBytes)
				{
					queue.currentChunk = null;
					return;
				}
			}
			else
			{
				if (queue.chunks.Count > 0)
				{
					VoxelManager.ChunkInitState chunkInitState = queue.chunks[0];
					VoxelManager.SendStartChunk(player, chunkInitState.worldId, chunkInitState.chunkId, chunkInitState.hash, chunkInitState.totalSerializedBytes);
					if (chunkInitState.totalSerializedBytes > 0)
					{
						queue.currentChunk = chunkInitState;
					}
					queue.chunks.RemoveAt(0);
					return;
				}
				if (queue.mineOps.Count > 0)
				{
					int num = Mathf.Min(20, queue.mineOps.Count);
					VoxelManager.SendMineCommand(player, VoxelManager.GetSpanFor(queue.mineOps, 0, num));
					queue.mineOps.RemoveRange(0, num);
					return;
				}
				if (queue.operations.Count > 0)
				{
					VoxelManager.VoxelOperationResult voxelOperationResult = queue.operations[0];
					VoxelManager.SendSetDensity(player, voxelOperationResult.worldId, voxelOperationResult.bounds, voxelOperationResult.data);
					queue.operations.RemoveAt(0);
				}
			}
		}

		// Token: 0x06007E63 RID: 32355 RVA: 0x0029693A File Offset: 0x00294B3A
		private static Span<VoxelManager.VoxelMineOperation> GetSpanFor(List<VoxelManager.VoxelMineOperation> ops, int start, int count)
		{
			if (VoxelManager._mineOpArray == null || VoxelManager._mineOpArray.Length < count)
			{
				VoxelManager._mineOpArray = new VoxelManager.VoxelMineOperation[count * 2];
			}
			ops.CopyTo(start, VoxelManager._mineOpArray, 0, count);
			return VoxelManager._mineOpArray.AsSpan(start, count);
		}

		// Token: 0x06007E64 RID: 32356 RVA: 0x00296974 File Offset: 0x00294B74
		private static void SendNextPacketForChunk(VoxelManager.ChunkInitState chunkState, NetPlayer player)
		{
			int numSerializedBytes = chunkState.numSerializedBytes;
			int num = Mathf.Min(chunkState.totalSerializedBytes - numSerializedBytes, 1000);
			if (num <= 0)
			{
				return;
			}
			Array.Copy(chunkState.serializedChunkState, numSerializedBytes, VoxelManager._packetData, 0, num);
			chunkState.numSerializedBytes += num;
			VoxelManager.SendContinueChunk(player, chunkState.hash, num, VoxelManager._packetData);
			VoxelManager.EnqueueTransferLog(num);
			int numSerializedBytes2 = chunkState.numSerializedBytes;
			int totalSerializedBytes = chunkState.totalSerializedBytes;
		}

		// Token: 0x06007E65 RID: 32357 RVA: 0x002969E8 File Offset: 0x00294BE8
		private static void OnStartChunkReceived(int worldId, int3 chunkId, int hash, int size)
		{
			int chunkIndex = VoxelManager._localInitQueue.GetChunkIndex(hash);
			if (chunkIndex >= 0)
			{
				VoxelManager._localInitQueue.chunks.RemoveAt(chunkIndex);
			}
			VoxelWorld voxelWorld;
			if (!VoxelManager._worlds.TryGetValue(worldId, out voxelWorld))
			{
				Debug.LogError(string.Format("Failed to find world {0}", worldId));
				return;
			}
			Chunk chunk;
			if (!voxelWorld.TryGetChunk(chunkId, out chunk))
			{
				Debug.LogError(string.Format("Tried to receive a non-loaded chunk {0}", chunkId));
				return;
			}
			if (size > 0)
			{
				VoxelManager.ChunkInitState chunkInitState = new VoxelManager.ChunkInitState
				{
					worldId = worldId,
					hash = hash,
					serializedChunkState = new byte[size],
					numSerializedBytes = 0,
					totalSerializedBytes = size
				};
				VoxelManager._localInitQueue.chunks.Add(chunkInitState);
				return;
			}
			voxelWorld.ResetChunk(chunkId);
		}

		// Token: 0x06007E66 RID: 32358 RVA: 0x00296AA8 File Offset: 0x00294CA8
		private static void OnChunkPacketReceived(int hash, int size, byte[] data)
		{
			if (size > data.Length)
			{
				Debug.LogError("Size value is is larger than data");
				return;
			}
			int chunkIndex = VoxelManager._localInitQueue.GetChunkIndex(hash);
			if (chunkIndex < 0)
			{
				Debug.LogError(string.Format("Couldn't fetch chunk state with hash {0}", hash));
				return;
			}
			VoxelManager.ChunkInitState chunkInitState = VoxelManager._localInitQueue.chunks[chunkIndex];
			if (chunkInitState.numSerializedBytes + size > chunkInitState.totalSerializedBytes)
			{
				Debug.LogError(string.Format("Received data larger than {0} bytes for chunk {1}", chunkInitState.totalSerializedBytes, hash));
				return;
			}
			Array.Copy(data, 0, chunkInitState.serializedChunkState, chunkInitState.numSerializedBytes, size);
			chunkInitState.numSerializedBytes += size;
			if (chunkInitState.numSerializedBytes == chunkInitState.totalSerializedBytes)
			{
				ChunkDTO chunkDTO;
				if (ChunkIO.TryDeserializeChunk(in chunkInitState.serializedChunkState, out chunkDTO))
				{
					VoxelWorld voxelWorld;
					if (VoxelManager._worlds.TryGetValue(chunkDTO.WorldId, out voxelWorld))
					{
						voxelWorld.UpdateChunkFrom(chunkDTO);
					}
					else
					{
						Debug.LogError(string.Format("Deserialized chunk for nonexistent world {0}", chunkDTO.WorldId));
					}
				}
				else
				{
					Debug.LogError(string.Format("Unable to deserialize chunk with hash {0}", chunkInitState.hash));
				}
				VoxelManager._localInitQueue.chunks.RemoveAt(chunkIndex);
			}
		}

		// Token: 0x06007E67 RID: 32359 RVA: 0x00296BD0 File Offset: 0x00294DD0
		private static void SendDensity(VoxelWorld world, global::UnityEngine.BoundsInt bounds)
		{
			byte[] array;
			VoxelManager.GetDensityForBounds(world, bounds, out array);
			byte[] array2 = LZ4Pickler.Pickle(array, LZ4Level.L00_FAST);
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				if (!netPlayer.IsLocal)
				{
					if (VoxelManager.WorldIsQueuedForPlayer(world, netPlayer))
					{
						VoxelManager.QueueOperationForPlayer(world, netPlayer, bounds, array2);
					}
					else
					{
						VoxelManager.SendSetDensity(netPlayer, world.Id, bounds, array2);
					}
				}
			}
		}

		// Token: 0x06007E68 RID: 32360 RVA: 0x00296C58 File Offset: 0x00294E58
		public static void PerformOperation(VoxelWorld world, Vector3 position, VoxelAction action)
		{
			position = world.GetLocalPosition(position);
			action.radius /= world.Scale;
			if (!VoxelManager.InRoom)
			{
				world.PerformLocalOperation(position, action, true);
				return;
			}
			if (VoxelManager.HasAuthority)
			{
				VoxelManager.OperateAuthority(world, position, action);
				return;
			}
			world.PerformLocalOperation(position, action, true);
			VoxelManager.SendOperationRequest(world.Id, position, action);
		}

		// Token: 0x06007E69 RID: 32361 RVA: 0x00296CB8 File Offset: 0x00294EB8
		private static void OperateAuthority(VoxelWorld world, Vector3 localPosition, VoxelAction action)
		{
			world.PerformLocalOperation(localPosition, action, true);
			global::UnityEngine.BoundsInt bounds = world.GetBounds(localPosition, action.radius);
			VoxelManager.SendDensity(world, bounds);
		}

		// Token: 0x06007E6A RID: 32362 RVA: 0x00296CE8 File Offset: 0x00294EE8
		private static void OnOperationRequestReceived(int worldId, Vector3 localPosition, VoxelAction action, PhotonMessageInfoWrapped info)
		{
			VoxelWorld voxelWorld;
			if (!VoxelManager._worlds.TryGetValue(worldId, out voxelWorld))
			{
				Debug.LogError(string.Format("Couldn't find voxel world {0}", worldId));
				return;
			}
			global::UnityEngine.BoundsInt bounds = voxelWorld.GetBounds(localPosition, action.radius);
			if (bounds.GetVoxelCount() > 1000)
			{
				GTDev.LogError<string>(string.Format("Received voxel operation request was too large [{0} = {1} voxels]", bounds, bounds.GetVoxelCount()), null);
				return;
			}
			VoxelManager.OperateAuthority(voxelWorld, localPosition, action);
		}

		// Token: 0x06007E6B RID: 32363 RVA: 0x00296D64 File Offset: 0x00294F64
		public static void Mine(VoxelWorld world, Vector3 hitPoint, Vector3 hitNormal, Vector3 origin, VoxelAction action)
		{
			VoxelManager.VoxelMineOperation voxelMineOperation = new VoxelManager.VoxelMineOperation(world, hitPoint, hitNormal, origin, action);
			if (!VoxelManager.InRoom)
			{
				world.PerformLocalMiningOperation(voxelMineOperation, true);
				return;
			}
			if (VoxelManager.HasAuthority)
			{
				VoxelManager.MineAuthority(world, voxelMineOperation, null);
				return;
			}
			world.PerformLocalMiningOperation(voxelMineOperation, true);
			VoxelManager.SendMineOperationRequest(voxelMineOperation);
		}

		// Token: 0x06007E6C RID: 32364 RVA: 0x00296DB0 File Offset: 0x00294FB0
		private static void MineAuthority(VoxelWorld world, VoxelManager.VoxelMineOperation op, NetPlayer sender = null)
		{
			if (sender == null)
			{
				sender = NetworkSystem.Instance.LocalPlayer;
			}
			if (sender.IsLocal)
			{
				int[] array = world.PerformLocalMiningOperation(op, true);
				VoxelManager.ProcessMiningResult(sender, world, array);
			}
			else
			{
				VoxelManager.QueueMineOperation(op, sender);
			}
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				if (!netPlayer.IsLocal && netPlayer != sender)
				{
					if (VoxelManager.WorldIsQueuedForPlayer(world, netPlayer))
					{
						VoxelManager.QueueMineOperationForPlayer(netPlayer, op);
					}
					else
					{
						VoxelManager.QueueMineCommand(netPlayer, op);
					}
				}
			}
		}

		// Token: 0x06007E6D RID: 32365 RVA: 0x00296E54 File Offset: 0x00295054
		private static void OnMineRequestReceived(VoxelManager.VoxelMineOperation op, PhotonMessageInfoWrapped info)
		{
			VoxelWorld voxelWorld;
			if (!VoxelManager._worlds.TryGetValue(op.worldId, out voxelWorld))
			{
				Debug.LogError(string.Format("Couldn't find voxel world {0}", op.worldId));
				return;
			}
			VoxelManager.MineAuthority(voxelWorld, op, info.Sender);
		}

		// Token: 0x06007E6E RID: 32366 RVA: 0x00296EA0 File Offset: 0x002950A0
		private static void GetVoxelsForBounds(VoxelWorld world, global::UnityEngine.BoundsInt bounds, out Voxel[] voxels)
		{
			int voxelCount = bounds.GetVoxelCount();
			voxels = new Voxel[voxelCount];
			int num = 0;
			for (int i = bounds.min.x; i <= bounds.max.x; i++)
			{
				for (int j = bounds.min.y; j <= bounds.max.y; j++)
				{
					for (int k = bounds.min.z; k <= bounds.max.z; k++)
					{
						voxels[num++] = world.GetVoxelData(new int3(i, j, k));
					}
				}
			}
		}

		// Token: 0x06007E6F RID: 32367 RVA: 0x00296F60 File Offset: 0x00295160
		private static void GetDensityForBounds(VoxelWorld world, global::UnityEngine.BoundsInt bounds, out byte[] voxels)
		{
			int voxelCount = bounds.GetVoxelCount();
			voxels = VoxelManager._byteArrayBag.GetStaticArray(voxelCount);
			int num = 0;
			for (int i = bounds.min.x; i <= bounds.max.x; i++)
			{
				for (int j = bounds.min.y; j <= bounds.max.y; j++)
				{
					for (int k = bounds.min.z; k <= bounds.max.z; k++)
					{
						voxels[num++] = world.GetVoxelDensity(new int3(i, j, k));
					}
				}
			}
		}

		// Token: 0x06007E70 RID: 32368 RVA: 0x00297020 File Offset: 0x00295220
		private static void OnSetDensityReceived(int worldId, global::UnityEngine.BoundsInt bounds, byte[] data)
		{
			VoxelWorld voxelWorld;
			if (!VoxelManager._worlds.TryGetValue(worldId, out voxelWorld))
			{
				throw new InvalidOperationException(string.Format("Couldn't find voxel world {0}", worldId));
			}
			byte[] array = LZ4Pickler.Unpickle(data);
			if (bounds.GetVoxelCount() != array.Length)
			{
				Debug.LogError(string.Format("Voxel count mismatch: {0} vs {1}", bounds.GetVoxelCount(), array.Length));
				return;
			}
			voxelWorld.SetVoxelDensity(bounds, array, false);
		}

		// Token: 0x06007E71 RID: 32369 RVA: 0x00297090 File Offset: 0x00295290
		private static void OnMineCommandReceived(VoxelManager.VoxelMineOperation op)
		{
			VoxelManager.QueueMineOperation(op, null);
		}

		// Token: 0x06007E72 RID: 32370 RVA: 0x00297099 File Offset: 0x00295299
		internal static bool IsValidAuthorityRPC(PhotonMessageInfoWrapped info, VoxelManager.RPC eventType)
		{
			return ((VoxelManager.HasAuthority && VoxelManager.InRoom) || info.Sender.IsLocal) && !VoxelManager.IsSpamming(info, eventType);
		}

		// Token: 0x06007E73 RID: 32371 RVA: 0x002970C2 File Offset: 0x002952C2
		internal static bool IsValidClientRPC(PhotonMessageInfoWrapped info, VoxelManager.RPC eventType)
		{
			return (info.Sender.IsMasterClient || info.Sender.IsLocal) && !VoxelManager.IsSpamming(info, eventType);
		}

		// Token: 0x06007E74 RID: 32372 RVA: 0x002970EA File Offset: 0x002952EA
		internal static bool IsSpamming(PhotonMessageInfoWrapped info, VoxelManager.RPC eventType)
		{
			return !VoxelManager.GetSpamChecksForUser(info.senderID)[(int)eventType].CheckCallTime(Time.unscaledTime);
		}

		// Token: 0x06007E75 RID: 32373 RVA: 0x00297108 File Offset: 0x00295308
		internal static CallLimiter[] GetSpamChecksForUser(int userID)
		{
			CallLimiter[] array;
			if (!VoxelManager._spamChecks.TryGetValue(userID, out array))
			{
				array = new CallLimiter[]
				{
					new CallLimiter(10, 30f, 0.5f),
					new CallLimiter(10, 1f, 0.5f),
					new CallLimiter(20, 1f, 0.5f),
					new CallLimiter(10, 1f, 0.5f),
					new CallLimiter(100, 1f, 0.5f),
					new CallLimiter(20, 1f, 0.5f),
					new CallLimiter(50, 1f, 0.5f),
					new CallLimiter(20, 1f, 0.5f)
				};
				VoxelManager._spamChecks[userID] = array;
			}
			return array;
		}

		// Token: 0x06007E76 RID: 32374 RVA: 0x002971DC File Offset: 0x002953DC
		internal static void RegisterNetEventCallbacks()
		{
			RoomSystem.netEventCallbacks[100] = new Action<object[], PhotonMessageInfoWrapped>(VoxelManager.DeserializeWorldStateRequest);
			RoomSystem.netEventCallbacks[101] = new Action<object[], PhotonMessageInfoWrapped>(VoxelManager.DeserializeOperationRequest);
			RoomSystem.netEventCallbacks[102] = new Action<object[], PhotonMessageInfoWrapped>(VoxelManager.DeserializeMineOperationRequest);
			RoomSystem.netEventCallbacks[103] = new Action<object[], PhotonMessageInfoWrapped>(VoxelManager.DeserializeStartChunk);
			RoomSystem.netEventCallbacks[104] = new Action<object[], PhotonMessageInfoWrapped>(VoxelManager.DeserializeContinueChunk);
			RoomSystem.netEventCallbacks[105] = new Action<object[], PhotonMessageInfoWrapped>(VoxelManager.DeserializeSetDensity);
			RoomSystem.netEventCallbacks[106] = new Action<object[], PhotonMessageInfoWrapped>(VoxelManager.DeserializeMineCommand);
		}

		// Token: 0x06007E77 RID: 32375 RVA: 0x00297291 File Offset: 0x00295491
		private static void SendWorldStateRequest(int worldId)
		{
			RoomSystem.SendEvent(100, new object[] { worldId }, in NetworkSystemRaiseEvent.neoMaster, true);
		}

		// Token: 0x06007E78 RID: 32376 RVA: 0x002972B0 File Offset: 0x002954B0
		internal static void DeserializeWorldStateRequest(object[] eventData, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "DeserializeWorldStateRequest");
			if (!VoxelManager.IsValidAuthorityRPC(info, VoxelManager.RPC.WorldRequest))
			{
				return;
			}
			int num;
			if (!eventData.TryDeserializeTo(out num))
			{
				return;
			}
			VoxelManager.OnWorldStateRequestReceived(num, info);
		}

		// Token: 0x06007E79 RID: 32377 RVA: 0x002972E4 File Offset: 0x002954E4
		private static void SendOperationRequest(int worldId, Vector3 localPosition, VoxelAction action)
		{
			object[] array = new object[] { worldId, localPosition, action };
			RoomSystem.SendEvent(101, array, in NetworkSystemRaiseEvent.neoMaster, true);
		}

		// Token: 0x06007E7A RID: 32378 RVA: 0x00297324 File Offset: 0x00295524
		private static void DeserializeOperationRequest(object[] eventData, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "DeserializeOperationRequest");
			if (!VoxelManager.IsValidAuthorityRPC(info, VoxelManager.RPC.OperationRequest))
			{
				return;
			}
			int num;
			Vector3 vector;
			VoxelAction voxelAction;
			if (!eventData.TryDeserializeTo(out num, out vector, out voxelAction))
			{
				return;
			}
			float num2 = 10000f;
			if (!(in vector).IsValid(in num2) || !voxelAction.IsValid() || voxelAction.radius > 5f)
			{
				return;
			}
			VoxelManager.OnOperationRequestReceived(num, vector, voxelAction, info);
		}

		// Token: 0x06007E7B RID: 32379 RVA: 0x00297388 File Offset: 0x00295588
		private static void SendMineOperationRequest(VoxelManager.VoxelMineOperation op)
		{
			object[] array = new object[] { op };
			RoomSystem.SendEvent(102, array, in NetworkSystemRaiseEvent.neoMaster, true);
		}

		// Token: 0x06007E7C RID: 32380 RVA: 0x002973B4 File Offset: 0x002955B4
		private static void DeserializeMineOperationRequest(object[] eventData, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "DeserializeMineOperationRequest");
			if (!VoxelManager.IsValidAuthorityRPC(info, VoxelManager.RPC.MineRequest))
			{
				return;
			}
			VoxelManager.VoxelMineOperation voxelMineOperation;
			if (!eventData.TryDeserializeTo(out voxelMineOperation))
			{
				return;
			}
			if (!VoxelManager._worlds.ContainsKey(voxelMineOperation.worldId))
			{
				return;
			}
			if (!voxelMineOperation.IsValid())
			{
				return;
			}
			VoxelManager.OnMineRequestReceived(voxelMineOperation, info);
		}

		// Token: 0x06007E7D RID: 32381 RVA: 0x00297408 File Offset: 0x00295608
		private static void SendStartChunk(NetPlayer player, int worldId, int3 chunkId, int hash, int totalSerializedBytes)
		{
			object[] array = new object[] { worldId, chunkId, hash, totalSerializedBytes };
			RoomSystem.SendEvent(103, array, in player, true);
		}

		// Token: 0x06007E7E RID: 32382 RVA: 0x0029744C File Offset: 0x0029564C
		private static void DeserializeStartChunk(object[] eventData, PhotonMessageInfoWrapped info)
		{
			int num;
			int3 @int;
			int num2;
			int num3;
			if (!eventData.TryDeserializeTo(out num, out @int, out num2, out num3))
			{
				return;
			}
			if (num3 > 0)
			{
				MonkeAgent.IncrementRPCCall(info, "DeserializeStartChunk");
			}
			if (!VoxelManager.IsValidClientRPC(info, (num3 > 0) ? VoxelManager.RPC.StartChunk : VoxelManager.RPC.StartEmptyChunk))
			{
				return;
			}
			VoxelManager.OnStartChunkReceived(num, @int, num2, num3);
		}

		// Token: 0x06007E7F RID: 32383 RVA: 0x00297494 File Offset: 0x00295694
		private static void SendContinueChunk(NetPlayer player, int hash, int size, byte[] data)
		{
			if (data.Length > 1000)
			{
				Debug.LogError(string.Format("Attempted to send ContinueChunk() with too many bytes ({0})", data.Length));
				return;
			}
			object[] array = new object[] { hash, size, data };
			RoomSystem.SendEvent(104, array, in player, true);
		}

		// Token: 0x06007E80 RID: 32384 RVA: 0x002974EC File Offset: 0x002956EC
		private static void DeserializeContinueChunk(object[] eventData, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "DeserializeContinueChunk");
			if (!VoxelManager.IsValidClientRPC(info, VoxelManager.RPC.ContinueChunk))
			{
				return;
			}
			int num;
			int num2;
			byte[] array;
			if (!eventData.TryDeserializeTo(out num, out num2, out array))
			{
				return;
			}
			if (num2 < 1 || num2 > 1000 || num2 > array.Length)
			{
				return;
			}
			VoxelManager.OnChunkPacketReceived(num, num2, array);
		}

		// Token: 0x06007E81 RID: 32385 RVA: 0x00297538 File Offset: 0x00295738
		private static void SendSetDensity(NetPlayer player, int worldId, global::UnityEngine.BoundsInt bounds, byte[] data)
		{
			if (data.Length > 1000)
			{
				Debug.LogError(string.Format("Attempted to send SetDensity() with too many bytes ({0})", data.Length));
				return;
			}
			object[] array = new object[] { worldId, bounds, data };
			RoomSystem.SendEvent(105, array, in player, true);
			VoxelManager.EnqueueTransferLog(data.Length);
		}

		// Token: 0x06007E82 RID: 32386 RVA: 0x00297598 File Offset: 0x00295798
		private static void DeserializeSetDensity(object[] eventData, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "DeserializeSetDensity");
			if (!VoxelManager.IsValidClientRPC(info, VoxelManager.RPC.SetDensity))
			{
				return;
			}
			int num;
			global::UnityEngine.BoundsInt boundsInt;
			byte[] array;
			if (!eventData.TryDeserializeTo(out num, out boundsInt, out array))
			{
				return;
			}
			if (array.Length > 1000)
			{
				return;
			}
			VoxelManager.OnSetDensityReceived(num, boundsInt, array);
		}

		// Token: 0x06007E83 RID: 32387 RVA: 0x002975DC File Offset: 0x002957DC
		private static void SendMineCommand(NetPlayer player, Span<VoxelManager.VoxelMineOperation> ops)
		{
			object[] array = new object[] { ops.ToArray() };
			RoomSystem.SendEvent(106, array, in player, true);
		}

		// Token: 0x06007E84 RID: 32388 RVA: 0x00297608 File Offset: 0x00295808
		private static void DeserializeMineCommand(object[] eventData, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "DeserializeMineCommand");
			if (!VoxelManager.IsValidClientRPC(info, VoxelManager.RPC.MineCommand))
			{
				return;
			}
			VoxelManager.VoxelMineOperation[] array;
			if (!eventData.TryDeserializeTo(out array))
			{
				return;
			}
			if (array.Length > 20)
			{
				return;
			}
			foreach (VoxelManager.VoxelMineOperation voxelMineOperation in array)
			{
				if (VoxelManager._worlds.ContainsKey(voxelMineOperation.worldId) && voxelMineOperation.IsValid())
				{
					VoxelManager.OnMineCommandReceived(voxelMineOperation);
				}
			}
		}

		// Token: 0x06007E85 RID: 32389 RVA: 0x00297676 File Offset: 0x00295876
		private void TestHitPoint()
		{
			VoxelManager.<TestHitPoint>g__Test|103_0(new float[] { 1f, 10f, 100f, 1000f, 10000f, 25000f });
		}

		// Token: 0x06007E88 RID: 32392 RVA: 0x0029771C File Offset: 0x0029591C
		[CompilerGenerated]
		internal static void <TestHitPoint>g__Test|103_0(float[] magnitudes)
		{
			VoxelWorld voxelWorld = global::UnityEngine.Object.FindAnyObjectByType<VoxelWorld>();
			if (voxelWorld)
			{
				Debug.Log(string.Format("Testing HitPoint with world {0}", voxelWorld), voxelWorld);
				foreach (float num in magnitudes)
				{
					float num2 = 0f;
					for (int j = 0; j < 100; j++)
					{
						Vector3 vector = new Vector3(global::UnityEngine.Random.Range(-num, num), global::UnityEngine.Random.Range(-num, num), global::UnityEngine.Random.Range(-num, num));
						Vector3 localPosition = voxelWorld.GetLocalPosition(vector);
						VoxelManager.VoxelMineOperation voxelMineOperation = new VoxelManager.VoxelMineOperation(voxelWorld, vector, Vector3.one, localPosition, default(VoxelAction));
						Vector3 worldPosition = voxelWorld.GetWorldPosition(voxelMineOperation.localHitPoint);
						num2 = Mathf.Max(num2, (vector - worldPosition).magnitude);
					}
					Debug.Log(string.Format("[HPTest] Magnitude: {0} Max diff: {1}", num, num2));
				}
				return;
			}
			Debug.LogError("[VOX][HPTest] Can't text HitPoint without at least one VoxelWorld in scene!");
		}

		// Token: 0x06007E89 RID: 32393 RVA: 0x00002E6F File Offset: 0x0000106F
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
		}

		// Token: 0x06007E8A RID: 32394 RVA: 0x00002E7B File Offset: 0x0000107B
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
		}

		// Token: 0x040090E4 RID: 37092
		public const int OP_SCALE = 256;

		// Token: 0x040090E5 RID: 37093
		private const int MAX_DATA_SIZE = 1000;

		// Token: 0x040090E6 RID: 37094
		private static VoxelManager _instance;

		// Token: 0x040090E7 RID: 37095
		private static Dictionary<int, VoxelWorld> _worlds = new Dictionary<int, VoxelWorld>();

		// Token: 0x040090E8 RID: 37096
		private static StaticArrayBag<byte> _byteArrayBag = new StaticArrayBag<byte>();

		// Token: 0x040090E9 RID: 37097
		private static StaticArrayBag<int> _intArrayBag = new StaticArrayBag<int>();

		// Token: 0x040090EA RID: 37098
		private NetPlayer _owner;

		// Token: 0x040090EB RID: 37099
		private static Dictionary<int, VoxelManager.StateInitQueue> _initQueues = new Dictionary<int, VoxelManager.StateInitQueue>();

		// Token: 0x040090EC RID: 37100
		private static VoxelManager.StateInitQueue _localInitQueue = new VoxelManager.StateInitQueue();

		// Token: 0x040090ED RID: 37101
		private static byte[] _packetData = new byte[1000];

		// Token: 0x040090EE RID: 37102
		private static bool _processInitQueus;

		// Token: 0x040090EF RID: 37103
		[TupleElementNames(new string[] { "time", "bytes" })]
		private static Queue<ValueTuple<float, int>> _sendHistory = new Queue<ValueTuple<float, int>>();

		// Token: 0x040090F0 RID: 37104
		private static int _sendRate;

		// Token: 0x040090F1 RID: 37105
		private const int MAX_DATA_RATE = 10000;

		// Token: 0x040090F2 RID: 37106
		private static Dictionary<int, List<VoxelManager.VoxelMineOperation>> _mineOpQueues = new Dictionary<int, List<VoxelManager.VoxelMineOperation>>();

		// Token: 0x040090F3 RID: 37107
		private const int _maxMineCommandLength = 20;

		// Token: 0x040090F4 RID: 37108
		private static float _mineCommandInterval = 0.1f;

		// Token: 0x040090F5 RID: 37109
		private static float _nextMineCommandTime;

		// Token: 0x040090F6 RID: 37110
		private static bool _mineCommandsQueued;

		// Token: 0x040090F7 RID: 37111
		[TupleElementNames(new string[] { "player", "op" })]
		private static List<ValueTuple<NetPlayer, VoxelManager.VoxelMineOperation>> _localOperationQueue = new List<ValueTuple<NetPlayer, VoxelManager.VoxelMineOperation>>();

		// Token: 0x040090F8 RID: 37112
		private static float _localOpInterval = 0.1f;

		// Token: 0x040090F9 RID: 37113
		private static float _nextLocalOpTime;

		// Token: 0x040090FA RID: 37114
		private static VoxelManager.VoxelMineOperation[] _mineOpArray;

		// Token: 0x040090FB RID: 37115
		private static Dictionary<int, CallLimiter[]> _spamChecks = new Dictionary<int, CallLimiter[]>();

		// Token: 0x020013BF RID: 5055
		public class StateInitQueue
		{
			// Token: 0x17000C51 RID: 3153
			// (get) Token: 0x06007E8B RID: 32395 RVA: 0x00297811 File Offset: 0x00295A11
			public bool IsEmpty
			{
				get
				{
					return this.currentChunk == null && this.chunks.Count == 0 && this.mineOps.Count == 0 && this.operations.Count == 0;
				}
			}

			// Token: 0x06007E8C RID: 32396 RVA: 0x00297845 File Offset: 0x00295A45
			public StateInitQueue()
			{
			}

			// Token: 0x06007E8D RID: 32397 RVA: 0x0029786E File Offset: 0x00295A6E
			public StateInitQueue(NetPlayer player)
			{
				this.player = player;
			}

			// Token: 0x06007E8E RID: 32398 RVA: 0x002978A0 File Offset: 0x00295AA0
			public int GetChunkIndex(int hash)
			{
				for (int i = 0; i < this.chunks.Count; i++)
				{
					if (this.chunks[i].hash == hash)
					{
						return i;
					}
				}
				return -1;
			}

			// Token: 0x06007E8F RID: 32399 RVA: 0x002978DC File Offset: 0x00295ADC
			public VoxelManager.ChunkInitState GetChunkState(int hash)
			{
				foreach (VoxelManager.ChunkInitState chunkInitState in this.chunks)
				{
					if (chunkInitState.hash == hash)
					{
						return chunkInitState;
					}
				}
				return null;
			}

			// Token: 0x040090FC RID: 37116
			public NetPlayer player;

			// Token: 0x040090FD RID: 37117
			public List<VoxelManager.ChunkInitState> chunks = new List<VoxelManager.ChunkInitState>();

			// Token: 0x040090FE RID: 37118
			public List<VoxelManager.VoxelMineOperation> mineOps = new List<VoxelManager.VoxelMineOperation>();

			// Token: 0x040090FF RID: 37119
			public List<VoxelManager.VoxelOperationResult> operations = new List<VoxelManager.VoxelOperationResult>();

			// Token: 0x04009100 RID: 37120
			public VoxelManager.ChunkInitState currentChunk;
		}

		// Token: 0x020013C0 RID: 5056
		public struct VoxelOperationResult
		{
			// Token: 0x04009101 RID: 37121
			public int worldId;

			// Token: 0x04009102 RID: 37122
			public global::UnityEngine.BoundsInt bounds;

			// Token: 0x04009103 RID: 37123
			public byte[] data;
		}

		// Token: 0x020013C1 RID: 5057
		public struct VoxelMineOperation
		{
			// Token: 0x17000C52 RID: 3154
			// (get) Token: 0x06007E90 RID: 32400 RVA: 0x00297938 File Offset: 0x00295B38
			// (set) Token: 0x06007E91 RID: 32401 RVA: 0x00297969 File Offset: 0x00295B69
			public Vector3 localHitPoint
			{
				get
				{
					return this.op.origin / 256 + this._hitOffset;
				}
				set
				{
					this._hitOffset = (half3)(value - this.op.origin / 256);
				}
			}

			// Token: 0x17000C53 RID: 3155
			// (get) Token: 0x06007E92 RID: 32402 RVA: 0x0029799C File Offset: 0x00295B9C
			// (set) Token: 0x06007E93 RID: 32403 RVA: 0x002979FC File Offset: 0x00295BFC
			public Vector3 hitNormal
			{
				get
				{
					return new Vector3((float)this._normX / 255f * 2f - 1f, (float)this._normY / 255f * 2f - 1f, (float)this._normZ / 255f * 2f - 1f);
				}
				set
				{
					ValueTuple<byte, byte, byte> valueTuple = VoxelManager.VoxelMineOperation.NormalToBytes(value);
					this._normX = valueTuple.Item1;
					this._normY = valueTuple.Item2;
					this._normZ = valueTuple.Item3;
				}
			}

			// Token: 0x06007E94 RID: 32404 RVA: 0x00297A34 File Offset: 0x00295C34
			public VoxelMineOperation(VoxelWorld world, Vector3 hitPoint, Vector3 hitNormal, Vector3 origin, VoxelAction action)
			{
				this.worldId = world.Id;
				this.op = new VoxelOperation(origin, action);
				this._hitOffset = (half3)(world.GetLocalPosition(hitPoint) - this.op.origin / 256);
				ValueTuple<byte, byte, byte> valueTuple = VoxelManager.VoxelMineOperation.NormalToBytes(hitNormal);
				this._normX = valueTuple.Item1;
				this._normY = valueTuple.Item2;
				this._normZ = valueTuple.Item3;
				if (math.length(this._hitOffset) >= 5f)
				{
					Debug.LogError(string.Format("[VOX] {0}-{1}={2} [{3:F4}]", new object[]
					{
						hitPoint,
						world.GetWorldPosition(this.localHitPoint),
						hitPoint - world.GetWorldPosition(this.localHitPoint),
						(hitPoint - world.GetWorldPosition(this.localHitPoint)).magnitude
					}));
				}
				Debug.Log(string.Format("[VOX] {0}-{1}={2} [{3:F4}]", new object[]
				{
					hitPoint,
					world.GetWorldPosition(this.localHitPoint),
					hitPoint - world.GetWorldPosition(this.localHitPoint),
					(hitPoint - world.GetWorldPosition(this.localHitPoint)).magnitude
				}));
			}

			// Token: 0x06007E95 RID: 32405 RVA: 0x00297BB0 File Offset: 0x00295DB0
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			[return: TupleElementNames(new string[] { "x", "y", "z" })]
			private static ValueTuple<byte, byte, byte> NormalToBytes(Vector3 hitNormal)
			{
				return new ValueTuple<byte, byte, byte>((byte)((hitNormal.x * 0.5f + 0.5f) * 255f), (byte)((hitNormal.y * 0.5f + 0.5f) * 255f), (byte)((hitNormal.z * 0.5f + 0.5f) * 255f));
			}

			// Token: 0x06007E96 RID: 32406 RVA: 0x00297C10 File Offset: 0x00295E10
			public override string ToString()
			{
				return string.Join(", ", new object[] { this.worldId, this.localHitPoint, this.hitNormal, this.op });
			}

			// Token: 0x06007E97 RID: 32407 RVA: 0x00297C68 File Offset: 0x00295E68
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool IsValid()
			{
				Vector3 localHitPoint = this.localHitPoint;
				float num = 10000f;
				return (in localHitPoint).IsValid(in num) && this.hitNormal.sqrMagnitude >= 0.5f && this.op.IsValid();
			}

			// Token: 0x04009104 RID: 37124
			public int worldId;

			// Token: 0x04009105 RID: 37125
			private half3 _hitOffset;

			// Token: 0x04009106 RID: 37126
			private byte _normX;

			// Token: 0x04009107 RID: 37127
			private byte _normY;

			// Token: 0x04009108 RID: 37128
			private byte _normZ;

			// Token: 0x04009109 RID: 37129
			public VoxelOperation op;
		}

		// Token: 0x020013C2 RID: 5058
		public class ChunkInitState
		{
			// Token: 0x0400910A RID: 37130
			public int worldId;

			// Token: 0x0400910B RID: 37131
			public int3 chunkId;

			// Token: 0x0400910C RID: 37132
			public int hash;

			// Token: 0x0400910D RID: 37133
			public byte[] serializedChunkState;

			// Token: 0x0400910E RID: 37134
			public int numSerializedBytes;

			// Token: 0x0400910F RID: 37135
			public int totalSerializedBytes;
		}

		// Token: 0x020013C3 RID: 5059
		public enum RPC
		{
			// Token: 0x04009111 RID: 37137
			WorldRequest,
			// Token: 0x04009112 RID: 37138
			OperationRequest,
			// Token: 0x04009113 RID: 37139
			MineRequest,
			// Token: 0x04009114 RID: 37140
			StartChunk,
			// Token: 0x04009115 RID: 37141
			StartEmptyChunk,
			// Token: 0x04009116 RID: 37142
			ContinueChunk,
			// Token: 0x04009117 RID: 37143
			SetDensity,
			// Token: 0x04009118 RID: 37144
			MineCommand,
			// Token: 0x04009119 RID: 37145
			Count
		}
	}
}
