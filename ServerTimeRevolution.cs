using System;
using UnityEngine;

// Token: 0x02000E02 RID: 3586
public class ServerTimeRevolution : MonoBehaviour
{
	// Token: 0x060057DD RID: 22493 RVA: 0x001CA204 File Offset: 0x001C8404
	private void LateUpdate()
	{
		double totalSeconds = (DateTime.UtcNow - this.anchor).TotalSeconds;
		double num = (double)this.orbit.x * Math.Sin(totalSeconds * this.speed);
		double num2 = (double)this.orbit.y * Math.Cos(totalSeconds * this.speed);
		double num3 = (double)this.orbit.z * Math.Cos(totalSeconds * this.speed);
		base.transform.position = this.pivot.position + this.pivotOffset + new Vector3((float)num, (float)num2, (float)num3);
	}

	// Token: 0x0400683D RID: 26685
	[SerializeField]
	private Vector3 orbit;

	// Token: 0x0400683E RID: 26686
	[SerializeField]
	private Transform pivot;

	// Token: 0x0400683F RID: 26687
	[SerializeField]
	private Vector3 pivotOffset;

	// Token: 0x04006840 RID: 26688
	[SerializeField]
	private double speed = 1.0;

	// Token: 0x04006841 RID: 26689
	private DateTime anchor = new DateTime(2026, 4, 1);
}
