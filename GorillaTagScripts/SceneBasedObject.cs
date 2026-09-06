using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000FAB RID: 4011
	public class SceneBasedObject : MonoBehaviour
	{
		// Token: 0x060063E3 RID: 25571 RVA: 0x00201DC7 File Offset: 0x001FFFC7
		public bool IsLocalPlayerInScene()
		{
			return (ZoneManagement.instance.GetAllLoadedScenes().Count <= 1 || this.zone != GTZone.forest) && ZoneManagement.instance.IsSceneLoaded(this.zone);
		}

		// Token: 0x04007293 RID: 29331
		public GTZone zone;
	}
}
