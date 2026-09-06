using System;
using UnityEngine;

// Token: 0x020004E9 RID: 1257
public class ScalerUpper : MonoBehaviour
{
	// Token: 0x06001E94 RID: 7828 RVA: 0x000A39B8 File Offset: 0x000A1BB8
	private void Update()
	{
		for (int i = 0; i < this.target.Length; i++)
		{
			this.target[i].transform.localScale = Vector3.one * this.scaleCurve.Evaluate(this.t);
		}
		this.t += Time.deltaTime;
	}

	// Token: 0x06001E95 RID: 7829 RVA: 0x000A3A17 File Offset: 0x000A1C17
	private void OnEnable()
	{
		this.t = 0f;
	}

	// Token: 0x06001E96 RID: 7830 RVA: 0x000A3A24 File Offset: 0x000A1C24
	private void OnDisable()
	{
		for (int i = 0; i < this.target.Length; i++)
		{
			this.target[i].transform.localScale = Vector3.one;
		}
	}

	// Token: 0x040028CD RID: 10445
	[SerializeField]
	private Transform[] target;

	// Token: 0x040028CE RID: 10446
	[SerializeField]
	private AnimationCurve scaleCurve;

	// Token: 0x040028CF RID: 10447
	private float t;
}
