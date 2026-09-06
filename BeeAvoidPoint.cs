using System;
using UnityEngine;

// Token: 0x02000225 RID: 549
public class BeeAvoidPoint : MonoBehaviour
{
	// Token: 0x06000E5E RID: 3678 RVA: 0x0004F663 File Offset: 0x0004D863
	private void Start()
	{
		BeeSwarmManager.RegisterAvoidPoint(base.gameObject);
		FlockingManager.RegisterAvoidPoint(base.gameObject);
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x0004F67B File Offset: 0x0004D87B
	private void OnDestroy()
	{
		BeeSwarmManager.UnregisterAvoidPoint(base.gameObject);
		FlockingManager.UnregisterAvoidPoint(base.gameObject);
	}
}
