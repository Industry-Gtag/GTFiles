using System;
using Liv.Lck.GorillaTag;

namespace Docking
{
	// Token: 0x020013EA RID: 5098
	public class LivCameraDockable : Dockable
	{
		// Token: 0x060080A7 RID: 32935 RVA: 0x0029D6F4 File Offset: 0x0029B8F4
		public override void Dock()
		{
			base.Dock();
			if (this.currentDock is LivCameraDock)
			{
				GTLckController gtlckController = base.GetComponent<GTLckController>() ?? base.GetComponentInParent<GTLckController>();
				if (gtlckController != null)
				{
					gtlckController.ApplyCameraSettings(((LivCameraDock)this.currentDock).cameraSettings);
					this.rotate = !gtlckController.IsTabletFollowingPlayer;
				}
			}
		}
	}
}
