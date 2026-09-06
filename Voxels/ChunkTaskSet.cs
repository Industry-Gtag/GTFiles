using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;

namespace Voxels
{
	// Token: 0x02001381 RID: 4993
	public class ChunkTaskSet
	{
		// Token: 0x17000C37 RID: 3127
		// (get) Token: 0x06007D0F RID: 32015 RVA: 0x0028E3F3 File Offset: 0x0028C5F3
		public Chunk Chunk
		{
			get
			{
				return this.Chunks[0];
			}
		}

		// Token: 0x17000C38 RID: 3128
		// (get) Token: 0x06007D10 RID: 32016 RVA: 0x0028E404 File Offset: 0x0028C604
		public bool HasChunks
		{
			get
			{
				if (this.Chunks == null || this.Chunks.Count == 0)
				{
					return false;
				}
				using (List<Chunk>.Enumerator enumerator = this.Chunks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == null)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		// Token: 0x17000C39 RID: 3129
		// (get) Token: 0x06007D11 RID: 32017 RVA: 0x0028E470 File Offset: 0x0028C670
		public bool IsEmpty
		{
			get
			{
				return !this.Current.IsCreated && this.Callback == null && this.Tasks.Count == 0;
			}
		}

		// Token: 0x06007D12 RID: 32018 RVA: 0x0028E497 File Offset: 0x0028C697
		public ChunkTaskSet(VoxelGenerator generator)
		{
			this.Chunks = new List<Chunk>();
			this.Generator = generator;
			this.Tasks = new Queue<ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>>();
		}

		// Token: 0x06007D13 RID: 32019 RVA: 0x0028E4BC File Offset: 0x0028C6BC
		public ChunkTaskSet(Chunk chunk, VoxelGenerator parameters, [TupleElementNames(new string[] { "task", "callback" })] params ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>[] tasks)
		{
			this.Chunks = new List<Chunk> { chunk };
			this.Generator = parameters;
			this.Tasks = new Queue<ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>>(tasks);
		}

		// Token: 0x06007D14 RID: 32020 RVA: 0x0028E4E9 File Offset: 0x0028C6E9
		public ChunkTaskSet(IList<Chunk> chunks, VoxelGenerator parameters, [TupleElementNames(new string[] { "task", "callback" })] params ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>[] tasks)
		{
			this.Chunks = new List<Chunk>(chunks);
			this.Generator = parameters;
			this.Tasks = new Queue<ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>>(tasks);
		}

		// Token: 0x06007D15 RID: 32021 RVA: 0x0028E510 File Offset: 0x0028C710
		public void AddTask(ChunkTaskSet.ChunkTaskDelegate task, Action<Chunk> callback = null)
		{
			this.Tasks.Enqueue(new ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>(task, callback));
		}

		// Token: 0x06007D16 RID: 32022 RVA: 0x0028E524 File Offset: 0x0028C724
		public void Start()
		{
			if (this.Current.IsCreated || this.Callback != null)
			{
				throw new InvalidOperationException("Cannot start a ChunkTaskSet that is already running.");
			}
			this.StartNext();
			this.UpdateDirty();
		}

		// Token: 0x06007D17 RID: 32023 RVA: 0x0028E553 File Offset: 0x0028C753
		public void Complete()
		{
			this.CompleteCurrent();
			while (this.StartNext())
			{
				this.CompleteCurrent();
			}
			this.UpdateDirty();
		}

		// Token: 0x06007D18 RID: 32024 RVA: 0x0028E571 File Offset: 0x0028C771
		public bool CompleteIfReady()
		{
			if (this.CompleteCurrentIfReady())
			{
				if (this.Tasks.Count == 0)
				{
					this.UpdateDirty();
					return true;
				}
				this.StartNext();
			}
			this.UpdateDirty();
			return false;
		}

		// Token: 0x06007D19 RID: 32025 RVA: 0x0028E59E File Offset: 0x0028C79E
		private bool CompleteCurrentIfReady()
		{
			if (this.Current.IsCompleted)
			{
				this.CompleteCurrent();
				return true;
			}
			return false;
		}

		// Token: 0x06007D1A RID: 32026 RVA: 0x0028E5B8 File Offset: 0x0028C7B8
		private void CompleteCurrent()
		{
			this.Current.Complete();
			foreach (Chunk chunk in this.Chunks)
			{
				Action<Chunk> callback = this.Callback;
				if (callback != null)
				{
					callback(chunk);
				}
			}
			this.Current = default(ChunkTask);
			this.Callback = null;
		}

		// Token: 0x06007D1B RID: 32027 RVA: 0x0028E634 File Offset: 0x0028C834
		private bool StartNext()
		{
			if (this.Tasks.Count == 0)
			{
				return false;
			}
			ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>> valueTuple = this.Tasks.Dequeue();
			ValueTuple<ChunkTask, Action<Chunk>> valueTuple2 = this.CreateTask(valueTuple);
			this.Current = valueTuple2.Item1;
			this.Callback = valueTuple2.Item2;
			if (this.Current.IsCompleted)
			{
				this.CompleteCurrent();
				return this.StartNext();
			}
			return true;
		}

		// Token: 0x06007D1C RID: 32028 RVA: 0x0028E698 File Offset: 0x0028C898
		private void UpdateDirty()
		{
			if (this.Current.IsCreated || this.Callback != null || this.Tasks.Count > 0)
			{
				using (List<Chunk>.Enumerator enumerator = this.Chunks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Chunk chunk = enumerator.Current;
						chunk.IsDirty = false;
					}
					return;
				}
			}
			foreach (Chunk chunk2 in this.Chunks)
			{
				chunk2.IsDirty = chunk2.State < ChunkState.MeshAssigned;
			}
		}

		// Token: 0x06007D1D RID: 32029 RVA: 0x0028E754 File Offset: 0x0028C954
		private ValueTuple<ChunkTask, Action<Chunk>> CreateTask([TupleElementNames(new string[] { "task", "callback" })] ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>> task)
		{
			return this.CreateTask(task.Item1, task.Item2);
		}

		// Token: 0x06007D1E RID: 32030 RVA: 0x0028E768 File Offset: 0x0028C968
		private ValueTuple<ChunkTask, Action<Chunk>> CreateTask(ChunkTaskSet.ChunkTaskDelegate task, Action<Chunk> callback = null)
		{
			if (this.Chunks.Count == 1)
			{
				return new ValueTuple<ChunkTask, Action<Chunk>>((task != null) ? task(this.Chunks[0]) : default(ChunkTask), callback);
			}
			NativeArray<JobHandle> nativeArray = new NativeArray<JobHandle>(this.Chunks.Count, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < this.Chunks.Count; i++)
			{
				nativeArray[i] = ((task != null) ? task(this.Chunks[i]).Handle : default(JobHandle));
			}
			JobHandle jobHandle = JobHandle.CombineDependencies(nativeArray);
			nativeArray.Dispose();
			return new ValueTuple<ChunkTask, Action<Chunk>>(new ChunkTask(this.Chunks[0], jobHandle, null), callback);
		}

		// Token: 0x04008FDE RID: 36830
		public List<Chunk> Chunks;

		// Token: 0x04008FDF RID: 36831
		public VoxelGenerator Generator;

		// Token: 0x04008FE0 RID: 36832
		[TupleElementNames(new string[] { "task", "callback" })]
		public Queue<ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>> Tasks;

		// Token: 0x04008FE1 RID: 36833
		public ChunkTask Current;

		// Token: 0x04008FE2 RID: 36834
		public Action<Chunk> Callback;

		// Token: 0x02001382 RID: 4994
		// (Invoke) Token: 0x06007D20 RID: 32032
		public delegate ChunkTask ChunkTaskDelegate(Chunk chunk);
	}
}
