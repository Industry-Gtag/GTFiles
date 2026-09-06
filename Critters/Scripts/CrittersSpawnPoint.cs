using System;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x020013F0 RID: 5104
	public class CrittersSpawnPoint : MonoBehaviour
	{
		// Token: 0x060080BE RID: 32958 RVA: 0x0029DB18 File Offset: 0x0029BD18
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(base.transform.position, 0.1f);
		}
	}
}
