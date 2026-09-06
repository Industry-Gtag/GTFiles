using System;
using UnityEngine;

// Token: 0x02000E7F RID: 3711
[RequireComponent(typeof(GorillaVelocityEstimator))]
public class VelocityBasedActivator : MonoBehaviour
{
	// Token: 0x06005A40 RID: 23104 RVA: 0x001D4A95 File Offset: 0x001D2C95
	private void Start()
	{
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
	}

	// Token: 0x06005A41 RID: 23105 RVA: 0x001D4AA4 File Offset: 0x001D2CA4
	private void Update()
	{
		this.k += this.velocityEstimator.linearVelocity.sqrMagnitude;
		this.k = Mathf.Max(this.k - Time.deltaTime * this.decay, 0f);
		if (!this.active && this.k > this.threshold)
		{
			this.activate(true);
		}
		if (this.active && this.k < this.threshold)
		{
			this.activate(false);
		}
	}

	// Token: 0x06005A42 RID: 23106 RVA: 0x001D4B30 File Offset: 0x001D2D30
	private void activate(bool v)
	{
		this.active = v;
		for (int i = 0; i < this.activationTargets.Length; i++)
		{
			this.activationTargets[i].SetActive(v);
		}
	}

	// Token: 0x06005A43 RID: 23107 RVA: 0x001D4B65 File Offset: 0x001D2D65
	private void OnDisable()
	{
		if (this.active)
		{
			this.activate(false);
		}
	}

	// Token: 0x04006B3D RID: 27453
	[SerializeField]
	private GameObject[] activationTargets;

	// Token: 0x04006B3E RID: 27454
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04006B3F RID: 27455
	private float k;

	// Token: 0x04006B40 RID: 27456
	private bool active;

	// Token: 0x04006B41 RID: 27457
	[SerializeField]
	private float decay = 1f;

	// Token: 0x04006B42 RID: 27458
	[SerializeField]
	private float threshold = 1f;
}
