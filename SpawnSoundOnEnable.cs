using System;
using UnityEngine;

// Token: 0x02000093 RID: 147
public class SpawnSoundOnEnable : MonoBehaviour
{
	// Token: 0x0600039C RID: 924 RVA: 0x00014F18 File Offset: 0x00013118
	private void OnEnable()
	{
		if (CrittersManager.instance == null || !CrittersManager.instance.LocalAuthority() || !CrittersManager.instance.LocalInZone)
		{
			return;
		}
		if (!this.triggerOnFirstEnable && !this.firstEnabledOccured)
		{
			this.firstEnabledOccured = true;
			return;
		}
		CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.SpawnActor(CrittersActor.CrittersActorType.LoudNoise, this.soundSubIndex);
		if (crittersLoudNoise == null)
		{
			return;
		}
		crittersLoudNoise.MoveActor(base.transform.position, base.transform.rotation, false, true, true);
		crittersLoudNoise.SetImpulseVelocity(Vector3.zero, Vector3.zero);
	}

	// Token: 0x04000428 RID: 1064
	public int soundSubIndex = 3;

	// Token: 0x04000429 RID: 1065
	public bool triggerOnFirstEnable;

	// Token: 0x0400042A RID: 1066
	private bool firstEnabledOccured;
}
