using System;
using System.Collections;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020011B1 RID: 4529
	public class TestRopePerf : MonoBehaviour
	{
		// Token: 0x06007243 RID: 29251 RVA: 0x00253081 File Offset: 0x00251281
		private IEnumerator Start()
		{
			yield break;
		}

		// Token: 0x0400830C RID: 33548
		[SerializeField]
		private GameObject ropesOld;

		// Token: 0x0400830D RID: 33549
		[SerializeField]
		private GameObject ropesCustom;

		// Token: 0x0400830E RID: 33550
		[SerializeField]
		private GameObject ropesCustomVectorized;
	}
}
