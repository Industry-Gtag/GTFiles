using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x0200033A RID: 826
public class FlattenerCrumb : MonoBehaviour
{
	// Token: 0x0600145C RID: 5212 RVA: 0x0006DBF0 File Offset: 0x0006BDF0
	private void OnDisable()
	{
		for (int i = this.flattenerList.Count - 1; i >= 0; i--)
		{
			this.flattenerList[i].CrumbDisabled();
		}
	}

	// Token: 0x0600145D RID: 5213 RVA: 0x0006DC26 File Offset: 0x0006BE26
	public void AddFlattenerReference(ObjectHierarchyFlattener flattener)
	{
		this.flattenerList.AddIfNew(flattener);
	}

	// Token: 0x04001927 RID: 6439
	[DebugReadout]
	private List<ObjectHierarchyFlattener> flattenerList = new List<ObjectHierarchyFlattener>();
}
