using System;
using UnityEngine;

// Token: 0x02000D1E RID: 3358
[Serializable]
public class CallLimiterWithCooldown : CallLimiter
{
	// Token: 0x06005345 RID: 21317 RVA: 0x001B75C5 File Offset: 0x001B57C5
	public CallLimiterWithCooldown(float coolDownSpam, int historyLength, float coolDown)
		: base(historyLength, coolDown, 0.5f)
	{
		this.spamCoolDown = coolDownSpam;
	}

	// Token: 0x06005346 RID: 21318 RVA: 0x001B75DB File Offset: 0x001B57DB
	public CallLimiterWithCooldown(float coolDownSpam, int historyLength, float coolDown, float latencyMax)
		: base(historyLength, coolDown, latencyMax)
	{
		this.spamCoolDown = coolDownSpam;
	}

	// Token: 0x06005347 RID: 21319 RVA: 0x001B75EE File Offset: 0x001B57EE
	public override CallLimiter GetCopy()
	{
		return new CallLimiterWithCooldown(this.spamCoolDown, this.callHistoryLength, this.timeCooldown, (float)this.maxLatency);
	}

	// Token: 0x06005348 RID: 21320 RVA: 0x001B760E File Offset: 0x001B580E
	public override bool CheckCallTime(float time)
	{
		if (this.blockCall && time < this.blockStartTime + this.spamCoolDown)
		{
			this.blockStartTime = time;
			return false;
		}
		return base.CheckCallTime(time);
	}

	// Token: 0x040064E5 RID: 25829
	[SerializeField]
	private float spamCoolDown;
}
