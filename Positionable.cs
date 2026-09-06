using System;
using UnityEngine;

// Token: 0x0200049F RID: 1183
public class Positionable : MonoBehaviour
{
	// Token: 0x06001CA5 RID: 7333 RVA: 0x0009B294 File Offset: 0x00099494
	public void CopyPostion(Transform t)
	{
		base.transform.position = t.position;
	}

	// Token: 0x06001CA6 RID: 7334 RVA: 0x0009B2A7 File Offset: 0x000994A7
	public void StickRightUnder(Transform t)
	{
		base.transform.position = t.position;
		base.transform.localPosition = Vector3.zero;
	}
}
