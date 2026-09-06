using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F50 RID: 3920
	public class MovingSurfaceManager : MonoBehaviour
	{
		// Token: 0x06006058 RID: 24664 RVA: 0x001E89F0 File Offset: 0x001E6BF0
		private void Awake()
		{
			if (MovingSurfaceManager.instance != null && MovingSurfaceManager.instance != this)
			{
				GTDev.LogWarning<string>("Instance of MovingSurfaceManager already exists. Destroying.", null);
				Object.Destroy(this);
				return;
			}
			if (MovingSurfaceManager.instance == null)
			{
				MovingSurfaceManager.instance = this;
			}
		}

		// Token: 0x06006059 RID: 24665 RVA: 0x001E8A3C File Offset: 0x001E6C3C
		public void RegisterMovingSurface(MovingSurface ms)
		{
			this.movingSurfaces.TryAdd(ms.GetID(), ms);
		}

		// Token: 0x0600605A RID: 24666 RVA: 0x001E8A51 File Offset: 0x001E6C51
		public void UnregisterMovingSurface(MovingSurface ms)
		{
			this.movingSurfaces.Remove(ms.GetID());
		}

		// Token: 0x0600605B RID: 24667 RVA: 0x001E8A65 File Offset: 0x001E6C65
		public void RegisterSurfaceMover(SurfaceMover sm)
		{
			if (!this.surfaceMovers.Contains(sm))
			{
				this.surfaceMovers.Add(sm);
				sm.InitMovingSurface();
			}
		}

		// Token: 0x0600605C RID: 24668 RVA: 0x001E8A87 File Offset: 0x001E6C87
		public void UnregisterSurfaceMover(SurfaceMover sm)
		{
			this.surfaceMovers.Remove(sm);
		}

		// Token: 0x0600605D RID: 24669 RVA: 0x001E8A96 File Offset: 0x001E6C96
		public bool TryGetMovingSurface(int id, out MovingSurface result)
		{
			return this.movingSurfaces.TryGetValue(id, out result) && result != null;
		}

		// Token: 0x0600605E RID: 24670 RVA: 0x001E8AB4 File Offset: 0x001E6CB4
		private void FixedUpdate()
		{
			foreach (SurfaceMover surfaceMover in this.surfaceMovers)
			{
				if (surfaceMover.isActiveAndEnabled)
				{
					surfaceMover.Move();
				}
			}
		}

		// Token: 0x04006EE7 RID: 28391
		private List<SurfaceMover> surfaceMovers = new List<SurfaceMover>(5);

		// Token: 0x04006EE8 RID: 28392
		private Dictionary<int, MovingSurface> movingSurfaces = new Dictionary<int, MovingSurface>(10);

		// Token: 0x04006EE9 RID: 28393
		public static MovingSurfaceManager instance;
	}
}
