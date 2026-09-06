using System;
using UnityEngine;

// Token: 0x02000587 RID: 1415
public class RotatableSurface : MonoBehaviour
{
	// Token: 0x060023F7 RID: 9207 RVA: 0x000C1EF0 File Offset: 0x000C00F0
	private void LateUpdate()
	{
		float angle = this.spinner.angle;
		base.transform.localRotation = Quaternion.Euler(0f, angle * this.rotationScale, 0f);
	}

	// Token: 0x04002F3F RID: 12095
	public ManipulatableSpinner spinner;

	// Token: 0x04002F40 RID: 12096
	public float rotationScale = 1f;
}
