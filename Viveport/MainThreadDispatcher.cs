using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viveport
{
	// Token: 0x02000E9E RID: 3742
	public class MainThreadDispatcher : MonoBehaviour
	{
		// Token: 0x06005ADC RID: 23260 RVA: 0x001D9275 File Offset: 0x001D7475
		private void Awake()
		{
			if (MainThreadDispatcher.instance == null)
			{
				MainThreadDispatcher.instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		// Token: 0x06005ADD RID: 23261 RVA: 0x001D9298 File Offset: 0x001D7498
		public void Update()
		{
			Queue<Action> queue = MainThreadDispatcher.actions;
			lock (queue)
			{
				while (MainThreadDispatcher.actions.Count > 0)
				{
					MainThreadDispatcher.actions.Dequeue()();
				}
			}
		}

		// Token: 0x06005ADE RID: 23262 RVA: 0x001D92F0 File Offset: 0x001D74F0
		public static MainThreadDispatcher Instance()
		{
			if (MainThreadDispatcher.instance == null)
			{
				throw new Exception("Could not find the MainThreadDispatcher GameObject. Please ensure you have added this script to an empty GameObject in your scene.");
			}
			return MainThreadDispatcher.instance;
		}

		// Token: 0x06005ADF RID: 23263 RVA: 0x001D930F File Offset: 0x001D750F
		private void OnDestroy()
		{
			MainThreadDispatcher.instance = null;
		}

		// Token: 0x06005AE0 RID: 23264 RVA: 0x001D9318 File Offset: 0x001D7518
		public void Enqueue(IEnumerator action)
		{
			Queue<Action> queue = MainThreadDispatcher.actions;
			lock (queue)
			{
				MainThreadDispatcher.actions.Enqueue(delegate
				{
					this.StartCoroutine(action);
				});
			}
		}

		// Token: 0x06005AE1 RID: 23265 RVA: 0x001D937C File Offset: 0x001D757C
		public void Enqueue(Action action)
		{
			this.Enqueue(this.ActionWrapper(action));
		}

		// Token: 0x06005AE2 RID: 23266 RVA: 0x001D938B File Offset: 0x001D758B
		public void Enqueue<T1>(Action<T1> action, T1 param1)
		{
			this.Enqueue(this.ActionWrapper<T1>(action, param1));
		}

		// Token: 0x06005AE3 RID: 23267 RVA: 0x001D939B File Offset: 0x001D759B
		public void Enqueue<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
		{
			this.Enqueue(this.ActionWrapper<T1, T2>(action, param1, param2));
		}

		// Token: 0x06005AE4 RID: 23268 RVA: 0x001D93AC File Offset: 0x001D75AC
		public void Enqueue<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
			this.Enqueue(this.ActionWrapper<T1, T2, T3>(action, param1, param2, param3));
		}

		// Token: 0x06005AE5 RID: 23269 RVA: 0x001D93BF File Offset: 0x001D75BF
		public void Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			this.Enqueue(this.ActionWrapper<T1, T2, T3, T4>(action, param1, param2, param3, param4));
		}

		// Token: 0x06005AE6 RID: 23270 RVA: 0x001D93D4 File Offset: 0x001D75D4
		private IEnumerator ActionWrapper(Action action)
		{
			action();
			yield return null;
			yield break;
		}

		// Token: 0x06005AE7 RID: 23271 RVA: 0x001D93E3 File Offset: 0x001D75E3
		private IEnumerator ActionWrapper<T1>(Action<T1> action, T1 param1)
		{
			action(param1);
			yield return null;
			yield break;
		}

		// Token: 0x06005AE8 RID: 23272 RVA: 0x001D93F9 File Offset: 0x001D75F9
		private IEnumerator ActionWrapper<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
		{
			action(param1, param2);
			yield return null;
			yield break;
		}

		// Token: 0x06005AE9 RID: 23273 RVA: 0x001D9416 File Offset: 0x001D7616
		private IEnumerator ActionWrapper<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
			action(param1, param2, param3);
			yield return null;
			yield break;
		}

		// Token: 0x06005AEA RID: 23274 RVA: 0x001D943B File Offset: 0x001D763B
		private IEnumerator ActionWrapper<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			action(param1, param2, param3, param4);
			yield return null;
			yield break;
		}

		// Token: 0x04006BD0 RID: 27600
		private static readonly Queue<Action> actions = new Queue<Action>();

		// Token: 0x04006BD1 RID: 27601
		private static MainThreadDispatcher instance = null;
	}
}
