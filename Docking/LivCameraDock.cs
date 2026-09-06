using System;
using Liv.Lck.GorillaTag;

namespace Docking
{
	// Token: 0x020013E9 RID: 5097
	public class LivCameraDock : Dock
	{
		// Token: 0x060080A4 RID: 32932 RVA: 0x0029D688 File Offset: 0x0029B888
		private void Reset()
		{
			this.cameraSettings.fov = 80f;
		}

		// Token: 0x060080A5 RID: 32933 RVA: 0x0029D69C File Offset: 0x0029B89C
		private void OnValidate()
		{
			if (this.cameraSettings.forceFov && (this.cameraSettings.fov < 30f || this.cameraSettings.fov > 110f))
			{
				this.cameraSettings.fov = 80f;
			}
		}

		// Token: 0x040091B5 RID: 37301
		public GtCameraDockSettings cameraSettings;
	}
}
