using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200122C RID: 4652
	public class ListProcessor<T>
	{
		// Token: 0x17000B6E RID: 2926
		// (get) Token: 0x060075E2 RID: 30178 RVA: 0x0026530E File Offset: 0x0026350E
		public int Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		// Token: 0x17000B6F RID: 2927
		// (get) Token: 0x060075E3 RID: 30179 RVA: 0x0026531B File Offset: 0x0026351B
		// (set) Token: 0x060075E4 RID: 30180 RVA: 0x00265323 File Offset: 0x00263523
		public InAction<T> ItemProcessor
		{
			get
			{
				return this.m_itemProcessorDelegate;
			}
			set
			{
				this.m_itemProcessorDelegate = value;
			}
		}

		// Token: 0x060075E5 RID: 30181 RVA: 0x0026532C File Offset: 0x0026352C
		public ListProcessor()
			: this(10, null)
		{
		}

		// Token: 0x060075E6 RID: 30182 RVA: 0x00265337 File Offset: 0x00263537
		public ListProcessor(int capacity, InAction<T> itemProcessorDelegate = null)
		{
			this.m_list = new List<T>(capacity);
			this.m_currentIndex = -1;
			this.m_listCount = -1;
			this.m_itemProcessorDelegate = itemProcessorDelegate;
		}

		// Token: 0x060075E7 RID: 30183 RVA: 0x00265360 File Offset: 0x00263560
		public virtual void Add(in T item)
		{
			this.m_listCount++;
			this.m_list.Add(item);
		}

		// Token: 0x060075E8 RID: 30184 RVA: 0x00265384 File Offset: 0x00263584
		public virtual bool Remove(in T item)
		{
			int num = this.m_list.IndexOf(item);
			if (num < 0)
			{
				return false;
			}
			if (num < this.m_currentIndex)
			{
				this.m_currentIndex--;
			}
			this.m_listCount--;
			this.m_list.RemoveAt(num);
			return true;
		}

		// Token: 0x060075E9 RID: 30185 RVA: 0x002653DB File Offset: 0x002635DB
		public void Clear()
		{
			this.m_list.Clear();
			this.m_currentIndex = -1;
			this.m_listCount = -1;
		}

		// Token: 0x060075EA RID: 30186 RVA: 0x002653F6 File Offset: 0x002635F6
		public bool Contains(in T item)
		{
			return this.m_list.Contains(item);
		}

		// Token: 0x060075EB RID: 30187 RVA: 0x00265409 File Offset: 0x00263609
		public virtual void ProcessListSafe()
		{
			this.ProcessListSafe(this.m_itemProcessorDelegate);
		}

		// Token: 0x060075EC RID: 30188 RVA: 0x00265418 File Offset: 0x00263618
		public virtual void ProcessListSafe(InAction<T> customDelegate)
		{
			if (customDelegate == null)
			{
				Debug.LogError("ListProcessor: ItemProcessor is null");
				return;
			}
			this.m_listCount = this.m_list.Count;
			this.m_currentIndex = 0;
			while (this.m_currentIndex < this.m_listCount)
			{
				try
				{
					T t = this.m_list[this.m_currentIndex];
					customDelegate(in t);
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.ToString());
				}
				this.m_currentIndex++;
			}
		}

		// Token: 0x060075ED RID: 30189 RVA: 0x002654A4 File Offset: 0x002636A4
		public virtual void ProcessList()
		{
			this.ProcessList(this.m_itemProcessorDelegate);
		}

		// Token: 0x060075EE RID: 30190 RVA: 0x002654B4 File Offset: 0x002636B4
		public virtual void ProcessList(InAction<T> customDelegate)
		{
			if (customDelegate == null)
			{
				Debug.LogError("ListProcessor: ItemProcessor is null");
				return;
			}
			this.m_listCount = this.m_list.Count;
			this.m_currentIndex = 0;
			while (this.m_currentIndex < this.m_listCount)
			{
				T t = this.m_list[this.m_currentIndex];
				customDelegate(in t);
				this.m_currentIndex++;
			}
		}

		// Token: 0x060075EF RID: 30191 RVA: 0x0026551F File Offset: 0x0026371F
		public IReadOnlyList<T> GetReadonlyList()
		{
			return this.m_list;
		}

		// Token: 0x0400859C RID: 34204
		protected readonly List<T> m_list;

		// Token: 0x0400859D RID: 34205
		protected int m_currentIndex;

		// Token: 0x0400859E RID: 34206
		protected int m_listCount;

		// Token: 0x0400859F RID: 34207
		protected InAction<T> m_itemProcessorDelegate;
	}
}
