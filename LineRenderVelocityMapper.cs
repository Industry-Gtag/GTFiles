using System;
using UnityEngine;

// Token: 0x0200041F RID: 1055
[RequireComponent(typeof(LineRenderer))]
public class LineRenderVelocityMapper : MonoBehaviour
{
	// Token: 0x0600190F RID: 6415 RVA: 0x0008E07B File Offset: 0x0008C27B
	private void Awake()
	{
		this._lr = base.GetComponent<LineRenderer>();
		this._lr.useWorldSpace = true;
	}

	// Token: 0x06001910 RID: 6416 RVA: 0x0008E098 File Offset: 0x0008C298
	private void LateUpdate()
	{
		if (this.velocityEstimator == null)
		{
			return;
		}
		this._lr.SetPosition(0, this.velocityEstimator.transform.position);
		if (this.velocityEstimator.linearVelocity.sqrMagnitude > 0.1f)
		{
			this._lr.SetPosition(1, this.velocityEstimator.transform.position + this.velocityEstimator.linearVelocity.normalized * 0.2f);
			return;
		}
		this._lr.SetPosition(1, this.velocityEstimator.transform.position);
	}

	// Token: 0x0400243B RID: 9275
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x0400243C RID: 9276
	private LineRenderer _lr;
}
