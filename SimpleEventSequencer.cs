using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000E08 RID: 3592
public class SimpleEventSequencer : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060057EF RID: 22511 RVA: 0x001CA8E7 File Offset: 0x001C8AE7
	private void StartSequence()
	{
		this.StartSequenceDelayed(0f);
	}

	// Token: 0x060057F0 RID: 22512 RVA: 0x001CA8F4 File Offset: 0x001C8AF4
	public async void StartSequenceDelayed(float delay)
	{
		this.startTime = Time.time + delay;
		if (this.serverTimeSync != null)
		{
			while (GorillaComputer.instance == null || GorillaComputer.instance.GetServerTime().Year < 2000)
			{
				await Task.Yield();
			}
			DateTime serverTime = GorillaComputer.instance.GetServerTime();
			serverTime.AddSeconds((double)delay);
			DateTime next = this.serverTimeSync.GetNext(serverTime);
			this.startTime += (float)(next - serverTime).TotalSeconds;
		}
		this.idx = 0;
	}

	// Token: 0x060057F1 RID: 22513 RVA: 0x001CA933 File Offset: 0x001C8B33
	private void startSequenceImmediate()
	{
		this.startTime = Time.time;
		this.idx = 0;
	}

	// Token: 0x060057F2 RID: 22514 RVA: 0x001CA947 File Offset: 0x001C8B47
	private void startSequenceFrom(int i)
	{
		this.startTime = Time.time;
		this.idx = i;
	}

	// Token: 0x060057F3 RID: 22515 RVA: 0x001CA95B File Offset: 0x001C8B5B
	private void stop(int i)
	{
		this.idx = -1;
	}

	// Token: 0x060057F4 RID: 22516 RVA: 0x001CA964 File Offset: 0x001C8B64
	private void Awake()
	{
		this.enabledNodes.Clear();
		for (int i = 0; i < this.nodes.Length; i++)
		{
			if (this.nodes[i].Enabled)
			{
				this.enabledNodes.Add(this.nodes[i]);
			}
		}
	}

	// Token: 0x060057F5 RID: 22517 RVA: 0x001CA9B1 File Offset: 0x001C8BB1
	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		if (this.startOnEnable)
		{
			this.StartSequence();
		}
	}

	// Token: 0x060057F6 RID: 22518 RVA: 0x00012134 File Offset: 0x00010334
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060057F7 RID: 22519 RVA: 0x001CA9C8 File Offset: 0x001C8BC8
	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (this.idx < 0 || this.idx == this.enabledNodes.Count)
		{
			return;
		}
		if (Time.time >= this.startTime + this.enabledNodes[this.idx].Time)
		{
			UnityEvent unityEvent = this.enabledNodes[this.idx].UnityEvent;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.startTime = Time.time;
			this.idx++;
			if (this.idx == this.enabledNodes.Count)
			{
				SimpleEventSequencer.OnCompleteAction onCompleteAction = this.onComplete;
				if (onCompleteAction != SimpleEventSequencer.OnCompleteAction.Disable)
				{
					if (onCompleteAction == SimpleEventSequencer.OnCompleteAction.Repeat)
					{
						this.StartSequenceDelayed(this.enabledNodes[this.idx - 1].Time);
						return;
					}
				}
				else
				{
					base.gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x060057F8 RID: 22520 RVA: 0x001CAAA0 File Offset: 0x001C8CA0
	private void onValueChanged()
	{
		float num = 0f;
		for (int i = 0; i < this.nodes.Length; i++)
		{
			if (this.nodes[i].Enabled)
			{
				num += this.nodes[i].Time;
			}
			this.nodes[i].TotalTime = num;
			this.nodes[i].onValueChanged();
		}
	}

	// Token: 0x060057F9 RID: 22521 RVA: 0x001CAB00 File Offset: 0x001C8D00
	public void SetOnCompleteActionDisable()
	{
		this.onComplete = SimpleEventSequencer.OnCompleteAction.Disable;
	}

	// Token: 0x060057FA RID: 22522 RVA: 0x001CAB09 File Offset: 0x001C8D09
	public void SetOnCompleteActionRepeat()
	{
		this.onComplete = SimpleEventSequencer.OnCompleteAction.Repeat;
	}

	// Token: 0x060057FB RID: 22523 RVA: 0x001CAB12 File Offset: 0x001C8D12
	public void ClearOnCompleteAction()
	{
		this.onComplete = SimpleEventSequencer.OnCompleteAction.None;
	}

	// Token: 0x060057FC RID: 22524 RVA: 0x001CAB1B File Offset: 0x001C8D1B
	public void TempAudio(string text)
	{
		Debug.Log("SimpleEventSequencer :: " + base.name + " :: TempAudio :: " + text);
	}

	// Token: 0x060057FD RID: 22525 RVA: 0x001CAB38 File Offset: 0x001C8D38
	public void TempVFX(string text)
	{
		Debug.Log("SimpleEventSequencer :: " + base.name + " :: TempVFX :: " + text);
	}

	// Token: 0x060057FE RID: 22526 RVA: 0x001CAB55 File Offset: 0x001C8D55
	public void Temp(string text)
	{
		Debug.Log("SimpleEventSequencer :: " + base.name + " :: Temp :: " + text);
	}

	// Token: 0x060057FF RID: 22527 RVA: 0x001CAB72 File Offset: 0x001C8D72
	public void DebugLog(string text)
	{
		Debug.Log("SimpleEventSequencer :: " + base.name + " :: DEBUG :: " + text);
	}

	// Token: 0x04006855 RID: 26709
	[SerializeField]
	private SimpleEventSequencer.SimpleEventSequencerNode[] nodes;

	// Token: 0x04006856 RID: 26710
	[SerializeField]
	private bool startOnEnable = true;

	// Token: 0x04006857 RID: 26711
	[SerializeField]
	private SimpleEventSequencer.OnCompleteAction onComplete = SimpleEventSequencer.OnCompleteAction.Disable;

	// Token: 0x04006858 RID: 26712
	[SerializeField]
	private ServerTimeSyncRule serverTimeSync;

	// Token: 0x04006859 RID: 26713
	private float startTime;

	// Token: 0x0400685A RID: 26714
	private int idx = -1;

	// Token: 0x0400685B RID: 26715
	private List<SimpleEventSequencer.SimpleEventSequencerNode> enabledNodes = new List<SimpleEventSequencer.SimpleEventSequencerNode>();

	// Token: 0x0400685C RID: 26716
	private SimpleEventSequencer.SimpleEventSequencerNode activeNode;

	// Token: 0x02000E09 RID: 3593
	[Serializable]
	private class SimpleEventSequencerNode
	{
		// Token: 0x17000854 RID: 2132
		// (get) Token: 0x06005801 RID: 22529 RVA: 0x001CABB7 File Offset: 0x001C8DB7
		private string nameTrim
		{
			get
			{
				if (this.name.Length <= 33)
				{
					return this.name;
				}
				return this.name.Substring(0, 30) + "...";
			}
		}

		// Token: 0x17000855 RID: 2133
		// (get) Token: 0x06005802 RID: 22530 RVA: 0x001CABE7 File Offset: 0x001C8DE7
		private string notesTrim
		{
			get
			{
				if (this.notes.Length <= 50)
				{
					return this.notes;
				}
				return this.notes.Substring(0, 47) + "...";
			}
		}

		// Token: 0x17000856 RID: 2134
		// (get) Token: 0x06005803 RID: 22531 RVA: 0x001CAC17 File Offset: 0x001C8E17
		public float Time
		{
			get
			{
				return this.time;
			}
		}

		// Token: 0x17000857 RID: 2135
		// (get) Token: 0x06005804 RID: 22532 RVA: 0x001CAC1F File Offset: 0x001C8E1F
		public UnityEvent UnityEvent
		{
			get
			{
				return this.unityEvent;
			}
		}

		// Token: 0x17000858 RID: 2136
		// (get) Token: 0x06005805 RID: 22533 RVA: 0x001CAC27 File Offset: 0x001C8E27
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		// Token: 0x17000859 RID: 2137
		// (set) Token: 0x06005806 RID: 22534 RVA: 0x001CAC2F File Offset: 0x001C8E2F
		public float TotalTime
		{
			set
			{
				this.totalTime = value;
			}
		}

		// Token: 0x1700085A RID: 2138
		// (get) Token: 0x06005807 RID: 22535 RVA: 0x001CAC38 File Offset: 0x001C8E38
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
		}

		// Token: 0x06005808 RID: 22536 RVA: 0x001CAC40 File Offset: 0x001C8E40
		public void onValueChanged()
		{
			if (this.enabled)
			{
				this.fancyName = string.Format("T+{0} ({1}) : {2}", this.totalTime, this.time, this.nameTrim);
				return;
			}
			this.fancyName = string.Format("Skip ({0}) : {1}", this.time, this.nameTrim);
		}

		// Token: 0x0400685D RID: 26717
		[Tooltip("Uncheck to skip this node")]
		[SerializeField]
		private bool enabled = true;

		// Token: 0x0400685E RID: 26718
		[Tooltip("Seconds after the previous node's events are dispatched")]
		[SerializeField]
		private float time;

		// Token: 0x0400685F RID: 26719
		[Tooltip("This is just for legibilty. Doesn't matter what you name it.")]
		[SerializeField]
		private string name = "New Node";

		// Token: 0x04006860 RID: 26720
		[SerializeField]
		private UnityEvent unityEvent;

		// Token: 0x04006861 RID: 26721
		[SerializeField]
		[TextArea(5, 10)]
		private string notes = "Notes";

		// Token: 0x04006862 RID: 26722
		private string fancyName = "New Node";

		// Token: 0x04006863 RID: 26723
		private float totalTime;
	}

	// Token: 0x02000E0A RID: 3594
	private enum OnCompleteAction
	{
		// Token: 0x04006865 RID: 26725
		None,
		// Token: 0x04006866 RID: 26726
		Disable,
		// Token: 0x04006867 RID: 26727
		Repeat
	}
}
