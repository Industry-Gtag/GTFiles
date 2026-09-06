using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Cosmetics
{
	// Token: 0x020011D8 RID: 4568
	public class CountDrivenEvents : MonoBehaviour
	{
		// Token: 0x17000B3C RID: 2876
		// (get) Token: 0x06007419 RID: 29721 RVA: 0x0025C40A File Offset: 0x0025A60A
		public int CurrentCount
		{
			get
			{
				return this.currentCount;
			}
		}

		// Token: 0x0600741A RID: 29722 RVA: 0x0025C412 File Offset: 0x0025A612
		private bool IsOnCooldown()
		{
			return this.cooldown > 0f && Time.time - this.lastEventTime < this.cooldown;
		}

		// Token: 0x0600741B RID: 29723 RVA: 0x0025C438 File Offset: 0x0025A638
		private void OnEnable()
		{
			if (this.myRig == null)
			{
				this.myRig = base.GetComponentInParent<VRRig>();
			}
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			}
			NetPlayer netPlayer = ((this.myRig != null) ? (this.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : NetworkSystem.Instance.LocalPlayer);
			if (netPlayer != null)
			{
				this._events.Init(netPlayer);
			}
			if (this._events != null)
			{
				this._events.Activate.reliable = true;
				this._events.Deactivate.reliable = true;
				this._events.Activate += this.OnCountChanged_SharedEvent;
				this._events.Deactivate += this.OnCountReached_SharedEvent;
			}
			if (this.evaluateOnEnable)
			{
				this.CheckTriggers(this.currentCount, this.currentCount);
			}
		}

		// Token: 0x0600741C RID: 29724 RVA: 0x0025C550 File Offset: 0x0025A750
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.OnCountChanged_SharedEvent;
				this._events.Deactivate -= this.OnCountReached_SharedEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x0600741D RID: 29725 RVA: 0x0025C5C4 File Offset: 0x0025A7C4
		private void OnValidate()
		{
			if (this.triggers == null)
			{
				return;
			}
			for (int i = 0; i < this.triggers.Count; i++)
			{
				if (this.triggers[i].triggerCount < 0)
				{
					this.triggers[i].triggerCount = 0;
				}
			}
		}

		// Token: 0x0600741E RID: 29726 RVA: 0x0025C616 File Offset: 0x0025A816
		public void Increment()
		{
			this.SetCount(this.currentCount + 1);
		}

		// Token: 0x0600741F RID: 29727 RVA: 0x0025C626 File Offset: 0x0025A826
		public void Decrement()
		{
			this.SetCount(this.currentCount - 1);
		}

		// Token: 0x06007420 RID: 29728 RVA: 0x0025C638 File Offset: 0x0025A838
		public void SetCount(int newCount)
		{
			if (this.myRig != null && !this.myRig.isLocal)
			{
				return;
			}
			if (this.IsOnCooldown())
			{
				return;
			}
			int num = this.currentCount;
			if (this.wrapCount)
			{
				int highestTriggerCount = this.GetHighestTriggerCount();
				if (highestTriggerCount > 0)
				{
					int num2 = highestTriggerCount + 1;
					newCount = (newCount % num2 + num2) % num2;
				}
				else if (newCount < 0)
				{
					newCount = 0;
				}
			}
			else if (newCount < 0)
			{
				newCount = 0;
			}
			if (newCount == num)
			{
				return;
			}
			bool flag = false;
			this.currentCount = newCount;
			this.lastEventTime = Time.time;
			UnityEvent<int> unityEvent = this.onCountChanged;
			if (unityEvent != null)
			{
				unityEvent.Invoke(this.currentCount);
			}
			UnityEvent<int> unityEvent2 = this.onCountChangedShared;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(this.currentCount);
			}
			if (this.currentCount > num)
			{
				UnityEvent<int> unityEvent3 = this.onCountIncreased;
				if (unityEvent3 != null)
				{
					unityEvent3.Invoke(this.currentCount);
				}
				UnityEvent<int> unityEvent4 = this.onCountIncreasedShared;
				if (unityEvent4 != null)
				{
					unityEvent4.Invoke(this.currentCount);
				}
				flag = true;
			}
			else if (this.currentCount < num)
			{
				UnityEvent<int> unityEvent5 = this.onCountDecreased;
				if (unityEvent5 != null)
				{
					unityEvent5.Invoke(this.currentCount);
				}
				UnityEvent<int> unityEvent6 = this.onCountDecreasedShared;
				if (unityEvent6 != null)
				{
					unityEvent6.Invoke(this.currentCount);
				}
			}
			this.CheckTriggers(num, this.currentCount);
			if (this.currentCount == 0)
			{
				UnityEvent unityEvent7 = this.onCountResetToZero;
				if (unityEvent7 != null)
				{
					unityEvent7.Invoke();
				}
				UnityEvent unityEvent8 = this.onCountResetToZeroShared;
				if (unityEvent8 != null)
				{
					unityEvent8.Invoke();
				}
			}
			int highestTriggerCount2 = this.GetHighestTriggerCount();
			if (highestTriggerCount2 > 0 && this.currentCount == highestTriggerCount2)
			{
				UnityEvent unityEvent9 = this.onReachedMaxTrigger;
				if (unityEvent9 != null)
				{
					unityEvent9.Invoke();
				}
				UnityEvent unityEvent10 = this.onReachedMaxTriggerShared;
				if (unityEvent10 != null)
				{
					unityEvent10.Invoke();
				}
			}
			if (this.syncAllEvents && PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				object[] array = new object[] { flag, this.currentCount };
				this._events.Activate.RaiseOthers(array);
			}
		}

		// Token: 0x06007421 RID: 29729 RVA: 0x0025C830 File Offset: 0x0025AA30
		[Tooltip("Resets all 'triggerOnce' flags, allowing one-time triggers to fire again.\n\nUse this when restarting a sequence, resetting an object,\nor testing trigger behavior multiple times in play mode.")]
		public void ResetTriggers()
		{
			for (int i = 0; i < this.triggers.Count; i++)
			{
				this.triggers[i].hasTriggered = false;
			}
		}

		// Token: 0x06007422 RID: 29730 RVA: 0x0025C868 File Offset: 0x0025AA68
		private int GetHighestTriggerCount()
		{
			int num = 0;
			for (int i = 0; i < this.triggers.Count; i++)
			{
				if (this.triggers[i].triggerCount > num)
				{
					num = this.triggers[i].triggerCount;
				}
			}
			return num;
		}

		// Token: 0x06007423 RID: 29731 RVA: 0x0025C8B4 File Offset: 0x0025AAB4
		private void CheckTriggers(int oldCount, int newCount)
		{
			if (this.myRig != null && !this.myRig.isLocal)
			{
				return;
			}
			for (int i = 0; i < this.triggers.Count; i++)
			{
				CountDrivenEvents.CountTrigger countTrigger = this.triggers[i];
				if (!countTrigger.triggerOnce || !countTrigger.hasTriggered)
				{
					bool flag = false;
					if (this.wrapCount)
					{
						if (newCount == countTrigger.triggerCount)
						{
							flag = true;
						}
					}
					else if (oldCount < countTrigger.triggerCount && newCount >= countTrigger.triggerCount)
					{
						flag = true;
					}
					else if (oldCount > countTrigger.triggerCount && newCount <= countTrigger.triggerCount)
					{
						flag = true;
					}
					else if (oldCount == newCount && newCount == countTrigger.triggerCount)
					{
						flag = true;
					}
					if (flag)
					{
						UnityEvent onCountReached = countTrigger.onCountReached;
						if (onCountReached != null)
						{
							onCountReached.Invoke();
						}
						UnityEvent onCountReachedShared = countTrigger.onCountReachedShared;
						if (onCountReachedShared != null)
						{
							onCountReachedShared.Invoke();
						}
						if (this.syncAllEvents && PhotonNetwork.InRoom && this._events != null && this._events.Deactivate != null)
						{
							object[] array = new object[] { i };
							this._events.Deactivate.RaiseOthers(array);
						}
						if (countTrigger.triggerOnce)
						{
							countTrigger.hasTriggered = true;
						}
					}
				}
			}
		}

		// Token: 0x06007424 RID: 29732 RVA: 0x0025C9F4 File Offset: 0x0025ABF4
		private void OnCountChanged_SharedEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target || info.senderID != this.myRig.creator.ActorNumber)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnCountChanged_SharedEvent");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			if (args.Length != 2)
			{
				return;
			}
			object obj = args[0];
			if (!(obj is bool))
			{
				return;
			}
			bool flag = (bool)obj;
			obj = args[1];
			if (!(obj is int))
			{
				return;
			}
			int num = (int)obj;
			UnityEvent<int> unityEvent = this.onCountChangedShared;
			if (unityEvent != null)
			{
				unityEvent.Invoke(num);
			}
			if (flag)
			{
				UnityEvent<int> unityEvent2 = this.onCountIncreasedShared;
				if (unityEvent2 != null)
				{
					unityEvent2.Invoke(num);
				}
			}
			else
			{
				UnityEvent<int> unityEvent3 = this.onCountDecreasedShared;
				if (unityEvent3 != null)
				{
					unityEvent3.Invoke(num);
				}
			}
			int highestTriggerCount = this.GetHighestTriggerCount();
			if (num != 0)
			{
				if (highestTriggerCount > 0 && num == highestTriggerCount)
				{
					UnityEvent unityEvent4 = this.onReachedMaxTriggerShared;
					if (unityEvent4 == null)
					{
						return;
					}
					unityEvent4.Invoke();
				}
				return;
			}
			UnityEvent unityEvent5 = this.onCountResetToZeroShared;
			if (unityEvent5 == null)
			{
				return;
			}
			unityEvent5.Invoke();
		}

		// Token: 0x06007425 RID: 29733 RVA: 0x0025CAE0 File Offset: 0x0025ACE0
		private void OnCountReached_SharedEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target || info.senderID != this.myRig.creator.ActorNumber)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnCountReached_SharedEvent");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			if (args.Length != 1)
			{
				return;
			}
			object obj = args[0];
			if (!(obj is int))
			{
				return;
			}
			int num = (int)obj;
			if (num < 0 || num >= this.triggers.Count)
			{
				return;
			}
			UnityEvent onCountReachedShared = this.triggers[num].onCountReachedShared;
			if (onCountReachedShared == null)
			{
				return;
			}
			onCountReachedShared.Invoke();
		}

		// Token: 0x040083E3 RID: 33763
		[Header("Network")]
		[SerializeField]
		private bool syncAllEvents;

		// Token: 0x040083E4 RID: 33764
		[Header("General Settings")]
		[Tooltip("If true, triggers will be evaluated once on enable using the initial count.")]
		[SerializeField]
		private bool evaluateOnEnable;

		// Token: 0x040083E5 RID: 33765
		[Tooltip("If enabled, the counter value will loop between 0 and the highest triggerCount.")]
		[SerializeField]
		private bool wrapCount;

		// Token: 0x040083E6 RID: 33766
		[Tooltip("Minimum time (in seconds) that must pass before events can fire again")]
		[SerializeField]
		private float cooldown;

		// Token: 0x040083E7 RID: 33767
		[Header("Count Triggers")]
		[SerializeField]
		private List<CountDrivenEvents.CountTrigger> triggers = new List<CountDrivenEvents.CountTrigger>();

		// Token: 0x040083E8 RID: 33768
		[Header("Local and Networked Events")]
		public UnityEvent<int> onCountChanged;

		// Token: 0x040083E9 RID: 33769
		public UnityEvent<int> onCountChangedShared;

		// Token: 0x040083EA RID: 33770
		public UnityEvent<int> onCountIncreased;

		// Token: 0x040083EB RID: 33771
		public UnityEvent<int> onCountIncreasedShared;

		// Token: 0x040083EC RID: 33772
		public UnityEvent<int> onCountDecreased;

		// Token: 0x040083ED RID: 33773
		public UnityEvent<int> onCountDecreasedShared;

		// Token: 0x040083EE RID: 33774
		public UnityEvent onCountResetToZero;

		// Token: 0x040083EF RID: 33775
		public UnityEvent onCountResetToZeroShared;

		// Token: 0x040083F0 RID: 33776
		public UnityEvent onReachedMaxTrigger;

		// Token: 0x040083F1 RID: 33777
		public UnityEvent onReachedMaxTriggerShared;

		// Token: 0x040083F2 RID: 33778
		[Header("Debug - Counter Settings")]
		[SerializeField]
		private int currentCount;

		// Token: 0x040083F3 RID: 33779
		private RubberDuckEvents _events;

		// Token: 0x040083F4 RID: 33780
		private VRRig myRig;

		// Token: 0x040083F5 RID: 33781
		private CallLimiter callLimiter = new CallLimiter(10, 1f, 0.5f);

		// Token: 0x040083F6 RID: 33782
		[NonSerialized]
		private float lastEventTime = float.NegativeInfinity;

		// Token: 0x020011D9 RID: 4569
		[Serializable]
		public class CountTrigger
		{
			// Token: 0x040083F7 RID: 33783
			[Tooltip("The count value that triggers this event")]
			public int triggerCount;

			// Token: 0x040083F8 RID: 33784
			[Tooltip("Events to invoke when count reaches this value")]
			public UnityEvent onCountReached;

			// Token: 0x040083F9 RID: 33785
			public UnityEvent onCountReachedShared;

			// Token: 0x040083FA RID: 33786
			[Tooltip("Should this trigger fire every time the count passes through this value, or only once?")]
			public bool triggerOnce;

			// Token: 0x040083FB RID: 33787
			[NonSerialized]
			public bool hasTriggered;
		}
	}
}
