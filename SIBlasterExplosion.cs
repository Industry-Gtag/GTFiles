using System;
using UnityEngine;

// Token: 0x020000D8 RID: 216
public class SIBlasterExplosion : MonoBehaviour
{
	// Token: 0x0600051C RID: 1308 RVA: 0x0001CA66 File Offset: 0x0001AC66
	private void OnDisable()
	{
		SIGadgetBlasterProjectile.DespawnExplosion(base.gameObject);
	}
}
