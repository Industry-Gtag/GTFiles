using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GorillaNetworking;
using GorillaNetworking.ScheduledEvents;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000E55 RID: 3669
public class TimedUnityEventDispatcher : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060059A0 RID: 22944 RVA: 0x001D1264 File Offset: 0x001CF464
	private async void Initialize()
	{
		if (this.readyState != TimedUnityEventDispatcher.ReadyState.Initializing && this.readyState != TimedUnityEventDispatcher.ReadyState.Ready)
		{
			this.readyState = TimedUnityEventDispatcher.ReadyState.Initializing;
			switch (this.mode)
			{
			case TimedUnityEventDispatcher.TimedUnityEventDispatcherMode.FIXED:
				this.onDateRetrieved(this.dateTime);
				break;
			case TimedUnityEventDispatcher.TimedUnityEventDispatcherMode.TITLE_DATA:
				while (PlayFabTitleDataCache.Instance == null)
				{
					await Task.Yield();
				}
				PlayFabTitleDataCache.Instance.GetTitleData(this.titleDataKey, new Action<string>(this.onDateRetrieved), new Action<PlayFabError>(this.onTDError), false);
				break;
			case TimedUnityEventDispatcher.TimedUnityEventDispatcherMode.SCHEDULED_EVENT:
			{
				this.nodeList.Clear();
				for (int i = 0; i < this.nodes.Length; i++)
				{
					this.nodes[i].Initialize();
					this.nodeList.Add(this.nodes[i]);
				}
				this.nodeList.Sort();
				this.activeNodeIndex = 0;
				this.readyState = TimedUnityEventDispatcher.ReadyState.Ready;
				break;
			}
			}
		}
	}

	// Token: 0x060059A1 RID: 22945 RVA: 0x001D129C File Offset: 0x001CF49C
	private void onDateRetrieved(string s)
	{
		try
		{
			this.setStartDate(DateTime.Parse(s));
			this.readyState = TimedUnityEventDispatcher.ReadyState.Ready;
		}
		catch (Exception ex)
		{
			Debug.LogError("TimedUnityEventDispatcher :: onDateRetrieved :: " + ex.Message + " :: " + ex.StackTrace);
			this.readyState = TimedUnityEventDispatcher.ReadyState.Crashed;
		}
	}

	// Token: 0x060059A2 RID: 22946 RVA: 0x001D12F8 File Offset: 0x001CF4F8
	public void StartNow(float delay)
	{
		throw new Exception("Oops! Function not available in production builds.");
	}

	// Token: 0x060059A3 RID: 22947 RVA: 0x001D1304 File Offset: 0x001CF504
	private void setStartDate(DateTime d)
	{
		this.nodeList.Clear();
		for (int i = 0; i < this.nodes.Length; i++)
		{
			this.nodes[i].Initialize(d);
			this.nodeList.Add(this.nodes[i]);
		}
		this.nodeList.Sort();
		this.activeNodeIndex = 0;
	}

	// Token: 0x060059A4 RID: 22948 RVA: 0x001D1362 File Offset: 0x001CF562
	private void onTDError(PlayFabError error)
	{
		Debug.LogError(string.Format("TitleDataDateRefActivation :: onTDError :: {0}", error));
		this.readyState = TimedUnityEventDispatcher.ReadyState.Crashed;
	}

	// Token: 0x060059A5 RID: 22949 RVA: 0x001D137B File Offset: 0x001CF57B
	private void OnEnable()
	{
		this.Initialize();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060059A6 RID: 22950 RVA: 0x00012134 File Offset: 0x00010334
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060059A7 RID: 22951 RVA: 0x001D138C File Offset: 0x001CF58C
	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (this.readyState != TimedUnityEventDispatcher.ReadyState.Ready)
		{
			return;
		}
		TimedUnityEventDispatcher.TimedUnityEventDispatcherMode timedUnityEventDispatcherMode = this.mode;
		if (timedUnityEventDispatcherMode > TimedUnityEventDispatcher.TimedUnityEventDispatcherMode.TITLE_DATA)
		{
			if (timedUnityEventDispatcherMode != TimedUnityEventDispatcher.TimedUnityEventDispatcherMode.SCHEDULED_EVENT)
			{
				return;
			}
			ScheduledEventManager instance = ScheduledEventManager.Instance;
			if (this.activeNodeIndex >= this.nodeList.Count || !instance.DataReady)
			{
				return;
			}
			ScheduledEventPhase currentPhase = instance.CurrentPhase;
			if (currentPhase != ScheduledEventPhase.During && currentPhase != ScheduledEventPhase.After)
			{
				return;
			}
			int num = ((currentPhase == ScheduledEventPhase.After) ? int.MaxValue : instance.EventSubphase);
			TimeSpan timeSpan = instance.EventSubphaseStartTime - instance.PreviousEventSubphaseStartTime;
			while (this.activeNodeIndex < this.nodeList.Count && this.nodeList[this.activeNodeIndex].SubphaseOrder < num)
			{
				TimedUnityEventDispatcher.TimedUnityEventDispatcherNode timedUnityEventDispatcherNode = this.nodeList[this.activeNodeIndex];
				timedUnityEventDispatcherNode.ActivatePersistent((float)(timeSpan - timedUnityEventDispatcherNode.ActivationDelay).TotalSeconds);
				this.activeNodeIndex++;
			}
			if (this.activeNodeIndex < this.nodeList.Count)
			{
				TimedUnityEventDispatcher.TimedUnityEventDispatcherNode timedUnityEventDispatcherNode2 = this.nodeList[this.activeNodeIndex];
				if (timedUnityEventDispatcherNode2.SubphaseOrder == num && timedUnityEventDispatcherNode2.ActivationDelay <= DateTime.Now - instance.EventSubphaseStartTime)
				{
					timedUnityEventDispatcherNode2.Activate();
					this.activeNodeIndex++;
				}
			}
		}
		else if (this.activeNodeIndex < this.nodeList.Count)
		{
			DateTime serverTime = GorillaComputer.instance.GetServerTime();
			if (serverTime.Year < 2000)
			{
				return;
			}
			if (this.nodeList[this.activeNodeIndex].ActivationTime <= serverTime)
			{
				this.nodeList[this.activeNodeIndex].Activate(serverTime);
				this.activeNodeIndex++;
				return;
			}
		}
	}

	// Token: 0x040069E0 RID: 27104
	[SerializeField]
	private TimedUnityEventDispatcher.TimedUnityEventDispatcherMode mode;

	// Token: 0x040069E1 RID: 27105
	[SerializeField]
	private string dateTime;

	// Token: 0x040069E2 RID: 27106
	[SerializeField]
	private string titleDataKey;

	// Token: 0x040069E3 RID: 27107
	[SerializeField]
	private TimedUnityEventDispatcher.TimedUnityEventDispatcherNode[] nodes;

	// Token: 0x040069E4 RID: 27108
	private TimedUnityEventDispatcher.ReadyState readyState;

	// Token: 0x040069E5 RID: 27109
	private List<TimedUnityEventDispatcher.TimedUnityEventDispatcherNode> nodeList = new List<TimedUnityEventDispatcher.TimedUnityEventDispatcherNode>();

	// Token: 0x040069E6 RID: 27110
	private int activeNodeIndex;

	// Token: 0x02000E56 RID: 3670
	private enum TimedUnityEventDispatcherMode
	{
		// Token: 0x040069E8 RID: 27112
		FIXED,
		// Token: 0x040069E9 RID: 27113
		TITLE_DATA,
		// Token: 0x040069EA RID: 27114
		SCHEDULED_EVENT
	}

	// Token: 0x02000E57 RID: 3671
	private enum ReadyState
	{
		// Token: 0x040069EC RID: 27116
		None,
		// Token: 0x040069ED RID: 27117
		Initializing,
		// Token: 0x040069EE RID: 27118
		Ready,
		// Token: 0x040069EF RID: 27119
		Crashed
	}

	// Token: 0x02000E58 RID: 3672
	[Serializable]
	private class TimedUnityEventDispatcherNode : IComparable<TimedUnityEventDispatcher.TimedUnityEventDispatcherNode>
	{
		// Token: 0x17000888 RID: 2184
		// (get) Token: 0x060059A9 RID: 22953 RVA: 0x001D156A File Offset: 0x001CF76A
		public int SubphaseOrder
		{
			get
			{
				return this.subphase;
			}
		}

		// Token: 0x17000889 RID: 2185
		// (get) Token: 0x060059AA RID: 22954 RVA: 0x001D1572 File Offset: 0x001CF772
		// (set) Token: 0x060059AB RID: 22955 RVA: 0x001D157A File Offset: 0x001CF77A
		public DateTime ActivationTime { get; private set; }

		// Token: 0x1700088A RID: 2186
		// (get) Token: 0x060059AC RID: 22956 RVA: 0x001D1583 File Offset: 0x001CF783
		// (set) Token: 0x060059AD RID: 22957 RVA: 0x001D158B File Offset: 0x001CF78B
		public TimeSpan ActivationDelay { get; private set; }

		// Token: 0x060059AE RID: 22958 RVA: 0x001D1594 File Offset: 0x001CF794
		public void Initialize(DateTime refTime)
		{
			this.Initialize();
			this.ActivationTime = refTime + this.ActivationDelay;
		}

		// Token: 0x060059AF RID: 22959 RVA: 0x001D15AE File Offset: 0x001CF7AE
		public void Initialize()
		{
			this.ActivationDelay = new TimeSpan(this.hrs, this.min, this.sec);
			if (this.afterEvent)
			{
				this.subphase = int.MaxValue;
			}
		}

		// Token: 0x060059B0 RID: 22960 RVA: 0x001D15E0 File Offset: 0x001CF7E0
		public void Activate(DateTime now)
		{
			float num = (float)(now - this.ActivationTime).TotalSeconds;
			this.Activate(num);
		}

		// Token: 0x060059B1 RID: 22961 RVA: 0x001D160A File Offset: 0x001CF80A
		public void Activate()
		{
			this.Activate(0f);
		}

		// Token: 0x060059B2 RID: 22962 RVA: 0x001D1617 File Offset: 0x001CF817
		private void Activate(float late)
		{
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

		// Token: 0x060059B3 RID: 22963 RVA: 0x001D1643 File Offset: 0x001CF843
		public void ActivatePersistent(float late)
		{
			UnityEvent<float> unityEvent = this.persistantPayload;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(late);
		}

		// Token: 0x060059B4 RID: 22964 RVA: 0x001D1658 File Offset: 0x001CF858
		int IComparable<TimedUnityEventDispatcher.TimedUnityEventDispatcherNode>.CompareTo(TimedUnityEventDispatcher.TimedUnityEventDispatcherNode other)
		{
			if (this.SubphaseOrder == other.SubphaseOrder)
			{
				return (this.hrs * 3600 + this.min * 60 + this.sec).CompareTo(other.hrs * 3600 + other.min * 60 + other.sec);
			}
			return this.SubphaseOrder.CompareTo(other.SubphaseOrder);
		}

		// Token: 0x040069F0 RID: 27120
		[SerializeField]
		private int subphase;

		// Token: 0x040069F1 RID: 27121
		[SerializeField]
		private bool afterEvent;

		// Token: 0x040069F2 RID: 27122
		[SerializeField]
		private int hrs;

		// Token: 0x040069F3 RID: 27123
		[SerializeField]
		private int min;

		// Token: 0x040069F4 RID: 27124
		[SerializeField]
		private int sec;

		// Token: 0x040069F5 RID: 27125
		[SerializeField]
		private UnityEvent payload;

		// Token: 0x040069F6 RID: 27126
		[SerializeField]
		private UnityEvent<float> persistantPayload;
	}
}
