using System;
using TMPro;
using UnityEngine;

// Token: 0x0200008D RID: 141
public class MenagerieSlot : MonoBehaviour
{
	// Token: 0x0600038C RID: 908 RVA: 0x00014BFE File Offset: 0x00012DFE
	private void Reset()
	{
		this.critterMountPoint = base.transform;
	}

	// Token: 0x0400040B RID: 1035
	public Transform critterMountPoint;

	// Token: 0x0400040C RID: 1036
	public TMP_Text label;

	// Token: 0x0400040D RID: 1037
	public MenagerieCritter critter;
}
