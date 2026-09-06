using System;
using UnityEngine;

// Token: 0x020005DB RID: 1499
public class GorillaHandHistory : MonoBehaviour
{
	// Token: 0x060025A2 RID: 9634 RVA: 0x000C85C8 File Offset: 0x000C67C8
	private void Start()
	{
		this.direction = default(Vector3);
		this.lastPosition = default(Vector3);
	}

	// Token: 0x060025A3 RID: 9635 RVA: 0x000C85E2 File Offset: 0x000C67E2
	private void FixedUpdate()
	{
		this.direction = this.lastPosition - base.transform.position;
		this.lastLastPosition = this.lastPosition;
		this.lastPosition = base.transform.position;
	}

	// Token: 0x04003113 RID: 12563
	public Vector3 direction;

	// Token: 0x04003114 RID: 12564
	private Vector3 lastPosition;

	// Token: 0x04003115 RID: 12565
	private Vector3 lastLastPosition;
}
