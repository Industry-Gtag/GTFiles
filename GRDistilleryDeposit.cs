using System;
using UnityEngine;

// Token: 0x02000771 RID: 1905
public class GRDistilleryDeposit : MonoBehaviour
{
	// Token: 0x06003058 RID: 12376 RVA: 0x00106490 File Offset: 0x00104690
	private void Start()
	{
		this._distillery = base.GetComponentInParent<GRDistillery>();
	}

	// Token: 0x06003059 RID: 12377 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnTriggerEnter(Collider other)
	{
	}

	// Token: 0x04003DDA RID: 15834
	public float hapticStrength;

	// Token: 0x04003DDB RID: 15835
	public float hapticDuration;

	// Token: 0x04003DDC RID: 15836
	private GRDistillery _distillery;
}
