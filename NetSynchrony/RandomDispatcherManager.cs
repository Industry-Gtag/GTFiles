using System;
using GorillaNetworking;
using UnityEngine;

namespace NetSynchrony
{
	// Token: 0x0200115D RID: 4445
	public class RandomDispatcherManager : MonoBehaviour
	{
		// Token: 0x06006FA0 RID: 28576 RVA: 0x0023F7C8 File Offset: 0x0023D9C8
		private void OnDisable()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (GorillaComputer.instance != null)
			{
				GorillaComputer instance = GorillaComputer.instance;
				instance.OnServerTimeUpdated = (Action)Delegate.Remove(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			}
		}

		// Token: 0x06006FA1 RID: 28577 RVA: 0x0023F814 File Offset: 0x0023DA14
		private void OnTimeChanged()
		{
			this.AdjustedServerTime();
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Sync(this.serverTime);
			}
		}

		// Token: 0x06006FA2 RID: 28578 RVA: 0x0023F850 File Offset: 0x0023DA50
		private void AdjustedServerTime()
		{
			DateTime dateTime = new DateTime(2020, 1, 1);
			long num = GorillaComputer.instance.GetServerTime().Ticks - dateTime.Ticks;
			this.serverTime = (double)((float)num / 10000000f);
		}

		// Token: 0x06006FA3 RID: 28579 RVA: 0x0023F898 File Offset: 0x0023DA98
		private void Start()
		{
			GorillaComputer instance = GorillaComputer.instance;
			instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Init(this.serverTime);
			}
		}

		// Token: 0x06006FA4 RID: 28580 RVA: 0x0023F8F4 File Offset: 0x0023DAF4
		private void Update()
		{
			for (int i = 0; i < this.randomDispatchers.Length; i++)
			{
				this.randomDispatchers[i].Tick(this.serverTime);
			}
			this.serverTime += (double)Time.deltaTime;
		}

		// Token: 0x04007F86 RID: 32646
		[SerializeField]
		private RandomDispatcher[] randomDispatchers;

		// Token: 0x04007F87 RID: 32647
		private static RandomDispatcherManager __instance;

		// Token: 0x04007F88 RID: 32648
		private double serverTime;
	}
}
