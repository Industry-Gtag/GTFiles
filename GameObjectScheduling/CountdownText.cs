using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace GameObjectScheduling
{
	// Token: 0x020013F1 RID: 5105
	public class CountdownText : MonoBehaviour
	{
		// Token: 0x17000C5C RID: 3164
		// (get) Token: 0x060080C0 RID: 32960 RVA: 0x0029DB39 File Offset: 0x0029BD39
		private bool ShouldLocalize
		{
			get
			{
				return this.shouldLocalize && (this._locTextComp != null && this._countdownLocStr != null && this._timeCountdownVar != null && this._timescaleCountdownVar != null) && this._isValidVar != null;
			}
		}

		// Token: 0x17000C5D RID: 3165
		// (get) Token: 0x060080C1 RID: 32961 RVA: 0x0029DB76 File Offset: 0x0029BD76
		// (set) Token: 0x060080C2 RID: 32962 RVA: 0x0029DB80 File Offset: 0x0029BD80
		public CountdownTextDate Countdown
		{
			get
			{
				return this.CountdownTo;
			}
			set
			{
				this.CountdownTo = value;
				if (this.CountdownTo.FormatString.Length > 0)
				{
					this.displayTextFormat = this.CountdownTo.FormatString;
				}
				this.displayText.text = this.CountdownTo.DefaultString;
				if (base.gameObject.activeInHierarchy && !this.useExternalTime && this.monitor == null && this.CountdownTo != null)
				{
					this.monitor = base.StartCoroutine(this.MonitorTime());
				}
			}
		}

		// Token: 0x060080C3 RID: 32963 RVA: 0x0029DC0C File Offset: 0x0029BE0C
		private void Awake()
		{
			this.displayText = base.GetComponent<TMP_Text>();
			this.displayTextFormat = string.Empty;
			this.displayText.text = string.Empty;
			if (this.CountdownTo == null)
			{
				return;
			}
			if (this.displayTextFormat.Length == 0 && this.CountdownTo.FormatString.Length > 0)
			{
				this.displayTextFormat = this.CountdownTo.FormatString;
			}
			this.displayText.text = this.CountdownTo.DefaultString;
			if (!this.shouldLocalize)
			{
				return;
			}
			this._locTextComp = base.GetComponent<LocalizedText>();
			if (this._locTextComp == null)
			{
				Debug.LogError("[LOCALIZATION::COUNTDOWN_TEXT] There is no [LocalizedText] component on [" + base.name + "]!", this);
				return;
			}
			this._countdownLocStr = this._locTextComp.StringReference;
			if (this._locTextComp.StringReference == null || this._locTextComp.StringReference.IsEmpty)
			{
				Debug.LogError("[LOCALIZATION::COUNTDOWN_TEXT] There is no [StringReference] assigned on [" + base.name + "]!", this);
				return;
			}
			this._timeCountdownVar = this._countdownLocStr["time-value"] as IntVariable;
			this._timescaleCountdownVar = this._countdownLocStr["timescale-index"] as IntVariable;
			this._isValidVar = this._countdownLocStr["is-valid"] as BoolVariable;
		}

		// Token: 0x060080C4 RID: 32964 RVA: 0x0029DD72 File Offset: 0x0029BF72
		private void OnEnable()
		{
			if (this.CountdownTo == null)
			{
				return;
			}
			if (this.monitor == null && !this.useExternalTime)
			{
				this.monitor = base.StartCoroutine(this.MonitorTime());
			}
		}

		// Token: 0x060080C5 RID: 32965 RVA: 0x0029DDA5 File Offset: 0x0029BFA5
		private void OnDisable()
		{
			this.StopMonitorTime();
			this.StopDisplayRefresh();
		}

		// Token: 0x060080C6 RID: 32966 RVA: 0x0029DDB3 File Offset: 0x0029BFB3
		private IEnumerator MonitorTime()
		{
			while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
			{
				yield return null;
			}
			this.monitor = null;
			this.targetTime = this.TryParseDateTime();
			if (this.updateDisplay)
			{
				this.StartDisplayRefresh();
			}
			else
			{
				this.RefreshDisplay();
			}
			yield break;
		}

		// Token: 0x060080C7 RID: 32967 RVA: 0x0029DDC2 File Offset: 0x0029BFC2
		private IEnumerator MonitorExternalTime(DateTime countdown)
		{
			while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
			{
				yield return null;
			}
			this.monitor = null;
			this.targetTime = countdown;
			if (this.updateDisplay)
			{
				this.StartDisplayRefresh();
			}
			else
			{
				this.RefreshDisplay();
			}
			yield break;
		}

		// Token: 0x060080C8 RID: 32968 RVA: 0x0029DDD8 File Offset: 0x0029BFD8
		private void StopMonitorTime()
		{
			if (this.monitor != null)
			{
				base.StopCoroutine(this.monitor);
			}
			this.monitor = null;
		}

		// Token: 0x060080C9 RID: 32969 RVA: 0x0029DDF5 File Offset: 0x0029BFF5
		public void SetCountdownTime(DateTime countdown)
		{
			this.StopMonitorTime();
			this.StopDisplayRefresh();
			this.monitor = base.StartCoroutine(this.MonitorExternalTime(countdown));
		}

		// Token: 0x060080CA RID: 32970 RVA: 0x0029DE16 File Offset: 0x0029C016
		public void SetFixedText(string text)
		{
			this.StopMonitorTime();
			this.StopDisplayRefresh();
			this.displayText.text = text;
		}

		// Token: 0x060080CB RID: 32971 RVA: 0x0029DE30 File Offset: 0x0029C030
		private void StartDisplayRefresh()
		{
			this.StopDisplayRefresh();
			this.displayRefresh = base.StartCoroutine(this.WaitForDisplayRefresh());
		}

		// Token: 0x060080CC RID: 32972 RVA: 0x0029DE4A File Offset: 0x0029C04A
		private void StopDisplayRefresh()
		{
			if (this.displayRefresh != null)
			{
				base.StopCoroutine(this.displayRefresh);
			}
			this.displayRefresh = null;
		}

		// Token: 0x060080CD RID: 32973 RVA: 0x0029DE67 File Offset: 0x0029C067
		private IEnumerator WaitForDisplayRefresh()
		{
			for (;;)
			{
				this.RefreshDisplay();
				TimeSpan timeSpan;
				if (this.countdownTime.Days > 0)
				{
					timeSpan = this.countdownTime - TimeSpan.FromDays((double)this.countdownTime.Days);
				}
				else if (this.countdownTime.Hours > 0)
				{
					timeSpan = this.countdownTime - TimeSpan.FromHours((double)this.countdownTime.Hours);
				}
				else if (this.countdownTime.Minutes > 0)
				{
					timeSpan = this.countdownTime - TimeSpan.FromMinutes((double)this.countdownTime.Minutes);
				}
				else
				{
					if (this.countdownTime.Seconds <= 0)
					{
						break;
					}
					timeSpan = this.countdownTime - TimeSpan.FromSeconds((double)this.countdownTime.Seconds);
				}
				yield return new WaitForSeconds((float)timeSpan.TotalSeconds);
			}
			yield break;
		}

		// Token: 0x060080CE RID: 32974 RVA: 0x0029DE78 File Offset: 0x0029C078
		private void RefreshDisplay()
		{
			this.countdownTime = this.targetTime.Subtract(GorillaComputer.instance.GetServerTime());
			ValueTuple<string, int, int, bool> timeDisplay = CountdownText.GetTimeDisplay(this.countdownTime, this.displayTextFormat, this.CountdownTo.DaysThreshold, string.Empty, this.CountdownTo.DefaultString);
			string item = timeDisplay.Item1;
			int item2 = timeDisplay.Item2;
			int item3 = timeDisplay.Item3;
			bool item4 = timeDisplay.Item4;
			if (!this.ShouldLocalize)
			{
				this.displayText.text = item;
				return;
			}
			this._timescaleCountdownVar.Value = item2;
			this._timeCountdownVar.Value = item3;
			this._isValidVar.Value = item4;
		}

		// Token: 0x060080CF RID: 32975 RVA: 0x0029DF22 File Offset: 0x0029C122
		public static string GetTimeDisplay(TimeSpan ts, string format)
		{
			return CountdownText.GetTimeDisplay(ts, format, int.MaxValue, string.Empty, string.Empty).Item1;
		}

		// Token: 0x060080D0 RID: 32976 RVA: 0x0029DF40 File Offset: 0x0029C140
		[return: TupleElementNames(new string[] { "msg", "timescaleVar", "countdownVar", "valid" })]
		public static ValueTuple<string, int, int, bool> GetTimeDisplay(TimeSpan ts, string format, int maxDaysToDisplay, string elapsedString, string overMaxString)
		{
			string text = overMaxString;
			int num = 0;
			int num2 = ts.Days;
			bool flag = false;
			if (ts.TotalSeconds < 0.0)
			{
				return new ValueTuple<string, int, int, bool>(elapsedString, num, num2, flag);
			}
			if (ts.TotalDays < (double)maxDaysToDisplay)
			{
				if (ts.Days > 0)
				{
					num = 3;
					num2 = ts.Days;
					flag = true;
					text = string.Format(format, ts.Days, CountdownText.getTimeChunkString(CountdownText.TimeChunk.DAY, ts.Days));
				}
				else if (ts.Hours > 0)
				{
					num = 2;
					num2 = ts.Hours;
					flag = true;
					text = string.Format(format, ts.Hours, CountdownText.getTimeChunkString(CountdownText.TimeChunk.HOUR, ts.Hours));
				}
				else if (ts.Minutes > 0)
				{
					num = 1;
					num2 = ts.Minutes;
					flag = true;
					text = string.Format(format, ts.Minutes, CountdownText.getTimeChunkString(CountdownText.TimeChunk.MINUTE, ts.Minutes));
				}
				else if (ts.Seconds > 0)
				{
					num = 0;
					num2 = ts.Seconds;
					flag = true;
					text = string.Format(format, ts.Seconds, CountdownText.getTimeChunkString(CountdownText.TimeChunk.SECOND, ts.Seconds));
				}
			}
			return new ValueTuple<string, int, int, bool>(text, num, num2, flag);
		}

		// Token: 0x060080D1 RID: 32977 RVA: 0x0029E074 File Offset: 0x0029C274
		private static string getTimeChunkString(CountdownText.TimeChunk chunk, int n)
		{
			switch (chunk)
			{
			case CountdownText.TimeChunk.DAY:
				if (n == 1)
				{
					return "DAY";
				}
				return "DAYS";
			case CountdownText.TimeChunk.HOUR:
				if (n == 1)
				{
					return "HOUR";
				}
				return "HOURS";
			case CountdownText.TimeChunk.MINUTE:
				if (n == 1)
				{
					return "MINUTE";
				}
				return "MINUTES";
			case CountdownText.TimeChunk.SECOND:
				if (n == 1)
				{
					return "SECOND";
				}
				return "SECONDS";
			default:
				return string.Empty;
			}
		}

		// Token: 0x060080D2 RID: 32978 RVA: 0x0029E0E0 File Offset: 0x0029C2E0
		private DateTime TryParseDateTime()
		{
			DateTime dateTime;
			try
			{
				dateTime = DateTime.Parse(this.CountdownTo.CountdownTo, CultureInfo.InvariantCulture);
			}
			catch
			{
				dateTime = DateTime.MinValue;
			}
			return dateTime;
		}

		// Token: 0x040091C5 RID: 37317
		[SerializeField]
		private CountdownTextDate CountdownTo;

		// Token: 0x040091C6 RID: 37318
		[SerializeField]
		private bool updateDisplay;

		// Token: 0x040091C7 RID: 37319
		[SerializeField]
		private bool useExternalTime;

		// Token: 0x040091C8 RID: 37320
		[SerializeField]
		private bool shouldLocalize = true;

		// Token: 0x040091C9 RID: 37321
		private TMP_Text displayText;

		// Token: 0x040091CA RID: 37322
		private string displayTextFormat;

		// Token: 0x040091CB RID: 37323
		private DateTime targetTime;

		// Token: 0x040091CC RID: 37324
		private TimeSpan countdownTime;

		// Token: 0x040091CD RID: 37325
		private Coroutine monitor;

		// Token: 0x040091CE RID: 37326
		private Coroutine displayRefresh;

		// Token: 0x040091CF RID: 37327
		private LocalizedText _locTextComp;

		// Token: 0x040091D0 RID: 37328
		private LocalizedString _countdownLocStr;

		// Token: 0x040091D1 RID: 37329
		private IntVariable _timeCountdownVar;

		// Token: 0x040091D2 RID: 37330
		private IntVariable _timescaleCountdownVar;

		// Token: 0x040091D3 RID: 37331
		private BoolVariable _isValidVar;

		// Token: 0x020013F2 RID: 5106
		private enum TimeChunk
		{
			// Token: 0x040091D5 RID: 37333
			DAY,
			// Token: 0x040091D6 RID: 37334
			HOUR,
			// Token: 0x040091D7 RID: 37335
			MINUTE,
			// Token: 0x040091D8 RID: 37336
			SECOND
		}
	}
}
