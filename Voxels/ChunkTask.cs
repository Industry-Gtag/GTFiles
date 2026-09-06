using System;
using Unity.Jobs;

namespace Voxels
{
	// Token: 0x02001383 RID: 4995
	public struct ChunkTask
	{
		// Token: 0x17000C3A RID: 3130
		// (get) Token: 0x06007D23 RID: 32035 RVA: 0x0028E82C File Offset: 0x0028CA2C
		public bool IsCreated
		{
			get
			{
				return !this.Handle.Equals(default(JobHandle));
			}
		}

		// Token: 0x17000C3B RID: 3131
		// (get) Token: 0x06007D24 RID: 32036 RVA: 0x0028E850 File Offset: 0x0028CA50
		public bool IsCompleted
		{
			get
			{
				return this.Handle.IsCompleted;
			}
		}

		// Token: 0x06007D25 RID: 32037 RVA: 0x0028E85D File Offset: 0x0028CA5D
		public bool CompleteIfReady()
		{
			if (this.Handle.IsCompleted)
			{
				this.Complete();
				return true;
			}
			return false;
		}

		// Token: 0x06007D26 RID: 32038 RVA: 0x0028E875 File Offset: 0x0028CA75
		public void Complete()
		{
			this.Handle.Complete();
			Action onJobComplete = this._onJobComplete;
			if (onJobComplete != null)
			{
				onJobComplete();
			}
			this._onJobComplete = null;
		}

		// Token: 0x06007D27 RID: 32039 RVA: 0x0028E89A File Offset: 0x0028CA9A
		public ChunkTask(Chunk chunk, JobHandle handle, Action onComplete = null)
		{
			this.Chunk = chunk;
			this.Handle = handle;
			this._onJobComplete = onComplete;
		}

		// Token: 0x06007D28 RID: 32040 RVA: 0x0028E8B4 File Offset: 0x0028CAB4
		public static ChunkTask CreateCollisionJob(Chunk chunk)
		{
			if (chunk.Mesh == null)
			{
				chunk.IsCollisionBaked = true;
				chunk.IsDirty = true;
				return default(ChunkTask);
			}
			JobHandle jobHandle = new CollisionJob
			{
				MeshId = chunk.Mesh.GetEntityId()
			}.Schedule(default(JobHandle));
			Action action = delegate
			{
				chunk.IsCollisionBaked = true;
				chunk.IsDirty = true;
			};
			return new ChunkTask
			{
				Handle = jobHandle,
				_onJobComplete = action,
				Chunk = chunk
			};
		}

		// Token: 0x04008FE3 RID: 36835
		public Chunk Chunk;

		// Token: 0x04008FE4 RID: 36836
		public JobHandle Handle;

		// Token: 0x04008FE5 RID: 36837
		private Action _onJobComplete;
	}
}
