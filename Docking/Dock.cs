using System;
using UnityEngine;
using UnityEngine.Events;

namespace Docking
{
	// Token: 0x020013E7 RID: 5095
	public class Dock : MonoBehaviour
	{
		// Token: 0x17000C5A RID: 3162
		// (get) Token: 0x06008099 RID: 32921 RVA: 0x0029D4B2 File Offset: 0x0029B6B2
		public bool Moveable
		{
			get
			{
				return this.moveable;
			}
		}

		// Token: 0x17000C5B RID: 3163
		// (get) Token: 0x0600809A RID: 32922 RVA: 0x0029D4BA File Offset: 0x0029B6BA
		public float ForceUndockTime
		{
			get
			{
				return this.forceUndockTime;
			}
		}

		// Token: 0x0600809B RID: 32923 RVA: 0x0029D4C2 File Offset: 0x0029B6C2
		public void NotifyDocked()
		{
			UnityEvent onDock = this.OnDock;
			if (onDock == null)
			{
				return;
			}
			onDock.Invoke();
		}

		// Token: 0x0600809C RID: 32924 RVA: 0x0029D4D4 File Offset: 0x0029B6D4
		public void NotifyUnDocked()
		{
			UnityEvent onUnDock = this.OnUnDock;
			if (onUnDock == null)
			{
				return;
			}
			onUnDock.Invoke();
		}

		// Token: 0x040091AD RID: 37293
		[SerializeField]
		protected bool moveable;

		// Token: 0x040091AE RID: 37294
		[SerializeField]
		protected float forceUndockTime;

		// Token: 0x040091AF RID: 37295
		[SerializeField]
		protected UnityEvent OnDock;

		// Token: 0x040091B0 RID: 37296
		[SerializeField]
		protected UnityEvent OnUnDock;
	}
}
