using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GorillaNetworking;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000E62 RID: 3682
public class TitleDataDateRefActivation : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060059E0 RID: 23008 RVA: 0x001D2128 File Offset: 0x001D0328
	private async void Initialize()
	{
		if (this.readyState != TitleDataDateRefActivation.ReadyState.Initializing && this.readyState != TitleDataDateRefActivation.ReadyState.Ready)
		{
			this.readyState = TitleDataDateRefActivation.ReadyState.Initializing;
			if (this.titleDataKey.IsNullOrEmpty())
			{
				this.onTD("1/1/3001");
			}
			else
			{
				while (PlayFabTitleDataCache.Instance == null)
				{
					await Task.Yield();
				}
				PlayFabTitleDataCache.Instance.GetTitleData(this.titleDataKey, new Action<string>(this.onTD), new Action<PlayFabError>(this.onTDError), false);
			}
		}
	}

	// Token: 0x060059E1 RID: 23009 RVA: 0x001D2160 File Offset: 0x001D0360
	private void onTD(string s)
	{
		try
		{
			this.setStartDate(DateTime.Parse(s));
			this.readyState = TitleDataDateRefActivation.ReadyState.Ready;
		}
		catch (Exception ex)
		{
			Debug.LogError("TitleDataDateRefActivation :: onTD :: " + ex.Message + " :: " + ex.StackTrace);
			this.readyState = TitleDataDateRefActivation.ReadyState.Crashed;
		}
	}

	// Token: 0x060059E2 RID: 23010 RVA: 0x001D21BC File Offset: 0x001D03BC
	public void StartNow(float delay)
	{
		this.setStartDate(GorillaComputer.instance.GetServerTime().AddSeconds((double)delay));
	}

	// Token: 0x060059E3 RID: 23011 RVA: 0x001D21E8 File Offset: 0x001D03E8
	private void setStartDate(DateTime d)
	{
		this.nodeList.Clear();
		for (int i = 0; i < this.nodes.Length; i++)
		{
			this.nodes[i].Initialize(d);
			this.nodeList.Add(this.nodes[i]);
		}
		this.nodeList.Sort();
		this.activations = 0;
	}

	// Token: 0x060059E4 RID: 23012 RVA: 0x001D2246 File Offset: 0x001D0446
	private void onTDError(PlayFabError error)
	{
		Debug.LogError(string.Format("TitleDataDateRefActivation :: onTDError :: {0}", error));
		this.readyState = TitleDataDateRefActivation.ReadyState.Crashed;
	}

	// Token: 0x060059E5 RID: 23013 RVA: 0x001D225F File Offset: 0x001D045F
	private void OnEnable()
	{
		this.Initialize();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060059E6 RID: 23014 RVA: 0x00012134 File Offset: 0x00010334
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060059E7 RID: 23015 RVA: 0x001D2270 File Offset: 0x001D0470
	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (this.readyState != TitleDataDateRefActivation.ReadyState.Ready || this.nodeList.Count == 0)
		{
			return;
		}
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		if (serverTime.Year < 2000)
		{
			return;
		}
		if (this.tmpStatus != null)
		{
			this.tmpStatus.text = string.Format("action {0} of {1} in {2:g} s", this.activations + 1, this.nodes.Length, this.nodeList[0].ActivationTime - GorillaComputer.instance.GetServerTime());
		}
		if (this.nodeList[0].ActivationTime <= serverTime)
		{
			this.nodeList[0].Activate(serverTime);
			this.nodeList.RemoveAt(0);
			this.activations++;
		}
	}

	// Token: 0x04006A1E RID: 27166
	[SerializeField]
	private string titleDataKey;

	// Token: 0x04006A1F RID: 27167
	[SerializeField]
	private TitleDataDateRefActivation.TitleDataDateRefActivationTarget[] nodes;

	// Token: 0x04006A20 RID: 27168
	[SerializeField]
	private TMP_Text tmpStatus;

	// Token: 0x04006A21 RID: 27169
	private TitleDataDateRefActivation.ReadyState readyState;

	// Token: 0x04006A22 RID: 27170
	private List<TitleDataDateRefActivation.TitleDataDateRefActivationTarget> nodeList = new List<TitleDataDateRefActivation.TitleDataDateRefActivationTarget>();

	// Token: 0x04006A23 RID: 27171
	private int activations;

	// Token: 0x02000E63 RID: 3683
	private enum ReadyState
	{
		// Token: 0x04006A25 RID: 27173
		None,
		// Token: 0x04006A26 RID: 27174
		Initializing,
		// Token: 0x04006A27 RID: 27175
		Ready,
		// Token: 0x04006A28 RID: 27176
		Crashed
	}

	// Token: 0x02000E64 RID: 3684
	[Serializable]
	private class TitleDataDateRefActivationTarget : IComparable<TitleDataDateRefActivation.TitleDataDateRefActivationTarget>
	{
		// Token: 0x17000893 RID: 2195
		// (get) Token: 0x060059E9 RID: 23017 RVA: 0x001D236C File Offset: 0x001D056C
		public GameObject GameObject
		{
			get
			{
				return this.gameObject;
			}
		}

		// Token: 0x17000894 RID: 2196
		// (get) Token: 0x060059EA RID: 23018 RVA: 0x001D2374 File Offset: 0x001D0574
		public DateTime ActivationTime
		{
			get
			{
				return this.dateTime;
			}
		}

		// Token: 0x060059EB RID: 23019 RVA: 0x001D237C File Offset: 0x001D057C
		public void Initialize(DateTime refTime)
		{
			this.dateTime = refTime.AddHours((double)this.hrs).AddMinutes((double)this.min).AddSeconds((double)this.sec);
		}

		// Token: 0x060059EC RID: 23020 RVA: 0x001D23BC File Offset: 0x001D05BC
		public void Activate(DateTime now)
		{
			float num = (float)(now - this.dateTime).TotalSeconds;
			this.Activate(num);
		}

		// Token: 0x060059ED RID: 23021 RVA: 0x001D23E6 File Offset: 0x001D05E6
		public void Activate()
		{
			this.Activate(0f);
		}

		// Token: 0x060059EE RID: 23022 RVA: 0x001D23F4 File Offset: 0x001D05F4
		private void Activate(float late)
		{
			if (this.gameObject != null)
			{
				this.gameObject.SetActive(this.activationState);
			}
			if (late < 1f)
			{
				UnityEvent unityEvent = this.payload;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
			}
			UnityEvent<float> unityEvent2 = this.persistantPayload;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(late);
		}

		// Token: 0x060059EF RID: 23023 RVA: 0x001D244C File Offset: 0x001D064C
		int IComparable<TitleDataDateRefActivation.TitleDataDateRefActivationTarget>.CompareTo(TitleDataDateRefActivation.TitleDataDateRefActivationTarget other)
		{
			return (this.hrs * 3600 + this.min * 60 + this.sec).CompareTo(other.hrs * 3600 + other.min * 60 + other.sec);
		}

		// Token: 0x04006A29 RID: 27177
		[SerializeField]
		private bool activationState;

		// Token: 0x04006A2A RID: 27178
		[SerializeField]
		private GameObject gameObject;

		// Token: 0x04006A2B RID: 27179
		[SerializeField]
		private int hrs;

		// Token: 0x04006A2C RID: 27180
		[SerializeField]
		private int min;

		// Token: 0x04006A2D RID: 27181
		[SerializeField]
		private int sec;

		// Token: 0x04006A2E RID: 27182
		[SerializeField]
		private UnityEvent payload;

		// Token: 0x04006A2F RID: 27183
		[SerializeField]
		private UnityEvent<float> persistantPayload;

		// Token: 0x04006A30 RID: 27184
		private DateTime dateTime = DateTime.MaxValue;
	}
}
