using System;
using UnityEngine;

// Token: 0x02000044 RID: 68
public class CritterDespawner : MonoBehaviour
{
	// Token: 0x0600011A RID: 282 RVA: 0x00006CB6 File Offset: 0x00004EB6
	public void DespawnAllCritters()
	{
		CrittersManager.instance.QueueDespawnAllCritters();
	}
}
