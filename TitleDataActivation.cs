using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GorillaNetworking;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;

// Token: 0x02000E5A RID: 3674
public class TitleDataActivation : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060059B8 RID: 22968 RVA: 0x001D1888 File Offset: 0x001CFA88
	[RuntimeInitializeOnLoadMethod]
	private static async void RuntimeInit()
	{
		TitleDataActivation.ReferenceDate = DateTime.Parse("1/1/2001");
		TitleDataActivation.UpdatedReferenceDateFromTitleData = false;
		while (PlayFabTitleDataCache.Instance == null)
		{
			await Task.Yield();
		}
		PlayFabTitleDataCache.Instance.GetTitleData("ActivationReferenceDate", new Action<string>(TitleDataActivation.onTDReferenceDate), new Action<PlayFabError>(TitleDataActivation.onTDReferenceDateError), false);
	}

	// Token: 0x060059B9 RID: 22969 RVA: 0x001D18B7 File Offset: 0x001CFAB7
	private static void onTDReferenceDate(string s)
	{
		if (!DateTime.TryParse(s, out TitleDataActivation.ReferenceDate))
		{
			Debug.LogError("TitleDataActivation :: onTDReferenceDate :: No Reference Date Set!!");
			return;
		}
		TitleDataActivation.UpdatedReferenceDateFromTitleData = true;
	}

	// Token: 0x060059BA RID: 22970 RVA: 0x001D18D7 File Offset: 0x001CFAD7
	private static void onTDReferenceDateError(PlayFabError error)
	{
		Debug.LogError("TitleDataActivation :: onTDReferenceDateError :: No Reference Date Set!! :: " + error.ErrorMessage);
	}

	// Token: 0x060059BB RID: 22971 RVA: 0x001D18F0 File Offset: 0x001CFAF0
	private async void Initialize()
	{
		if (!this.initialized)
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < base.transform.childCount; i++)
			{
				GameObject gameObject = base.transform.GetChild(i).gameObject;
				gameObject.SetActive(false);
				list.Add(gameObject);
			}
			this.gameObjects = list.ToArray();
			this.initialized = true;
			if (!this.titleDataKey.IsNullOrEmpty())
			{
				while (PlayFabTitleDataCache.Instance == null || !TitleDataActivation.UpdatedReferenceDateFromTitleData)
				{
					await Task.Yield();
				}
				PlayFabTitleDataCache.Instance.GetTitleData(this.titleDataKey, new Action<string>(this.onTD), new Action<PlayFabError>(this.onTDError), false);
			}
		}
	}

	// Token: 0x060059BC RID: 22972 RVA: 0x001D1928 File Offset: 0x001CFB28
	private void onTD(string s)
	{
		TitleDataActivation.TitleDataActivationData titleDataActivationData = null;
		try
		{
			titleDataActivationData = JsonConvert.DeserializeObject<TitleDataActivation.TitleDataActivationData>(s);
		}
		catch (Exception ex)
		{
			Debug.LogError("TitleDataActivation :: onTD ::" + ex.Message + " string was " + s);
			return;
		}
		for (int i = 0; i < titleDataActivationData.Data.Length; i++)
		{
			if (titleDataActivationData.Data[i].TitleDataObjectID == this.titleDataObjectID)
			{
				this.activationData = titleDataActivationData.Data[i];
				return;
			}
		}
	}

	// Token: 0x060059BD RID: 22973 RVA: 0x001D19AC File Offset: 0x001CFBAC
	private void onTDError(PlayFabError error)
	{
		Debug.LogError(string.Format("TitleDataActivation on {0} :: onTDError :: {1} :: {2}", AssetUtils.GetGameObjectPath(base.gameObject), this.titleDataKey, error));
	}

	// Token: 0x060059BE RID: 22974 RVA: 0x001D19CF File Offset: 0x001CFBCF
	private void OnEnable()
	{
		this.Initialize();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060059BF RID: 22975 RVA: 0x00012134 File Offset: 0x00010334
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060059C0 RID: 22976 RVA: 0x001D19E0 File Offset: 0x001CFBE0
	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (this.activationData == null)
		{
			return;
		}
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		if (serverTime.Year < 2000)
		{
			return;
		}
		bool flag = false;
		float num = 0f;
		int num2 = 0;
		while (this.activationData.AbsoluteDateTimeWindow != null && num2 < this.activationData.AbsoluteDateTimeWindow.Length && !flag)
		{
			this.activationData.AbsoluteDateTimeWindow[num2].IsInWindow(serverTime, out flag, out num);
			num2++;
		}
		int num3 = 0;
		while (this.activationData.RelativeDateTimeWindow != null && num3 < this.activationData.RelativeDateTimeWindow.Length && !flag)
		{
			this.activationData.RelativeDateTimeWindow[num3].IsInWindow(serverTime, out flag, out num);
			num3++;
		}
		if (flag != this.onOffState)
		{
			this.SetState(flag, num);
			this.onOffState = flag;
		}
	}

	// Token: 0x060059C1 RID: 22977 RVA: 0x001D1AB8 File Offset: 0x001CFCB8
	private void SetState(bool onOff, float delayedActivation)
	{
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			this.gameObjects[i].SetActive(onOff);
			if (onOff && delayedActivation > 0f)
			{
				Animator[] componentsInChildren = this.gameObjects[i].GetComponentsInChildren<Animator>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					int fullPathHash = componentsInChildren[j].GetCurrentAnimatorStateInfo(0).fullPathHash;
					componentsInChildren[j].PlayInFixedTime(fullPathHash, 0, delayedActivation);
				}
			}
		}
	}

	// Token: 0x060059C2 RID: 22978 RVA: 0x001D1B2C File Offset: 0x001CFD2C
	public float GetDelayedActivationTime()
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		if (serverTime.Year < 2000)
		{
			return 0f;
		}
		bool flag = false;
		float num = 0f;
		int num2 = 0;
		while (this.activationData.AbsoluteDateTimeWindow != null && num2 < this.activationData.AbsoluteDateTimeWindow.Length && !flag)
		{
			this.activationData.AbsoluteDateTimeWindow[num2].IsInWindow(serverTime, out flag, out num);
			num2++;
		}
		int num3 = 0;
		while (this.activationData.RelativeDateTimeWindow != null && num3 < this.activationData.RelativeDateTimeWindow.Length && !flag)
		{
			this.activationData.RelativeDateTimeWindow[num3].IsInWindow(serverTime, out flag, out num);
			num3++;
		}
		return Mathf.Max(0f, num);
	}

	// Token: 0x060059C3 RID: 22979 RVA: 0x001D1BF0 File Offset: 0x001CFDF0
	public void PlayAnimatorAtScheduledTime(Animator animator)
	{
		float delayedActivationTime = this.GetDelayedActivationTime();
		int fullPathHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
		animator.PlayInFixedTime(fullPathHash, 0, this.GetDelayedActivationTime());
		AudioSource[] componentsInChildren = animator.GetComponentsInChildren<AudioSource>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].playOnAwake && componentsInChildren[i].clip != null && componentsInChildren[i].clip.length > delayedActivationTime)
			{
				componentsInChildren[i].time = delayedActivationTime;
			}
		}
	}

	// Token: 0x040069FD RID: 27133
	public static DateTime ReferenceDate = DateTime.Parse("1/1/2001");

	// Token: 0x040069FE RID: 27134
	public static bool UpdatedReferenceDateFromTitleData = false;

	// Token: 0x040069FF RID: 27135
	[SerializeField]
	private string titleDataKey;

	// Token: 0x04006A00 RID: 27136
	[SerializeField]
	private string titleDataObjectID;

	// Token: 0x04006A01 RID: 27137
	private TitleDataActivation.TitleDataObjectActivationData activationData;

	// Token: 0x04006A02 RID: 27138
	private GameObject[] gameObjects;

	// Token: 0x04006A03 RID: 27139
	private bool initialized;

	// Token: 0x04006A04 RID: 27140
	private bool onOffState;

	// Token: 0x02000E5B RID: 3675
	[Serializable]
	public class TitleDataActivationData
	{
		// Token: 0x1700088B RID: 2187
		// (get) Token: 0x060059C6 RID: 22982 RVA: 0x001D1C89 File Offset: 0x001CFE89
		// (set) Token: 0x060059C7 RID: 22983 RVA: 0x001D1C91 File Offset: 0x001CFE91
		public TitleDataActivation.TitleDataObjectActivationData[] Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		// Token: 0x04006A05 RID: 27141
		[SerializeField]
		private TitleDataActivation.TitleDataObjectActivationData[] data;

		// Token: 0x04006A06 RID: 27142
		private bool validated;
	}

	// Token: 0x02000E5C RID: 3676
	[Serializable]
	public class TitleDataObjectActivationData
	{
		// Token: 0x1700088C RID: 2188
		// (get) Token: 0x060059C9 RID: 22985 RVA: 0x001D1C9A File Offset: 0x001CFE9A
		// (set) Token: 0x060059CA RID: 22986 RVA: 0x001D1CA2 File Offset: 0x001CFEA2
		public string TitleDataObjectID
		{
			get
			{
				return this.titleDataObjectID;
			}
			set
			{
				this.titleDataObjectID = value;
			}
		}

		// Token: 0x1700088D RID: 2189
		// (get) Token: 0x060059CB RID: 22987 RVA: 0x001D1CAB File Offset: 0x001CFEAB
		// (set) Token: 0x060059CC RID: 22988 RVA: 0x001D1CB3 File Offset: 0x001CFEB3
		public TitleDataActivation.AbsoluteDateTimeWindow[] AbsoluteDateTimeWindow
		{
			get
			{
				return this.absoluteDateTimeWindow;
			}
			set
			{
				this.absoluteDateTimeWindow = value;
			}
		}

		// Token: 0x1700088E RID: 2190
		// (get) Token: 0x060059CD RID: 22989 RVA: 0x001D1CBC File Offset: 0x001CFEBC
		// (set) Token: 0x060059CE RID: 22990 RVA: 0x001D1CC4 File Offset: 0x001CFEC4
		public TitleDataActivation.RelativeDateTimeWindow[] RelativeDateTimeWindow
		{
			get
			{
				return this.relativeDateTimeWindow;
			}
			set
			{
				this.relativeDateTimeWindow = value;
			}
		}

		// Token: 0x04006A07 RID: 27143
		[SerializeField]
		private string titleDataObjectID;

		// Token: 0x04006A08 RID: 27144
		[SerializeField]
		private TitleDataActivation.AbsoluteDateTimeWindow[] absoluteDateTimeWindow;

		// Token: 0x04006A09 RID: 27145
		[SerializeField]
		private TitleDataActivation.RelativeDateTimeWindow[] relativeDateTimeWindow;

		// Token: 0x04006A0A RID: 27146
		private bool validated;
	}

	// Token: 0x02000E5D RID: 3677
	[Serializable]
	public class AbsoluteDateTimeWindow
	{
		// Token: 0x1700088F RID: 2191
		// (get) Token: 0x060059D0 RID: 22992 RVA: 0x001D1CCD File Offset: 0x001CFECD
		// (set) Token: 0x060059D1 RID: 22993 RVA: 0x001D1CD5 File Offset: 0x001CFED5
		public string StartDateTime
		{
			get
			{
				return this.startDateTime;
			}
			set
			{
				if (DateTime.TryParse(value, out this.dtStart))
				{
					this.startDateTime = this.dtStart.ToString();
				}
			}
		}

		// Token: 0x17000890 RID: 2192
		// (get) Token: 0x060059D2 RID: 22994 RVA: 0x001D1CF6 File Offset: 0x001CFEF6
		// (set) Token: 0x060059D3 RID: 22995 RVA: 0x001D1CFE File Offset: 0x001CFEFE
		public string EndDateTime
		{
			get
			{
				return this.endDateTime;
			}
			set
			{
				if (DateTime.TryParse(value, out this.dtEnd))
				{
					this.endDateTime = this.dtEnd.ToString();
				}
			}
		}

		// Token: 0x060059D4 RID: 22996 RVA: 0x001D1D20 File Offset: 0x001CFF20
		public void IsInWindow(DateTime d, out bool inRange, out float delay)
		{
			inRange = d >= this.dtStart && d <= this.dtEnd;
			delay = (float)(d - this.dtStart).TotalSeconds;
		}

		// Token: 0x04006A0B RID: 27147
		protected DateTime dtStart;

		// Token: 0x04006A0C RID: 27148
		protected DateTime dtEnd;

		// Token: 0x04006A0D RID: 27149
		[SerializeField]
		private string startDateTime;

		// Token: 0x04006A0E RID: 27150
		[SerializeField]
		private string endDateTime;
	}

	// Token: 0x02000E5E RID: 3678
	[Serializable]
	public class RelativeDateTimeWindow
	{
		// Token: 0x17000891 RID: 2193
		// (get) Token: 0x060059D6 RID: 22998 RVA: 0x001D1D63 File Offset: 0x001CFF63
		// (set) Token: 0x060059D7 RID: 22999 RVA: 0x001D1D6C File Offset: 0x001CFF6C
		public TitleDataActivation.RelativeDateTime StartDateTime
		{
			get
			{
				return this.startDateTime;
			}
			set
			{
				this.startDateTime = value;
				this.dtStart = TitleDataActivation.ReferenceDate.AddDays((double)this.startDateTime.DaysPast).AddHours((double)this.startDateTime.Hours).AddMinutes((double)this.startDateTime.Minutes)
					.AddSeconds((double)this.startDateTime.Seconds);
			}
		}

		// Token: 0x17000892 RID: 2194
		// (get) Token: 0x060059D8 RID: 23000 RVA: 0x001D1DD8 File Offset: 0x001CFFD8
		// (set) Token: 0x060059D9 RID: 23001 RVA: 0x001D1DE0 File Offset: 0x001CFFE0
		public TitleDataActivation.RelativeDateTime EndDateTime
		{
			get
			{
				return this.endDateTime;
			}
			set
			{
				this.endDateTime = value;
				this.dtEnd = TitleDataActivation.ReferenceDate.AddDays((double)this.endDateTime.DaysPast).AddHours((double)this.endDateTime.Hours).AddMinutes((double)this.endDateTime.Minutes)
					.AddSeconds((double)this.endDateTime.Seconds);
			}
		}

		// Token: 0x060059DA RID: 23002 RVA: 0x001D1E4C File Offset: 0x001D004C
		public void IsInWindow(DateTime d, out bool inRange, out float delay)
		{
			inRange = d >= this.dtStart && d <= this.dtEnd;
			delay = (float)(d - this.dtStart).TotalSeconds;
		}

		// Token: 0x04006A0F RID: 27151
		protected DateTime dtStart;

		// Token: 0x04006A10 RID: 27152
		protected DateTime dtEnd;

		// Token: 0x04006A11 RID: 27153
		[SerializeField]
		private TitleDataActivation.RelativeDateTime startDateTime;

		// Token: 0x04006A12 RID: 27154
		[SerializeField]
		private TitleDataActivation.RelativeDateTime endDateTime;
	}

	// Token: 0x02000E5F RID: 3679
	[Serializable]
	public struct RelativeDateTime
	{
		// Token: 0x04006A13 RID: 27155
		public int DaysPast;

		// Token: 0x04006A14 RID: 27156
		public int Hours;

		// Token: 0x04006A15 RID: 27157
		public int Minutes;

		// Token: 0x04006A16 RID: 27158
		public int Seconds;
	}
}
