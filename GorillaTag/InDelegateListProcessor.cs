using System;

namespace GorillaTag
{
	// Token: 0x0200122A RID: 4650
	public class InDelegateListProcessor<T> : DelegateListProcessorPlusMinus<InDelegateListProcessor<T>, InAction<T>>
	{
		// Token: 0x060075D6 RID: 30166 RVA: 0x00265228 File Offset: 0x00263428
		public InDelegateListProcessor()
		{
		}

		// Token: 0x060075D7 RID: 30167 RVA: 0x00265230 File Offset: 0x00263430
		public InDelegateListProcessor(int capacity)
			: base(capacity)
		{
		}

		// Token: 0x060075D8 RID: 30168 RVA: 0x00265239 File Offset: 0x00263439
		public void InvokeSafe(in T data)
		{
			this.m_data = data;
			this.ProcessListSafe();
			this.m_data = default(T);
		}

		// Token: 0x060075D9 RID: 30169 RVA: 0x00265259 File Offset: 0x00263459
		public void Invoke(in T data)
		{
			this.m_data = data;
			this.ProcessList();
			this.m_data = default(T);
		}

		// Token: 0x060075DA RID: 30170 RVA: 0x00265279 File Offset: 0x00263479
		protected override void ProcessItem(in InAction<T> item)
		{
			item(in this.m_data);
		}

		// Token: 0x04008599 RID: 34201
		private T m_data;
	}
}
