using System;

// Token: 0x02000B1B RID: 2843
public struct TimeSince
{
	// Token: 0x170006E7 RID: 1767
	// (get) Token: 0x060048F2 RID: 18674 RVA: 0x00186FB0 File Offset: 0x001851B0
	public double secondsElapsed
	{
		get
		{
			double totalSeconds = (DateTime.UtcNow - this._dt).TotalSeconds;
			if (totalSeconds <= 2147483647.0)
			{
				return totalSeconds;
			}
			return 2147483647.0;
		}
	}

	// Token: 0x170006E8 RID: 1768
	// (get) Token: 0x060048F3 RID: 18675 RVA: 0x00186FED File Offset: 0x001851ED
	public float secondsElapsedFloat
	{
		get
		{
			return (float)this.secondsElapsed;
		}
	}

	// Token: 0x170006E9 RID: 1769
	// (get) Token: 0x060048F4 RID: 18676 RVA: 0x00186FF6 File Offset: 0x001851F6
	public int secondsElapsedInt
	{
		get
		{
			return (int)this.secondsElapsed;
		}
	}

	// Token: 0x170006EA RID: 1770
	// (get) Token: 0x060048F5 RID: 18677 RVA: 0x00186FFF File Offset: 0x001851FF
	public uint secondsElapsedUint
	{
		get
		{
			return (uint)this.secondsElapsed;
		}
	}

	// Token: 0x170006EB RID: 1771
	// (get) Token: 0x060048F6 RID: 18678 RVA: 0x00187008 File Offset: 0x00185208
	public long secondsElapsedLong
	{
		get
		{
			return (long)this.secondsElapsed;
		}
	}

	// Token: 0x170006EC RID: 1772
	// (get) Token: 0x060048F7 RID: 18679 RVA: 0x00187011 File Offset: 0x00185211
	public TimeSpan secondsElapsedSpan
	{
		get
		{
			return TimeSpan.FromSeconds(this.secondsElapsed);
		}
	}

	// Token: 0x060048F8 RID: 18680 RVA: 0x0018701E File Offset: 0x0018521E
	public TimeSince(DateTime dt)
	{
		this._dt = dt;
	}

	// Token: 0x060048F9 RID: 18681 RVA: 0x00187028 File Offset: 0x00185228
	public TimeSince(int elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x060048FA RID: 18682 RVA: 0x0018704C File Offset: 0x0018524C
	public TimeSince(uint elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds(-1.0 * elapsed);
	}

	// Token: 0x060048FB RID: 18683 RVA: 0x0018707C File Offset: 0x0018527C
	public TimeSince(float elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x060048FC RID: 18684 RVA: 0x001870A0 File Offset: 0x001852A0
	public TimeSince(double elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds(-elapsed);
	}

	// Token: 0x060048FD RID: 18685 RVA: 0x001870C4 File Offset: 0x001852C4
	public TimeSince(long elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x060048FE RID: 18686 RVA: 0x001870E8 File Offset: 0x001852E8
	public TimeSince(TimeSpan elapsed)
	{
		this._dt = DateTime.UtcNow.Add(-elapsed);
	}

	// Token: 0x060048FF RID: 18687 RVA: 0x0018710E File Offset: 0x0018530E
	public bool HasElapsed(int seconds)
	{
		return this.secondsElapsedInt >= seconds;
	}

	// Token: 0x06004900 RID: 18688 RVA: 0x0018711C File Offset: 0x0018531C
	public bool HasElapsed(uint seconds)
	{
		return this.secondsElapsedUint >= seconds;
	}

	// Token: 0x06004901 RID: 18689 RVA: 0x0018712A File Offset: 0x0018532A
	public bool HasElapsed(float seconds)
	{
		return this.secondsElapsedFloat >= seconds;
	}

	// Token: 0x06004902 RID: 18690 RVA: 0x00187138 File Offset: 0x00185338
	public bool HasElapsed(double seconds)
	{
		return this.secondsElapsed >= seconds;
	}

	// Token: 0x06004903 RID: 18691 RVA: 0x00187146 File Offset: 0x00185346
	public bool HasElapsed(long seconds)
	{
		return this.secondsElapsedLong >= seconds;
	}

	// Token: 0x06004904 RID: 18692 RVA: 0x00187154 File Offset: 0x00185354
	public bool HasElapsed(TimeSpan seconds)
	{
		return this.secondsElapsedSpan >= seconds;
	}

	// Token: 0x06004905 RID: 18693 RVA: 0x00187162 File Offset: 0x00185362
	public void Reset()
	{
		this._dt = DateTime.UtcNow;
	}

	// Token: 0x06004906 RID: 18694 RVA: 0x0018716F File Offset: 0x0018536F
	public bool HasElapsed(int seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedInt >= seconds;
		}
		if (this.secondsElapsedInt < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x06004907 RID: 18695 RVA: 0x00187193 File Offset: 0x00185393
	public bool HasElapsed(uint seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedUint >= seconds;
		}
		if (this.secondsElapsedUint < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x06004908 RID: 18696 RVA: 0x001871B7 File Offset: 0x001853B7
	public bool HasElapsed(float seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedFloat >= seconds;
		}
		if (this.secondsElapsedFloat < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x06004909 RID: 18697 RVA: 0x001871DB File Offset: 0x001853DB
	public bool HasElapsed(double seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsed >= seconds;
		}
		if (this.secondsElapsed < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x0600490A RID: 18698 RVA: 0x001871FF File Offset: 0x001853FF
	public bool HasElapsed(long seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedLong >= seconds;
		}
		if (this.secondsElapsedLong < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x0600490B RID: 18699 RVA: 0x00187223 File Offset: 0x00185423
	public bool HasElapsed(TimeSpan seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedSpan >= seconds;
		}
		if (this.secondsElapsedSpan < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x0600490C RID: 18700 RVA: 0x0018724C File Offset: 0x0018544C
	public override string ToString()
	{
		return string.Format("{0:F3} seconds since {{{1:s}", this.secondsElapsed, this._dt);
	}

	// Token: 0x0600490D RID: 18701 RVA: 0x0018726E File Offset: 0x0018546E
	public override int GetHashCode()
	{
		return StaticHash.Compute(this._dt);
	}

	// Token: 0x0600490E RID: 18702 RVA: 0x0018727B File Offset: 0x0018547B
	public static TimeSince Now()
	{
		return new TimeSince(DateTime.UtcNow);
	}

	// Token: 0x0600490F RID: 18703 RVA: 0x00187287 File Offset: 0x00185487
	public static implicit operator long(TimeSince ts)
	{
		return ts.secondsElapsedLong;
	}

	// Token: 0x06004910 RID: 18704 RVA: 0x00187290 File Offset: 0x00185490
	public static implicit operator double(TimeSince ts)
	{
		return ts.secondsElapsed;
	}

	// Token: 0x06004911 RID: 18705 RVA: 0x00187299 File Offset: 0x00185499
	public static implicit operator float(TimeSince ts)
	{
		return ts.secondsElapsedFloat;
	}

	// Token: 0x06004912 RID: 18706 RVA: 0x001872A2 File Offset: 0x001854A2
	public static implicit operator int(TimeSince ts)
	{
		return ts.secondsElapsedInt;
	}

	// Token: 0x06004913 RID: 18707 RVA: 0x001872AB File Offset: 0x001854AB
	public static implicit operator uint(TimeSince ts)
	{
		return ts.secondsElapsedUint;
	}

	// Token: 0x06004914 RID: 18708 RVA: 0x001872B4 File Offset: 0x001854B4
	public static implicit operator TimeSpan(TimeSince ts)
	{
		return ts.secondsElapsedSpan;
	}

	// Token: 0x06004915 RID: 18709 RVA: 0x001872BD File Offset: 0x001854BD
	public static implicit operator TimeSince(int elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06004916 RID: 18710 RVA: 0x001872C5 File Offset: 0x001854C5
	public static implicit operator TimeSince(uint elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06004917 RID: 18711 RVA: 0x001872CD File Offset: 0x001854CD
	public static implicit operator TimeSince(float elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06004918 RID: 18712 RVA: 0x001872D5 File Offset: 0x001854D5
	public static implicit operator TimeSince(double elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06004919 RID: 18713 RVA: 0x001872DD File Offset: 0x001854DD
	public static implicit operator TimeSince(long elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x0600491A RID: 18714 RVA: 0x001872E5 File Offset: 0x001854E5
	public static implicit operator TimeSince(TimeSpan elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x0600491B RID: 18715 RVA: 0x001872ED File Offset: 0x001854ED
	public static implicit operator TimeSince(DateTime dt)
	{
		return new TimeSince(dt);
	}

	// Token: 0x04005B39 RID: 23353
	private DateTime _dt;

	// Token: 0x04005B3A RID: 23354
	private const double INT32_MAX = 2147483647.0;
}
