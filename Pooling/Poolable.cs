using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace Pooling
{
	// Token: 0x02000F1A RID: 3866
	public class Poolable : MonoBehaviour, IPoolable<Poolable>
	{
		// Token: 0x1700092A RID: 2346
		// (get) Token: 0x06005ED4 RID: 24276 RVA: 0x001E33E2 File Offset: 0x001E15E2
		// (set) Token: 0x06005ED5 RID: 24277 RVA: 0x001E33EA File Offset: 0x001E15EA
		public IObjectPool<Poolable> Pool { get; set; }

		// Token: 0x06005ED6 RID: 24278 RVA: 0x001E33F3 File Offset: 0x001E15F3
		public void OnCreate()
		{
			UnityEvent unityEvent = this.onCreate;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x06005ED7 RID: 24279 RVA: 0x001E3405 File Offset: 0x001E1605
		public void OnPreGet()
		{
			UnityEvent unityEvent = this.onPreGet;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x06005ED8 RID: 24280 RVA: 0x001E3417 File Offset: 0x001E1617
		public void OnPostGet()
		{
			UnityEvent unityEvent = this.onPostGet;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x06005ED9 RID: 24281 RVA: 0x001E3429 File Offset: 0x001E1629
		public void OnRelease()
		{
			UnityEvent unityEvent = this.onRelease;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x04006D8C RID: 28044
		public UnityEvent onCreate;

		// Token: 0x04006D8D RID: 28045
		public UnityEvent onPreGet;

		// Token: 0x04006D8E RID: 28046
		public UnityEvent onPostGet;

		// Token: 0x04006D8F RID: 28047
		public UnityEvent onRelease;
	}
}
