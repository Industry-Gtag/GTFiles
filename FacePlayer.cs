using System;
using UnityEngine;

// Token: 0x020005C5 RID: 1477
public class FacePlayer : MonoBehaviour
{
	// Token: 0x06002540 RID: 9536 RVA: 0x000C77CC File Offset: 0x000C59CC
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.LookRotation(base.transform.position - GorillaTagger.Instance.headCollider.transform.position) * Quaternion.AngleAxis(-90f, Vector3.up);
	}
}
