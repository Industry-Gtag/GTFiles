using System;
using UnityEngine;

// Token: 0x02000226 RID: 550
public class BeePerchPoint : MonoBehaviour
{
	// Token: 0x06000E61 RID: 3681 RVA: 0x0004F693 File Offset: 0x0004D893
	public Vector3 GetPoint()
	{
		return base.transform.TransformPoint(this.localPosition);
	}

	// Token: 0x04001166 RID: 4454
	[SerializeField]
	private Vector3 localPosition;
}
