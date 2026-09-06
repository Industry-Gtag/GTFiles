using System;

namespace GorillaTag
{
	// Token: 0x0200122D RID: 4653
	public abstract class ListProcessorAbstract<T> : ListProcessor<T>
	{
		// Token: 0x060075F0 RID: 30192 RVA: 0x00265527 File Offset: 0x00263727
		protected ListProcessorAbstract()
		{
			this.m_itemProcessorDelegate = new InAction<T>(this.ProcessItem);
		}

		// Token: 0x060075F1 RID: 30193 RVA: 0x00265542 File Offset: 0x00263742
		protected ListProcessorAbstract(int capacity)
			: base(capacity, null)
		{
			this.m_itemProcessorDelegate = new InAction<T>(this.ProcessItem);
		}

		// Token: 0x060075F2 RID: 30194
		protected abstract void ProcessItem(in T item);
	}
}
