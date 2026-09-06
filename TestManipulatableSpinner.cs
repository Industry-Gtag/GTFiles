using System;
using UnityEngine;

// Token: 0x02000536 RID: 1334
public class TestManipulatableSpinner : MonoBehaviour
{
	// Token: 0x0600219F RID: 8607 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Start()
	{
	}

	// Token: 0x060021A0 RID: 8608 RVA: 0x000B3708 File Offset: 0x000B1908
	private void LateUpdate()
	{
		float angle = this.spinner.angle;
		base.transform.rotation = Quaternion.Euler(0f, angle * this.rotationScale, 0f);
	}

	// Token: 0x04002C60 RID: 11360
	public ManipulatableSpinner spinner;

	// Token: 0x04002C61 RID: 11361
	public float rotationScale = 1f;
}
