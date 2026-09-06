using System;

// Token: 0x02000D93 RID: 3475
internal class CallbackContainerUnique<T> : CallbackContainer<T> where T : class, ICallbackUnique
{
	// Token: 0x0600557A RID: 21882 RVA: 0x001BF8E1 File Offset: 0x001BDAE1
	public CallbackContainerUnique()
		: base(10)
	{
	}

	// Token: 0x0600557B RID: 21883 RVA: 0x001BF8EB File Offset: 0x001BDAEB
	public CallbackContainerUnique(int capacity)
		: base(capacity)
	{
	}

	// Token: 0x0600557C RID: 21884 RVA: 0x001BF8F4 File Offset: 0x001BDAF4
	public override void Add(in T item)
	{
		T t = item;
		if (t.Registered)
		{
			return;
		}
		base.Add(in item);
		t = item;
		t.Registered = true;
	}

	// Token: 0x0600557D RID: 21885 RVA: 0x001BF934 File Offset: 0x001BDB34
	public override bool Remove(in T item)
	{
		T t = item;
		if (!t.Registered)
		{
			return false;
		}
		base.Remove(in item);
		t = item;
		t.Registered = false;
		return true;
	}
}
