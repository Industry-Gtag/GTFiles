using System;
using UnityEngine;

// Token: 0x02000E21 RID: 3617
public class TimeOfDayEvent : TimeEvent
{
	// Token: 0x1700086F RID: 2159
	// (get) Token: 0x06005889 RID: 22665 RVA: 0x001CBEB5 File Offset: 0x001CA0B5
	public float currentTime
	{
		get
		{
			return this._currentTime;
		}
	}

	// Token: 0x17000870 RID: 2160
	// (get) Token: 0x0600588A RID: 22666 RVA: 0x001CBEBD File Offset: 0x001CA0BD
	// (set) Token: 0x0600588B RID: 22667 RVA: 0x001CBEC5 File Offset: 0x001CA0C5
	public float timeStart
	{
		get
		{
			return this._timeStart;
		}
		set
		{
			this._timeStart = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17000871 RID: 2161
	// (get) Token: 0x0600588C RID: 22668 RVA: 0x001CBED3 File Offset: 0x001CA0D3
	// (set) Token: 0x0600588D RID: 22669 RVA: 0x001CBEDB File Offset: 0x001CA0DB
	public float timeEnd
	{
		get
		{
			return this._timeEnd;
		}
		set
		{
			this._timeEnd = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17000872 RID: 2162
	// (get) Token: 0x0600588E RID: 22670 RVA: 0x001CBEE9 File Offset: 0x001CA0E9
	public bool isOngoing
	{
		get
		{
			return this._ongoing;
		}
	}

	// Token: 0x0600588F RID: 22671 RVA: 0x001CBEF4 File Offset: 0x001CA0F4
	private void Start()
	{
		if (!this._dayNightManager)
		{
			this._dayNightManager = BetterDayNightManager.instance;
		}
		if (!this._dayNightManager)
		{
			return;
		}
		for (int i = 0; i < this._dayNightManager.timeOfDayRange.Length; i++)
		{
			this._totalSecondsInRange += this._dayNightManager.timeOfDayRange[i] * 3600.0;
		}
		this._totalSecondsInRange = Math.Floor(this._totalSecondsInRange);
	}

	// Token: 0x06005890 RID: 22672 RVA: 0x001CBF76 File Offset: 0x001CA176
	private void Update()
	{
		this._elapsed += Time.deltaTime;
		if (this._elapsed < 1f)
		{
			return;
		}
		this._elapsed = 0f;
		this.UpdateTime();
	}

	// Token: 0x06005891 RID: 22673 RVA: 0x001CBFAC File Offset: 0x001CA1AC
	private void UpdateTime()
	{
		this._currentSeconds = ((ITimeOfDaySystem)this._dayNightManager).currentTimeInSeconds;
		this._currentSeconds = Math.Floor(this._currentSeconds);
		this._currentTime = (float)(this._currentSeconds / this._totalSecondsInRange);
		bool flag = this._currentTime >= 0f && this._currentTime >= this._timeStart && this._currentTime <= this._timeEnd;
		if (!this._ongoing && flag)
		{
			base.StartEvent();
		}
		if (this._ongoing && !flag)
		{
			base.StopEvent();
		}
	}

	// Token: 0x06005892 RID: 22674 RVA: 0x001CC043 File Offset: 0x001CA243
	public static implicit operator bool(TimeOfDayEvent ev)
	{
		return ev && ev.isOngoing;
	}

	// Token: 0x040068B9 RID: 26809
	[SerializeField]
	[Range(0f, 1f)]
	private float _timeStart;

	// Token: 0x040068BA RID: 26810
	[SerializeField]
	[Range(0f, 1f)]
	private float _timeEnd = 1f;

	// Token: 0x040068BB RID: 26811
	[SerializeField]
	private float _currentTime = -1f;

	// Token: 0x040068BC RID: 26812
	[Space]
	[SerializeField]
	private double _currentSeconds = -1.0;

	// Token: 0x040068BD RID: 26813
	[SerializeField]
	private double _totalSecondsInRange = -1.0;

	// Token: 0x040068BE RID: 26814
	[NonSerialized]
	private float _elapsed = -1f;

	// Token: 0x040068BF RID: 26815
	[SerializeField]
	private BetterDayNightManager _dayNightManager;
}
