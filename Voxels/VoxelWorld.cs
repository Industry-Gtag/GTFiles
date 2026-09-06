using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Voxels
{
	// Token: 0x020013B0 RID: 5040
	[DefaultExecutionOrder(5)]
	public class VoxelWorld : MonoBehaviour
	{
		// Token: 0x17000C42 RID: 3138
		// (get) Token: 0x06007DAB RID: 32171 RVA: 0x00292BB0 File Offset: 0x00290DB0
		public Transform Root
		{
			get
			{
				if (!this.root)
				{
					return base.transform;
				}
				return this.root;
			}
		}

		// Token: 0x17000C43 RID: 3139
		// (get) Token: 0x06007DAC RID: 32172 RVA: 0x00292BCC File Offset: 0x00290DCC
		public IEnumerable<Chunk> Chunks
		{
			get
			{
				return this.chunks.Values;
			}
		}

		// Token: 0x17000C44 RID: 3140
		// (get) Token: 0x06007DAD RID: 32173 RVA: 0x00292BD9 File Offset: 0x00290DD9
		// (set) Token: 0x06007DAE RID: 32174 RVA: 0x00292BE1 File Offset: 0x00290DE1
		public bool Initialized { get; private set; }

		// Token: 0x17000C45 RID: 3141
		// (get) Token: 0x06007DAF RID: 32175 RVA: 0x00292BEA File Offset: 0x00290DEA
		public bool IsInfinite
		{
			get
			{
				return this.worldType == VoxelWorld.WorldType.Infinite;
			}
		}

		// Token: 0x17000C46 RID: 3142
		// (get) Token: 0x06007DB0 RID: 32176 RVA: 0x00292BF5 File Offset: 0x00290DF5
		public global::UnityEngine.BoundsInt WorldBounds
		{
			get
			{
				return this.worldBounds;
			}
		}

		// Token: 0x17000C47 RID: 3143
		// (get) Token: 0x06007DB1 RID: 32177 RVA: 0x00292BFD File Offset: 0x00290DFD
		// (set) Token: 0x06007DB2 RID: 32178 RVA: 0x00292C05 File Offset: 0x00290E05
		public int Id { get; private set; }

		// Token: 0x17000C48 RID: 3144
		// (get) Token: 0x06007DB3 RID: 32179 RVA: 0x00292C0E File Offset: 0x00290E0E
		// (set) Token: 0x06007DB4 RID: 32180 RVA: 0x00292C16 File Offset: 0x00290E16
		public bool UpdateWorld
		{
			get
			{
				return this._updateWorld;
			}
			set
			{
				this._updateWorld = value;
			}
		}

		// Token: 0x17000C49 RID: 3145
		// (get) Token: 0x06007DB5 RID: 32181 RVA: 0x00292C1F File Offset: 0x00290E1F
		// (set) Token: 0x06007DB6 RID: 32182 RVA: 0x00292C27 File Offset: 0x00290E27
		public int3 ChunkSize { get; private set; }

		// Token: 0x17000C4A RID: 3146
		// (get) Token: 0x06007DB7 RID: 32183 RVA: 0x00292C30 File Offset: 0x00290E30
		// (set) Token: 0x06007DB8 RID: 32184 RVA: 0x00292C38 File Offset: 0x00290E38
		public int VoxelDimension { get; private set; }

		// Token: 0x17000C4B RID: 3147
		// (get) Token: 0x06007DB9 RID: 32185 RVA: 0x00292C41 File Offset: 0x00290E41
		// (set) Token: 0x06007DBA RID: 32186 RVA: 0x00292C49 File Offset: 0x00290E49
		public int VoxelCount { get; private set; }

		// Token: 0x17000C4C RID: 3148
		// (get) Token: 0x06007DBB RID: 32187 RVA: 0x00292C52 File Offset: 0x00290E52
		public MeshGenerationMode MeshGenerationMode
		{
			get
			{
				return this.generator.meshParameters.MeshGenerationMode;
			}
		}

		// Token: 0x17000C4D RID: 3149
		// (get) Token: 0x06007DBC RID: 32188 RVA: 0x00292C64 File Offset: 0x00290E64
		public bool WorldGenerationComplete
		{
			get
			{
				return this.chunks.Count > 0 && this.chunksToGenerate.Count == 0;
			}
		}

		// Token: 0x06007DBD RID: 32189 RVA: 0x00292C84 File Offset: 0x00290E84
		public static bool ExistsFor(Scene scene)
		{
			return VoxelWorld.WorldLookup.ContainsKey(scene.GetHashCode());
		}

		// Token: 0x06007DBE RID: 32190 RVA: 0x00292C9D File Offset: 0x00290E9D
		public static bool ExistsFor(GameObject gameObject)
		{
			return VoxelWorld.ExistsFor(gameObject.scene);
		}

		// Token: 0x06007DBF RID: 32191 RVA: 0x00292CAA File Offset: 0x00290EAA
		public static bool ExistsFor(Component component)
		{
			return VoxelWorld.ExistsFor(component.gameObject.scene);
		}

		// Token: 0x06007DC0 RID: 32192 RVA: 0x00292CBC File Offset: 0x00290EBC
		public static void SetFor(Scene scene, VoxelWorld voxelWorld)
		{
			if (!VoxelWorld.WorldLookup.TryAdd(scene.GetHashCode(), voxelWorld))
			{
				throw new InvalidOperationException(string.Format("Scene {0} already has a VoxelWorld set.", scene));
			}
		}

		// Token: 0x06007DC1 RID: 32193 RVA: 0x00292CEE File Offset: 0x00290EEE
		public static void SetFor(GameObject gameObject, VoxelWorld voxelWorld)
		{
			VoxelWorld.SetFor(gameObject.scene, voxelWorld);
		}

		// Token: 0x06007DC2 RID: 32194 RVA: 0x00292CFC File Offset: 0x00290EFC
		public static void SetFor(Component component, VoxelWorld voxelWorld)
		{
			VoxelWorld.SetFor(component.gameObject.scene, voxelWorld);
		}

		// Token: 0x06007DC3 RID: 32195 RVA: 0x00292D10 File Offset: 0x00290F10
		public static VoxelWorld GetFor(Scene scene)
		{
			VoxelWorld voxelWorld;
			if (!VoxelWorld.WorldLookup.TryGetValue(scene.GetHashCode(), out voxelWorld))
			{
				Debug.LogError(string.Format("No VoxelWorld found for scene {0}", scene));
			}
			return voxelWorld;
		}

		// Token: 0x06007DC4 RID: 32196 RVA: 0x00292D4E File Offset: 0x00290F4E
		public static VoxelWorld GetFor(GameObject gameObject)
		{
			return VoxelWorld.GetFor(gameObject.scene);
		}

		// Token: 0x06007DC5 RID: 32197 RVA: 0x00292D5B File Offset: 0x00290F5B
		public static VoxelWorld GetFor(Component component)
		{
			return VoxelWorld.GetFor(component.gameObject.scene);
		}

		// Token: 0x06007DC6 RID: 32198 RVA: 0x00292D70 File Offset: 0x00290F70
		private void Awake()
		{
			if (this.registerAsSceneWorld && !VoxelWorld.ExistsFor(this))
			{
				VoxelWorld.SetFor(base.gameObject, this);
			}
			MeshGenerationMode meshGenerationMode = this.generator.meshParameters.MeshGenerationMode;
			if (meshGenerationMode != MeshGenerationMode.MarchingCubes)
			{
				if (meshGenerationMode != MeshGenerationMode.SurfaceNets)
				{
					throw new ArgumentOutOfRangeException();
				}
				Chunk.Pad = 2;
			}
			else
			{
				Chunk.Pad = 1;
			}
			if (!this.target)
			{
				this.target = base.transform;
			}
			if (!this.root)
			{
				this.root = base.transform;
			}
			this.Id = this.GenerateHashcodeFromPath();
		}

		// Token: 0x06007DC7 RID: 32199 RVA: 0x00292E08 File Offset: 0x00291008
		private void Start()
		{
			if (this.worldType == VoxelWorld.WorldType.Bounded)
			{
				this.OptimizeWorld();
			}
			this.generator.InitWorld(this);
			Chunk.DefaultSize = this.chunkSize;
			this.ChunkSize = Chunk.DefaultSize;
			this.VoxelDimension = this.chunkSize + Chunk.Pad;
			this.VoxelCount = this.VoxelDimension * this.VoxelDimension * this.VoxelDimension;
			int num = this.viewDistance * 2 + 1;
			this.chunksToGenerate = new NativeHashSet<int3>(num * num * num, Allocator.Persistent);
			this.sortedChunks = new NativeList<int3>(Allocator.Persistent);
			this.ConfigurePools();
			this.Initialized = true;
		}

		// Token: 0x06007DC8 RID: 32200 RVA: 0x00292EB6 File Offset: 0x002910B6
		private void OnEnable()
		{
			VoxelManager.Register(this);
		}

		// Token: 0x06007DC9 RID: 32201 RVA: 0x00292EBE File Offset: 0x002910BE
		private void OnDisable()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			VoxelManager.Unregister(this);
		}

		// Token: 0x06007DCA RID: 32202 RVA: 0x00292ED0 File Offset: 0x002910D0
		private void OnDestroy()
		{
			if (this.persistChanges)
			{
				this.SaveChunks();
			}
			foreach (Chunk chunk in this.chunks.Values)
			{
				chunk.Dispose();
			}
			if (this.sortedChunks.IsCreated)
			{
				this.sortedChunks.Dispose();
			}
			if (this.chunksToGenerate.IsCreated)
			{
				this.chunksToGenerate.Dispose();
			}
			this.sortedChunks = default(NativeList<int3>);
			this.chunksToGenerate = default(NativeHashSet<int3>);
		}

		// Token: 0x06007DCB RID: 32203 RVA: 0x00292F7C File Offset: 0x0029117C
		private void Update()
		{
			if (!this._updateWorld)
			{
				return;
			}
			if (this.generationQueueChanged)
			{
				this.sortJobHandle.Complete();
				this.sortedChunkCount = this.sortedChunks.Length;
				this.chunkSortIndex = 0;
				this.generationQueueChanged = false;
			}
			foreach (ChunkTaskSet chunkTaskSet in this.chunkJobs.Values)
			{
				if (chunkTaskSet.CompleteIfReady())
				{
					this.HandleJobCompletion(chunkTaskSet);
				}
			}
			foreach (int3 @int in this.completedJobs)
			{
				this.chunkJobs.Remove(@int);
			}
			this.completedJobs.Clear();
			using (Dictionary<int3, Chunk>.ValueCollection.Enumerator enumerator3 = this.chunks.Values.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					Chunk chunk = enumerator3.Current;
					if (chunk.IsDirty)
					{
						this.chunksToGenerate.Add(chunk.Id);
						this.generationQueueChanged = true;
					}
				}
				goto IL_0155;
			}
			IL_011E:
			int num = this.chunkSortIndex;
			this.chunkSortIndex = num + 1;
			int3 int2 = this.sortedChunks[num];
			this.chunksToGenerate.Remove(int2);
			this.ProcessChunk(int2);
			IL_0155:
			if (this.chunkSortIndex >= this.sortedChunks.Length || this.chunkJobs.Count >= this.maxJobs)
			{
				this.UpdateVisibleChunks(false);
				return;
			}
			goto IL_011E;
		}

		// Token: 0x06007DCC RID: 32204 RVA: 0x00293134 File Offset: 0x00291334
		private void SaveChunks()
		{
			Debug.Log("Saving chunks...");
			foreach (Chunk chunk in this.chunks.Values)
			{
				if (chunk.IsDataChanged)
				{
					ChunkIO.SaveChunk(new ChunkDTO(chunk));
				}
			}
		}

		// Token: 0x06007DCD RID: 32205 RVA: 0x002931A4 File Offset: 0x002913A4
		private void ConfigurePools()
		{
			this._chunkPool = new global::UnityEngine.Pool.ObjectPool<Chunk>(() => new Chunk(int3.zero, this.ChunkSize, -1), delegate(Chunk chunk)
			{
			}, delegate(Chunk chunk)
			{
				if (chunk.Component)
				{
					this._chunkComponentPool.Release(chunk.Component);
					chunk.SetComponent(null);
				}
				chunk.Clear();
			}, delegate(Chunk chunk)
			{
				chunk.Dispose();
			}, true, 100, 100);
			this._chunkComponentPool = new global::UnityEngine.Pool.ObjectPool<ChunkComponent>(() => Object.Instantiate<ChunkComponent>(this.chunkPrefab), delegate(ChunkComponent chunkComponent)
			{
				chunkComponent.gameObject.SetActive(false);
				chunkComponent.transform.SetParent(base.transform, false);
			}, delegate(ChunkComponent chunkComponent)
			{
				if (chunkComponent.meshFilter.sharedMesh)
				{
					Mesh sharedMesh = chunkComponent.meshFilter.sharedMesh;
					chunkComponent.meshFilter.sharedMesh = null;
					chunkComponent.meshCollider.sharedMesh = null;
					this._meshPool.Release(sharedMesh);
				}
				chunkComponent.gameObject.SetActive(false);
			}, delegate(ChunkComponent chunkComponent)
			{
				if (chunkComponent)
				{
					Object.Destroy(chunkComponent.gameObject);
				}
			}, true, 100, 100);
			this._meshPool = new global::UnityEngine.Pool.ObjectPool<Mesh>(() => new Mesh(), null, delegate(Mesh mesh)
			{
				mesh.Clear(false);
			}, null, true, 100, 100);
		}

		// Token: 0x06007DCE RID: 32206 RVA: 0x002932BA File Offset: 0x002914BA
		public bool TryGetChunk(int3 chunkId, out Chunk chunk)
		{
			return this.chunks.TryGetValue(chunkId, out chunk);
		}

		// Token: 0x06007DCF RID: 32207 RVA: 0x002932C9 File Offset: 0x002914C9
		private Chunk GetPooledChunk(int3 chunkId)
		{
			Chunk chunk = this._chunkPool.Get();
			chunk.World = this;
			chunk.Id = chunkId;
			return chunk;
		}

		// Token: 0x06007DD0 RID: 32208 RVA: 0x002932E4 File Offset: 0x002914E4
		private Chunk CreateOrLoadChunk(int3 chunkId)
		{
			Chunk pooledChunk = this.GetPooledChunk(chunkId);
			ChunkDTO chunkDTO;
			if (this.persistChanges && ChunkIO.TryLoadChunk(chunkId, out chunkDTO))
			{
				pooledChunk.SetFrom(chunkDTO);
			}
			else
			{
				pooledChunk.Id = chunkId;
			}
			return pooledChunk;
		}

		// Token: 0x06007DD1 RID: 32209 RVA: 0x0029331C File Offset: 0x0029151C
		public void SetChunkFrom(ChunkDTO dto)
		{
			Chunk pooledChunk;
			if (!this.chunks.TryGetValue(dto.Id, out pooledChunk))
			{
				pooledChunk = this.GetPooledChunk(dto.Id);
				this.chunks[dto.Id] = pooledChunk;
			}
			pooledChunk.SetFrom(dto);
		}

		// Token: 0x06007DD2 RID: 32210 RVA: 0x00293364 File Offset: 0x00291564
		public void UpdateChunkFrom(ChunkDTO dto)
		{
			Chunk pooledChunk;
			if (!this.chunks.TryGetValue(dto.Id, out pooledChunk))
			{
				pooledChunk = this.GetPooledChunk(dto.Id);
				this.chunks[dto.Id] = pooledChunk;
				pooledChunk.SetFrom(dto);
				return;
			}
			pooledChunk.UpdateFrom(dto);
		}

		// Token: 0x06007DD3 RID: 32211 RVA: 0x002933B4 File Offset: 0x002915B4
		private void Save(Chunk chunk)
		{
			if (chunk.IsDataChanged)
			{
				ChunkIO.SaveChunk(new ChunkDTO(chunk));
			}
		}

		// Token: 0x06007DD4 RID: 32212 RVA: 0x002933C9 File Offset: 0x002915C9
		private void Unload(Chunk chunk)
		{
			if (this.persistChanges)
			{
				this.Save(chunk);
			}
			this._chunkPool.Release(chunk);
		}

		// Token: 0x06007DD5 RID: 32213 RVA: 0x002933E8 File Offset: 0x002915E8
		private void UpdateVisibleChunks(bool isFirstTime = false)
		{
			int3 chunkIdForWorldPosition = this.GetChunkIdForWorldPosition(this.target.position);
			if (chunkIdForWorldPosition.Equals(this.playerChunk) && !this.generationQueueChanged)
			{
				return;
			}
			this.playerChunk = chunkIdForWorldPosition;
			this.generationQueueChanged = true;
			VoxelWorld.WorldType worldType = this.worldType;
			if (worldType != VoxelWorld.WorldType.Infinite)
			{
				if (worldType != VoxelWorld.WorldType.Bounded)
				{
					throw new ArgumentOutOfRangeException();
				}
				ValueTuple<int3, int3> chunkBoundsForLocalBounds = this.GetChunkBoundsForLocalBounds(this.worldBounds, false);
				int3 item = chunkBoundsForLocalBounds.Item1;
				int3 item2 = chunkBoundsForLocalBounds.Item2;
				for (int i = item.x; i <= item2.x; i++)
				{
					for (int j = item.y; j <= item2.y; j++)
					{
						for (int k = item.z; k <= item2.z; k++)
						{
							int3 @int = new int3(i, j, k);
							if (!this.chunks.ContainsKey(@int) && !this.chunksToGenerate.Contains(@int))
							{
								this.chunksToGenerate.Add(@int);
							}
						}
					}
				}
			}
			else
			{
				for (int l = -this.viewDistance; l <= this.viewDistance; l++)
				{
					for (int m = -this.viewDistance; m <= this.viewDistance; m++)
					{
						for (int n = -this.viewDistance; n <= this.viewDistance; n++)
						{
							int3 int2 = this.playerChunk + new int3(l, m, n);
							if (!this.chunks.ContainsKey(int2) && !this.chunksToGenerate.Contains(int2))
							{
								this.chunksToGenerate.Add(int2);
							}
						}
					}
				}
			}
			SortChunksJob sortChunksJob = new SortChunksJob
			{
				ChunkSet = this.chunksToGenerate,
				SortedChunks = this.sortedChunks
			};
			this.sortJobHandle = sortChunksJob.Schedule(default(JobHandle));
			if (this.worldType == VoxelWorld.WorldType.Infinite)
			{
				int num = this.viewDistance + 2;
				this.chunksToRemove.Clear();
				foreach (int3 int3 in this.chunks.Keys)
				{
					if (Mathf.Abs(int3.x - this.playerChunk.x) > num || Mathf.Abs(int3.y - this.playerChunk.y) > num || Mathf.Abs(int3.z - this.playerChunk.z) > num)
					{
						this.chunksToRemove.Add(int3);
					}
				}
				foreach (int3 int4 in this.chunksToRemove)
				{
					Chunk chunk;
					if (this.chunks.TryGetValue(int4, out chunk))
					{
						ChunkTaskSet chunkTaskSet;
						if (this.chunkJobs.TryGetValue(int4, out chunkTaskSet))
						{
							chunkTaskSet.Complete();
							this.chunkJobs.Remove(int4);
						}
						this.Unload(chunk);
						this.chunks.Remove(int4);
					}
				}
			}
		}

		// Token: 0x06007DD6 RID: 32214 RVA: 0x0029371C File Offset: 0x0029191C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int3 GetChunkIdForWorldPosition(Vector3 worldPosition)
		{
			return this.GetLocalPosition(worldPosition).LocalPositionToChunkId(this.ChunkSize);
		}

		// Token: 0x06007DD7 RID: 32215 RVA: 0x00293730 File Offset: 0x00291930
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int3 GetChunkIdForLocalPosition(Vector3 voxelWorldPosition)
		{
			return voxelWorldPosition.LocalPositionToChunkId(this.ChunkSize);
		}

		// Token: 0x06007DD8 RID: 32216 RVA: 0x0029373E File Offset: 0x0029193E
		public void SetWorldType(VoxelWorld.WorldType newWorldType, bool force = false)
		{
			if (!force && this.worldType == newWorldType)
			{
				return;
			}
			this.worldType = newWorldType;
			this.RegenerateAllChunks();
		}

		// Token: 0x06007DD9 RID: 32217 RVA: 0x0029375A File Offset: 0x0029195A
		public void SetWorldBounds(global::UnityEngine.BoundsInt bounds)
		{
			this.worldBounds = bounds;
			this.SetWorldType(VoxelWorld.WorldType.Bounded, true);
		}

		// Token: 0x06007DDA RID: 32218 RVA: 0x0029376C File Offset: 0x0029196C
		public static void SaveWorld(Scene scene)
		{
			VoxelWorld @for = VoxelWorld.GetFor(scene);
			foreach (Chunk chunk in @for.chunks.Values)
			{
				@for.Save(chunk);
			}
		}

		// Token: 0x06007DDB RID: 32219 RVA: 0x002937CC File Offset: 0x002919CC
		public static void ResetWorld(Scene scene)
		{
			ChunkIO.DeleteWorld();
			VoxelWorld @for = VoxelWorld.GetFor(scene);
			if (@for)
			{
				@for.RegenerateAllChunks();
			}
		}

		// Token: 0x06007DDC RID: 32220 RVA: 0x002937F4 File Offset: 0x002919F4
		private void RegenerateAllChunks()
		{
			if (!this.Initialized)
			{
				return;
			}
			foreach (ChunkTaskSet chunkTaskSet in this.chunkJobs.Values)
			{
				chunkTaskSet.Complete();
			}
			this.chunkJobs.Clear();
			this.completedJobs.Clear();
			this.chunksToGenerate.Clear();
			this.sortedChunks.Clear();
			foreach (Chunk chunk in this.chunks.Values)
			{
				chunk.Clear();
			}
			this.generationQueueChanged = true;
		}

		// Token: 0x06007DDD RID: 32221 RVA: 0x002938CC File Offset: 0x00291ACC
		private void OptimizeWorld()
		{
			global::UnityEngine.BoundsInt boundsInt = this.generator.GetWorldBounds();
			if (boundsInt != default(global::UnityEngine.BoundsInt))
			{
				if (!this.root || this.root == base.transform)
				{
					this.root = new GameObject("VoxelWorldRoot").transform;
					this.root.SetParent(base.transform);
					this.root.localPosition = Vector3.zero;
					this.root.localRotation = Quaternion.identity;
					this.root.localScale = Vector3.one;
				}
				else if (this.root.parent != base.transform)
				{
					this.root.SetParent(base.transform, true);
					this.root.localRotation = Quaternion.identity;
					this.root.localScale = Vector3.one;
				}
				Vector3Int vector3Int = Vector3Int.one - boundsInt.min;
				Debug.Log(string.Format("Min: {0} Shift: {1}", boundsInt.min, vector3Int));
				this.generator.ShiftWorldBounds(vector3Int);
				this.root.localPosition += -vector3Int * this.Scale;
				if (this.worldType == VoxelWorld.WorldType.Bounded)
				{
					this.worldBounds = this.generator.GetWorldBounds();
					this.OptimizeChunkSize();
				}
			}
		}

		// Token: 0x06007DDE RID: 32222 RVA: 0x00293A48 File Offset: 0x00291C48
		private void OptimizeChunkSize()
		{
			if (this.worldType == VoxelWorld.WorldType.Bounded)
			{
				Vector3Int vector3Int = math.max(this.worldBounds.min.ToInt3() + this.worldBounds.size.ToInt3(), this.worldBounds.size.ToInt3()).ToVectorInt();
				vector3Int += Vector3Int.one;
				int num = Mathf.Max(new int[] { vector3Int.x, vector3Int.y, vector3Int.z });
				this.chunkSize = Mathf.Min(Mathf.Max(1, num), 32);
				return;
			}
			this.chunkSize = 32;
		}

		// Token: 0x06007DDF RID: 32223 RVA: 0x00293AF4 File Offset: 0x00291CF4
		public void ResetChunk(int3 chunkId)
		{
			Chunk chunk;
			if (!this.chunks.TryGetValue(chunkId, out chunk))
			{
				return;
			}
			if (!chunk.IsDataChanged)
			{
				return;
			}
			ChunkTaskSet chunkTaskSet;
			if (this.chunkJobs.TryGetValue(chunkId, out chunkTaskSet))
			{
				chunkTaskSet.Complete();
				this.RemoveChunkTask(chunkTaskSet);
			}
			chunk.IsDataGenerated = false;
			chunk.IsDirty = true;
		}

		// Token: 0x06007DE0 RID: 32224 RVA: 0x00293B48 File Offset: 0x00291D48
		private void ProcessChunk(int3 chunkId)
		{
			Chunk chunk;
			if (!this.chunks.TryGetValue(chunkId, out chunk))
			{
				chunk = this.CreateOrLoadChunk(chunkId);
				this.chunks[chunkId] = chunk;
			}
			if (!chunk.IsDirty)
			{
				Debug.LogWarning(string.Format("{0} is not dirty, skipping processing", chunk));
				return;
			}
			chunk.IsDirty = false;
			ChunkTaskSet chunkTaskSet = new ChunkTaskSet(chunk, this.generator, Array.Empty<ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>>());
			ChunkState state = chunk.State;
			if (state < ChunkState.VoxelDataGenerated)
			{
				chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(this.generator.CreateVoxelDataJob), null);
			}
			if (state < ChunkState.MeshDataGenerated)
			{
				if (this.generator.PostProcessMesh)
				{
					chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(this.generator.CreateMeshDataJob), null);
					chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(this.generator.CreateMeshPostProcessJob), new Action<Chunk>(this.CreateChunkMesh));
				}
				else
				{
					chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(this.generator.CreateMeshDataJob), new Action<Chunk>(this.CreateChunkMesh));
				}
			}
			else if (state < ChunkState.MeshCreated)
			{
				chunkTaskSet.AddTask(null, new Action<Chunk>(this.CreateChunkMesh));
			}
			if (state < ChunkState.CollisionBaked)
			{
				chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(ChunkTask.CreateCollisionJob), new Action<Chunk>(this.AssignMesh));
			}
			else if (state < ChunkState.MeshAssigned)
			{
				chunkTaskSet.AddTask(null, new Action<Chunk>(this.AssignMesh));
			}
			if (!chunkTaskSet.IsEmpty)
			{
				this.AddChunkTask(chunkTaskSet);
				chunkTaskSet.Start();
				return;
			}
			Debug.LogWarning(string.Format("{0} was dirty but nothing to do?", chunk));
		}

		// Token: 0x06007DE1 RID: 32225 RVA: 0x00293CBC File Offset: 0x00291EBC
		private void MeshChunkImmediately(Chunk chunk)
		{
			this.generator.CreateMeshDataJob(chunk).Complete();
			if (this.generator.PostProcessMesh)
			{
				this.generator.CreateMeshPostProcessJob(chunk).Complete();
			}
			chunk.Mesh = this.CreateMesh(chunk);
			if (chunk.Mesh)
			{
				ChunkTask.CreateCollisionJob(chunk).Complete();
			}
			this.AssignMesh(chunk);
			this.chunkJobs.Remove(chunk.Id);
		}

		// Token: 0x06007DE2 RID: 32226 RVA: 0x00293D40 File Offset: 0x00291F40
		private void MeshChunks(List<Chunk> chunks)
		{
			if (chunks.Count == 0)
			{
				return;
			}
			ChunkTaskSet chunkTaskSet = new ChunkTaskSet(chunks, this.generator, Array.Empty<ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>>());
			if (this.generator.PostProcessMesh)
			{
				chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(this.generator.CreateMeshDataJob), null);
				chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(this.generator.CreateMeshPostProcessJob), new Action<Chunk>(this.CreateChunkMesh));
			}
			else
			{
				chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(this.generator.CreateMeshDataJob), new Action<Chunk>(this.CreateChunkMesh));
			}
			chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(ChunkTask.CreateCollisionJob), new Action<Chunk>(this.AssignMesh));
			this.AddChunkTask(chunkTaskSet);
			chunkTaskSet.Start();
		}

		// Token: 0x06007DE3 RID: 32227 RVA: 0x00293E00 File Offset: 0x00292000
		private void AddChunkTask(ChunkTaskSet chunkTask)
		{
			foreach (Chunk chunk in chunkTask.Chunks)
			{
				this.chunkJobs.Add(chunk.Id, chunkTask);
			}
		}

		// Token: 0x06007DE4 RID: 32228 RVA: 0x00293E60 File Offset: 0x00292060
		private void RemoveChunkTask(ChunkTaskSet chunkTask)
		{
			foreach (Chunk chunk in chunkTask.Chunks)
			{
				this.chunkJobs.Remove(chunk.Id);
			}
		}

		// Token: 0x06007DE5 RID: 32229 RVA: 0x00293EC0 File Offset: 0x002920C0
		private void CreateChunkMesh(Chunk chunk)
		{
			chunk.Mesh = this.CreateMesh(chunk);
		}

		// Token: 0x06007DE6 RID: 32230 RVA: 0x00293ED0 File Offset: 0x002920D0
		private Mesh CreateMesh(Chunk chunk)
		{
			if (chunk.VertexCount == 0)
			{
				chunk.DisposeMeshData();
				chunk.IsMeshCreated = true;
				return null;
			}
			int vertexCount = chunk.VertexCount;
			Mesh mesh = this._meshPool.Get();
			if (vertexCount > chunk.VertexData.Length)
			{
				Debug.LogError(string.Format("Vertex count {0} exceeds allocated vertex data length {1} for chunk {2}. This is likely a bug in the meshing job.", vertexCount, chunk.VertexData.Length, chunk.Id));
				return null;
			}
			mesh.SetVertexBufferParams(vertexCount, MeshVertexData.VertexBufferMemoryLayout);
			mesh.SetIndexBufferParams(vertexCount, IndexFormat.UInt16);
			mesh.SetVertexBufferData<MeshVertexData>(chunk.VertexData, 0, 0, vertexCount, 0, MeshUpdateFlags.DontValidateIndices);
			mesh.SetIndexBufferData<ushort>(chunk.TriangleData, 0, 0, vertexCount, MeshUpdateFlags.DontValidateIndices);
			mesh.subMeshCount = 1;
			mesh.SetSubMesh(0, new SubMeshDescriptor(0, vertexCount, MeshTopology.Triangles), MeshUpdateFlags.Default);
			mesh.RecalculateBounds();
			chunk.DisposeMeshData();
			chunk.IsMeshCreated = true;
			return mesh;
		}

		// Token: 0x06007DE7 RID: 32231 RVA: 0x00293FA8 File Offset: 0x002921A8
		private void AssignMesh(Chunk chunk)
		{
			Mesh mesh = chunk.Mesh;
			if (mesh)
			{
				if (!chunk.Component)
				{
					ChunkComponent chunkComponent = this._chunkComponentPool.Get();
					chunkComponent.transform.SetParent(this.root, false);
					chunkComponent.transform.localScale = Vector3.one * this.worldScale;
					chunkComponent.transform.localPosition = (chunk.Id * this.ChunkSize).ToVector3() * this.worldScale;
					chunk.SetComponent(chunkComponent);
				}
				Mesh sharedMesh = chunk.MeshFilter.sharedMesh;
				chunk.MeshFilter.sharedMesh = mesh;
				chunk.MeshCollider.sharedMesh = mesh;
				if (sharedMesh)
				{
					this._meshPool.Release(sharedMesh);
				}
				chunk.GameObject.SetActive(true);
			}
			else if (chunk.Component)
			{
				this._chunkComponentPool.Release(chunk.Component);
				chunk.SetComponent(null);
			}
			chunk.IsMeshAssigned = true;
			chunk.IsDirty = false;
		}

		// Token: 0x06007DE8 RID: 32232 RVA: 0x002940BC File Offset: 0x002922BC
		private void PrepForOperationOnChunks(global::UnityEngine.BoundsInt bounds)
		{
			VoxelWorld._opBounds = bounds;
			this.GetChunksForBounds(VoxelWorld._opBounds, ref VoxelWorld._opChunks);
			VoxelWorld._opChangedChunks.Clear();
			VoxelWorld._opChunkJobs.Clear();
			foreach (Chunk chunk in VoxelWorld._opChunks)
			{
				ChunkTaskSet chunkTaskSet;
				if (this.chunkJobs.TryGetValue(chunk.Id, out chunkTaskSet))
				{
					chunkTaskSet.Complete();
					if (!VoxelWorld._opChunkJobs.Contains(chunkTaskSet))
					{
						VoxelWorld._opChunkJobs.Add(chunkTaskSet);
					}
				}
			}
		}

		// Token: 0x06007DE9 RID: 32233 RVA: 0x00294164 File Offset: 0x00292364
		private void FinalizeOperationOnChunks(bool immediate)
		{
			foreach (ChunkTaskSet chunkTaskSet in VoxelWorld._opChunkJobs)
			{
				for (int i = 0; i < chunkTaskSet.Chunks.Count; i++)
				{
					Chunk chunk = chunkTaskSet.Chunks[i];
					if (VoxelWorld._opChangedChunks.Contains(chunk))
					{
						this.chunkJobs.Remove(chunk.Id);
						if (chunkTaskSet.Chunks.Count > 1)
						{
							chunkTaskSet.Chunks.RemoveAt(i--);
						}
					}
					else
					{
						this.completedJobs.Add(chunk.Id);
					}
				}
			}
			if (VoxelWorld._opChangedChunks.Count > 0)
			{
				if (immediate)
				{
					this.MeshChunks(VoxelWorld._opChangedChunks);
					return;
				}
				foreach (Chunk chunk2 in VoxelWorld._opChangedChunks)
				{
					chunk2.IsMeshGenerated = false;
					chunk2.IsDirty = true;
				}
			}
		}

		// Token: 0x06007DEA RID: 32234 RVA: 0x00294288 File Offset: 0x00292488
		public void SetVoxelDensityCustom(global::UnityEngine.BoundsInt worldBounds, Func<int3, byte, byte> setDensityFunction, bool immediate = true)
		{
			VoxelWorld.<>c__DisplayClass117_0 CS$<>8__locals1 = new VoxelWorld.<>c__DisplayClass117_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.setDensityFunction = setDensityFunction;
			this.PrepForOperationOnChunks(worldBounds);
			this.ForEachChunk(VoxelWorld._opChunks, new Action(CS$<>8__locals1.<SetVoxelDensityCustom>g__SetVoxelDensityInChunk|0));
			this.FinalizeOperationOnChunks(immediate);
		}

		// Token: 0x06007DEB RID: 32235 RVA: 0x002942CE File Offset: 0x002924CE
		public void SetVoxelDataCustom(global::UnityEngine.BoundsInt worldBounds, [TupleElementNames(new string[] { "density", "material", "density", "material" })] Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>> setDataFunction, bool immediate = true)
		{
			VoxelWorld._opSetDataFunction = setDataFunction;
			this.PrepForOperationOnChunks(worldBounds);
			this.ForEachChunk(VoxelWorld._opChunks, new Action(this.<SetVoxelDataCustom>g__SetVoxelDataInChunk|118_0));
			this.FinalizeOperationOnChunks(immediate);
		}

		// Token: 0x06007DEC RID: 32236 RVA: 0x002942FC File Offset: 0x002924FC
		public void SetVoxelDataCustom(int3[] voxels, [TupleElementNames(new string[] { "density", "material", "density", "material" })] Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>> setDataFunction, bool immediate = true)
		{
			VoxelWorld.<>c__DisplayClass119_0 CS$<>8__locals1 = new VoxelWorld.<>c__DisplayClass119_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.voxels = voxels;
			CS$<>8__locals1.immediate = immediate;
			CS$<>8__locals1.setDataFunction = setDataFunction;
			global::UnityEngine.BoundsInt boundsFor = VoxelWorld.GetBoundsFor(CS$<>8__locals1.voxels);
			this.ForEachChunkInBounds(boundsFor, new Action(CS$<>8__locals1.<SetVoxelDataCustom>g__SetVoxelDataInChunk|0));
		}

		// Token: 0x06007DED RID: 32237 RVA: 0x0029434C File Offset: 0x0029254C
		public void SetVoxels(global::UnityEngine.BoundsInt bounds, Voxel[] voxels, bool immediate = true)
		{
			VoxelWorld.<>c__DisplayClass120_0 CS$<>8__locals1 = new VoxelWorld.<>c__DisplayClass120_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.immediate = immediate;
			CS$<>8__locals1.voxels = voxels;
			bounds.GetVoxelCount();
			this.ForEachChunkInBounds(bounds, new Action(CS$<>8__locals1.<SetVoxels>g__SetVoxelDataInChunk|0));
		}

		// Token: 0x06007DEE RID: 32238 RVA: 0x00294390 File Offset: 0x00292590
		public void SetVoxelDensity(global::UnityEngine.BoundsInt bounds, byte[] data, bool immediate = true)
		{
			VoxelWorld.<>c__DisplayClass121_0 CS$<>8__locals1 = new VoxelWorld.<>c__DisplayClass121_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.immediate = immediate;
			CS$<>8__locals1.data = data;
			this.ForEachChunkInBounds(bounds, new Action(CS$<>8__locals1.<SetVoxelDensity>g__SetVoxelDensityInChunk|0));
		}

		// Token: 0x06007DEF RID: 32239 RVA: 0x002943CC File Offset: 0x002925CC
		public byte GetVoxelMaterial(int3 voxelId)
		{
			int3 @int = voxelId.LocalPositionToChunkId(this.ChunkSize);
			Chunk chunk;
			if (!this.chunks.TryGetValue(@int, out chunk))
			{
				return 0;
			}
			int3 int2 = voxelId - chunk.Id * chunk.Size;
			int num = int2.x + this.VoxelDimension * (int2.y + this.VoxelDimension * int2.z);
			return chunk.Material[num];
		}

		// Token: 0x06007DF0 RID: 32240 RVA: 0x00294440 File Offset: 0x00292640
		public byte GetVoxelDensity(int3 voxelId)
		{
			int3 @int = voxelId.LocalPositionToChunkId(this.ChunkSize);
			Chunk chunk;
			if (!this.chunks.TryGetValue(@int, out chunk))
			{
				return 0;
			}
			int3 int2 = voxelId - chunk.Id * chunk.Size;
			int num = int2.x + this.VoxelDimension * (int2.y + this.VoxelDimension * int2.z);
			return chunk.Density[num];
		}

		// Token: 0x06007DF1 RID: 32241 RVA: 0x002944B4 File Offset: 0x002926B4
		public Voxel GetVoxelData(int3 voxelId)
		{
			int3 @int = voxelId.LocalPositionToChunkId(this.ChunkSize);
			Chunk chunk;
			if (!this.chunks.TryGetValue(@int, out chunk))
			{
				return default(Voxel);
			}
			int3 int2 = voxelId - chunk.Id * chunk.Size;
			int num = int2.x + this.VoxelDimension * (int2.y + this.VoxelDimension * int2.z);
			return new Voxel(chunk.Material[num], chunk.Density[num]);
		}

		// Token: 0x06007DF2 RID: 32242 RVA: 0x00294544 File Offset: 0x00292744
		public void SetVoxelMaterial(int3 voxelId, byte material)
		{
			int3 @int = voxelId.LocalPositionToChunkId(this.ChunkSize);
			Chunk chunk;
			if (!this.chunks.TryGetValue(@int, out chunk))
			{
				return;
			}
			int3 int2 = voxelId - chunk.Id * chunk.Size;
			int num = int2.x + this.VoxelDimension * (int2.y + this.VoxelDimension * int2.z);
			chunk.Material[num] = material;
		}

		// Token: 0x06007DF3 RID: 32243 RVA: 0x002945B8 File Offset: 0x002927B8
		public void SetVoxelDensity(int3 voxelId, byte density)
		{
			int3 @int = voxelId.LocalPositionToChunkId(this.ChunkSize);
			Chunk chunk;
			if (!this.chunks.TryGetValue(@int, out chunk))
			{
				return;
			}
			int3 int2 = voxelId - chunk.Id * chunk.Size;
			int num = int2.x + this.VoxelDimension * (int2.y + this.VoxelDimension * int2.z);
			chunk.Density[num] = density;
		}

		// Token: 0x06007DF4 RID: 32244 RVA: 0x0029462C File Offset: 0x0029282C
		public void SetVoxelData(int3 voxelId, Voxel data)
		{
			int3 @int = voxelId.LocalPositionToChunkId(this.ChunkSize);
			Chunk chunk;
			if (!this.chunks.TryGetValue(@int, out chunk))
			{
				return;
			}
			int3 int2 = voxelId - chunk.Id * chunk.Size;
			int num = int2.x + this.VoxelDimension * (int2.y + this.VoxelDimension * int2.z);
			chunk.Material[num] = data.Material;
			chunk.Density[num] = data.Density;
		}

		// Token: 0x06007DF5 RID: 32245 RVA: 0x002946B8 File Offset: 0x002928B8
		public static global::UnityEngine.BoundsInt GetBoundsFor(int3[] voxels)
		{
			int3 @int = new int3(int.MaxValue, int.MaxValue, int.MaxValue);
			int3 int2 = new int3(int.MinValue, int.MinValue, int.MinValue);
			foreach (int3 int3 in voxels)
			{
				@int = math.min(int3, @int);
				int2 = math.max(int3, int2);
			}
			return new global::UnityEngine.BoundsInt
			{
				min = @int.ToVectorInt(),
				max = int2.ToVectorInt()
			};
		}

		// Token: 0x06007DF6 RID: 32246 RVA: 0x0029473C File Offset: 0x0029293C
		[return: TupleElementNames(new string[] { "min", "max" })]
		public ValueTuple<int3, int3> GetChunkBoundsForLocalBounds(global::UnityEngine.BoundsInt worldBounds, bool includeLLC = false)
		{
			return new ValueTuple<int3, int3>((worldBounds.min - (includeLLC ? (Vector3Int.one * Chunk.Pad) : Vector3Int.zero)).LocalPositionToChunkId(this.ChunkSize), worldBounds.max.LocalPositionToChunkId(this.ChunkSize));
		}

		// Token: 0x06007DF7 RID: 32247 RVA: 0x00294790 File Offset: 0x00292990
		public bool BoundsChunksLoaded(global::UnityEngine.BoundsInt localWorldBounds, bool includeLLC = false)
		{
			if (!this.Initialized)
			{
				return false;
			}
			ValueTuple<int3, int3> chunkBoundsForLocalBounds = this.GetChunkBoundsForLocalBounds(localWorldBounds, includeLLC);
			int3 item = chunkBoundsForLocalBounds.Item1;
			int3 item2 = chunkBoundsForLocalBounds.Item2;
			for (int i = item.x; i <= item2.x; i++)
			{
				for (int j = item.y; j <= item2.y; j++)
				{
					for (int k = item.z; k <= item2.z; k++)
					{
						int3 @int = new int3(i, j, k);
						Chunk chunk;
						if (!this.chunks.TryGetValue(@int, out chunk))
						{
							return false;
						}
						if (chunk.State < ChunkState.MeshAssigned)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x06007DF8 RID: 32248 RVA: 0x0029482D File Offset: 0x00292A2D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Vector3Int ClampToWorldBounds(Vector3Int coord)
		{
			if (this.worldType == VoxelWorld.WorldType.Bounded)
			{
				coord.Clamp(this.worldBounds.min, this.worldBounds.max);
			}
			return coord;
		}

		// Token: 0x06007DF9 RID: 32249 RVA: 0x00294858 File Offset: 0x00292A58
		private void ForEachChunkInBounds(global::UnityEngine.BoundsInt bounds, Action action)
		{
			VoxelWorld._opBounds = bounds;
			int3 @int = this.ClampToWorldBounds(bounds.min - Vector3Int.one * Chunk.Pad).LocalPositionToChunkId(this.ChunkSize);
			int3 int2 = this.ClampToWorldBounds(bounds.max).LocalPositionToChunkId(this.ChunkSize);
			for (int i = @int.x; i <= int2.x; i++)
			{
				for (int j = @int.y; j <= int2.y; j++)
				{
					for (int k = @int.z; k <= int2.z; k++)
					{
						int3 int3 = new int3(i, j, k);
						Chunk chunk;
						if (this.chunks.TryGetValue(int3, out chunk))
						{
							VoxelWorld._opChunk = chunk;
							action();
						}
						else
						{
							Debug.LogError(string.Format("Couldn't find chunk {0} to perform operation", int3));
						}
					}
				}
			}
		}

		// Token: 0x06007DFA RID: 32250 RVA: 0x00294938 File Offset: 0x00292B38
		private void ForEachChunk(List<Chunk> opChunks, Action action)
		{
			foreach (Chunk chunk in opChunks)
			{
				VoxelWorld._opChunk = chunk;
				action();
			}
		}

		// Token: 0x06007DFB RID: 32251 RVA: 0x0029498C File Offset: 0x00292B8C
		public bool ChunksHaveJobs(global::UnityEngine.BoundsInt worldBounds)
		{
			this.GetChunksForBounds(worldBounds, ref this._tempChunkList);
			return this.ChunksHaveJobs(this._tempChunkList);
		}

		// Token: 0x06007DFC RID: 32252 RVA: 0x002949A8 File Offset: 0x00292BA8
		public bool ChunksHaveJobs(IList<Chunk> chunks)
		{
			foreach (Chunk chunk in chunks)
			{
				if (this.chunkJobs.ContainsKey(chunk.Id))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007DFD RID: 32253 RVA: 0x00294A04 File Offset: 0x00292C04
		public void GetChunksForBounds(global::UnityEngine.BoundsInt worldBounds, ref List<Chunk> list)
		{
			if (list == null)
			{
				list = new List<Chunk>();
			}
			list.Clear();
			int3 @int = this.ClampToWorldBounds(worldBounds.min - Vector3Int.one * Chunk.Pad).LocalPositionToChunkId(this.ChunkSize);
			int3 int2 = this.ClampToWorldBounds(worldBounds.max).LocalPositionToChunkId(this.ChunkSize);
			for (int i = @int.x; i <= int2.x; i++)
			{
				for (int j = @int.y; j <= int2.y; j++)
				{
					for (int k = @int.z; k <= int2.z; k++)
					{
						int3 int3 = new int3(i, j, k);
						Chunk chunk;
						if (this.chunks.TryGetValue(int3, out chunk))
						{
							list.Add(chunk);
						}
					}
				}
			}
		}

		// Token: 0x06007DFE RID: 32254 RVA: 0x00294AD4 File Offset: 0x00292CD4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Chunk GetChunkForLocalPosition(int3 worldPosition)
		{
			return this.chunks.GetValueOrDefault(worldPosition.LocalPositionToChunkId(this.ChunkSize));
		}

		// Token: 0x06007DFF RID: 32255 RVA: 0x00294AED File Offset: 0x00292CED
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Chunk GetChunkForLocalPosition(Vector3 worldPosition)
		{
			return this.chunks.GetValueOrDefault(worldPosition.LocalPositionToChunkId(this.ChunkSize));
		}

		// Token: 0x06007E00 RID: 32256 RVA: 0x00294B08 File Offset: 0x00292D08
		private void ForEachVoxelInChunkInBounds(global::UnityEngine.BoundsInt worldBounds, Chunk chunk, Action<int3, int3, int, byte> action)
		{
			int3 @int = new int3(worldBounds.min.x, worldBounds.min.y, worldBounds.min.z);
			int3 int2 = new int3(worldBounds.max.x, worldBounds.max.y, worldBounds.max.z);
			int3 int3 = chunk.Id * chunk.Size;
			int3 int4 = int3 + chunk.Dimensions - 1;
			@int.x = math.max(@int.x, int3.x);
			@int.y = math.max(@int.y, int3.y);
			@int.z = math.max(@int.z, int3.z);
			int2.x = math.min(int2.x, int4.x);
			int2.y = math.min(int2.y, int4.y);
			int2.z = math.min(int2.z, int4.z);
			if (@int.x > int2.x || @int.y > int2.y || @int.z > int2.z)
			{
				Debug.LogWarning(string.Format("No overlap between chunk {0} and bounds {1}", chunk.Id, worldBounds));
				return;
			}
			for (int i = @int.x; i <= int2.x; i++)
			{
				for (int j = @int.y; j <= int2.y; j++)
				{
					for (int k = @int.z; k <= int2.z; k++)
					{
						int3 int5 = new int3(i, j, k);
						int3 int6 = int5 - chunk.Id * chunk.Size;
						int num = int6.x + this.VoxelDimension * (int6.y + this.VoxelDimension * int6.z);
						byte b = chunk.Density[num];
						action(int5, int6, num, b);
					}
				}
			}
		}

		// Token: 0x06007E01 RID: 32257 RVA: 0x00294D48 File Offset: 0x00292F48
		private void ForEachVoxelInChunkInBounds(global::UnityEngine.BoundsInt worldBounds, Chunk chunk, Action<int3, int3, int, byte, byte> action)
		{
			int3 @int = new int3(worldBounds.min.x, worldBounds.min.y, worldBounds.min.z);
			int3 int2 = new int3(worldBounds.max.x, worldBounds.max.y, worldBounds.max.z);
			int3 int3 = chunk.Id * chunk.Size;
			int3 int4 = int3 + chunk.Dimensions - 1;
			@int.x = math.max(@int.x, int3.x);
			@int.y = math.max(@int.y, int3.y);
			@int.z = math.max(@int.z, int3.z);
			int2.x = math.min(int2.x, int4.x);
			int2.y = math.min(int2.y, int4.y);
			int2.z = math.min(int2.z, int4.z);
			if (@int.x > int2.x || @int.y > int2.y || @int.z > int2.z)
			{
				Debug.LogWarning(string.Format("No overlap between chunk {0} and bounds {1}", chunk.Id, worldBounds));
				return;
			}
			for (int i = @int.x; i <= int2.x; i++)
			{
				for (int j = @int.y; j <= int2.y; j++)
				{
					for (int k = @int.z; k <= int2.z; k++)
					{
						int3 int5 = new int3(i, j, k);
						int3 int6 = int5 - chunk.Id * chunk.Size;
						int num = int6.x + this.VoxelDimension * (int6.y + this.VoxelDimension * int6.z);
						byte b = chunk.Density[num];
						byte b2 = chunk.Material[num];
						action(int5, int6, num, b, b2);
					}
				}
			}
		}

		// Token: 0x06007E02 RID: 32258 RVA: 0x00294FA0 File Offset: 0x002931A0
		private void ForEachSpecifiedVoxelInChunk(int3[] voxels, Chunk chunk, Action<int3, int3, int, byte, byte> action)
		{
			int3 @int = chunk.Id * chunk.Size;
			int3 int2 = @int + chunk.Dimensions - 1;
			foreach (int3 int3 in voxels)
			{
				if (int3.IsInBounds(@int, int2))
				{
					int3 int4 = int3 - chunk.Id * chunk.Size;
					int num = int4.x + this.VoxelDimension * (int4.y + this.VoxelDimension * int4.z);
					byte b = chunk.Density[num];
					byte b2 = chunk.Material[num];
					action(int3, int4, num, b, b2);
				}
			}
		}

		// Token: 0x06007E03 RID: 32259 RVA: 0x0029506C File Offset: 0x0029326C
		private void HandleJobCompletion(ChunkTaskSet chunkTask)
		{
			if (!chunkTask.HasChunks)
			{
				Debug.LogError("Chunk is null in HandleJobCompletion");
			}
			foreach (Chunk chunk in chunkTask.Chunks)
			{
				if (!this.completedJobs.Contains(chunk.Id))
				{
					this.completedJobs.Add(chunk.Id);
				}
			}
		}

		// Token: 0x06007E04 RID: 32260 RVA: 0x002950F0 File Offset: 0x002932F0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte GetDensityAt(Vector3 voxelWorldPosition)
		{
			return this.GetDensityAt(voxelWorldPosition.ToInt3(), 0);
		}

		// Token: 0x06007E05 RID: 32261 RVA: 0x00295100 File Offset: 0x00293300
		public byte GetDensityAt(int3 voxelWorldPosition, byte defaultDensity = 0)
		{
			Chunk chunkForLocalPosition = this.GetChunkForLocalPosition(voxelWorldPosition);
			if (chunkForLocalPosition != null && chunkForLocalPosition.IsDataGenerated)
			{
				int3 localPosition = chunkForLocalPosition.GetLocalPosition(voxelWorldPosition);
				return chunkForLocalPosition.Density[localPosition.x + this.VoxelDimension * (localPosition.y + this.VoxelDimension * localPosition.z)];
			}
			return defaultDensity;
		}

		// Token: 0x06007E06 RID: 32262 RVA: 0x00295157 File Offset: 0x00293357
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetDensityAt(Vector3 voxelWorldPosition, byte density)
		{
			this.SetDensityAt(voxelWorldPosition.ToInt3(), density);
		}

		// Token: 0x06007E07 RID: 32263 RVA: 0x00295168 File Offset: 0x00293368
		public void SetDensityAt(int3 voxelWorldPosition, byte density)
		{
			Chunk chunkForLocalPosition = this.GetChunkForLocalPosition(voxelWorldPosition);
			if (chunkForLocalPosition != null && chunkForLocalPosition.IsDataGenerated)
			{
				int3 localPosition = chunkForLocalPosition.GetLocalPosition(voxelWorldPosition);
				int num = localPosition.x + this.VoxelDimension * (localPosition.y + this.VoxelDimension * localPosition.z);
				if (chunkForLocalPosition.Density[num] != density)
				{
					chunkForLocalPosition.Density[num] = density;
					chunkForLocalPosition.IsDataChanged = true;
					chunkForLocalPosition.IsMeshCreated = false;
					chunkForLocalPosition.IsDirty = true;
					return;
				}
			}
			else
			{
				Debug.LogWarning(string.Format("No chunk found for world position {0}, cannot set density.", voxelWorldPosition));
			}
		}

		// Token: 0x06007E08 RID: 32264 RVA: 0x002951FC File Offset: 0x002933FC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetLocalPosition(Vector3 worldPosition)
		{
			return Matrix4x4.TRS(this.root.position, this.root.rotation, Vector3.one * this.worldScale).inverse.MultiplyPoint(worldPosition);
		}

		// Token: 0x06007E09 RID: 32265 RVA: 0x00295248 File Offset: 0x00293448
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetWorldPosition(Vector3 localPosition)
		{
			return Matrix4x4.TRS(this.root.position, this.root.rotation, Vector3.one * this.worldScale).MultiplyPoint(localPosition);
		}

		// Token: 0x06007E0A RID: 32266 RVA: 0x00295289 File Offset: 0x00293489
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetWorldPosition(int3 localPosition)
		{
			return this.GetWorldPosition(localPosition.ToVector3());
		}

		// Token: 0x06007E0B RID: 32267 RVA: 0x00295297 File Offset: 0x00293497
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3 GetVoxelForWorldPosition(Vector3 worldPosition)
		{
			return this.GetLocalPosition(worldPosition).RoundToInt();
		}

		// Token: 0x06007E0C RID: 32268 RVA: 0x002952A5 File Offset: 0x002934A5
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3 GetVoxelForLocalPosition(Vector3 localPosition)
		{
			return localPosition.RoundToInt();
		}

		// Token: 0x17000C4E RID: 3150
		// (get) Token: 0x06007E0D RID: 32269 RVA: 0x002952AD File Offset: 0x002934AD
		public float Scale
		{
			get
			{
				return this.worldScale;
			}
		}

		// Token: 0x06007E0E RID: 32270 RVA: 0x002952B8 File Offset: 0x002934B8
		private void OnDrawGizmos()
		{
			if (this.worldType == VoxelWorld.WorldType.Bounded)
			{
				Transform transform = this.Root;
				Matrix4x4 matrix4x = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one * this.worldScale);
				Vector3 vector = this.worldBounds.min;
				Vector3 vector2 = this.worldBounds.max;
				Vector3 vector3 = (vector + vector2) / 2f;
				Vector3 vector4 = vector2 - vector;
				Gizmos.color = Color.green;
				Gizmos.DrawLine(base.transform.position, matrix4x.MultiplyPoint(vector3));
				Gizmos.matrix = matrix4x;
				Gizmos.DrawWireCube(vector3, vector4);
			}
			VoxelGenerator voxelGenerator = this.generator;
			if (voxelGenerator == null)
			{
				return;
			}
			voxelGenerator.DrawGizmos(this);
		}

		// Token: 0x06007E16 RID: 32278 RVA: 0x00295500 File Offset: 0x00293700
		[CompilerGenerated]
		private void <SetVoxelDataCustom>g__SetVoxelDataInChunk|118_0()
		{
			VoxelWorld._opAnyChanged = false;
			this.ForEachVoxelInChunkInBounds(VoxelWorld._opBounds, VoxelWorld._opChunk, new Action<int3, int3, int, byte, byte>(VoxelWorld.<SetVoxelDataCustom>g__SetVoxelData|118_1));
			if (VoxelWorld._opAnyChanged)
			{
				VoxelWorld._opChunk.IsDataChanged = true;
				VoxelWorld._opChangedChunks.Add(VoxelWorld._opChunk);
			}
		}

		// Token: 0x06007E17 RID: 32279 RVA: 0x00295550 File Offset: 0x00293750
		[CompilerGenerated]
		internal static void <SetVoxelDataCustom>g__SetVoxelData|118_1(int3 voxelWorldPosition, int3 voxelLocalPosition, int voxelIndex, byte density, byte material)
		{
			ValueTuple<byte, byte> valueTuple = VoxelWorld._opSetDataFunction(voxelWorldPosition, new ValueTuple<byte, byte>(density, material));
			byte item = valueTuple.Item1;
			byte item2 = valueTuple.Item2;
			if (item != density || item2 != material)
			{
				VoxelWorld._opChunk.Density[voxelIndex] = item;
				VoxelWorld._opChunk.Material[voxelIndex] = item2;
				VoxelWorld._opAnyChanged = true;
			}
		}

		// Token: 0x0400909D RID: 37021
		private static readonly Dictionary<int, VoxelWorld> WorldLookup = new Dictionary<int, VoxelWorld>();

		// Token: 0x0400909E RID: 37022
		[Header("World Settings")]
		public VoxelMaterialSet MaterialSet;

		// Token: 0x0400909F RID: 37023
		[SerializeReference]
		public VoxelGenerator generator;

		// Token: 0x040090A0 RID: 37024
		[SerializeField]
		private VoxelWorld.WorldType worldType;

		// Token: 0x040090A1 RID: 37025
		[SerializeField]
		private global::UnityEngine.BoundsInt worldBounds;

		// Token: 0x040090A2 RID: 37026
		[SerializeField]
		private float worldScale = 1f;

		// Token: 0x040090A3 RID: 37027
		[SerializeField]
		private int chunkSize = 32;

		// Token: 0x040090A4 RID: 37028
		[SerializeField]
		private int viewDistance = 5;

		// Token: 0x040090A5 RID: 37029
		[SerializeField]
		private int maxJobs = 10;

		// Token: 0x040090A6 RID: 37030
		[SerializeField]
		private bool registerAsSceneWorld = true;

		// Token: 0x040090A7 RID: 37031
		[SerializeField]
		private bool persistChanges = true;

		// Token: 0x040090A8 RID: 37032
		[Header("References")]
		[SerializeField]
		private ChunkComponent chunkPrefab;

		// Token: 0x040090A9 RID: 37033
		[SerializeField]
		private Transform target;

		// Token: 0x040090AA RID: 37034
		[SerializeField]
		private Transform root;

		// Token: 0x040090AB RID: 37035
		protected Dictionary<int3, Chunk> chunks = new Dictionary<int3, Chunk>();

		// Token: 0x040090AC RID: 37036
		private Dictionary<int3, ChunkTaskSet> chunkJobs = new Dictionary<int3, ChunkTaskSet>();

		// Token: 0x040090AD RID: 37037
		private List<int3> completedJobs = new List<int3>();

		// Token: 0x040090AE RID: 37038
		protected NativeHashSet<int3> chunksToGenerate;

		// Token: 0x040090AF RID: 37039
		protected NativeList<int3> sortedChunks;

		// Token: 0x040090B0 RID: 37040
		protected int chunkSortIndex;

		// Token: 0x040090B1 RID: 37041
		protected JobHandle sortJobHandle;

		// Token: 0x040090B2 RID: 37042
		protected int sortedChunkCount;

		// Token: 0x040090B3 RID: 37043
		protected int3 playerChunk = new int3(int.MaxValue, int.MaxValue, int.MaxValue);

		// Token: 0x040090B4 RID: 37044
		protected bool generationQueueChanged;

		// Token: 0x040090B5 RID: 37045
		private List<int3> chunksToRemove = new List<int3>();

		// Token: 0x040090B6 RID: 37046
		private global::UnityEngine.Pool.ObjectPool<Chunk> _chunkPool;

		// Token: 0x040090B7 RID: 37047
		private global::UnityEngine.Pool.ObjectPool<ChunkComponent> _chunkComponentPool;

		// Token: 0x040090B8 RID: 37048
		private global::UnityEngine.Pool.ObjectPool<Mesh> _meshPool;

		// Token: 0x040090BB RID: 37051
		private bool _updateWorld = true;

		// Token: 0x040090BF RID: 37055
		private static global::UnityEngine.BoundsInt _opBounds;

		// Token: 0x040090C0 RID: 37056
		private static List<Chunk> _opChunks = new List<Chunk>();

		// Token: 0x040090C1 RID: 37057
		private static List<Chunk> _opChangedChunks = new List<Chunk>();

		// Token: 0x040090C2 RID: 37058
		private static List<ChunkTaskSet> _opChunkJobs = new List<ChunkTaskSet>();

		// Token: 0x040090C3 RID: 37059
		private static Chunk _opChunk;

		// Token: 0x040090C4 RID: 37060
		private static bool _opAnyChanged;

		// Token: 0x040090C5 RID: 37061
		[TupleElementNames(new string[] { "density", "material", "density", "material" })]
		private static Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>> _opSetDataFunction;

		// Token: 0x040090C6 RID: 37062
		private List<Chunk> _tempChunkList;

		// Token: 0x020013B1 RID: 5041
		public enum WorldType
		{
			// Token: 0x040090C8 RID: 37064
			Infinite,
			// Token: 0x040090C9 RID: 37065
			Bounded
		}
	}
}
