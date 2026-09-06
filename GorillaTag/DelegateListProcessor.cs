using System;

namespace GorillaTag
{
	// Token: 0x02001227 RID: 4647
	public class DelegateListProcessor : DelegateListProcessorPlusMinus<DelegateListProcessor, Action>
	{
		// Token: 0x060075C5 RID: 30149 RVA: 0x00265118 File Offset: 0x00263318
		public DelegateListProcessor()
		{
		}

		// Token: 0x060075C6 RID: 30150 RVA: 0x00265120 File Offset: 0x00263320
		public DelegateListProcessor(int capacity)
			: base(capacity)
		{
		}

		// Token: 0x060075C7 RID: 30151 RVA: 0x00265129 File Offset: 0x00263329
		public void Invoke()
		{
			this.ProcessList();
		}

		// Token: 0x060075C8 RID: 30152 RVA: 0x00265131 File Offset: 0x00263331
		public void InvokeSafe()
		{
			this.ProcessListSafe();
		}

		// Token: 0x060075C9 RID: 30153 RVA: 0x00265139 File Offset: 0x00263339
		protected override void ProcessItem(in Action del)
		{
			del();
		}
	}
}
