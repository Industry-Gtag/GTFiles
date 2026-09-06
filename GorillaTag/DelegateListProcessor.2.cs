using System;

namespace GorillaTag
{
	// Token: 0x02001228 RID: 4648
	public class DelegateListProcessor<T> : DelegateListProcessorPlusMinus<DelegateListProcessor<T>, Action<T>>
	{
		// Token: 0x060075CA RID: 30154 RVA: 0x00265142 File Offset: 0x00263342
		public DelegateListProcessor()
		{
		}

		// Token: 0x060075CB RID: 30155 RVA: 0x0026514A File Offset: 0x0026334A
		public DelegateListProcessor(int capacity)
			: base(capacity)
		{
		}

		// Token: 0x060075CC RID: 30156 RVA: 0x00265153 File Offset: 0x00263353
		public void InvokeSafe(in T data)
		{
			this.m_data = data;
			this.ProcessListSafe();
			this.m_data = default(T);
		}

		// Token: 0x060075CD RID: 30157 RVA: 0x00265173 File Offset: 0x00263373
		public void Invoke(in T data)
		{
			this.m_data = data;
			this.ProcessList();
			this.m_data = default(T);
		}

		// Token: 0x060075CE RID: 30158 RVA: 0x00265193 File Offset: 0x00263393
		protected override void ProcessItem(in Action<T> item)
		{
			item(this.m_data);
		}

		// Token: 0x04008596 RID: 34198
		private T m_data;
	}
}
