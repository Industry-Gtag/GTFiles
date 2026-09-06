using System;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000D1F RID: 3359
[Serializable]
public class CallLimitersList<Titem, Tenum> where Titem : CallLimiter, new() where Tenum : Enum
{
	// Token: 0x06005349 RID: 21321 RVA: 0x00002050 File Offset: 0x00000250
	public CallLimitersList()
	{
	}

	// Token: 0x0600534A RID: 21322 RVA: 0x001B7638 File Offset: 0x001B5838
	public CallLimitersList(CallLimitersList<Titem, Tenum> source)
	{
		Titem[] callLimiters = source.m_callLimiters;
		this.m_callLimiters = new Titem[callLimiters.Length];
		for (int i = 0; i < this.m_callLimiters.Length; i++)
		{
			Titem titem = callLimiters[i];
			this.m_callLimiters[i] = (Titem)((object)titem.GetCopy());
		}
	}

	// Token: 0x0600534B RID: 21323 RVA: 0x001B7697 File Offset: 0x001B5897
	public CallLimitersList<Titem, Tenum> GetCopy()
	{
		return new CallLimitersList<Titem, Tenum>(this);
	}

	// Token: 0x0600534C RID: 21324 RVA: 0x001B769F File Offset: 0x001B589F
	public bool IsSpamming(Tenum index)
	{
		return this.IsSpamming((int)((object)index));
	}

	// Token: 0x0600534D RID: 21325 RVA: 0x001B76B2 File Offset: 0x001B58B2
	public bool IsSpamming(int index)
	{
		return !this.m_callLimiters[index].CheckCallTime(Time.unscaledTime);
	}

	// Token: 0x0600534E RID: 21326 RVA: 0x001B76D2 File Offset: 0x001B58D2
	public bool IsSpamming(Tenum index, double serverTime)
	{
		return this.IsSpamming((int)((object)index), serverTime);
	}

	// Token: 0x0600534F RID: 21327 RVA: 0x001B76E6 File Offset: 0x001B58E6
	public bool IsSpamming(int index, double serverTime)
	{
		return !this.m_callLimiters[index].CheckCallServerTime(serverTime);
	}

	// Token: 0x06005350 RID: 21328 RVA: 0x001B7704 File Offset: 0x001B5904
	public void Reset()
	{
		Titem[] callLimiters = this.m_callLimiters;
		for (int i = 0; i < callLimiters.Length; i++)
		{
			callLimiters[i].Reset();
		}
	}

	// Token: 0x040064E6 RID: 25830
	[RequiredListLength("GetMaxLength")]
	[SerializeField]
	private Titem[] m_callLimiters;
}
