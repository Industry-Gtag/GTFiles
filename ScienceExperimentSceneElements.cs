using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x020009E4 RID: 2532
public class ScienceExperimentSceneElements : MonoBehaviour
{
	// Token: 0x06004105 RID: 16645 RVA: 0x0015AB8B File Offset: 0x00158D8B
	private void Awake()
	{
		ScienceExperimentManager.instance.InitElements(this);
	}

	// Token: 0x06004106 RID: 16646 RVA: 0x0015AB9A File Offset: 0x00158D9A
	private void OnDestroy()
	{
		ScienceExperimentManager.instance.DeInitElements();
	}

	// Token: 0x0400519E RID: 20894
	public List<ScienceExperimentSceneElements.DisableByLiquidData> disableByLiquidList = new List<ScienceExperimentSceneElements.DisableByLiquidData>();

	// Token: 0x0400519F RID: 20895
	public ParticleSystem sodaFizzParticles;

	// Token: 0x040051A0 RID: 20896
	public ParticleSystem sodaEruptionParticles;

	// Token: 0x020009E5 RID: 2533
	[Serializable]
	public struct DisableByLiquidData
	{
		// Token: 0x040051A1 RID: 20897
		public Transform target;

		// Token: 0x040051A2 RID: 20898
		public float heightOffset;
	}
}
