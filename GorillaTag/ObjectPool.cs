using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x0200122F RID: 4655
	public class ObjectPool<T> where T : ObjectPoolEvents, new()
	{
		// Token: 0x060075F5 RID: 30197 RVA: 0x0026555F File Offset: 0x0026375F
		protected ObjectPool()
		{
		}

		// Token: 0x060075F6 RID: 30198 RVA: 0x00265572 File Offset: 0x00263772
		public ObjectPool(int amount)
			: this(amount, amount)
		{
		}

		// Token: 0x060075F7 RID: 30199 RVA: 0x0026557C File Offset: 0x0026377C
		public ObjectPool(int initialAmount, int maxAmount)
		{
			this.InitializePool(initialAmount, maxAmount);
		}

		// Token: 0x060075F8 RID: 30200 RVA: 0x00265598 File Offset: 0x00263798
		protected void InitializePool(int initialAmount, int maxAmount)
		{
			this.maxInstances = maxAmount;
			this.pool = new Stack<T>(initialAmount);
			for (int i = 0; i < initialAmount; i++)
			{
				this.pool.Push(this.CreateInstance());
			}
		}

		// Token: 0x060075F9 RID: 30201 RVA: 0x002655D8 File Offset: 0x002637D8
		public T Take()
		{
			T t;
			if (this.pool.Count < 1)
			{
				t = this.CreateInstance();
			}
			else
			{
				t = this.pool.Pop();
			}
			t.OnTaken();
			return t;
		}

		// Token: 0x060075FA RID: 30202 RVA: 0x00265616 File Offset: 0x00263816
		public void Return(T instance)
		{
			instance.OnReturned();
			if (this.pool.Count == this.maxInstances)
			{
				return;
			}
			this.pool.Push(instance);
		}

		// Token: 0x060075FB RID: 30203 RVA: 0x001E33DB File Offset: 0x001E15DB
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual T CreateInstance()
		{
			return new T();
		}

		// Token: 0x040085A0 RID: 34208
		private Stack<T> pool;

		// Token: 0x040085A1 RID: 34209
		public int maxInstances = 500;
	}
}
