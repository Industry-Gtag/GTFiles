using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000D1D RID: 3357
[Serializable]
public class CallLimiter
{
	// Token: 0x0600533F RID: 21311 RVA: 0x00002050 File Offset: 0x00000250
	public CallLimiter()
	{
	}

	// Token: 0x06005340 RID: 21312 RVA: 0x001B73E0 File Offset: 0x001B55E0
	public CallLimiter(int historyLength, float coolDown, float latencyMax = 0.5f)
	{
		this.callTimeHistory = new float[historyLength];
		this.callHistoryLength = historyLength;
		for (int i = 0; i < historyLength; i++)
		{
			this.callTimeHistory[i] = float.MinValue;
		}
		this.timeCooldown = coolDown;
		this.maxLatency = (double)latencyMax;
	}

	// Token: 0x06005341 RID: 21313 RVA: 0x001B742E File Offset: 0x001B562E
	public virtual CallLimiter GetCopy()
	{
		return new CallLimiter(this.callHistoryLength, this.timeCooldown, (float)this.maxLatency);
	}

	// Token: 0x06005342 RID: 21314 RVA: 0x001B7448 File Offset: 0x001B5648
	public bool CheckCallServerTime(double time)
	{
		double currentTime = PhotonNetwork.CurrentTime;
		double num = this.maxLatency;
		double num2 = 4294967.295 - this.maxLatency;
		double num3;
		if (currentTime > num || time < num)
		{
			if (time > currentTime + 0.05)
			{
				return false;
			}
			num3 = currentTime - time;
		}
		else
		{
			double num4 = num2 + currentTime;
			if (time > currentTime + 0.5 && time < num4)
			{
				return false;
			}
			num3 = currentTime + (4294967.295 - time);
		}
		if (num3 > this.maxLatency)
		{
			return false;
		}
		int num5 = ((this.oldTimeIndex > 0) ? (this.oldTimeIndex - 1) : (this.callHistoryLength - 1));
		double num6 = (double)this.callTimeHistory[num5];
		if (num6 > num2 && time < num6)
		{
			this.Reset();
		}
		else if (time < num6)
		{
			return false;
		}
		return this.CheckCallTime((float)time);
	}

	// Token: 0x06005343 RID: 21315 RVA: 0x001B7510 File Offset: 0x001B5710
	public virtual bool CheckCallTime(float time)
	{
		if (this.callTimeHistory[this.oldTimeIndex] > time)
		{
			this.blockCall = true;
			this.blockStartTime = time;
			return false;
		}
		this.callTimeHistory[this.oldTimeIndex] = time + this.timeCooldown;
		int num = this.oldTimeIndex + 1;
		this.oldTimeIndex = num;
		this.oldTimeIndex = num % this.callHistoryLength;
		this.blockCall = false;
		return true;
	}

	// Token: 0x06005344 RID: 21316 RVA: 0x001B7578 File Offset: 0x001B5778
	public virtual void Reset()
	{
		if (this.callTimeHistory == null)
		{
			return;
		}
		for (int i = 0; i < this.callHistoryLength; i++)
		{
			this.callTimeHistory[i] = float.MinValue;
		}
		this.oldTimeIndex = 0;
		this.blockStartTime = 0f;
		this.blockCall = false;
	}

	// Token: 0x040064DD RID: 25821
	protected const double k_serverMaxTime = 4294967.295;

	// Token: 0x040064DE RID: 25822
	[SerializeField]
	protected float[] callTimeHistory;

	// Token: 0x040064DF RID: 25823
	[Space]
	[SerializeField]
	protected int callHistoryLength;

	// Token: 0x040064E0 RID: 25824
	[SerializeField]
	protected float timeCooldown;

	// Token: 0x040064E1 RID: 25825
	[SerializeField]
	protected double maxLatency;

	// Token: 0x040064E2 RID: 25826
	private int oldTimeIndex;

	// Token: 0x040064E3 RID: 25827
	protected bool blockCall;

	// Token: 0x040064E4 RID: 25828
	protected float blockStartTime;
}
