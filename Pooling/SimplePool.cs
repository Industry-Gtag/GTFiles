using System;
using UnityEngine.Pool;

namespace Pooling
{
	// Token: 0x02000F17 RID: 3863
	public static class SimplePool<T> where T : class, new()
	{
		// Token: 0x06005EC7 RID: 24263 RVA: 0x001E3382 File Offset: 0x001E1582
		public static T Get()
		{
			return SimplePool<T>.s_Pool.Get();
		}

		// Token: 0x06005EC8 RID: 24264 RVA: 0x001E338E File Offset: 0x001E158E
		public static PooledObject<T> Get(out T value)
		{
			return SimplePool<T>.s_Pool.Get(out value);
		}

		// Token: 0x06005EC9 RID: 24265 RVA: 0x001E339B File Offset: 0x001E159B
		public static void Release(T toRelease)
		{
			SimplePool<T>.s_Pool.Release(toRelease);
		}

		// Token: 0x04006D8A RID: 28042
		private static readonly ObjectPool<T> s_Pool = new ObjectPool<T>(() => new T(), null, null, null, true, 10, 10000);
	}
}
