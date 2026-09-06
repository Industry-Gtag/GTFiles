using System;
using UnityEngine;

namespace Docking
{
	// Token: 0x020013E8 RID: 5096
	public class Dockable : MonoBehaviour
	{
		// Token: 0x0600809E RID: 32926 RVA: 0x0029D4E6 File Offset: 0x0029B6E6
		protected virtual void OnTriggerEnter(Collider other)
		{
			this.potentialDock = other.GetComponent<Dock>();
		}

		// Token: 0x0600809F RID: 32927 RVA: 0x0029D4F4 File Offset: 0x0029B6F4
		protected virtual void OnTriggerExit(Collider other)
		{
			if (this.potentialDock != null && this.potentialDock.transform == other.transform)
			{
				this.potentialDock = null;
			}
		}

		// Token: 0x060080A0 RID: 32928 RVA: 0x0029D524 File Offset: 0x0029B724
		public virtual void Dock()
		{
			if (this.potentialDock == null)
			{
				return;
			}
			base.transform.position = this.potentialDock.transform.position;
			base.transform.rotation = this.potentialDock.transform.rotation;
			this.potentialDock.NotifyDocked();
			if (this.potentialDock.Moveable)
			{
				this.currentDock = this.potentialDock;
				if (this.currentDock.ForceUndockTime > 0f)
				{
					this.undockTime = Time.time + this.currentDock.ForceUndockTime;
				}
			}
			this.potentialDock = null;
		}

		// Token: 0x060080A1 RID: 32929 RVA: 0x0029D5CA File Offset: 0x0029B7CA
		public virtual void UnDock()
		{
			if (this.currentDock != null)
			{
				this.currentDock.NotifyUnDocked();
			}
			this.currentDock = null;
			this.potentialDock = null;
			this.undockTime = 0f;
		}

		// Token: 0x060080A2 RID: 32930 RVA: 0x0029D600 File Offset: 0x0029B800
		private void LateUpdate()
		{
			if (this.currentDock != null)
			{
				base.transform.position = this.currentDock.transform.position;
				if (this.rotate)
				{
					base.transform.rotation = this.currentDock.transform.rotation;
				}
				if (this.undockTime > 0f && this.undockTime < Time.time)
				{
					this.UnDock();
				}
			}
		}

		// Token: 0x040091B1 RID: 37297
		protected Dock currentDock;

		// Token: 0x040091B2 RID: 37298
		protected Dock potentialDock;

		// Token: 0x040091B3 RID: 37299
		private float undockTime;

		// Token: 0x040091B4 RID: 37300
		protected bool rotate = true;
	}
}
