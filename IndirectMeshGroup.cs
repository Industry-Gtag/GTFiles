using System;
using UnityEngine;

// Token: 0x0200037A RID: 890
public sealed class IndirectMeshGroup : MonoBehaviour
{
	// Token: 0x060015D5 RID: 5589 RVA: 0x000736C4 File Offset: 0x000718C4
	private void OnEnable()
	{
		IndirectMeshRenderer.SetGroupVisible(base.GetInstanceID(), true);
	}

	// Token: 0x060015D6 RID: 5590 RVA: 0x000736D2 File Offset: 0x000718D2
	private void OnDisable()
	{
		IndirectMeshRenderer.SetGroupVisible(base.GetInstanceID(), false);
	}
}
