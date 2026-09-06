using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Voxels
{
	// Token: 0x02001389 RID: 5001
	public struct NativeCounter : IDisposable
	{
		// Token: 0x17000C3C RID: 3132
		// (get) Token: 0x06007D34 RID: 32052 RVA: 0x0028F180 File Offset: 0x0028D380
		// (set) Token: 0x06007D35 RID: 32053 RVA: 0x0028F189 File Offset: 0x0028D389
		public unsafe int Count
		{
			get
			{
				return *this._counter;
			}
			set
			{
				*this._counter = value;
			}
		}

		// Token: 0x06007D36 RID: 32054 RVA: 0x0028F193 File Offset: 0x0028D393
		public unsafe NativeCounter(Allocator allocator)
		{
			this._allocator = allocator;
			this._counter = (int*)UnsafeUtility.Malloc(4L, 4, this._allocator);
			this.Count = 0;
		}

		// Token: 0x06007D37 RID: 32055 RVA: 0x0028F1B7 File Offset: 0x0028D3B7
		public unsafe int Increment()
		{
			return Interlocked.Increment(ref *this._counter) - 1;
		}

		// Token: 0x06007D38 RID: 32056 RVA: 0x0028F1C6 File Offset: 0x0028D3C6
		public unsafe void Dispose()
		{
			UnsafeUtility.Free((void*)this._counter, this._allocator);
		}

		// Token: 0x04008FFB RID: 36859
		private readonly Allocator _allocator;

		// Token: 0x04008FFC RID: 36860
		[NativeDisableUnsafePtrRestriction]
		private unsafe readonly int* _counter;
	}
}
