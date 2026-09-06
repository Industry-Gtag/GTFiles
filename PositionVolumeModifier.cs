using System;
using UnityEngine;

// Token: 0x02000369 RID: 873
public class PositionVolumeModifier : MonoBehaviour
{
	// Token: 0x0600155F RID: 5471 RVA: 0x0007198D File Offset: 0x0006FB8D
	public void OnTriggerStay(Collider other)
	{
		this.audioToMod.isModified = true;
	}

	// Token: 0x04001A37 RID: 6711
	public TimeOfDayDependentAudio audioToMod;
}
