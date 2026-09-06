using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GorillaNetworking;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x020013FA RID: 5114
	public class GameObjectScheduler : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x060080F5 RID: 33013 RVA: 0x0029E6A4 File Offset: 0x0029C8A4
		private async void Start()
		{
			this.schedule.Validate();
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < base.transform.childCount; i++)
			{
				list.Add(base.transform.GetChild(i).gameObject);
			}
			this.scheduledGameObject = list.ToArray();
			for (int j = 0; j < this.scheduledGameObject.Length; j++)
			{
				this.scheduledGameObject[j].SetActive(false);
			}
			this.dispatcher = base.GetComponent<GameObjectSchedulerEventDispatcher>();
			while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
			{
				await Task.Yield();
			}
			this.SetInitialState();
			this.ready = true;
		}

		// Token: 0x060080F6 RID: 33014 RVA: 0x0029E6DC File Offset: 0x0029C8DC
		private void SetInitialState()
		{
			double num;
			this.getActiveState(out this.previousState, out num);
			for (int i = 0; i < this.scheduledGameObject.Length; i++)
			{
				this.scheduledGameObject[i].SetActive(this.previousState);
				if (num > 0.0)
				{
					Animator[] componentsInChildren = this.scheduledGameObject[i].GetComponentsInChildren<Animator>();
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						int fullPathHash = componentsInChildren[j].GetCurrentAnimatorStateInfo(0).fullPathHash;
						componentsInChildren[j].PlayInFixedTime(fullPathHash, 0, (float)num);
					}
				}
			}
			this.lastMinuteCheck = this.getServerTime().Minute;
		}

		// Token: 0x060080F7 RID: 33015 RVA: 0x0029E77B File Offset: 0x0029C97B
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			if (this.ready)
			{
				this.SetInitialState();
			}
		}

		// Token: 0x060080F8 RID: 33016 RVA: 0x00019269 File Offset: 0x00017469
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x060080F9 RID: 33017 RVA: 0x0029E794 File Offset: 0x0029C994
		private void getActiveState(out bool state, out double totalSeconds)
		{
			DateTime serverTime = this.getServerTime();
			DateTime dateTime;
			this.currentNodeIndex = this.schedule.GetCurrentNodeIndex(serverTime, out dateTime);
			if (this.currentNodeIndex == -1)
			{
				state = this.schedule.InitialState;
				totalSeconds = 0.0;
				return;
			}
			if (this.currentNodeIndex < this.schedule.Nodes.Length)
			{
				state = this.schedule.Nodes[this.currentNodeIndex].ActiveState;
				totalSeconds = (serverTime - this.schedule.Nodes[this.currentNodeIndex].DateTime).TotalSeconds;
				return;
			}
			state = this.schedule.Nodes[this.schedule.Nodes.Length - 1].ActiveState;
			totalSeconds = (serverTime - this.schedule.Nodes[this.schedule.Nodes.Length - 1].DateTime).TotalSeconds;
		}

		// Token: 0x060080FA RID: 33018 RVA: 0x0023E7CF File Offset: 0x0023C9CF
		private DateTime getServerTime()
		{
			return GorillaComputer.instance.GetServerTime();
		}

		// Token: 0x060080FB RID: 33019 RVA: 0x0029E888 File Offset: 0x0029CA88
		private void changeActiveState(bool state)
		{
			if (state)
			{
				for (int i = 0; i < this.scheduledGameObject.Length; i++)
				{
					this.scheduledGameObject[i].SetActive(true);
				}
				if (this.dispatcher != null && this.dispatcher.OnScheduledActivation != null)
				{
					this.dispatcher.OnScheduledActivation.Invoke();
					return;
				}
			}
			else
			{
				if (this.dispatcher != null && this.dispatcher.OnScheduledDeactivation != null)
				{
					this.dispatcher.OnScheduledActivation.Invoke();
					return;
				}
				for (int j = 0; j < this.scheduledGameObject.Length; j++)
				{
					this.scheduledGameObject[j].SetActive(false);
				}
			}
		}

		// Token: 0x060080FC RID: 33020 RVA: 0x0029E934 File Offset: 0x0029CB34
		public void SliceUpdate()
		{
			if (!this.ready || (!this.useSecondsFidelity && this.getServerTime().Minute == this.lastMinuteCheck))
			{
				return;
			}
			bool flag;
			double num;
			this.getActiveState(out flag, out num);
			if (this.previousState != flag)
			{
				this.changeActiveState(flag);
				this.previousState = flag;
			}
			this.lastMinuteCheck = this.getServerTime().Minute;
		}

		// Token: 0x040091F0 RID: 37360
		[SerializeField]
		private GameObjectSchedule schedule;

		// Token: 0x040091F1 RID: 37361
		private GameObject[] scheduledGameObject;

		// Token: 0x040091F2 RID: 37362
		private GameObjectSchedulerEventDispatcher dispatcher;

		// Token: 0x040091F3 RID: 37363
		private int currentNodeIndex = -1;

		// Token: 0x040091F4 RID: 37364
		private bool ready;

		// Token: 0x040091F5 RID: 37365
		private bool previousState;

		// Token: 0x040091F6 RID: 37366
		private int lastMinuteCheck = -1;

		// Token: 0x040091F7 RID: 37367
		public bool useSecondsFidelity;

		// Token: 0x040091F8 RID: 37368
		public bool debugTime;
	}
}
