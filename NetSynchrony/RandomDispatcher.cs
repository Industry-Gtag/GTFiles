using System;
using System.Collections.Generic;
using UnityEngine;

namespace NetSynchrony
{
	// Token: 0x0200115B RID: 4443
	[CreateAssetMenu(fileName = "RandomDispatcher", menuName = "NetSynchrony/RandomDispatcher", order = 0)]
	public class RandomDispatcher : ScriptableObject
	{
		// Token: 0x140000C4 RID: 196
		// (add) Token: 0x06006F96 RID: 28566 RVA: 0x0023F5CC File Offset: 0x0023D7CC
		// (remove) Token: 0x06006F97 RID: 28567 RVA: 0x0023F604 File Offset: 0x0023D804
		public event RandomDispatcher.RandomDispatcherEvent Dispatch;

		// Token: 0x06006F98 RID: 28568 RVA: 0x0023F63C File Offset: 0x0023D83C
		public void Init(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			this.index = 0;
			this.dispatchTimes = new List<float>();
			float num = 0f;
			float num2 = this.totalMinutes * 60f;
			Random.InitState(StaticHash.Compute(Application.buildGUID));
			while (num < num2)
			{
				float num3 = Random.Range(this.minWaitTime, this.maxWaitTime);
				num += num3;
				if ((double)num < seconds)
				{
					this.index = this.dispatchTimes.Count;
				}
				this.dispatchTimes.Add(num);
			}
			Random.InitState((int)DateTime.Now.Ticks);
		}

		// Token: 0x06006F99 RID: 28569 RVA: 0x0023F6E0 File Offset: 0x0023D8E0
		public void Sync(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			this.index = 0;
			for (int i = 0; i < this.dispatchTimes.Count; i++)
			{
				if ((double)this.dispatchTimes[i] < seconds)
				{
					this.index = i;
				}
			}
		}

		// Token: 0x06006F9A RID: 28570 RVA: 0x0023F734 File Offset: 0x0023D934
		public void Tick(double seconds)
		{
			seconds %= (double)(this.totalMinutes * 60f);
			if ((double)this.dispatchTimes[this.index] < seconds)
			{
				this.index = (this.index + 1) % this.dispatchTimes.Count;
				if (this.Dispatch != null)
				{
					this.Dispatch(this);
				}
			}
		}

		// Token: 0x04007F81 RID: 32641
		[SerializeField]
		private float minWaitTime = 1f;

		// Token: 0x04007F82 RID: 32642
		[SerializeField]
		private float maxWaitTime = 10f;

		// Token: 0x04007F83 RID: 32643
		[SerializeField]
		private float totalMinutes = 60f;

		// Token: 0x04007F84 RID: 32644
		private List<float> dispatchTimes;

		// Token: 0x04007F85 RID: 32645
		private int index = -1;

		// Token: 0x0200115C RID: 4444
		// (Invoke) Token: 0x06006F9D RID: 28573
		public delegate void RandomDispatcherEvent(RandomDispatcher randomDispatcher);
	}
}
