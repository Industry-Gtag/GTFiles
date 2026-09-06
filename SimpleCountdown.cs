using System;
using System.Threading.Tasks;
using GorillaNetworking;
using GorillaNetworking.ScheduledEvents;
using PlayFab;
using TMPro;
using UnityEngine;

// Token: 0x020001D8 RID: 472
[RequireComponent(typeof(TextMeshPro))]
public class SimpleCountdown : ObservableBehavior
{
	// Token: 0x06000CA0 RID: 3232 RVA: 0x00045374 File Offset: 0x00043574
	private async void Start()
	{
		this.tmp = base.GetComponent<TextMeshPro>();
		switch (this.mode)
		{
		case SimpleCountdown.Mode.TitleData:
			while (PlayFabTitleDataCache.Instance == null)
			{
				await Task.Yield();
			}
			PlayFabTitleDataCache.Instance.GetTitleData(this.titleDataKey, new Action<string>(this.onTD), new Action<PlayFabError>(this.onTDError), false);
			break;
		case SimpleCountdown.Mode.FixedDate:
			this.ParseDateTime();
			break;
		case SimpleCountdown.Mode.TimeSync:
			if (GorillaComputer.instance != null)
			{
				DateTime serverTime = GorillaComputer.instance.GetServerTime();
				this.dt = this.timeSyncRule.GetPrevious(serverTime);
			}
			break;
		}
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x000453AB File Offset: 0x000435AB
	private void onTD(string s)
	{
		this.date = s;
		this.ParseDateTime();
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x000453BC File Offset: 0x000435BC
	private void onTDError(PlayFabError error)
	{
		Debug.Log(string.Concat(new string[] { "SimpleCountdown component on ", base.name, " failed to get '", this.titleDataKey, "' from title data. Using Fallback: '", this.date, "'" }));
		this.ParseDateTime();
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x0004541C File Offset: 0x0004361C
	private void ParseDateTime()
	{
		if (!DateTime.TryParse(this.date, out this.dt))
		{
			Debug.Log(string.Concat(new string[] { "SimpleCountdown component on ", base.name, " has an unparsable date string: '", this.date, "'" }));
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x00045484 File Offset: 0x00043684
	protected override void ObservableSliceUpdate()
	{
		if (GorillaComputer.instance == null)
		{
			return;
		}
		DateTime dateTime = this.dt;
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		TimeSpan timeSpan;
		if (this.overrideDt < serverTime)
		{
			if (this.overrideDt > DateTime.MinValue)
			{
				Action manualCountdownComplete = this.ManualCountdownComplete;
				if (manualCountdownComplete != null)
				{
					manualCountdownComplete();
				}
				this.overrideDt = DateTime.MinValue;
			}
			if (this.mode == SimpleCountdown.Mode.TimeSync)
			{
				this.dt = this.timeSyncRule.GetNext(serverTime);
			}
			else if (this.mode == SimpleCountdown.Mode.ScheduledEvent)
			{
				double num = ((ScheduledEventManager.Instance != null && ScheduledEventManager.Instance.SecondsUntilEventStart > 0.0) ? ScheduledEventManager.Instance.SecondsUntilEventStart : 0.0);
				this.dt = serverTime.AddSeconds(num);
			}
			timeSpan = this.dt - serverTime;
		}
		else
		{
			timeSpan = this.overrideDt - serverTime;
		}
		if (timeSpan.TotalHours <= (double)this.hourRange.x || timeSpan.TotalHours >= (double)this.hourRange.y)
		{
			timeSpan = timeSpan.Multiply(0.0);
		}
		switch (this.displayFormat)
		{
		case SimpleCountdown.DisplayFormat.DD_HH_MM_SS:
			this.tmp.text = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", new object[] { timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds });
			return;
		case SimpleCountdown.DisplayFormat.HH_MM_SS:
			this.tmp.text = string.Format("{0:00}:{1:00}:{2:00}", Math.Floor(timeSpan.TotalHours), timeSpan.Minutes, timeSpan.Seconds);
			return;
		case SimpleCountdown.DisplayFormat.DD_HH_MM:
			this.tmp.text = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
			return;
		case SimpleCountdown.DisplayFormat.HH_MM:
			this.tmp.text = string.Format("{0:00}:{1:00}", Math.Floor(timeSpan.TotalHours), timeSpan.Minutes);
			return;
		case SimpleCountdown.DisplayFormat.MM_SS:
			this.tmp.text = string.Format("{0:00}:{1:00}", Math.Floor(timeSpan.TotalMinutes), timeSpan.Seconds);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void OnBecameObservable()
	{
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void OnLostObservable()
	{
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x00045714 File Offset: 0x00043914
	public void StartCountdown(int seconds)
	{
		this.overrideDt = GorillaComputer.instance.GetServerTime().AddSeconds((double)seconds);
	}

	// Token: 0x04000F59 RID: 3929
	[SerializeField]
	private SimpleCountdown.DisplayFormat displayFormat;

	// Token: 0x04000F5A RID: 3930
	[SerializeField]
	private SimpleCountdown.Mode mode = SimpleCountdown.Mode.TitleData;

	// Token: 0x04000F5B RID: 3931
	[SerializeField]
	private string titleDataKey;

	// Token: 0x04000F5C RID: 3932
	[SerializeField]
	private string date;

	// Token: 0x04000F5D RID: 3933
	[SerializeField]
	private ServerTimeSyncRule timeSyncRule;

	// Token: 0x04000F5E RID: 3934
	[SerializeField]
	private Vector2 hourRange = new Vector2(float.MinValue, float.MaxValue);

	// Token: 0x04000F5F RID: 3935
	private DateTime dt;

	// Token: 0x04000F60 RID: 3936
	private TextMeshPro tmp;

	// Token: 0x04000F61 RID: 3937
	private DateTime overrideDt = DateTime.MinValue;

	// Token: 0x04000F62 RID: 3938
	public Action ManualCountdownComplete;

	// Token: 0x020001D9 RID: 473
	private enum Mode
	{
		// Token: 0x04000F64 RID: 3940
		None,
		// Token: 0x04000F65 RID: 3941
		TitleData,
		// Token: 0x04000F66 RID: 3942
		FixedDate,
		// Token: 0x04000F67 RID: 3943
		TimeSync,
		// Token: 0x04000F68 RID: 3944
		ScheduledEvent
	}

	// Token: 0x020001DA RID: 474
	private enum DisplayFormat
	{
		// Token: 0x04000F6A RID: 3946
		DD_HH_MM_SS,
		// Token: 0x04000F6B RID: 3947
		HH_MM_SS,
		// Token: 0x04000F6C RID: 3948
		DD_HH_MM,
		// Token: 0x04000F6D RID: 3949
		HH_MM,
		// Token: 0x04000F6E RID: 3950
		MM_SS
	}
}
