using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameObjectScheduling
{
	// Token: 0x020013FC RID: 5116
	public class GameObjectSchedulerEventDispatcher : MonoBehaviour
	{
		// Token: 0x17000C68 RID: 3176
		// (get) Token: 0x06008100 RID: 33024 RVA: 0x0029EB36 File Offset: 0x0029CD36
		public UnityEvent OnScheduledActivation
		{
			get
			{
				return this.onScheduledActivation;
			}
		}

		// Token: 0x17000C69 RID: 3177
		// (get) Token: 0x06008101 RID: 33025 RVA: 0x0029EB3E File Offset: 0x0029CD3E
		public UnityEvent OnScheduledDeactivation
		{
			get
			{
				return this.onScheduledDeactivation;
			}
		}

		// Token: 0x040091FD RID: 37373
		[SerializeField]
		private UnityEvent onScheduledActivation;

		// Token: 0x040091FE RID: 37374
		[SerializeField]
		private UnityEvent onScheduledDeactivation;
	}
}
