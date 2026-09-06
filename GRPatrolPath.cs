using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007CB RID: 1995
public class GRPatrolPath : MonoBehaviour
{
	// Token: 0x060032D3 RID: 13011 RVA: 0x0011670C File Offset: 0x0011490C
	private void Awake()
	{
		this.patrolNodes = new List<Transform>(base.transform.childCount);
		for (int i = 0; i < base.transform.childCount; i++)
		{
			this.patrolNodes.Add(base.transform.GetChild(i));
		}
	}

	// Token: 0x060032D4 RID: 13012 RVA: 0x0011675C File Offset: 0x0011495C
	public void OnDrawGizmosSelected()
	{
		if (this.patrolNodes == null || base.transform.childCount != this.patrolNodes.Count)
		{
			this.patrolNodes = new List<Transform>(base.transform.childCount);
			for (int i = 0; i < base.transform.childCount; i++)
			{
				this.patrolNodes.Add(base.transform.GetChild(i));
			}
		}
		if (this.patrolNodes != null)
		{
			for (int j = 0; j < this.patrolNodes.Count; j++)
			{
				Gizmos.color = Color.magenta;
				Gizmos.DrawCube(this.patrolNodes[j].transform.position, Vector3.one * 0.5f);
				if (j < this.patrolNodes.Count - 1)
				{
					Gizmos.DrawLine(this.patrolNodes[j].transform.position, this.patrolNodes[j + 1].transform.position);
				}
			}
		}
	}

	// Token: 0x040041DF RID: 16863
	[NonSerialized]
	public List<Transform> patrolNodes;

	// Token: 0x040041E0 RID: 16864
	public int index;
}
