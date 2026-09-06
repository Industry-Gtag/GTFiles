using System;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F4F RID: 3919
	[RequireComponent(typeof(Collider))]
	public class MovingSurface : MonoBehaviour
	{
		// Token: 0x06006053 RID: 24659 RVA: 0x001E8997 File Offset: 0x001E6B97
		private void Start()
		{
			MovingSurfaceManager.instance == null;
			MovingSurfaceManager.instance.RegisterMovingSurface(this);
		}

		// Token: 0x06006054 RID: 24660 RVA: 0x001E89B0 File Offset: 0x001E6BB0
		private void OnDestroy()
		{
			if (MovingSurfaceManager.instance != null)
			{
				MovingSurfaceManager.instance.UnregisterMovingSurface(this);
			}
		}

		// Token: 0x06006055 RID: 24661 RVA: 0x001E89CA File Offset: 0x001E6BCA
		public int GetID()
		{
			return this.uniqueId;
		}

		// Token: 0x06006056 RID: 24662 RVA: 0x001E89D2 File Offset: 0x001E6BD2
		public void CopySettings(MovingSurfaceSettings movingSurfaceSettings)
		{
			this.uniqueId = movingSurfaceSettings.uniqueId;
		}

		// Token: 0x04006EE6 RID: 28390
		[SerializeField]
		private int uniqueId = -1;
	}
}
