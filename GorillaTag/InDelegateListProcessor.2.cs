using System;

namespace GorillaTag
{
	// Token: 0x0200122B RID: 4651
	public class InDelegateListProcessor<T1, T2> : DelegateListProcessorPlusMinus<InDelegateListProcessor<T1, T2>, InAction<T1, T2>>
	{
		// Token: 0x060075DB RID: 30171 RVA: 0x00265288 File Offset: 0x00263488
		public InDelegateListProcessor()
		{
		}

		// Token: 0x060075DC RID: 30172 RVA: 0x00265290 File Offset: 0x00263490
		public InDelegateListProcessor(int capacity)
			: base(capacity)
		{
		}

		// Token: 0x060075DD RID: 30173 RVA: 0x00265299 File Offset: 0x00263499
		public void InvokeSafe(in T1 data1, in T2 data2)
		{
			this.SetData(in data1, in data2);
			this.ProcessListSafe();
			this.ResetData();
		}

		// Token: 0x060075DE RID: 30174 RVA: 0x002652AF File Offset: 0x002634AF
		public void Invoke(in T1 data1, in T2 data2)
		{
			this.SetData(in data1, in data2);
			this.ProcessList();
			this.ResetData();
		}

		// Token: 0x060075DF RID: 30175 RVA: 0x002652C5 File Offset: 0x002634C5
		protected override void ProcessItem(in InAction<T1, T2> item)
		{
			item(in this.m_data1, in this.m_data2);
		}

		// Token: 0x060075E0 RID: 30176 RVA: 0x002652DA File Offset: 0x002634DA
		private void SetData(in T1 data1, in T2 data2)
		{
			this.m_data1 = data1;
			this.m_data2 = data2;
		}

		// Token: 0x060075E1 RID: 30177 RVA: 0x002652F4 File Offset: 0x002634F4
		private void ResetData()
		{
			this.m_data1 = default(T1);
			this.m_data2 = default(T2);
		}

		// Token: 0x0400859A RID: 34202
		private T1 m_data1;

		// Token: 0x0400859B RID: 34203
		private T2 m_data2;
	}
}
