using System;
using UnityEngine;

// Token: 0x020002FA RID: 762
public class SpeedDrivenAnim : MonoBehaviour
{
	// Token: 0x06001372 RID: 4978 RVA: 0x00066DF2 File Offset: 0x00064FF2
	private void Start()
	{
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
		this.animator = base.GetComponent<Animator>();
		this.keyHash = Animator.StringToHash(this.animKey);
	}

	// Token: 0x06001373 RID: 4979 RVA: 0x00066E20 File Offset: 0x00065020
	private void Update()
	{
		float num = Mathf.InverseLerp(this.speed0, this.speed1, this.velocityEstimator.linearVelocity.magnitude);
		this.currentBlend = Mathf.MoveTowards(this.currentBlend, num, this.maxChangePerSecond * Time.deltaTime);
		this.animator.SetFloat(this.keyHash, this.currentBlend);
	}

	// Token: 0x040017C3 RID: 6083
	[SerializeField]
	private float speed0;

	// Token: 0x040017C4 RID: 6084
	[SerializeField]
	private float speed1 = 1f;

	// Token: 0x040017C5 RID: 6085
	[SerializeField]
	private float maxChangePerSecond = 1f;

	// Token: 0x040017C6 RID: 6086
	[SerializeField]
	private string animKey = "speed";

	// Token: 0x040017C7 RID: 6087
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x040017C8 RID: 6088
	private Animator animator;

	// Token: 0x040017C9 RID: 6089
	private int keyHash;

	// Token: 0x040017CA RID: 6090
	private float currentBlend;
}
