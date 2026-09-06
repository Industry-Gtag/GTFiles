using System;

namespace GorillaTag
{
	// Token: 0x02001229 RID: 4649
	public class DelegateListProcessor<T1, T2> : DelegateListProcessorPlusMinus<DelegateListProcessor<T1, T2>, Action<T1, T2>>
	{
		// Token: 0x060075CF RID: 30159 RVA: 0x002651A2 File Offset: 0x002633A2
		public DelegateListProcessor()
		{
		}

		// Token: 0x060075D0 RID: 30160 RVA: 0x002651AA File Offset: 0x002633AA
		public DelegateListProcessor(int capacity)
			: base(capacity)
		{
		}

		// Token: 0x060075D1 RID: 30161 RVA: 0x002651B3 File Offset: 0x002633B3
		public void InvokeSafe(in T1 data1, in T2 data2)
		{
			this.SetData(in data1, in data2);
			this.ProcessListSafe();
			this.ResetData();
		}

		// Token: 0x060075D2 RID: 30162 RVA: 0x002651C9 File Offset: 0x002633C9
		public void Invoke(in T1 data1, in T2 data2)
		{
			this.SetData(in data1, in data2);
			this.ProcessList();
			this.ResetData();
		}

		// Token: 0x060075D3 RID: 30163 RVA: 0x002651DF File Offset: 0x002633DF
		protected override void ProcessItem(in Action<T1, T2> item)
		{
			item(this.m_data1, this.m_data2);
		}

		// Token: 0x060075D4 RID: 30164 RVA: 0x002651F4 File Offset: 0x002633F4
		private void SetData(in T1 data1, in T2 data2)
		{
			this.m_data1 = data1;
			this.m_data2 = data2;
		}

		// Token: 0x060075D5 RID: 30165 RVA: 0x0026520E File Offset: 0x0026340E
		private void ResetData()
		{
			this.m_data1 = default(T1);
			this.m_data2 = default(T2);
		}

		// Token: 0x04008597 RID: 34199
		private T1 m_data1;

		// Token: 0x04008598 RID: 34200
		private T2 m_data2;
	}
}
