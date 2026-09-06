using System;
using UnityEngine;

// Token: 0x020007CC RID: 1996
public class GRPatrolPathNode : MonoBehaviour
{
	// Token: 0x060032D6 RID: 13014 RVA: 0x00116868 File Offset: 0x00114A68
	public void OnDrawGizmosSelected()
	{
		if (base.transform.parent == null)
		{
			return;
		}
		GRPatrolPath component = base.transform.parent.GetComponent<GRPatrolPath>();
		if (component == null)
		{
			return;
		}
		component.OnDrawGizmosSelected();
	}
}
