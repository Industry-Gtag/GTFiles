using System;
using GorillaTag;

// Token: 0x02000D91 RID: 3473
internal class CallbackContainer<T> : ListProcessorAbstract<T> where T : ICallBack
{
	// Token: 0x06005573 RID: 21875 RVA: 0x001BF89C File Offset: 0x001BDA9C
	public CallbackContainer()
		: base(100)
	{
	}

	// Token: 0x06005574 RID: 21876 RVA: 0x001BF8A6 File Offset: 0x001BDAA6
	public CallbackContainer(int capacity)
		: base(capacity)
	{
	}

	// Token: 0x06005575 RID: 21877 RVA: 0x001BF8AF File Offset: 0x001BDAAF
	public void TryRunCallbacks()
	{
		this.ProcessListSafe();
	}

	// Token: 0x06005576 RID: 21878 RVA: 0x001BF8B7 File Offset: 0x001BDAB7
	public void RunCallbacks()
	{
		this.ProcessList();
	}

	// Token: 0x06005577 RID: 21879 RVA: 0x001BF8C0 File Offset: 0x001BDAC0
	protected override void ProcessItem(in T item)
	{
		T t = item;
		t.CallBack();
	}
}
